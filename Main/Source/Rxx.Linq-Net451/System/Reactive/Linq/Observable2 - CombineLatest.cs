using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Disposables;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Merges two or more observable sequences into one observable sequence of lists, each containing the latest values from the latest consecutive 
    /// observable sequences whenever one of the sequences produces an element.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="sources">An observable sequence containing two or more observable sequences to be merged.</param>
    /// <remarks>
    /// <para>
    /// Unlike the native Rx overloads of <strong>CombineLatest</strong>, the generated sequence may contain singleton lists as soon as the first inner
    /// observable notifies, without waiting for additional inner observables to arrive and without waiting for additional inner observables to notify. No elements 
    /// are discarded. As new inner observables arrive, the size of the generated lists are increased to accommodate the new observables; the default value of 
    /// <typeparamref name="TSource"/> is inserted until a new inner observable notifies.
    /// </para>
    /// <para>
    /// The latest value of an observable sequence is always located in the generated lists at the same index in which that sequence is located in the outer sequence.
    /// For example, the values from the first inner observable sequence will always be at index zero (0) in the lists that are generated. Furthermore, once a generated 
    /// list contains the value for a particular observable sequence, all subsequent lists will also contain the latest value for that sequence.  As a result, the number 
    /// of items in the generated lists may stay the same or grow when new observable sequences arrive, but the size of the lists will never shrink, even in the case that
    /// an inner observable completes.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence containing the result of combining the latest elements of all sources into lists.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is composited by the subscription.")]
    public static IObservable<IList<TSource>> CombineLatest<TSource>(this IObservable<IObservable<TSource>> sources)
    {
      Contract.Requires(sources != null);
      Contract.Ensures(Contract.Result<IObservable<IList<TSource>>>() != null);

      return Observable.Create<IList<TSource>>(
        observer =>
        {
          var gate = new object();

          var latest = new List<TSource>();

          var sourceCount = 0;
          var outerCompleted = false;

          var outerSubscription = new SingleAssignmentDisposable();
          var disposables = new CompositeDisposable(outerSubscription);

          outerSubscription.Disposable = sources.Synchronize(gate).SubscribeSafe(Observer.Create<IObservable<TSource>>(
            source =>
            {
              sourceCount++;

              var index = latest.Count;

              latest.Add(default(TSource));

              var subscription = new SingleAssignmentDisposable();

              disposables.Add(subscription);

              subscription.Disposable = source.Synchronize(gate).SubscribeSafe(Observer.Create<TSource>(
                value =>
                {
                  latest[index] = value;

                  observer.OnNext(latest.ToList().AsReadOnly());
                },
                observer.OnError,
                () =>
                {
                  sourceCount--;

                  disposables.Remove(subscription);

                  if (outerCompleted && sourceCount == 0)
                  {
                    observer.OnCompleted();
                  }
                }));
            },
            observer.OnError,
            () =>
            {
              outerCompleted = true;

              if (sourceCount == 0)
              {
                observer.OnCompleted();
              }
            }));

          return disposables;
        });
    }
  }
}