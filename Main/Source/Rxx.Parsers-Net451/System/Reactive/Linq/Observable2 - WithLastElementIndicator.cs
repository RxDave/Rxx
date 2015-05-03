using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  internal static partial class Observable2
  {
    private static IObservable<Tuple<T, bool>> WithLastElementIndicator<T>(this IObservable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<Tuple<T, bool>>>() != null);

      return Observable.Create<Tuple<T, bool>>(
        observer =>
        {
          bool hasPreviousValue = false;
          T previousValue = default(T);

          return source.SubscribeSafe(
            value =>
            {
              if (hasPreviousValue)
              {
                observer.OnNext(Tuple.Create(previousValue, false));
              }

              previousValue = value;
              hasPreviousValue = true;
            },
            observer.OnError,
            () =>
            {
              if (hasPreviousValue)
              {
                observer.OnNext(Tuple.Create(previousValue, true));
              }

              observer.OnCompleted();
            });
        });
    }
  }
}