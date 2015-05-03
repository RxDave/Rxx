using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Rxx.Parsers.Properties;

namespace Rxx.Parsers.Reactive
{
  internal sealed class AnyObservableParser<TSource, TResult> : IObservableParser<TSource, TResult>
  {
    #region Public Properties
    public IObservableParser<TSource, TSource> Next
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
    private IObservableParser<TSource, TResult> selectedParser;
    #endregion

    #region Constructors
    public AnyObservableParser(IEnumerable<IObservableParser<TSource, TResult>> parsers)
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

    public IObservable<IParseResult<TResult>> Parse(IObservableCursor<TSource> source)
    {
      return Observable.Defer(() =>
        {
          var completed = new AsyncSubject<Unit>();

          bool hasResult = false;

          return parsers.Select(
            parser => Observable.Create<IParseResult<TResult>>(
              observer =>
              {
                return parser.Parse(source).SubscribeSafe(
                  result =>
                  {
                    if (!hasResult)
                    {
                      hasResult = true;

                      /* The XML lab has shown that Parse may be called multiple times on the same AnyObservableParser 
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

                    observer.OnNext(result);
                  },
                  observer.OnError,
                  () =>
                  {
                    if (hasResult)
                    {
                      completed.OnNext(new Unit());
                      completed.OnCompleted();
                    }

                    observer.OnCompleted();
                  });
              }))
            .Concat()
            .TakeUntil(completed);
        });
    }

    internal IObservable<IParseResult<TResult>> Parse(
      ICollection<IObservableParser<TSource, TResult>> except,
      IObservableCursor<TSource> source)
    {
      Contract.Requires(except != null);
      Contract.Requires(!except.IsReadOnly);
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<IParseResult<TResult>>>() != null);

      return Observable.Defer(() =>
      {
        var completed = new AsyncSubject<Unit>();

        bool hasResult = false;

        return parsers.Except(except).Select(
          parser => Observable.Create<IParseResult<TResult>>(
            observer =>
            {
              return parser.Parse(source).SubscribeSafe(
                result =>
                {
                  if (!hasResult)
                  {
                    hasResult = true;

                    except.Add(parser);
                  }

                  observer.OnNext(result);
                },
                observer.OnError,
                () =>
                {
                  if (hasResult)
                  {
                    completed.OnNext(new Unit());
                    completed.OnCompleted();
                  }

                  observer.OnCompleted();
                });
            }))
          .Concat()
          .TakeUntil(completed);
      });
    }
    #endregion
  }
}