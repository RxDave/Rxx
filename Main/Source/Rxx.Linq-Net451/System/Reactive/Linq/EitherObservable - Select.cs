using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  public static partial class EitherObservable
  {
    /// <summary>
    /// Projects the values from both notification channels into a new sequence.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <typeparam name="TLeftResult">Result type of the left notification channel.</typeparam>
    /// <typeparam name="TRightResult">Result type of the right notification channel.</typeparam>
    /// <param name="source">The observable from which values are projected.</param>
    /// <param name="leftSelector">Projects values from the left notification channel.</param>
    /// <param name="rightSelector">Projects value from the right notification channel.</param>
    /// <returns>An observable of results from the projection of values in both notification channels.</returns>
    public static IObservable<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
      this IObservable<Either<TLeft, TRight>> source,
      Func<TLeft, TLeftResult> leftSelector,
      Func<TRight, TRightResult> rightSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(leftSelector != null);
      Contract.Requires(rightSelector != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TLeftResult, TRightResult>>>() != null);

      return EitherObservable.Create<TLeftResult, TRightResult>(
        observer =>
        {
          return source.SubscribeEither(
            left => observer.OnNextLeft(leftSelector(left)),
            right => observer.OnNextRight(rightSelector(right)),
            observer.OnError,
            observer.OnCompleted);
        });
    }

    /// <summary>
    /// Projects the values from the left notification channel into a new sequence.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="source">The observable from which values are projected.</param>
    /// <param name="selector">Projects values from the left notification channel.</param>
    /// <returns>An observable of results from the projection of values in the left notification channel.</returns>
    public static IObservable<TResult> SelectLeft<TLeft, TRight, TResult>(
      this IObservable<Either<TLeft, TRight>> source,
      Func<TLeft, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      return Observable.Create<TResult>(
        observer =>
        {
          return source.SubscribeEither(
            left => observer.OnNext(selector(left)),
            right => { },
            observer.OnError,
            observer.OnCompleted);
        });
    }

    /// <summary>
    /// Projects the values from the right notification channel into a new sequence.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="source">The observable from which values are projected.</param>
    /// <param name="selector">Projects values from the right notification channel.</param>
    /// <returns>An observable of results from the projection of values in the right notification channel.</returns>
    public static IObservable<TResult> SelectRight<TLeft, TRight, TResult>(
      this IObservable<Either<TLeft, TRight>> source,
      Func<TRight, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      return Observable.Create<TResult>(
        observer =>
        {
          return source.SubscribeEither(
            left => { },
            right => observer.OnNext(selector(right)),
            observer.OnError,
            observer.OnCompleted);
        });
    }
  }
}