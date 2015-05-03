using System.Diagnostics.Contracts;
using Rxx.Linq.Properties;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Creates an observable with two pairwise notification channels containing values from the specified 
    /// observable sequence in the left channel and values projected by the specified function in the right
    /// channel.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="leftSource">The observable sequence that provides notifications for the left channel.</param>
    /// <param name="rightSelector">Selects a value for the right channel from each value in the specified observable sequence.</param>
    /// <returns>The specified observable sequence paired with the values produced by the specified selector function.</returns>
    public static IObservable<Either<TLeft, TRight>> Pair<TLeft, TRight>(
      this IObservable<TLeft> leftSource,
      Func<TLeft, TRight> rightSelector)
    {
      Contract.Requires(leftSource != null);
      Contract.Requires(rightSelector != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TLeft, TRight>>>() != null);

      return EitherObservable.Create<TLeft, TRight>(
        observer =>
        {
          return leftSource.SubscribeSafe(
            left =>
            {
              observer.OnNextLeft(left);
              observer.OnNextRight(rightSelector(left));
            },
            observer.OnError,
            observer.OnCompleted);
        });
    }

    /// <summary>
    /// Creates an observable with two pairwise notification channels containing values from the specified 
    /// observable sequence projected by the specified function in the left channel and values projected
    /// by the other specified function in the right channel.
    /// </summary>
    /// <typeparam name="TSource">Type of the notifications in the source sequence.</typeparam>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="source">The observable sequence from which values are projected.</param>
    /// <param name="leftSelector">Selects a value for the left channel from each value in the specified observable sequence.</param>
    /// <param name="rightSelector">Selects a value for the right channel from each value in the specified observable sequence.</param>
    /// <returns>The specified observable sequence projected into paired values produced by the specified selector functions.</returns>
    public static IObservable<Either<TLeft, TRight>> Pair<TSource, TLeft, TRight>(
      this IObservable<TSource> source,
      Func<TSource, TLeft> leftSelector,
      Func<TSource, TRight> rightSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(leftSelector != null);
      Contract.Requires(rightSelector != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TLeft, TRight>>>() != null);

      return EitherObservable.Create<TLeft, TRight>(
        observer =>
        {
          return source.SubscribeSafe(
            value =>
            {
              observer.OnNextLeft(leftSelector(value));
              observer.OnNextRight(rightSelector(value));
            },
            observer.OnError,
            observer.OnCompleted);
        });
    }

    /// <summary>
    /// Creates an observable with two pairwise notification channels by combining the latest values of the specified 
    /// observable sequences.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="leftSource">The observable sequence that provides notifications for the left channel.</param>
    /// <param name="rightSource">The observable sequence that provides notifications for the right channel.</param>
    /// <returns>The latest values of both observable sequences paired together into a new observable sequence.</returns>
    public static IObservable<Either<TLeft, TRight>> Pair<TLeft, TRight>(
      this IObservable<TLeft> leftSource,
      IObservable<TRight> rightSource)
    {
      Contract.Requires(leftSource != null);
      Contract.Requires(rightSource != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TLeft, TRight>>>() != null);

      return Pair(leftSource, rightSource, (left, right) => PairDirection.Both);
    }

    /// <summary>
    /// Creates an observable with two pairwise notification channels by combining the latest values of the specified 
    /// observable sequences and choosing which channels will receive values based on the specified function.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="leftSource">The observable sequence that provides notifications for the left channel.</param>
    /// <param name="rightSource">The observable sequence that provides notifications for the right channel.</param>
    /// <param name="directionSelector">Selects the channels that will receive notifications for every pair.</param>
    /// <returns>A new observable sequence containing the latest values in the specified observable sequences.</returns>
    public static IObservable<Either<TLeft, TRight>> Pair<TLeft, TRight>(
      this IObservable<TLeft> leftSource,
      IObservable<TRight> rightSource,
      Func<TLeft, TRight, PairDirection> directionSelector)
    {
      Contract.Requires(leftSource != null);
      Contract.Requires(rightSource != null);
      Contract.Requires(directionSelector != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TLeft, TRight>>>() != null);

      return EitherObservable.Create<TLeft, TRight>(
        observer =>
          leftSource.Always().StartWith(Maybe.Empty<TLeft>())
          .CombineLatest(
            rightSource.Always().StartWith(Maybe.Empty<TRight>()),
            (left, right) => new
            {
              left,
              right
            })
          .SubscribeSafe(
            pair =>
            {
              if (!pair.left.HasValue)
              {
                if (pair.right.HasValue)
                {
                  observer.OnNextRight(pair.right.Value);
                }
              }
              else if (!pair.right.HasValue)
              {
                observer.OnNextLeft(pair.left.Value);
              }
              else
              {
                var left = pair.left.Value;
                var right = pair.right.Value;

                switch (directionSelector(left, right))
                {
                  case PairDirection.Left:
                    observer.OnNextLeft(left);
                    break;
                  case PairDirection.Right:
                    observer.OnNextRight(right);
                    break;
                  case PairDirection.Both:
                    observer.OnNextLeft(left);
                    observer.OnNextRight(right);
                    break;
                  case PairDirection.Neither:
                    break;
                  default:
                    throw new InvalidOperationException(Errors.InvalidPairDirectionValue);
                }
              }
            },
            observer.OnError,
            observer.OnCompleted));
    }

    /// <summary>
    /// Creates an observable with two pairwise notification channels from the specified observable sequence 
    /// by choosing which channels will receive each value based on the specified function.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which values will be paired based on the specified selector function.</param>
    /// <param name="directionSelector">Selects the channels that will receive notifications for every value in the <paramref name="source"/>.</param>
    /// <returns>An observable sequence with two pairwise notification channels projected from the specified 
    /// observable sequence based on the specified selector function.</returns>
    public static IObservable<Either<TSource, TSource>> Pair<TSource>(
      this IObservable<TSource> source,
      Func<TSource, PairDirection> directionSelector)
    {
      return EitherObservable.Create<TSource, TSource>(
        observer =>
        {
          return source.SubscribeSafe(
            value =>
            {
              switch (directionSelector(value))
              {
                case PairDirection.Left:
                  observer.OnNextLeft(value);
                  break;
                case PairDirection.Right:
                  observer.OnNextRight(value);
                  break;
                case PairDirection.Both:
                  observer.OnNextLeft(value);
                  observer.OnNextRight(value);
                  break;
                case PairDirection.Neither:
                  break;
                default:
                  throw new InvalidOperationException(Errors.InvalidPairDirectionValue);
              }
            },
            observer.OnError,
            observer.OnCompleted);
        });
    }
  }
}