using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Time-shifts each value in the specified observable sequence to a sequence returned by the specified selector
    /// and concatenates the timer sequences returned by the selector.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TTimer">Type of the timer notifications.</typeparam>
    /// <param name="source">The observable sequence to be time-shifted.</param>
    /// <param name="timeSelector">Selects an observable sequence that indicates when the current value should be pushed.</param>
    /// <returns>The specified observable sequence time-shifted to the concatenated timer sequences returned by the selector.</returns>
    public static IObservable<TSource> TimeShift<TSource, TTimer>(
      this IObservable<TSource> source,
      Func<TSource, IObservable<TTimer>> timeSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(timeSelector != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return source.Publish(published => published.TimeShift(published.Select(timeSelector).Concat()));
    }

    /// <summary>
    /// Time-shifts each value in the specified observable sequence to the specified <paramref name="timer"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TTimer">Type of the timer notifications.</typeparam>
    /// <param name="source">The observable sequence to be time-shifted.</param>
    /// <param name="timer">The observable sequence to which the values from the source sequence are time-shifted.</param>
    /// <returns>The specified observable sequence time-shifted to the specified <paramref name="timer"/> sequence.</returns>
    public static IObservable<TSource> TimeShift<TSource, TTimer>(
      this IObservable<TSource> source,
      IObservable<TTimer> timer)
    {
      Contract.Requires(source != null);
      Contract.Requires(timer != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return Observable.Create<TSource>(
        observer =>
        {
          var values = new Queue<TSource>();
          var last = System.Maybe.Empty<TSource>();

          object gate = new object();
          bool sourceCompleted = false;

          var sourceSubscription = source.SubscribeSafe(
            value =>
            {
              lock (gate)
              {
                values.Enqueue(value);
              }
            },
            observer.OnError,
            () =>
            {
              bool completeNow = false;

              lock (gate)
              {
                sourceCompleted = true;

                if (values.Count == 0)
                {
                  completeNow = true;
                }
              }

              if (completeNow)
              {
                observer.OnCompleted();
              }
            });

          var timerSubscription = timer.SubscribeSafe(
            _ =>
            {
              bool completeNow = false;
              bool hasValue = false;

              var next = default(TSource);

              lock (gate)
              {
                if (values.Count > 0)
                {
                  next = values.Dequeue();

                  hasValue = true;
                }

                completeNow = sourceCompleted && values.Count == 0;
              }

              if (hasValue)
              {
                last = System.Maybe.Return(next);

                observer.OnNext(next);
              }
              else if (!completeNow && last.HasValue)
              {
                observer.OnNext(last.Value);
              }

              if (completeNow)
              {
                observer.OnCompleted();
              }
            },
            observer.OnError,
            observer.OnCompleted);

          return new CompositeDisposable(sourceSubscription, timerSubscription);
        });
    }
  }
}
