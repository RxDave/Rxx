using System;
using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Represents a parser that begins a parse operation at the beginning of the source sequence.
  /// </summary>
  /// <typeparam name="TSource">The type of the source elements.</typeparam>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
  internal sealed class ObservableParserStart<TSource, TResult> : ObservableParserStartBase<TSource, TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private readonly Func<IObservableParser<TSource, TSource>, IObservableParser<TSource, TResult>> grammar;
    #endregion

    #region Constructors
    public ObservableParserStart(
      Func<IObservableParser<TSource, TSource>, IObservableParser<TSource, TResult>> grammar)
    {
      Contract.Requires(grammar != null);

      this.grammar = grammar;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(grammar != null);
    }

#if !SILVERLIGHT && !PORT_45 && !PORT_40
    protected override IObservable<IParseResult<TResult>> Parse(ParserTraceExecutionContext traceContext)
#else
    protected override IObservable<IParseResult<TResult>> Parse()
#endif
    {
      return Observable.Create<IParseResult<TResult>>(
        observer =>
        {
          var cursor = Cursor;

          IObservableParser<TSource, TResult> compiledGrammar;

#if !SILVERLIGHT && !PORT_45 && !PORT_40
          using (ParserTraceSources.TraceGrammarCompilation())
          {
            compiledGrammar = grammar(cursor);
          }
#else
          compiledGrammar = grammar(cursor);
#endif

          var subscription = new SerialDisposable();

          /* This scheduler is only used for recursion.  It must be Immediate.
           * 
           * In testing, using a scheduler such as CurrentThread would cause the
           * ambiguous lab to miss values due to the order in which the ambiguous 
           * parser enqueues the subscription to the underyling source and the
           * memoizing cursor, in relation to this parser's scheduling of recursion.
           */
          var schedule = Scheduler.Immediate.Schedule(
            self =>
            {
#if !SILVERLIGHT && !PORT_45 && !PORT_40
              traceContext.TraceBeginIteration();
#endif

              bool hasResult = false;

              subscription.SetDisposableIndirectly(
                () => compiledGrammar.Parse(cursor).SubscribeSafe(
                  result =>
                  {
                    var lookAhead = result as ILookAheadParseResult<TResult>;

                    if (lookAhead != null)
                    {
                      lookAhead.OnCompleted(success: true);
                      return;
                    }

                    hasResult = true;

#if !SILVERLIGHT && !PORT_45 && !PORT_40
                    traceContext.TraceResult(result);
#endif

                    observer.OnNext(result);

                    if (!cursor.AtEndOfSequence)
                    {
                      cursor.Move(result.Length);
                    }
                  },
                  observer.OnError,
                  () =>
                  {
                    if (!hasResult)
                    {
                      cursor.MoveToEnd();
                    }

                    if (cursor.AtEndOfSequence)
                    {
                      observer.OnCompleted();
                    }
                    else
                    {
                      self();
                    }
                  }));
            });

          return new CompositeDisposable(schedule, subscription);
        });
    }
    #endregion
  }
}