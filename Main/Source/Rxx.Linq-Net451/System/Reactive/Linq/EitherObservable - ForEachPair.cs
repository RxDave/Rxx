using System.Diagnostics.Contracts;
#if !WINDOWS_PHONE
using System.Threading;
using System.Threading.Tasks;
#endif

namespace System.Reactive.Linq
{
  public static partial class EitherObservable
  {
    /// <summary>
    /// Invokes the actions for their side-effects on each value in the observable sequence and blocks till the sequence is terminated.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="onNextLeft">The handler of notifications in the left channel.</param>
    /// <param name="onNextRight">The handler of notifications in the right channel.</param>
    [Obsolete("This blocking operation is no longer supported.  Instead, use ForEachAsync with async/await, or simply call the Wait method to block.")]
    public static void ForEach<TLeft, TRight>(
      this IObservable<Either<TLeft, TRight>> source,
      Action<TLeft> onNextLeft,
      Action<TRight> onNextRight)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextLeft != null);
      Contract.Requires(onNextRight != null);

#if !WINDOWS_PHONE
      source.ForEachAsync(onNextLeft, onNextRight).Wait();
#else
      source.ForEach(value => value.Switch(onNextLeft, onNextRight));
#endif
    }

#if !WINDOWS_PHONE
    /// <summary>
    /// Invokes the actions for their side-effects on each value in the observable sequence and returns a <see cref="Task"/> object 
    /// that can be awaited on until the sequence terminates.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="onNextLeft">The handler of notifications in the left channel.</param>
    /// <param name="onNextRight">The handler of notifications in the right channel.</param>
    /// <returns>An object that can be awaited on until the sequence terminates.</returns>
    public static Task ForEachAsync<TLeft, TRight>(
      this IObservable<Either<TLeft, TRight>> source,
      Action<TLeft> onNextLeft,
      Action<TRight> onNextRight)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextLeft != null);
      Contract.Requires(onNextRight != null);

      return source.ForEachAsync(value => value.Switch(onNextLeft, onNextRight));
    }

    /// <summary>
    /// Invokes the actions for their side-effects on each value in the observable sequence and returns a <see cref="Task"/> object 
    /// that can be awaited on until the sequence terminates.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left notification channel.</typeparam>
    /// <typeparam name="TRight">Type of the right notification channel.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="onNextLeft">The handler of notifications in the left channel.</param>
    /// <param name="onNextRight">The handler of notifications in the right channel.</param>
    /// <param name="cancellationToken">Cancellation token used to stop the loop.</param>
    /// <returns>An object that can be awaited on until the sequence terminates.</returns>
    public static Task ForEachAsync<TLeft, TRight>(
      this IObservable<Either<TLeft, TRight>> source,
      Action<TLeft> onNextLeft,
      Action<TRight> onNextRight,
      CancellationToken cancellationToken)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextLeft != null);
      Contract.Requires(onNextRight != null);

      return source.ForEachAsync(value => value.Switch(onNextLeft, onNextRight), cancellationToken);
    }
#endif
  }
}