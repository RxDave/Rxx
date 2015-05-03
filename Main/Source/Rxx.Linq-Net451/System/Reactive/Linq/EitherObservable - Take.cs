using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  public static partial class EitherObservable
  {
    /// <summary>
    /// Returns an observable that contains only the values from the left notification channel.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="source">The observable from which values are taken.</param>
    /// <returns>An observable of values from the left notification channel.</returns>
    public static IObservable<TLeft> TakeLeft<TLeft, TRight>(
      this IObservable<Either<TLeft, TRight>> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<TLeft>>() != null);

      return Observable.Create<TLeft>(
        observer =>
        {
          return source.SubscribeEither(
            observer.OnNext,
            right => { },
            observer.OnError,
            observer.OnCompleted);
        });
    }

    /// <summary>
    /// Returns an observable that contains only the values from the right notification channel.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="source">The observable from which values are taken.</param>
    /// <returns>An observable of values from the right notification channel.</returns>
    public static IObservable<TRight> TakeRight<TLeft, TRight>(
      this IObservable<Either<TLeft, TRight>> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<TRight>>() != null);

      return Observable.Create<TRight>(
        observer =>
        {
          return source.SubscribeEither(
            left => { },
            observer.OnNext,
            observer.OnError,
            observer.OnCompleted);
        });
    }

    /// <summary>
    /// Returns an observable that contains only the values from the left notification channel
    /// up to the specified <paramref name="count"/>.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="source">The observable from which values are taken.</param>
    /// <param name="count">The number of values to take.</param>
    /// <returns>An observable of values containing the maximum specified number from the left 
    /// notification channel and all values from the right notification channel.</returns>
    public static IObservable<Either<TLeft, TRight>> TakeLeft<TLeft, TRight>(
      this IObservable<Either<TLeft, TRight>> source,
      int count)
    {
      Contract.Requires(source != null);
      Contract.Requires(count >= 0);
      Contract.Ensures(Contract.Result<IObservable<Either<TLeft, TRight>>>() != null);

      return EitherObservable.Create<TLeft, TRight>(
        observer =>
        {
          int remaining = count;

          return source.SubscribeEither(
            left =>
            {
              if (remaining > 0)
              {
                remaining--;

                observer.OnNextLeft(left);

                if (remaining == 0)
                {
                  observer.OnCompleted();
                }
              }
            },
            observer.OnNextRight,
            observer.OnError,
            observer.OnCompleted);
        });
    }

    /// <summary>
    /// Returns an observable that contains only the values from the right notification channel
    /// up to the specified <paramref name="count"/>.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="source">The observable from which values are taken.</param>
    /// <param name="count">The number of values to take.</param>
    /// <returns>An observable of values containing the maximum specified number from the right 
    /// notification channel and all values from the left notification channel.</returns>
    public static IObservable<Either<TLeft, TRight>> TakeRight<TLeft, TRight>(
      this IObservable<Either<TLeft, TRight>> source,
      int count)
    {
      Contract.Requires(source != null);
      Contract.Requires(count >= 0);
      Contract.Ensures(Contract.Result<IObservable<Either<TLeft, TRight>>>() != null);

      return EitherObservable.Create<TLeft, TRight>(
        observer =>
        {
          int remaining = count;

          return source.SubscribeEither(
            observer.OnNextLeft,
            right =>
            {
              if (remaining > 0)
              {
                remaining--;

                observer.OnNextRight(right);

                if (remaining == 0)
                {
                  observer.OnCompleted();
                }
              }
            },
            observer.OnError,
            observer.OnCompleted);
        });
    }
  }
}