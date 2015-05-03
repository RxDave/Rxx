using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Rxx.Parsers.Properties;

namespace Rxx.Parsers.Reactive
{
  internal sealed class AllObservableParser<TSource, TResult> : IObservableParser<TSource, IObservable<TResult>>
  {
    #region Public Properties
    public IObservableParser<TSource, TSource> Next
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

    public IEnumerable<IObservableParser<TSource, TResult>> Parsers
    {
      get
      {
        Contract.Ensures(Contract.Result<IEnumerable<IObservableParser<TSource, TResult>>>() != null);

        return parsers;
      }
    }
    #endregion

    #region Private / Protected
    private readonly IEnumerable<IObservableParser<TSource, TResult>> parsers;
    private IObservableParser<TSource, TResult> firstParser;
    #endregion

    #region Constructors
    public AllObservableParser(IEnumerable<IObservableParser<TSource, TResult>> parsers)
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "All disposables are attached to observables.")]
    public IObservable<IParseResult<IObservable<TResult>>> Parse(IObservableCursor<TSource> source)
    {
      return Observable.Defer(() =>
        {
          firstParser = null;

          bool first = true;

          /* The TContext of branches allows for an optimization on top of the previous implementation, which used to create a new 
           * branch for every parse result in every parse result sequence.  This new implementation changes that behavior slightly 
           * by not creating a new branch for the last result in each sequence.  All parser rules (at the time of writing) in Rxx 
           * generate zero or one result only; therefore, the current branch can be moved forward and reused by child sequences 
           * without affecting the parent result sequence, because it's already completed.  This optimization has proven to be 
           * slightly beneficial across normal parser queries that use And, All and Exactly operators.  It should also be beneficial 
           * for multi-result queries since most of the individual parser rules in these queries will only generate a single result.
           * A new branch would only be created for each result in the multi-result sequence.  Scalar-result parsers that follow 
           * sequentially in the All query would simply move their shared parent branch instead of creating new branches.
           */
          var root = source.Branch();
          var rootDisposable = new RefCountDisposable(root);

          return ObservableParseResult.ReturnSuccessMany<TResult>(0)
            .SelectMany(
              Tuple.Create(root, rootDisposable),
              parsers.Select(
                parser => (Func<Tuple<IObservableCursor<TSource>, RefCountDisposable>, Tuple<IParseResult<IObservable<TResult>>, bool>, Tuple<Tuple<IObservableCursor<TSource>, RefCountDisposable>, IObservable<IParseResult<IObservable<TResult>>>>>)
                  ((context, value) =>
                  {
                    if (first)
                    {
                      first = false;
                      firstParser = parser;
                    }

                    var branch = context.Item1;
                    var disposable = context.Item2;
                    var refDisposable = disposable.GetDisposable();

                    IObservable<IParseResult<IObservable<TResult>>> results;

                    // Item2 is only true when value.Item1 is the last element of its sequence.
                    if (value.Item2)
                    {
                      branch.Move(value.Item1.Length);

                      results = parser.Parse(branch)
                                      .Select(result => result.YieldMany())
                                      .Finally(refDisposable.Dispose);
                    }
                    else
                    {
                      branch = branch.Remainder(value.Item1.Length);

                      disposable = new RefCountDisposable(new CompositeDisposable(branch, refDisposable));

                      results = parser.Parse(branch)
                                      .Select(result => result.YieldMany())
                                      .Finally(disposable.Dispose);
                    }

                    return Tuple.Create(Tuple.Create(branch, disposable), results);
                  })),
                (firstResult, otherResults) => firstResult.Concat(otherResults))
            .Finally(rootDisposable.Dispose);
        });
    }
    #endregion
  }
}