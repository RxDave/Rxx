using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Generates a sequence using the producer/consumer pattern by iteratively calling a consumer function when data is available.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the data in the generated sequence.</typeparam>
    /// <param name="source">Indicates when data becomes available from one or more producers.</param>
    /// <param name="consumeNext">A function that is called iteratively to generate values from out-of-band data.</param>
    /// <remarks>
    /// <para>
    /// The purpose of the <paramref name="source"/> sequence is simply to notify the consumer when data becomes available in
    /// out-of-band storage.  The data in the <paramref name="source"/> sequence provides additional information to the consumer, 
    /// but it does not have to be the actual data produced for the consumer.
    /// </para>
    /// <para>
    /// The <paramref name="consumeNext"/> function is called iteratively when data becomes available, until it returns 
    /// <see cref="System.Maybe.Empty{TResult}()"/>.  All non-empty values that are returned are concatenated into the generated sequence.
    /// </para>
    /// <para>
    /// <see cref="Consume{TSource,TResult}(IObservable{TSource},Func{TSource,Maybe{TResult}})"/> ensures that there 
    /// is always one active iterator over the <paramref name="consumeNext"/> function when the <paramref name="source"/> sequence notifies
    /// that data is available; however, it's not necessarily called for every value in the <paramref name="source"/> sequence.
    /// It is only called if the previous iteration has completed; otherwise, the current notification is ignored.  This ensures that
    /// only one consumer is active at any given time, but it also means that the <paramref name="consumeNext"/> function is not guaranteed
    /// to receive every value in the <paramref name="source"/> sequence; therefore, the <paramref name="consumeNext"/> function must read
    /// data from out-of-band storage instead; e.g., from a shared stream or queue.
    /// </para>
    /// <para>
    /// The <paramref name="consumeNext"/> function may also be called when data is not available.  For example, if the current consuming 
    /// iterator completes and additional notifications from the <paramref name="source"/> were received, then <paramref name="consumeNext"/>
    /// is called again to check whether new data was missed.  This avoids a race condition between the <paramref name="source"/> sequence 
    /// and <paramref name="consumeNext"/> reading shared data.  If no data is available when <paramref name="consumeNext"/> is called, then 
    /// <see cref="System.Maybe.Empty{TResult}()"/> should be returned and <paramref name="consumeNext"/> will not be called again until another 
    /// notification is observed from the <paramref name="source"/>.
    /// </para>
    /// <alert type="tip">
    /// Producers and the single active consumer are intended to access shared objects concurrently, yet it remains their responsibility
    /// to ensure thread-safety.  The <strong>Consume</strong> operator cannot do so without breaking concurrency.  For example, a producer/consumer
    /// implementation that uses an in-memory queue must manually ensure that reads and writes to the queue are thread-safe.
    /// </alert>
    /// <para>
    /// Multiple producers are supported.  Simply create an observable sequence for each producer that notifies when data is generated, 
    /// merge them together using the <see cref="Observable.Merge{TSource}(IObservable{IObservable{TSource}})"/> operator, and use the merged
    /// observable as the <paramref name="source"/> argument in the <strong>Consume</strong> operator.
    /// </para>
    /// <para>
    /// Multiple consumers are supported by calling <strong>Consume</strong> once and then calling <see cref="IObservable{TSource}.Subscribe"/>
    /// multiple times on the <em>cold</em> observable that is returned.  Just be sure that the <paramref name="source"/> sequence is 
    /// <em>hot</em> so that each subscription will consume based on the same producers' notifications.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence that is the concatenation of the values returned by the <paramref name="consumeNext"/> function.</returns>
    public static IObservable<TResult> Consume<TSource, TResult>(
      this IObservable<TSource> source,
      Func<TSource, Maybe<TResult>> consumeNext)
    {
      Contract.Requires(source != null);
      Contract.Requires(consumeNext != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      return source.Consume(consumeNext, PlatformSchedulers.Concurrent);
    }

    /// <summary>
    /// Generates a sequence using the producer/consumer pattern by iteratively calling a consumer function on the specified 
    /// <paramref name="scheduler"/> when data is available.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the data in the generated sequence.</typeparam>
    /// <param name="source">Indicates when data becomes available from one or more producers.</param>
    /// <param name="consumeNext">A function that is called iteratively to generate values from out-of-band data.</param>
    /// <param name="scheduler">An object that schedules iteration over the <paramref name="consumeNext"/> function.</param>
    /// <remarks>
    /// <para>
    /// The purpose of the <paramref name="source"/> sequence is simply to notify the consumer when data becomes available in
    /// out-of-band storage.  The data in the <paramref name="source"/> sequence provides additional information to the consumer, 
    /// but it does not have to be the actual data produced for the consumer.
    /// </para>
    /// <para>
    /// The <paramref name="consumeNext"/> function is called iteratively when data becomes available, until it returns 
    /// <see cref="System.Maybe.Empty{TResult}()"/>.  All non-empty values that are returned are concatenated into the generated sequence.
    /// </para>
    /// <para>
    /// <see cref="Consume{TSource,TResult}(IObservable{TSource},Func{TSource,Maybe{TResult}},IScheduler)"/> ensures that there 
    /// is always one active iterator over the <paramref name="consumeNext"/> function when the <paramref name="source"/> sequence notifies
    /// that data is available; however, it's not necessarily called for every value in the <paramref name="source"/> sequence.
    /// It is only called if the previous iteration has completed; otherwise, the current notification is ignored.  This ensures that
    /// only one consumer is active at any given time, but it also means that the <paramref name="consumeNext"/> function is not guaranteed
    /// to receive every value in the <paramref name="source"/> sequence; therefore, the <paramref name="consumeNext"/> function must read
    /// data from out-of-band storage instead; e.g., from a shared stream or queue.
    /// </para>
    /// <para>
    /// The <paramref name="consumeNext"/> function may also be called when data is not available.  For example, if the current consuming 
    /// iterator completes and additional notifications from the <paramref name="source"/> were received, then <paramref name="consumeNext"/>
    /// is called again to check whether new data was missed.  This avoids a race condition between the <paramref name="source"/> sequence 
    /// and <paramref name="consumeNext"/> reading shared data.  If no data is available when <paramref name="consumeNext"/> is called, then 
    /// <see cref="System.Maybe.Empty{TResult}()"/> should be returned and <paramref name="consumeNext"/> will not be called again until another 
    /// notification is observed from the <paramref name="source"/>.
    /// </para>
    /// <alert type="tip">
    /// Producers and the single active consumer are intended to access shared objects concurrently, yet it remains their responsibility
    /// to ensure thread-safety.  The <strong>Consume</strong> operator cannot do so without breaking concurrency.  For example, a producer/consumer
    /// implementation that uses an in-memory queue must manually ensure that reads and writes to the queue are thread-safe.
    /// </alert>
    /// <para>
    /// Multiple producers are supported.  Simply create an observable sequence for each producer that notifies when data is generated, 
    /// merge them together using the <see cref="Observable.Merge{TSource}(IObservable{IObservable{TSource}})"/> operator, and use the merged
    /// observable as the <paramref name="source"/> argument in the <strong>Consume</strong> operator.
    /// </para>
    /// <para>
    /// Multiple consumers are supported by calling <strong>Consume</strong> once and then calling <see cref="IObservable{TSource}.Subscribe"/>
    /// multiple times on the <em>cold</em> observable that is returned.  Just be sure that the <paramref name="source"/> sequence is 
    /// <em>hot</em> so that each subscription will consume based on the same producers' notifications.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence that is the concatenation of the values returned by the <paramref name="consumeNext"/> function.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "All exceptions are sent to observers.")]
    public static IObservable<TResult> Consume<TSource, TResult>(
      this IObservable<TSource> source,
      Func<TSource, Maybe<TResult>> consumeNext,
      IScheduler scheduler)
    {
      Contract.Requires(source != null);
      Contract.Requires(consumeNext != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      return source.Consume(
        value => Observable.Create<TResult>(
          observer => scheduler.Schedule(() =>
          {
            Maybe<TResult> result;

            do
            {
              try
              {
                result = consumeNext(value);
              }
              catch (Exception ex)
              {
                observer.OnError(ex);
                return;
              }

              if (result.HasValue)
              {
                observer.OnNext(result.Value);
              }
            }
            while (result.HasValue);

            observer.OnCompleted();
          })));
    }

    /// <summary>
    /// Generates a sequence using the producer/consumer pattern.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the data in the generated sequence.</typeparam>
    /// <param name="source">Indicates when data becomes available from one or more producers.</param>
    /// <param name="consumer">An observable that is generated from out-of-band data.</param>
    /// <remarks>
    /// <para>
    /// The purpose of the <paramref name="source"/> sequence is simply to notify when data becomes available in out-of-band storage.
    /// The data in the <paramref name="source"/> sequence is ignored.
    /// </para>
    /// <para>
    /// <see cref="Consume{TSource,TResult}(IObservable{TSource},IObservable{TResult})"/> ensures that there is always an active 
    /// subscription to the <paramref name="consumer"/> sequence when the <paramref name="source"/> sequence notifies that data is
    /// available; however, a new subscription is not necessarily created for every value in the <paramref name="source"/> sequence.
    /// <see cref="IObservable{TResult}.Subscribe"/> is only called if the previous subscription has completed; otherwise, the current 
    /// notification is ignored.  This ensures that only one consumer is active at any given time and that all available data has a 
    /// chance to be read.
    /// </para>
    /// <para>
    /// A new subscription may also be created when data is not available.  For example, if the current subscription completes and 
    /// additional notifications from the <paramref name="source"/> were received, then another subscription is created to check whether
    /// new data was missed.  This avoids a race condition between the <paramref name="source"/> sequence and the consumer's completion 
    /// notification.  If no data is available for the new subscription, then an empty sequence should be generated and a new subscription
    /// will not be created again until another notification is observed from the <paramref name="source"/>.
    /// </para>
    /// <alert type="tip">
    /// Producers and the single active consumer are intended to access shared objects concurrently, yet it remains their responsibility
    /// to ensure thread-safety.  The <strong>Consume</strong> operator cannot do so without breaking concurrency.  For example, a producer/consumer
    /// implementation that uses an in-memory queue must manually ensure that reads and writes to the queue are thread-safe.
    /// </alert>
    /// <para>
    /// Multiple producers are supported.  Simply create an observable sequence for each producer that notifies when data is generated, 
    /// merge them together using the <see cref="Observable.Merge{TSource}(IObservable{IObservable{TSource}})"/> operator, and use the merged
    /// observable as the <paramref name="source"/> argument in the <strong>Consume</strong> operator.
    /// </para>
    /// <para>
    /// Multiple consumers are supported by calling <strong>Consume</strong> once and then calling <see cref="IObservable{TSource}.Subscribe"/>
    /// multiple times on the <em>cold</em> observable that is returned.  Just be sure that the <paramref name="source"/> sequence is 
    /// <em>hot</em> so that each subscription will consume based on the same producers' notifications.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence that is the concatenation of all subscriptions to the <paramref name="consumer"/> observable.</returns>
    public static IObservable<TResult> Consume<TSource, TResult>(
      this IObservable<TSource> source,
      IObservable<TResult> consumer)
    {
      Contract.Requires(source != null);
      Contract.Requires(consumer != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      return source.Consume(_ => consumer);
    }

    /// <summary>
    /// Generates a sequence using the producer/consumer pattern.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the data in the generated sequence.</typeparam>
    /// <param name="source">Indicates when data becomes available from one or more producers.</param>
    /// <param name="consumerSelector">A function that generates an observable sequence from out-of-band data.</param>
    /// <remarks>
    /// <para>
    /// The purpose of the <paramref name="source"/> sequence is simply to notify the consumer when out-of-band data becomes available.
    /// The data in the <paramref name="source"/> sequence provides additional information to the <paramref name="consumerSelector"/> function, 
    /// but it does not have to be the actual data being produced.
    /// </para>
    /// <para>
    /// The <paramref name="consumerSelector"/> function is not necessarily called for every value in the <paramref name="source"/> sequence.
    /// It is only called if the previous consumer's observable has completed; otherwise, the current notification is ignored.  This ensures 
    /// that only one consumer is active at any given time, but it also means that the <paramref name="consumerSelector"/> function is not guaranteed
    /// to receive every value in the <paramref name="source"/> sequence; therefore, the <paramref name="consumerSelector"/> function must read
    /// data from out-of-band storage instead; e.g., from a shared stream or queue.
    /// </para>
    /// <para>
    /// The <paramref name="consumerSelector"/> function may also be called when data is not available.  For example, if the current consuming 
    /// observable completes and additional notifications from the <paramref name="source"/> were received, then <paramref name="consumerSelector"/>
    /// is called again to check whether new data was missed.  This avoids a race condition between the <paramref name="source"/> sequence 
    /// and the consuming observable's completion notification.  If no data is available when <paramref name="consumerSelector"/> is called, then 
    /// an empty sequence should be returned and <paramref name="consumerSelector"/> will not be called again until another notification is observed 
    /// from the <paramref name="source"/>.
    /// </para>
    /// <alert type="tip">
    /// Producers and the single active consumer are intended to access shared objects concurrently, yet it remains their responsibility
    /// to ensure thread-safety.  The <strong>Consume</strong> operator cannot do so without breaking concurrency.  For example, 
    /// a producer/consumer implementation that uses an in-memory queue must manually ensure that reads and writes to the queue are thread-safe.
    /// </alert>
    /// <para>
    /// Multiple producers are supported.  Simply create an observable sequence for each producer that notifies when data is generated, 
    /// merge them together using the <see cref="Observable.Merge{TSource}(IObservable{IObservable{TSource}})"/> operator, and use the merged
    /// observable as the <paramref name="source"/> argument in the <strong>Consume</strong> operator.
    /// </para>
    /// <para>
    /// Multiple consumers are supported by calling <strong>Consume</strong> once and then calling <see cref="IObservable{TSource}.Subscribe"/>
    /// multiple times on the <em>cold</em> observable that is returned.  Just be sure that the <paramref name="source"/> sequence is 
    /// <em>hot</em> so that each subscription will consume based on the same producers' notifications.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence that is the concatenation of the sequences returned by <paramref name="consumerSelector"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "All exceptions are sent to observers.")]
    public static IObservable<TResult> Consume<TSource, TResult>(
      this IObservable<TSource> source,
      Func<TSource, IObservable<TResult>> consumerSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(consumerSelector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      return Observable.Create<TResult>(
        observer =>
        {
          object gate = new object();
          var consumingSubscription = new SerialDisposable();
          var schedule = new SerialDisposable();

          TSource lastSkippedNotification = default(TSource);
          bool hasSkippedNotification = false;
          bool consuming = false;
          bool stopped = false;

          var subscription = source.SubscribeSafe(
            value =>
            {
              lock (gate)
              {
                if (consuming)
                {
                  lastSkippedNotification = value;
                  hasSkippedNotification = true;
                  return;
                }
                else
                {
                  consuming = true;
                  hasSkippedNotification = false;
                }
              }

              var additionalData = value;

              schedule.Disposable = Scheduler.Immediate.Schedule(
                self =>
                {
                  IObservable<TResult> observable;

                  try
                  {
                    observable = consumerSelector(additionalData);
                  }
                  catch (Exception ex)
                  {
                    observer.OnError(ex);
                    return;
                  }

                  consumingSubscription.SetDisposableIndirectly(() =>
                    observable.SubscribeSafe(
                      observer.OnNext,
                      observer.OnError,
                      () =>
                      {
                        bool consumeAgain = false;
                        bool completeNow = false;

                        lock (gate)
                        {
                          /* The hasSkippedNotification field avoids a race condition between source notifications and the consuming observable 
                           * calling OnCompleted that could cause data to become available without any active consumer.  The solution is to 
                           * check whether additional notifications were skipped before the consuming observable completed.  If so, then we 
                           * try to consume once more; and if there isn't any data available, because it was already consumed before the previous 
                           * observable completed, then the new observable will be empty.  If no additional notifications are received 
                           * from the source before the new observable completes, then hasSkippedNotification will be false the second time 
                           * around and there will be no active consumer until the next source notification.
                           * 
                           * This behavior reactively mimics typical interactive consumer behavior; e.g., lock and loop if queue.Count > 0.
                           */
                          consuming = hasSkippedNotification;

                          if (consuming)
                          {
                            additionalData = lastSkippedNotification;
                            hasSkippedNotification = false;
                            consumeAgain = true;
                          }
                          else
                          {
                            completeNow = stopped;

                            Contract.Assert(!hasSkippedNotification);
                          }

                          Contract.Assert(!(consumeAgain && completeNow));
                        }

                        if (consumeAgain)
                        {
                          self();
                        }
                        else if (completeNow)
                        {
                          observer.OnCompleted();
                        }
                      }));
                });
            },
            observer.OnError,
            () =>
            {
              bool completeNow;

              lock (gate)
              {
                stopped = true;

                completeNow = !consuming;
              }

              if (completeNow)
              {
                observer.OnCompleted();
              }
            });

          return new CompositeDisposable(consumingSubscription, subscription, schedule);
        });
    }
  }
}