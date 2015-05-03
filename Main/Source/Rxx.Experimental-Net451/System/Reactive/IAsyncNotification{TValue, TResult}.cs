namespace System.Reactive
{
  /// <summary>
  /// Represents an asynchronous notification to an observer with a result.
  /// </summary>
  /// <typeparam name="TValue">The type of the notification information.</typeparam>
  /// <typeparam name="TResult">The type of the asynchronous result.</typeparam>
  public interface IAsyncNotification<out TValue, TResult> : IAsyncNotification<TValue>, IAsyncNotificationWithResult<TResult>
  {
  }
}