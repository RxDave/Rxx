using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Merges two or more observable sequences into one observable sequence of lists by combining their elements in a pairwise fashion.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="sources">An observable sequence containing two or more observable sequences to be merged.</param>
    /// <remarks>
    /// <para>
    /// Unlike the native Rx overloads of <strong>Zip</strong>, the generated sequence may contain singleton lists as soon as the first inner
    /// observable notifies, without waiting for additional inner observables to arrive.
    /// </para>
    /// <para>
    /// All of the current inner observables must produce at least one element; otherwise, the generated sequence will be empty. Furthermore, if an inner observable 
    /// sequence produces more than one element before each of the other inner observable sequences have produced their first elements, then all of the elements 
    /// will be enqueued for future pairing. No elements are discarded.
    /// </para>
    /// <para>
    /// The latest value of an observable sequence is always located in the generated lists at the same index in which that sequence is located in the outer sequence.
    /// For example, the values from the first observable sequence in the outer sequence will always be at index zero (0) in the lists that are generated.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence containing the result of pairwise combining the elements of all sources into lists.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is composited by the subscription.")]
    public static IObservable<IList<TSource>> Zip<TSource>(this IObservable<IObservable<TSource>> sources)
    {
      Contract.Requires(sources != null);
      Contract.Ensures(Contract.Result<IObservable<IList<TSource>>>() != null);

      return Observable.Create<IList<TSource>>(
        observer =>
        {
          var gate = new object();

          var queues = new List<Queue<Notification<TSource>>>();

          var outerCompleted = false;

          var outerSubscription = new SingleAssignmentDisposable();
          var disposables = new CompositeDisposable(outerSubscription);

          outerSubscription.Disposable = sources.Synchronize(gate).SubscribeSafe(Observer.Create<IObservable<TSource>>(
            source =>
            {
              var index = queues.Count;

              queues.Add(new Queue<Notification<TSource>>());

              var subscription = new SingleAssignmentDisposable();

              disposables.Add(subscription);

              subscription.Disposable = source.Materialize().Synchronize(gate).SubscribeSafe(Observer.Create<Notification<TSource>>(
                notification =>
                {
                  if (notification.Kind == NotificationKind.OnError)
                  {
                    observer.OnError(notification.Exception);
                    return;
                  }

                  var completeNow = false;
                  var enqueue = !outerCompleted;

                  if (outerCompleted)
                  {
                    Contract.Assert(!enqueue);

                    var zipped = ZipNext(queues, ref completeNow, notification, index);

                    if (zipped == null)
                    {
                      if (completeNow)
                      {
                        queues.Clear();
                      }
                      else
                      {
                        enqueue = true;
                      }
                    }
                    else
                    {
                      observer.OnNext(zipped.AsReadOnly());

                      Contract.Assert(!enqueue && !completeNow);
                    }
                  }

                  Contract.Assert(!(enqueue && completeNow));

                  if (enqueue)
                  {
                    queues[index].Enqueue(notification);
                  }
                  else if (completeNow)
                  {
                    observer.OnCompleted();
                  }
                }));
            },
            observer.OnError,
            () =>
            {
              outerCompleted = true;

              var completeNow = queues.Count == 0;

              if (!completeNow)
              {
                while (true)
                {
                  var zipped = ZipNext(queues, ref completeNow);

                  if (zipped == null)
                  {
                    if (completeNow)
                    {
                      queues.Clear();
                    }
                    break;
                  }

                  observer.OnNext(zipped.AsReadOnly());
                }
              }

              if (completeNow)
              {
                observer.OnCompleted();
              }
            }));

          return disposables;
        });
    }

    private static List<TSource> ZipNext<TSource>(
      IList<Queue<Notification<TSource>>> queues,
      ref bool completeNow,
      Notification<TSource> current = null,
      int currentQueueIndex = -1)
    {
      Contract.Requires(queues != null);
      Contract.Requires((current != null) == (currentQueueIndex > -1));

      var hasValuesForAllQueues = true;

      for (int i = 0; i < queues.Count; i++)
      {
        Notification<TSource> notification;

        if (i == currentQueueIndex)
        {
          notification = current;
        }
        else
        {
          var queue = queues[i];

          Contract.Assume(queue != null);

          hasValuesForAllQueues &= queue.Count > 0;

          if (queue.Count == 0)
          {
            notification = null;
          }
          else
          {
            notification = queue.Peek();
          }
        }

        if (notification != null && notification.Kind == NotificationKind.OnCompleted)
        {
          completeNow = true;
          return null;
        }
      }

      List<TSource> zipped = null;

      if (hasValuesForAllQueues)
      {
        zipped = new List<TSource>(queues.Count);

        for (int i = 0; i < queues.Count; i++)
        {
          Notification<TSource> notification;

          if (i == currentQueueIndex)
          {
            notification = current;
          }
          else
          {
            var queue = queues[i];

            Contract.Assume(queue != null);

            notification = queue.Dequeue();
          }

          Contract.Assume(notification != null);
          Contract.Assume(notification.Kind == NotificationKind.OnNext);

          zipped.Add(notification.Value);
        }
      }

      return zipped;
    }
  }
}