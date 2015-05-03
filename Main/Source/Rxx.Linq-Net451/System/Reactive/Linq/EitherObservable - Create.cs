using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  public static partial class EitherObservable
  {
    /// <summary>
    /// Creates an observable sequence with two notification channels from the <paramref name="subscribe"/> implementation.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="subscribe">Subscribes observers to the observable.</param>
    /// <returns>An observable with two notification channels that calls the specified <paramref name="subscribe"/> function 
    /// when an observer subscribes.</returns>
    public static IObservable<Either<TLeft, TRight>> Create<TLeft, TRight>(
      Func<IObserver<Either<TLeft, TRight>>, Action> subscribe)
    {
      Contract.Requires(subscribe != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TLeft, TRight>>>() != null);

      return Observable.Create<Either<TLeft, TRight>>(observer => subscribe(observer));
    }

    /// <summary>
    /// Creates an observable sequence with two notification channels from the <paramref name="subscribe"/> implementation.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="subscribe">Subscribes observers to the observable.</param>
    /// <returns>An observable with two notification channels that calls the specified <paramref name="subscribe"/> function
    /// when an observer subscribes.</returns>
    public static IObservable<Either<TLeft, TRight>> Create<TLeft, TRight>(
      Func<IObserver<Either<TLeft, TRight>>, IDisposable> subscribe)
    {
      Contract.Requires(subscribe != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TLeft, TRight>>>() != null);

      return Observable.Create<Either<TLeft, TRight>>(observer => subscribe(observer));
    }
  }
}