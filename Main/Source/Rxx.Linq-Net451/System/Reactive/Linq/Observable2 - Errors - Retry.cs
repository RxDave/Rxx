using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Repeats the source observable sequence when it throws the specified type of exception 
    /// until it successfully terminates or the specified count has been reached and pairs it
    /// with an error channel.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TException">The type of exception to catch.</typeparam>
    /// <param name="source">The observable to be repeated.</param>
    /// <param name="retryCount">The maximum number of times to retry the sequence when it's faulted.</param>
    /// <returns>The specified observable sequence with an error channel.</returns>
    public static IObservable<Either<TSource, TException>> Retry<TSource, TException>(
      this IObservable<TSource> source,
      int retryCount)
      where TException : Exception
    {
      Contract.Requires(source != null);
      Contract.Requires(retryCount >= 0);
      Contract.Ensures(Contract.Result<IObservable<Either<TSource, TException>>>() != null);

      return Retry<TSource, TException>(source, retryCount, (ex, i) => TimeSpan.Zero);
    }

    /// <summary>
    /// Repeats the source observable sequence using the specified back-off algorithm until it 
    /// successfully terminates or the specified count has been reached and pairs it with an 
    /// error channel.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be repeated.</param>
    /// <param name="retryCount">The maximum number of times to retry the sequence when it's faulted.</param>
    /// <param name="backOffSelector">Selects the amount of time to delay before repeating when the sequence has faulted.</param>
    /// <returns>The specified observable sequence with an error channel.</returns>
    public static IObservable<Either<TSource, Exception>> Retry<TSource>(
      this IObservable<TSource> source,
      int retryCount,
      Func<Exception, int, TimeSpan> backOffSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(retryCount >= 0);
      Contract.Requires(backOffSelector != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TSource, Exception>>>() != null);

      return Retry<TSource, Exception>(source, retryCount, backOffSelector);
    }

    /// <summary>
    /// Repeats the source observable sequence when it throws the specified type of exception
    /// using the specified back-off algorithm until it successfully terminates or the specified 
    /// count has been reached and pairs it with an error channel.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TException">The type of exception to catch.</typeparam>
    /// <param name="source">The observable to be repeated.</param>
    /// <param name="retryCount">The maximum number of times to retry the sequence when it's faulted.</param>
    /// <param name="backOffSelector">Selects the amount of time to delay before repeating when the sequence has faulted.</param>
    /// <returns>The specified observable sequence with an error channel.</returns>
    public static IObservable<Either<TSource, TException>> Retry<TSource, TException>(
      this IObservable<TSource> source,
      int retryCount,
      Func<TException, int, TimeSpan> backOffSelector)
      where TException : Exception
    {
      Contract.Requires(source != null);
      Contract.Requires(retryCount >= 0);
      Contract.Requires(backOffSelector != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TSource, TException>>>() != null);

      return Observable.Defer<Either<TSource, TException>>(() =>
        {
          int attemptCount = 1;

          var sources = Enumerable.Repeat(source, retryCount).GetEnumerator();

          return sources.Catch<TSource, TException>(
            ex => sources,
            ex => backOffSelector(ex, attemptCount++));
        });
    }

    /// <summary>
    /// Repeats the source observable sequence when it throws consecutively 
    /// until it produces a value, successfully terminates or the specified count 
    /// has been reached and pairs it with an error channel.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be repeated.</param>
    /// <param name="consecutiveRetryCount">The maximum number of times to retry the sequence consecutively 
    /// when it's faulted.</param>
    /// <remarks>
    /// <see cref="RetryConsecutive{TSource}(IObservable{TSource},int)"/> is appropriate when permanent recovery is required for sequences 
    /// that experience ephemeral consecutive errors at unpredictable intervals, such as those originating 
    /// from network streams.  For example, it can produce a sequence that automatically reconnects upon 
    /// consecutive network failures up to the specified <paramref name="consecutiveRetryCount"/> number of 
    /// times; furthermore, if the sequence is able to successfully generate a value after an error, then 
    /// the retry count is reset for subsequent consecutive failures.
    /// </remarks>
    /// <returns>The specified observable sequence with an error channel.</returns>
    public static IObservable<Either<TSource, Exception>> RetryConsecutive<TSource>(
      this IObservable<TSource> source,
      int consecutiveRetryCount)
    {
      Contract.Requires(source != null);
      Contract.Requires(consecutiveRetryCount >= 0);
      Contract.Ensures(Contract.Result<IObservable<Either<TSource, Exception>>>() != null);

      return RetryConsecutive<TSource, Exception>(source, consecutiveRetryCount, (ex, i) => TimeSpan.Zero);
    }

    /// <summary>
    /// Repeats the source observable sequence when it throws the specified type of exception 
    /// consecutively until it produces a value, successfully terminates or the specified count 
    /// has been reached and pairs it with an error channel.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TException">The type of exception to catch.</typeparam>
    /// <param name="source">The observable to be repeated.</param>
    /// <param name="consecutiveRetryCount">The maximum number of times to retry the sequence consecutively 
    /// when it's faulted.</param>
    /// <remarks>
    /// <see cref="RetryConsecutive{TSource,TException}(IObservable{TSource},int)"/> is appropriate when permanent recovery is required for sequences 
    /// that experience ephemeral consecutive errors at unpredictable intervals, such as those originating 
    /// from network streams.  For example, it can produce a sequence that automatically reconnects upon 
    /// consecutive network failures up to the specified <paramref name="consecutiveRetryCount"/> number of 
    /// times; furthermore, if the sequence is able to successfully generate a value after an error, then 
    /// the retry count is reset for subsequent consecutive failures.
    /// </remarks>
    /// <returns>The specified observable sequence with an error channel.</returns>
    public static IObservable<Either<TSource, TException>> RetryConsecutive<TSource, TException>(
      this IObservable<TSource> source,
      int consecutiveRetryCount)
      where TException : Exception
    {
      Contract.Requires(source != null);
      Contract.Requires(consecutiveRetryCount >= 0);
      Contract.Ensures(Contract.Result<IObservable<Either<TSource, TException>>>() != null);

      return RetryConsecutive<TSource, TException>(source, consecutiveRetryCount, (ex, i) => TimeSpan.Zero);
    }

    /// <summary>
    /// Repeats the source observable sequence when it throws consecutively 
    /// using the specified back-off algorithm until it produces a value, successfully terminates or 
    /// the specified count has been reached and pairs it with an error channel.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be repeated.</param>
    /// <param name="consecutiveRetryCount">The maximum number of times to retry the sequence consecutively 
    /// when it's faulted.</param>
    /// <param name="backOffSelector">Selects the amount of time to delay before repeating when the sequence has faulted.</param>
    /// <remarks>
    /// <see cref="RetryConsecutive{TSource}(IObservable{TSource},int,Func{Exception,int,TimeSpan})"/> is appropriate when permanent recovery is required for sequences 
    /// that experience ephemeral consecutive errors at unpredictable intervals, such as those originating 
    /// from network streams.  For example, it can produce a sequence that automatically reconnects upon 
    /// consecutive network failures up to the specified <paramref name="consecutiveRetryCount"/> number of 
    /// times; furthermore, if the sequence is able to successfully generate a value after an error, then 
    /// the retry count is reset for subsequent consecutive failures.
    /// </remarks>
    /// <returns>The specified observable sequence with an error channel.</returns>
    /// <seealso href="http://en.wikipedia.org/wiki/Exponential_backoff">
    /// Exponential backoff
    /// </seealso>
    public static IObservable<Either<TSource, Exception>> RetryConsecutive<TSource>(
      this IObservable<TSource> source,
      int consecutiveRetryCount,
      Func<Exception, int, TimeSpan> backOffSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(consecutiveRetryCount >= 0);
      Contract.Requires(backOffSelector != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TSource, Exception>>>() != null);

      return RetryConsecutive<TSource, Exception>(source, consecutiveRetryCount, backOffSelector);
    }

    /// <summary>
    /// Repeats the source observable sequence when it throws the specified type of exception 
    /// consecutively using the specified back-off algorithm until it produces a value, successfully terminates 
    /// or the specified count has been reached and pairs it with an error channel.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TException">The type of exception to catch.</typeparam>
    /// <param name="source">The observable to be repeated.</param>
    /// <param name="consecutiveRetryCount">The maximum number of times to retry the sequence consecutively 
    /// when it's faulted.</param>
    /// <param name="backOffSelector">Selects the amount of time to delay before repeating when the sequence has faulted.</param>s
    /// <remarks>
    /// <see cref="RetryConsecutive{TSource,TException}(IObservable{TSource},int,Func{TException,int,TimeSpan})"/> is appropriate when permanent recovery is required for sequences 
    /// that experience ephemeral consecutive errors at unpredictable intervals, such as those originating 
    /// from network streams.  For example, it can produce a sequence that automatically reconnects upon 
    /// consecutive network failures up to the specified <paramref name="consecutiveRetryCount"/> number of 
    /// times; furthermore, if the sequence is able to successfully generate a value after an error, then 
    /// the retry count is reset for subsequent consecutive failures.
    /// </remarks>
    /// <returns>The specified observable sequence with an error channel.</returns>
    /// <seealso href="http://en.wikipedia.org/wiki/Exponential_backoff">
    /// Exponential backoff
    /// </seealso>
    public static IObservable<Either<TSource, TException>> RetryConsecutive<TSource, TException>(
      this IObservable<TSource> source,
      int consecutiveRetryCount,
      Func<TException, int, TimeSpan> backOffSelector)
      where TException : Exception
    {
      Contract.Requires(source != null);
      Contract.Requires(consecutiveRetryCount >= 0);
      Contract.Requires(backOffSelector != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TSource, TException>>>() != null);

      return EitherObservable.Create<TSource, TException>(
        observer =>
        {
          int attemptCount = 1;
          bool decremented = false;
          bool resetRequired = false;

          var sources = Enumerable.Repeat(source, consecutiveRetryCount).GetEnumerator();

          return sources
            .Catch<TSource, TException>(
              ex =>
              {
                if (resetRequired)
                {
                  if (!decremented)
                  /* This behavior matches the Rx behavior of the retryCount parameter in the Retry method.
                    * The first iteration always counts as the first "retry", even though technically it's 
                    * not a "retry" because it's first.  (If consecutiveRetryCount is set to zero, then the 
                    * sequence will end because the enumerator's MoveNext method called below will return false.)
                    */
                  {
                    Contract.Assume(consecutiveRetryCount > 0);

                    consecutiveRetryCount--;
                    decremented = true;
                  }

                  attemptCount = 1;
                  resetRequired = false;

                  sources = Enumerable.Repeat(source, consecutiveRetryCount).GetEnumerator();
                }

                return sources;
              },
              ex => backOffSelector(ex, attemptCount++))
            .SubscribeEither(
              value =>
              {
                resetRequired = true;

                observer.OnNextLeft(value);
              },
              observer.OnNextRight,
              observer.OnError,
              observer.OnCompleted);
        });
    }
  }
}