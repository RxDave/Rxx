using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace System.Reactive
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for <see cref="IAsyncNotification{TValue}"/> and related interfaces.
  /// </summary>
  public static class AsyncNotificationExtensions
  {
    /// <summary>
    /// Projects the specified <paramref name="notification"/> into a new notification containing the value returned by the specified <paramref name="selector"/> function
    /// and linked to the state of the specified <paramref name="notification"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the old notification information.</typeparam>
    /// <typeparam name="TResult">The type of the new notification information.</typeparam>
    /// <param name="notification">The notification from which to project a new notification.</param>
    /// <param name="selector">A function that projects the specified <paramref name="notification"/> into a new notification.</param>
    /// <returns>A new <see cref="IAsyncNotification{TResult}"/> containing the value returned by the specified <paramref name="selector"/> function.</returns>
    public static IAsyncNotification<TResult> Select<TValue, TResult>(this IAsyncNotification<TValue> notification, Func<TValue, TResult> selector)
    {
      Contract.Requires(notification != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IAsyncNotification<TResult>>() != null);

      return new AsyncNotificationProjection<TResult>(selector(notification.Value), notification);
    }

    /// <summary>
    /// Projects the specified <paramref name="notification"/> into a new notification containing the value returned by the specified <paramref name="selector"/> function
    /// and linked to the state of the specified <paramref name="notification"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the old notification information.</typeparam>
    /// <typeparam name="TTaskResult">The type of the asynchronous result of the old and new notifications.</typeparam>
    /// <typeparam name="TResult">The type of the new notification information.</typeparam>
    /// <param name="notification">The notification from which to project a new notification.</param>
    /// <param name="selector">A function that projects the specified <paramref name="notification"/> into a new notification.</param>
    /// <returns>A new <see cref="IAsyncNotification{TResult,TTaskResult}"/> containing the value returned by the specified <paramref name="selector"/> function.</returns>
    public static IAsyncNotification<TResult, TTaskResult> Select<TValue, TTaskResult, TResult>(this IAsyncNotification<TValue, TTaskResult> notification, Func<TValue, TResult> selector)
    {
      Contract.Requires(notification != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IAsyncNotification<TResult, TTaskResult>>() != null);

      return new AsyncNotificationProjection<TResult, TTaskResult>(selector(notification.Value), notification);
    }

    /// <summary>
    /// Invokes the specified <paramref name="action"/> with the value of the specified <paramref name="notification"/> and sets the state of the 
    /// <paramref name="notification"/> based on the result of the <paramref name="action"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the notification information.</typeparam>
    /// <param name="notification">The notification to be completed.</param>
    /// <param name="action">The delegate to be invoked.</param>
    /// <returns>The value of the notification after the <paramref name="action"/> has been invoked.</returns>
    public static TValue Complete<TValue>(this IAsyncNotification<TValue> notification, Action<TValue> action)
    {
      Contract.Requires(notification != null);
      Contract.Requires(action != null);

      try
      {
        action(notification.Value);
      }
      catch (OperationCanceledException)
      {
        notification.TrySetCanceled();
        throw;
      }
      catch (Exception ex)
      {
        notification.TrySetException(ex);
        throw;
      }

      notification.TrySetCompleted();

      return notification.Value;
    }

    /// <summary>
    /// Invokes the specified <paramref name="function"/> with the value of the specified <paramref name="notification"/> and sets the state of the 
    /// <paramref name="notification"/> based on the result of the <paramref name="function"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the notification information.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the specified <paramref name="function"/>.</typeparam>
    /// <param name="notification">The notification to be completed.</param>
    /// <param name="function">The delegate to be invoked.</param>
    /// <returns>The value returned by the specified <paramref name="function"/>.</returns>
    public static TResult Complete<TValue, TResult>(this IAsyncNotification<TValue> notification, Func<TValue, TResult> function)
    {
      Contract.Requires(notification != null);
      Contract.Requires(function != null);

      TResult result;
      try
      {
        result = function(notification.Value);
      }
      catch (OperationCanceledException)
      {
        notification.TrySetCanceled();
        throw;
      }
      catch (Exception ex)
      {
        notification.TrySetException(ex);
        throw;
      }

      notification.TrySetCompleted();

      return result;
    }

    /// <summary>
    /// Invokes the specified <paramref name="function"/> with the value of the specified <paramref name="notification"/> and sets the state of the 
    /// <paramref name="notification"/> based on the result of the <paramref name="function"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the notification information.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the specified <paramref name="function"/>.</typeparam>
    /// <param name="notification">The notification to be completed.</param>
    /// <param name="function">The delegate to be invoked.</param>
    /// <returns>The value returned by the specified <paramref name="function"/>.</returns>
    public static TResult Complete<TValue, TResult>(this IAsyncNotification<TValue, TResult> notification, Func<TValue, TResult> function)
    {
      Contract.Requires(notification != null);
      Contract.Requires(function != null);

      TResult result;
      try
      {
        result = function(notification.Value);
      }
      catch (OperationCanceledException)
      {
        notification.TrySetCanceled();
        throw;
      }
      catch (Exception ex)
      {
        notification.TrySetException(ex);
        throw;
      }

      notification.TrySetCompleted(result);

      return result;
    }

    /// <summary>
    /// Invokes the specified delegate with the value of the specified <paramref name="notification"/> and sets the state of the 
    /// <paramref name="notification"/> asynchronously based on the <see cref="Task"/> returned by the delegate.
    /// </summary>
    /// <typeparam name="TValue">The type of the notification information.</typeparam>
    /// <param name="notification">The notification to be completed.</param>
    /// <param name="actionAsync">The delegate to be invoked.</param>
    /// <returns>A <see cref="Task{TValue}"/> containing the value of the notification, which signals when the <see cref="Task"/> that is returned 
    /// by the specified delegate has signaled.</returns>
    public static async Task<TValue> CompleteAsync<TValue>(this IAsyncNotification<TValue> notification, Func<TValue, Task> actionAsync)
    {
      Contract.Requires(notification != null);
      Contract.Requires(actionAsync != null);

      await notification.TrySetFromAsync(actionAsync(notification.Value)).ConfigureAwait(false);

      return notification.Value;
    }

    /// <summary>
    /// Invokes the specified delegate with the value of the specified <paramref name="notification"/> and sets the state of the 
    /// <paramref name="notification"/> asynchronously based on the <see cref="Task{TResult}"/> returned by the delegate.
    /// </summary>
    /// <typeparam name="TValue">The type of the notification information.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the specified delegate.</typeparam>
    /// <param name="notification">The notification to be completed.</param>
    /// <param name="functionAsync">The delegate to be invoked.</param>
    /// <returns>The <see cref="Task{TResult}"/> returned by the specified delegate.</returns>
    public static Task<TResult> CompleteAsync<TValue, TResult>(this IAsyncNotification<TValue> notification, Func<TValue, Task<TResult>> functionAsync)
    {
      Contract.Requires(notification != null);
      Contract.Requires(functionAsync != null);

      var task = functionAsync(notification.Value);

      return notification.TrySetFromAsync(task).ContinueWith(_ => task).Unwrap();
    }

    /// <summary>
    /// Invokes the specified delegate with the value of the specified <paramref name="notification"/> and sets the state of the 
    /// <paramref name="notification"/> asynchronously based on the <see cref="Task{TResult}"/> returned by the delegate.
    /// </summary>
    /// <typeparam name="TValue">The type of the notification information.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the specified delegate.</typeparam>
    /// <param name="notification">The notification to be completed.</param>
    /// <param name="functionAsync">The delegate to be invoked.</param>
    /// <returns>The <see cref="Task{TResult}"/> returned by the specified delegate.</returns>
    public static Task<TResult> CompleteAsync<TValue, TResult>(this IAsyncNotification<TValue, TResult> notification, Func<TValue, Task<TResult>> functionAsync)
    {
      Contract.Requires(notification != null);
      Contract.Requires(functionAsync != null);
      Contract.Ensures(Contract.Result<Task<TResult>>() != null);

      return notification.TrySetFromAsync(functionAsync(notification.Value));
    }

    /// <summary>
    /// Sets the state of the specified <paramref name="notification"/> asynchronously based on the result of the specified <paramref name="task"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the notification information.</typeparam>
    /// <param name="notification">The notification that is to be set asynchronously.</param>
    /// <param name="task">The <see cref="Task"/> from which to set the state of the specified <paramref name="notification"/>.</param>
    /// <returns>A <see cref="Task{T}"/> containing a value that indicates whether the specified <paramref name="notification"/> was able to be set, 
    /// which signals when the specified <paramref name="task"/> has signaled.</returns>
    public static Task<bool> TrySetFromAsync<TValue>(this IAsyncNotification<TValue> notification, Task task)
    {
      Contract.Requires(notification != null);
      Contract.Requires(task != null);
      Contract.Ensures(Contract.Result<Task<bool>>() != null);

      return task.ContinueWith(
        t =>
        {
          if (t.IsCompleted)
          {
            return notification.TrySetCompleted();
          }
          else if (t.IsFaulted)
          {
            return notification.TrySetException(t.Exception);
          }
          else if (t.IsCanceled)
          {
            return notification.TrySetCanceled();
          }
          else
          {
            throw new InvalidOperationException();
          }
        },
        TaskContinuationOptions.ExecuteSynchronously);
    }

    /// <summary>
    /// Sets the state of the specified <paramref name="notification"/> asynchronously based on the result of the specified <paramref name="task"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the notification information.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the specified <paramref name="task"/>.</typeparam>
    /// <param name="notification">The notification that is to be set asynchronously.</param>
    /// <param name="task">The <see cref="Task{TResult}"/> from which to set the state of the specified <paramref name="notification"/>.</param>
    /// <returns>The specified <paramref name="task"/>.</returns>
    public static async Task<TResult> TrySetFromAsync<TValue, TResult>(this IAsyncNotification<TValue, TResult> notification, Task<TResult> task)
    {
      Contract.Requires(notification != null);
      Contract.Requires(task != null);

      TResult result;
      try
      {
        result = await task.ConfigureAwait(false);
      }
      catch (OperationCanceledException)
      {
        notification.TrySetCanceled();
        throw;
      }
      catch (Exception ex)
      {
        notification.TrySetException(ex);
        throw;
      }

      notification.TrySetCompleted(result);

      return result;
    }
  }
}