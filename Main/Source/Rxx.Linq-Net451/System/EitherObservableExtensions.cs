using System.Diagnostics.Contracts;

namespace System
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for <see cref="IObservable{T}"/> regarding <see cref="Either{TLeft,TRight}"/>.
  /// </summary>
  public static partial class EitherObservableExtensions
  {
    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="onNextLeft">The handler of notifications in the left channel.</param>
    /// <param name="onNextRight">The handler of notifications in the right channel.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable SubscribeEither<TLeft, TRight>(
      this IObservable<Either<TLeft, TRight>> source,
      Action<TLeft> onNextLeft,
      Action<TRight> onNextRight)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextLeft != null);
      Contract.Requires(onNextRight != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(EitherObserver.Create(
        onNextLeft,
        onNextRight));
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="onNextLeft">The handler of notifications in the left channel.</param>
    /// <param name="onNextRight">The handler of notifications in the right channel.</param>
    /// <param name="onError">The handler of an error notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable SubscribeEither<TLeft, TRight>(
      this IObservable<Either<TLeft, TRight>> source,
      Action<TLeft> onNextLeft,
      Action<TRight> onNextRight,
      Action<Exception> onError)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextLeft != null);
      Contract.Requires(onNextRight != null);
      Contract.Requires(onError != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(EitherObserver.Create(
        onNextLeft,
        onNextRight,
        onError));
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="onNextLeft">The handler of notifications in the left channel.</param>
    /// <param name="onNextRight">The handler of notifications in the right channel.</param>
    /// <param name="onCompleted">The handler of a completion notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable SubscribeEither<TLeft, TRight>(
      this IObservable<Either<TLeft, TRight>> source,
      Action<TLeft> onNextLeft,
      Action<TRight> onNextRight,
      Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextLeft != null);
      Contract.Requires(onNextRight != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(EitherObserver.Create(
        onNextLeft,
        onNextRight,
        onCompleted));
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="onNextLeft">The handler of notifications in the left channel.</param>
    /// <param name="onNextRight">The handler of notifications in the right channel.</param>
    /// <param name="onError">The handler of an error notification.</param>
    /// <param name="onCompleted">The handler of a completion notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable SubscribeEither<TLeft, TRight>(
      this IObservable<Either<TLeft, TRight>> source,
      Action<TLeft> onNextLeft,
      Action<TRight> onNextRight,
      Action<Exception> onError,
      Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextLeft != null);
      Contract.Requires(onNextRight != null);
      Contract.Requires(onError != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(EitherObserver.Create(
        onNextLeft,
        onNextRight,
        onError,
        onCompleted));
    }
  }
}