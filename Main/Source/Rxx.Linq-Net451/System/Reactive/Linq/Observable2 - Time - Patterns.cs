using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Returns the elements of the specified sequence time-shifted to the specified minimum period
    /// between elements, starting with the specified minimum period.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be time-shifted.</param>
    /// <param name="minimumPeriod">The minimum amount of time to delay before the first element 
    /// and between elements.</param>
    /// <returns>The specified observable sequence time-shifted to the specified minimum period
    /// between elements, starting with the specified minimum period.</returns>
    public static IObservable<TSource> AsInterval<TSource>(
      this IObservable<TSource> source,
      TimeSpan minimumPeriod)
    {
      Contract.Requires(source != null);
      Contract.Requires(minimumPeriod >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return AsInterval(source, minimumPeriod, PlatformSchedulers.Concurrent);
    }

    /// <summary>
    /// Returns the elements of the specified sequence time-shifted to the specified minimum period
    /// between elements, starting with the specified minimum period.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be time-shifted.</param>
    /// <param name="minimumPeriod">The minimum amount of time to delay before the first element 
    /// and between elements.</param>
    /// <param name="scheduler">An object used to schedule notifications.</param>
    /// <returns>The specified observable sequence time-shifted to the specified minimum period
    /// between elements, starting with the specified minimum period.</returns>
    public static IObservable<TSource> AsInterval<TSource>(
      this IObservable<TSource> source,
      TimeSpan minimumPeriod,
      IScheduler scheduler)
    {
      Contract.Requires(source != null);
      Contract.Requires(minimumPeriod >= TimeSpan.Zero);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return Observable.Defer(() =>
      {
        bool first = true;
        var now = default(DateTimeOffset);
        var total = TimeSpan.Zero;

        return source.TimeShift(
          _ =>
          {
            var previous = now;
            now = scheduler.Now;

            if (first)
            {
              first = false;
              total += minimumPeriod;
            }
            else
            {
              total += minimumPeriod - (now - previous);

              if (total < TimeSpan.Zero)
              /* Delays greater than the minimum period must restart the 
               * timer to maintain the minimum period; otherwise, the next
               * value will be pushed at the originally calculated offset
               * causing these two consecutive values to be observed in 
               * less time than the minimum period.
               */
              {
                total = TimeSpan.Zero;
              }
            }

            return Observable.Timer(now + total, scheduler);
          });
      });
    }

    /// <summary>
    /// Returns the elements of the specified sequence time-shifted to the specified minimum period
    /// between elements.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be time-shifted.</param>
    /// <param name="minimumPeriod">The minimum amount of time to delay between elements.</param>
    /// <returns>The specified observable sequence time-shifted to the specified minimum period
    /// between elements.</returns>
    public static IObservable<TSource> AsTimer<TSource>(
      this IObservable<TSource> source,
      TimeSpan minimumPeriod)
    {
      Contract.Requires(source != null);
      Contract.Requires(minimumPeriod >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return AsTimer(source, minimumPeriod, PlatformSchedulers.Concurrent);
    }

    /// <summary>
    /// Returns the elements of the specified sequence time-shifted to the specified minimum period
    /// between elements.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be time-shifted.</param>
    /// <param name="minimumPeriod">The minimum amount of time to delay between elements.</param>
    /// <param name="scheduler">An object used to schedule notifications.</param>
    /// <returns>The specified observable sequence time-shifted to the specified minimum period
    /// between elements.</returns>
    public static IObservable<TSource> AsTimer<TSource>(
      this IObservable<TSource> source,
      TimeSpan minimumPeriod,
      IScheduler scheduler)
    {
      Contract.Requires(source != null);
      Contract.Requires(minimumPeriod >= TimeSpan.Zero);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return Observable.Defer(() =>
      {
        bool first = true;
        var now = default(DateTimeOffset);
        var total = TimeSpan.Zero;

        return source.TimeShift(
          _ =>
          {
            var previous = now;
            now = scheduler.Now;

            if (first)
            {
              first = false;
            }
            else
            {
              total += minimumPeriod - (now - previous);

              if (total < TimeSpan.Zero)
              /* Delays greater than the minimum period must restart the 
               * timer to maintain the minimum period; otherwise, the next
               * value will be pushed at the originally calculated offset
               * causing these two consecutive values to be observed in 
               * less time than the minimum period.
               */
              {
                total = TimeSpan.Zero;
              }
            }

            return Observable.Timer(now + total, scheduler);
          });
      });
    }

    /// <summary>
    /// Returns the elements of the specified sequence time-shifted to the specified <paramref name="period"/>, 
    /// with the latest value repeated when the specified sequence is silent for longer than the specified 
    /// <paramref name="period"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Pulse{TSource}(IObservable{TSource},TimeSpan)"/> is similar to <see cref="AsTimer{TSource}(IObservable{TSource},TimeSpan)"/>
    /// in that it begins without delaying as soon as the first value is observed; however, it differs in that the 
    /// <paramref name="period"/> between notifications is constant and the last value is repeated at the specified interval 
    /// while the observable is silent.
    /// </para>
    /// <alert type="tip">
    /// To achieve a pulse without duplicate values, pass the returned observable to Observable.DistinctUntilChanged.
    /// </alert>
    /// </remarks>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be time-shifted.</param>
    /// <param name="period">The exact amount of time to delay between elements.</param>
    /// <returns>The specified observable sequence time-shifted to the specified minimum period
    /// between elements, starting with the specified minimum period.</returns>
    public static IObservable<TSource> Pulse<TSource>(
      this IObservable<TSource> source,
      TimeSpan period)
    {
      Contract.Requires(source != null);
      Contract.Requires(period >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return Pulse(source, period, PlatformSchedulers.Concurrent);
    }

    /// <summary>
    /// Returns the elements of the specified sequence time-shifted to the specified <paramref name="period"/>, 
    /// with the latest value repeated when the specified sequence is silent for longer than the specified 
    /// <paramref name="period"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Pulse{TSource}(IObservable{TSource},TimeSpan)"/> is similar to <see cref="AsTimer{TSource}(IObservable{TSource},TimeSpan)"/>
    /// in that it begins without delaying as soon as the first value is observed; however, it differs in that the 
    /// <paramref name="period"/> between notifications is constant and the last value is repeated at the specified interval 
    /// while the observable is silent.
    /// </para>
    /// <alert type="tip">
    /// To achieve a pulse without duplicate values, pass the returned observable to Observable.DistinctUntilChanged.
    /// </alert>
    /// </remarks>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be time-shifted.</param>
    /// <param name="period">The exact amount of time to delay between elements.</param>
    /// <param name="scheduler">An object used to schedule notifications.</param>
    /// <returns>The specified observable sequence time-shifted to the specified minimum period
    /// between elements, starting with the specified minimum period.</returns>
    public static IObservable<TSource> Pulse<TSource>(
      this IObservable<TSource> source,
      TimeSpan period,
      IScheduler scheduler)
    {
      Contract.Requires(source != null);
      Contract.Requires(period >= TimeSpan.Zero);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return source.TimeShift(Observable.Timer(TimeSpan.Zero, period, scheduler));
    }
  }
}