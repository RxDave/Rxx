using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Rxx.Parsers.Properties;

namespace Rxx.Parsers
{
  internal sealed class AllParser<TSource, TResult> : IParser<TSource, IEnumerable<TResult>>
  {
    #region Public Properties
    public IParser<TSource, TSource> Next
    {
      get
      {
        if (firstParser == null)
        {
          throw new InvalidOperationException(Errors.ParseNotCalledOrFailed);
        }

        return firstParser.Next;
      }
    }

    public IEnumerable<IParser<TSource, TResult>> Parsers
    {
      get
      {
        Contract.Ensures(Contract.Result<IEnumerable<IParser<TSource, TResult>>>() != null);

        return parsers;
      }
    }
    #endregion

    #region Private / Protected
    private readonly IEnumerable<IParser<TSource, TResult>> parsers;
    private IParser<TSource, TResult> firstParser;
    #endregion

    #region Constructors
    public AllParser(IEnumerable<IParser<TSource, TResult>> parsers)
    {
      Contract.Requires(parsers != null);

      this.parsers = parsers;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(parsers != null);
    }

    public IEnumerable<IParseResult<IEnumerable<TResult>>> Parse(ICursor<TSource> source)
    {
      firstParser = null;

      bool first = true;

      /* The stack of branches allows for an optimization on top of the previous implementation, which used to create a new 
       * branch for every parse result in every parse result sequence.  This new implementation changes that behavior slightly 
       * by not creating a new branch for the last result in each sequence.  All parser rules (at the time of writing) in Rxx 
       * generate zero or one result only; therefore, the current branch can be moved forward and reused by child sequences 
       * without affecting the parent result sequence, because it's already completed.  This optimization has proven to be 
       * greatly beneficial across normal parser queries that use And, All and Exactly operators.  It should also be beneficial 
       * for multi-result queries since most of the individual parser rules in these queries will only generate a single result.
       * A new branch would only be created for each result in the multi-result sequence.  Scalar-result parsers that follow 
       * sequentially in the All query would simply move their shared parent branch instead of creating new branches.
       */
      var branches = new Stack<ICursor<TSource>>();

      using (var root = source.Branch())
      {
        branches.Push(root);

        var query = ParseResult.ReturnSuccessMany<TResult>(0)
          .SelectMany(
            parsers.Select(
              parser => (Func<Tuple<IParseResult<IEnumerable<TResult>>, bool>, IEnumerable<IParseResult<IEnumerable<TResult>>>>)
                (value =>
                {
                  if (first)
                  {
                    first = false;
                    firstParser = parser;
                  }

                  var branch = branches.Peek();

                  // Item2 is only true when value.Item1 is the last element of its sequence.
                  if (value.Item2)
                  {
                    branch.Move(value.Item1.Length);

                    return parser.Parse(branch).Select(result => result.YieldMany());
                  }
                  else
                  {
                    var remainder = branch.Remainder(value.Item1.Length);

                    branches.Push(remainder);

                    return parser.Parse(remainder)
                                 .Select(result => result.YieldMany())
                                 .OnErrorOrDisposed(() =>
                                 {
                                   /* Finally() cannot be used here.  SelectMany generates each value as a Tuple, indicating whether it's 
                                    * the last value in the sequence.  To gather that info it uses a side-effect that causes it to look-ahead 
                                    * in the enumerator, which would cause Finally to execute and dispose of the branch before it's used 
                                    * by the child sequence.  SelectMany only calls Dispose when the sequence is no longer in use.
                                    */
                                   branches.Pop().Dispose();
                                 });
                  }
                })),
              (firstResult, otherResults) => firstResult.Concat(otherResults));

        // Use an iterator block to ensure that the local variables in this method aren't shared between enumerators.
        foreach (var result in query)
        {
          yield return result;
        }
      }
    }
    #endregion
  }
}