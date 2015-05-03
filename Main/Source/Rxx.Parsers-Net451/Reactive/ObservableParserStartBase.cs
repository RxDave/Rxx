using System;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Rxx.Parsers.Properties;

namespace Rxx.Parsers.Reactive
{
  [ContractClass(typeof(ObservableParserStartBaseContract<,>))]
  internal abstract class ObservableParserStartBase<TSource, TResult> : IObservableParser<TSource, TResult>
  {
    #region Public Properties
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "DeferredObservableParserCursor must not be disposed since it's the return value.")]
    public IObservableParser<TSource, TSource> Next
    {
      get
      {
        if (cursor == null)
        {
          return new DeferredObservableParserCursor<TSource>(() =>
            {
              if (cursor == null)
              {
                throw new InvalidOperationException(Errors.ParserNextNotReady);
              }

              return cursor;
            });
        }
        else
        {
          return cursor;
        }
      }
    }
    #endregion

    #region Private / Protected
    protected virtual string TraceName
    {
      get
      {
        return null;
      }
    }

    protected ObservableParserCursor<TSource> Cursor
    {
      get
      {
        Contract.Ensures(Contract.Result<ObservableParserCursor<TSource>>() == cursor);

        return cursor;
      }
    }

    private ObservableParserCursor<TSource> cursor;
    private int parsing;
    #endregion

    #region Constructors
    #endregion

    #region Methods
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "This is a factory method.")]
    protected virtual ObservableParserCursor<TSource> CreateCursor(IObservableCursor<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Ensures(Contract.Result<ObservableParserCursor<TSource>>() != null);

      return new ObservableParserCursor<TSource>(source);
    }

#if !SILVERLIGHT && !PORT_45 && !PORT_40
    protected abstract IObservable<IParseResult<TResult>> Parse(ParserTraceExecutionContext traceContext);
#else
    protected abstract IObservable<IParseResult<TResult>> Parse();
#endif

    public IObservable<IParseResult<TResult>> Parse(IObservableCursor<TSource> source)
    {
      if (Interlocked.Exchange(ref parsing, 1) == 1)
      {
        throw new InvalidOperationException(Errors.ParseCalledWhileParsing);
      }

      if (cursor != null)
      {
        cursor.Dispose();
      }

      cursor = CreateCursor(source);

      object gate = new object();
      bool allowSubscription = true;

      return Observable.Create<IParseResult<TResult>>(
        observer =>
        {
          lock (gate)
          {
            if (!allowSubscription)
            {
              throw new InvalidOperationException(Errors.ParseSubscribeCalledWhileParsing);
            }

            allowSubscription = false;
          }

#if !SILVERLIGHT && !PORT_45 && !PORT_40
          var parse = Observable.Using(() => ParserTraceSources.TraceExecution(Cursor, TraceName), Parse);
#else
          var parse = Parse();
#endif

          var parseSubscription = parse.SubscribeSafe(observer);
          var sourceSubscription = cursor.Connect();

          return new CompositeDisposable(sourceSubscription, parseSubscription);
        })
        .Finally(() =>
          {
            lock (gate)
            {
              allowSubscription = true;
            }

            Interlocked.Exchange(ref parsing, 0);
          });
    }

    public override string ToString()
    {
      return (TraceName == null ? string.Empty : "(" + TraceName + ") ")
           + (cursor == null ? "?" : cursor.ToString());
    }
    #endregion
  }

  [ContractClassFor(typeof(ObservableParserStartBase<,>))]
  internal abstract class ObservableParserStartBaseContract<TSource, TResult> : ObservableParserStartBase<TSource, TResult>
  {
#if !SILVERLIGHT && !PORT_45 && !PORT_40
    protected override IObservable<IParseResult<TResult>> Parse(ParserTraceExecutionContext traceContext)
#else
    protected override IObservable<IParseResult<TResult>> Parse()
#endif
    {
      Contract.Requires(Cursor != null);
#if !SILVERLIGHT && !PORT_45 && !PORT_40
      Contract.Requires(traceContext != null);
#endif
      Contract.Ensures(Contract.Result<IObservable<IParseResult<TResult>>>() != null);
      return null;
    }
  }
}