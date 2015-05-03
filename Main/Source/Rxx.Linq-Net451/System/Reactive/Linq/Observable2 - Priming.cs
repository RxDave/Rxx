using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Connects the specified connectable observable sequence upon the first subscription and never disconnects.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The connectable observable sequence to be connected upon the first subscription.</param>
    /// <remarks>
    /// <para>
    /// It may be easier to think of priming as creating a warm observable, instead of a cold or hot observable.
    /// A cold observable allows subscription side-effects to occur each time, whereas a hot observable does not.
    /// A hot observable is already active before the first subscription, whereas a warm observable is not.
    /// </para>
    /// </remarks>
    /// <returns>The specified observable sequence primed for use.</returns>
    public static IObservable<TSource> Prime<TSource>(this IConnectableObservable<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return Prime(source, _ => { });
    }

    /// <summary>
    /// Connects the specified connectable observable sequence upon the first subscription and provides 
    /// the ability to disconnect to a callback.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The connectable observable sequence to be connected upon the first subscription.</param>
    /// <param name="whenConnected">Receives the subscription each time the observable is connected.</param>
    /// <remarks>
    /// <para>
    /// It may be easier to think of priming as creating a warm observable, instead of a cold or hot observable.
    /// A cold observable allows subscription side-effects to occur each time, whereas a hot observable does not.
    /// A hot observable is already active before the first subscription, whereas a warm observable is not.
    /// </para>
    /// </remarks>
    /// <returns>The specified observable sequence primed for use.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Connection can be disposed by the whenConnected action.")]
    public static IObservable<TSource> Prime<TSource>(
      this IConnectableObservable<TSource> source,
      Action<IDisposable> whenConnected)
    {
      Contract.Requires(source != null);
      Contract.Requires(whenConnected != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      object gate = new object();
      bool isFirst = true;

      return Observable.Create<TSource>(
        observer =>
        {
          var subscription = source.SubscribeSafe(observer);

          lock (gate)
          {
            if (isFirst)
            {
              isFirst = false;

              var connection = source.Connect();

              whenConnected(Disposable.Create(() =>
              {
                lock (gate)
                {
                  if (!isFirst)
                  {
                    connection.Dispose();

                    isFirst = true;
                  }
                }
              }));
            }
          }

          return subscription;
        });
    }

    /// <summary>
    /// Invokes the specified <paramref name="action"/> asynchronously upon the first subscription to the returned observable.
    /// </summary>
    /// <param name="action">The action to be invoked upon the first subscription.</param>
    /// <remarks>
    /// <para>
    /// <see cref="StartPrimed(Action)"/> differs from <see cref="Observable.Start(Action)"/> in that the former
    /// does not call the action until the first subscription, although the observable remains hot for subsequent 
    /// subscriptions.
    /// </para>
    /// <para>
    /// It may be easier to think of priming as creating a warm observable, instead of a cold or hot observable.
    /// A cold observable allows subscription side-effects to occur each time, whereas a hot observable does not.
    /// A hot observable is already active before the first subscription, whereas a warm observable is not.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence primed to call the specified <paramref name="action"/> upon the first subscription.</returns>
    public static IObservable<Unit> StartPrimed(Action action)
    {
      Contract.Requires(action != null);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      return StartPrimed(action, PlatformSchedulers.Concurrent);
    }

    /// <summary>
    /// Invokes the specified <paramref name="action"/> asynchronously upon the first subscription to the returned observable.
    /// </summary>
    /// <param name="action">The action to be invoked upon the first subscription.</param>
    /// <param name="scheduler">Schedules the call to the action.</param>
    /// <remarks>
    /// <para>
    /// <see cref="StartPrimed(Action)"/> differs from <see cref="Observable.Start(Action)"/> in that the former
    /// does not call the action until the first subscription, although the observable remains hot for subsequent 
    /// subscriptions.
    /// </para>
    /// <para>
    /// It may be easier to think of priming as creating a warm observable, instead of a cold or hot observable.
    /// A cold observable allows subscription side-effects to occur each time, whereas a hot observable does not.
    /// A hot observable is already active before the first subscription, whereas a warm observable is not.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence primed to call the specified <paramref name="action"/> upon the first subscription.</returns>
    public static IObservable<Unit> StartPrimed(Action action, IScheduler scheduler)
    {
      Contract.Requires(action != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      int isConnected = 0;
      IObservable<Unit> startObservable = null;

      return Observable.Create<Unit>(
        observer =>
        {
          if (Interlocked.Exchange(ref isConnected, 1) == 0)
          {
            startObservable = Observable.Start(action, scheduler);
          }

          return startObservable.SubscribeSafe(observer);
        });
    }

    /// <summary>
    /// Invokes the specified <paramref name="function"/> asynchronously upon the first subscription to the returned observable.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="function">The function to be invoked upon the first subscription.</param>
    /// <remarks>
    /// <para>
    /// <see cref="StartPrimed{TSource}(Func{TSource})"/> differs from <see cref="Observable.Start{TSource}(Func{TSource})"/> in that the former
    /// does not call the function until the first subscription, although the observable remains hot for subsequent 
    /// subscriptions.
    /// </para>
    /// <para>
    /// It may be easier to think of priming as creating a warm observable, instead of a cold or hot observable.
    /// A cold observable allows subscription side-effects to occur each time, whereas a hot observable does not.
    /// A hot observable is already active before the first subscription, whereas a warm observable is not.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence primed to call the specified <paramref name="function"/> upon the first subscription.</returns>
    public static IObservable<TSource> StartPrimed<TSource>(Func<TSource> function)
    {
      Contract.Requires(function != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return StartPrimed(function, PlatformSchedulers.Concurrent);
    }

    /// <summary>
    /// Invokes the specified <paramref name="function"/> asynchronously upon the first subscription to the returned observable.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="function">The function to be invoked upon the first subscription.</param>
    /// <param name="scheduler">Schedules the call to the function.</param>
    /// <remarks>
    /// <para>
    /// <see cref="StartPrimed{TSource}(Func{TSource})"/> differs from <see cref="Observable.Start{TSource}(Func{TSource})"/> in that the former
    /// does not call the function until the first subscription, although the observable remains hot for subsequent 
    /// subscriptions.
    /// </para>
    /// <para>
    /// It may be easier to think of priming as creating a warm observable, instead of a cold or hot observable.
    /// A cold observable allows subscription side-effects to occur each time, whereas a hot observable does not.
    /// A hot observable is already active before the first subscription, whereas a warm observable is not.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence primed to call the specified <paramref name="function"/> upon the first subscription.</returns>
    public static IObservable<TSource> StartPrimed<TSource>(Func<TSource> function, IScheduler scheduler)
    {
      Contract.Requires(function != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      int isConnected = 0;
      IObservable<TSource> startObservable = null;

      return Observable.Create<TSource>(
        observer =>
        {
          if (Interlocked.Exchange(ref isConnected, 1) == 0)
          {
            startObservable = Observable.Start(function, scheduler);
          }

          return startObservable.SubscribeSafe(observer);
        });
    }
  }
}