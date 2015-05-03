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
  internal sealed class AllManyUnorderedObservableParser<TSource, TResult> : IObservableParser<TSource, IObservable<TResult>>
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

    public IEnumerable<IObservableParser<TSource, IObservable<TResult>>> Parsers
    {
      get
      {
        Contract.Ensures(Contract.Result<IEnumerable<IObservableParser<TSource, IObservable<TResult>>>>() != null);

        return parsers;
      }
    }
    #endregion

    #region Private / Protected
    private readonly IEnumerable<IObservableParser<TSource, IObservable<TResult>>> parsers;
    private IObservableParser<TSource, IObservable<TResult>> firstParser;
    #endregion

    #region Constructors
    public AllManyUnorderedObservableParser(IEnumerable<IObservableParser<TSource, IObservable<TResult>>> parsers)
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
          var any = new AnyObservableParser<TSource, IObservable<TResult>>(parsers);
          var except = new List<IObservableParser<TSource, IObservable<TResult>>>();

          // See the AllObservableParser.Parse method for an explanation of the optimization that is provided by SelectMany's TContext argument
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

                      results = any.Parse(except, branch).Finally(refDisposable.Dispose);
                    }
                    else
                    {
                      branch = branch.Remainder(value.Item1.Length);

                      disposable = new RefCountDisposable(new CompositeDisposable(branch, refDisposable));

                      results = any.Parse(except, branch).Finally(disposable.Dispose);
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