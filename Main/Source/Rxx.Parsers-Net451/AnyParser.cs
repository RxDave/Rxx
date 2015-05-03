using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Rxx.Parsers.Properties;

namespace Rxx.Parsers
{
  internal sealed class AnyParser<TSource, TResult> : IParser<TSource, TResult>
  {
    #region Public Properties
    public IParser<TSource, TSource> Next
    {
      get
      {
        if (selectedParser == null)
        {
          throw new InvalidOperationException(Errors.ParseNotCalledOrFailed);
        }

        return selectedParser.Next;
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
    private IParser<TSource, TResult> selectedParser;
    #endregion

    #region Constructors
    public AnyParser(IEnumerable<IParser<TSource, TResult>> parsers)
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

    public IEnumerable<IParseResult<TResult>> Parse(ICursor<TSource> source)
    {
      bool hasResult = false;

      foreach (var parser in parsers)
      {
        foreach (var result in parser.Parse(source))
        {
          if (!hasResult)
          {
            hasResult = true;

            /* The XML lab has shown that Parse may be called multiple times on the same AnyParser 
             * instance during a single Parse operation, sometimes with the same source but most of the time
             * with a different source; therefore, the selected parser must be reassigned to the latest selection 
             * for each call to Parse, maintaining a local variable (hasResult) to determine whether the current 
             * call to Parse has matched while enumerating the choices.
             * 
             * It is currently unknown whether it is possible for a nested Parse operation to overwrite the 
             * selected parser, or whether it will have any negative impact.
             */
            selectedParser = parser;
          }

          yield return result;
        }

        if (hasResult)
        {
          yield break;
        }
      }

      // completing without results is failure
    }

    internal IEnumerable<IParseResult<TResult>> Parse(
      ICollection<IParser<TSource, TResult>> except,
      ICursor<TSource> source)
    {
      Contract.Requires(except != null);
      Contract.Requires(!except.IsReadOnly);
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<TResult>>>() != null);

      bool hasResult = false;

      foreach (var parser in parsers.Except(except))
      {
        foreach (var result in parser.Parse(source))
        {
          if (!hasResult)
          {
            hasResult = true;

            except.Add(parser);
          }

          yield return result;
        }

        if (hasResult)
        {
          yield break;
        }
      }

      // completing without results is failure
    }
    #endregion
  }
}