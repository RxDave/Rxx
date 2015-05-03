using System.Diagnostics.Contracts;
using System.Reactive;
using System.Threading.Tasks;

namespace System
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for subscribing to asynchronous sequences of <see cref="IAsyncNotification{T}"/>.
  /// </summary>
  public static partial class AsyncObservableExtensions
  {
    /// <summary>
    /// Subscribes an asynchronous element handler to an observable sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <param name="source">Observable sequence to subscribe to.</param>
    /// <param name="onNextAsync">Asynchronous action to invoke for each element in the observable sequence.</param>
    /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
    public static IDisposable SubscribeAsync<T>(
      this IObservable<IAsyncNotification<T>> source,
      Func<T, Task> onNextAsync)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextAsync != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(CreateOnNextAsync(onNextAsync));
    }

    /// <summary>
    /// Subscribes an asynchronous element handler to an observable sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <param name="source">Observable sequence to subscribe to.</param>
    /// <param name="onNextAsync">Asynchronous action to invoke for each element in the observable sequence.</param>
    /// <param name="onError">Action to invoke upon exceptional termination of the observable sequence.</param>
    /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
    public static IDisposable SubscribeAsync<T>(
      this IObservable<IAsyncNotification<T>> source,
      Func<T, Task> onNextAsync,
      Action<Exception> onError)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextAsync != null);
      Contract.Requires(onError != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(CreateOnNextAsync(onNextAsync), onError);
    }

    /// <summary>
    /// Subscribes an asynchronous element handler to an observable sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <param name="source">Observable sequence to subscribe to.</param>
    /// <param name="onNextAsync">Asynchronous action to invoke for each element in the observable sequence.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination of the observable sequence.</param>
    /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
    public static IDisposable SubscribeAsync<T>(
      this IObservable<IAsyncNotification<T>> source,
      Func<T, Task> onNextAsync,
      Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextAsync != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(CreateOnNextAsync(onNextAsync), onCompleted);
    }

    /// <summary>
    /// Subscribes an asynchronous element handler to an observable sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <param name="source">Observable sequence to subscribe to.</param>
    /// <param name="onNextAsync">Asynchronous action to invoke for each element in the observable sequence.</param>
    /// <param name="onError">Action to invoke upon exceptional termination of the observable sequence.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination of the observable sequence.</param>
    /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
    public static IDisposable SubscribeAsync<T>(
      this IObservable<IAsyncNotification<T>> source,
      Func<T, Task> onNextAsync,
      Action<Exception> onError,
      Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextAsync != null);
      Contract.Requires(onError != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(CreateOnNextAsync(onNextAsync), onError, onCompleted);
    }

    /// <summary>
    /// Subscribes an asynchronous element handler to an observable sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TTaskResult">The type of the asynchronous result returned by the element handler.</typeparam>
    /// <param name="source">Observable sequence to subscribe to.</param>
    /// <param name="onNextAsync">Asynchronous action to invoke for each element in the observable sequence.</param>
    /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
    public static IDisposable SubscribeAsync<T, TTaskResult>(
      this IObservable<IAsyncNotification<T, TTaskResult>> source,
      Func<T, Task<TTaskResult>> onNextAsync)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextAsync != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(CreateOnNextAsync(onNextAsync));
    }

    /// <summary>
    /// Subscribes an asynchronous element handler to an observable sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TTaskResult">The type of the asynchronous result returned by the element handler.</typeparam>
    /// <param name="source">Observable sequence to subscribe to.</param>
    /// <param name="onNextAsync">Asynchronous action to invoke for each element in the observable sequence.</param>
    /// <param name="onError">Action to invoke upon exceptional termination of the observable sequence.</param>
    /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
    public static IDisposable SubscribeAsync<T, TTaskResult>(
      this IObservable<IAsyncNotification<T, TTaskResult>> source,
      Func<T, Task<TTaskResult>> onNextAsync,
      Action<Exception> onError)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextAsync != null);
      Contract.Requires(onError != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(CreateOnNextAsync(onNextAsync), onError);
    }

    /// <summary>
    /// Subscribes an asynchronous element handler to an observable sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TTaskResult">The type of the asynchronous result returned by the element handler.</typeparam>
    /// <param name="source">Observable sequence to subscribe to.</param>
    /// <param name="onNextAsync">Asynchronous action to invoke for each element in the observable sequence.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination of the observable sequence.</param>
    /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
    public static IDisposable SubscribeAsync<T, TTaskResult>(
      this IObservable<IAsyncNotification<T, TTaskResult>> source,
      Func<T, Task<TTaskResult>> onNextAsync,
      Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextAsync != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(CreateOnNextAsync(onNextAsync), onCompleted);
    }

    /// <summary>
    /// Subscribes an asynchronous element handler to an observable sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TTaskResult">The type of the asynchronous result returned by the element handler.</typeparam>
    /// <param name="source">Observable sequence to subscribe to.</param>
    /// <param name="onNextAsync">Asynchronous action to invoke for each element in the observable sequence.</param>
    /// <param name="onError">Action to invoke upon exceptional termination of the observable sequence.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination of the observable sequence.</param>
    /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
    public static IDisposable SubscribeAsync<T, TTaskResult>(
      this IObservable<IAsyncNotification<T, TTaskResult>> source,
      Func<T, Task<TTaskResult>> onNextAsync,
      Action<Exception> onError,
      Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNextAsync != null);
      Contract.Requires(onError != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(CreateOnNextAsync(onNextAsync), onError, onCompleted);
    }

    private static Action<IAsyncNotification<T>> CreateOnNextAsync<T>(Func<T, Task> onNextAsync)
    {
      Contract.Requires(onNextAsync != null);
      Contract.Ensures(Contract.Result<Action<IAsyncNotification<T>>>() != null);

      return async notification =>
      {
        Contract.Requires(notification != null);

        try
        {
          await onNextAsync(notification.Value);
        }
        catch (OperationCanceledException)
        {
          notification.TrySetCanceled();
          return;
        }
        catch (Exception ex)
        {
          notification.TrySetException(ex);
          throw;
        }

        notification.TrySetCompleted();
      };
    }

    private static Action<IAsyncNotification<T, TResult>> CreateOnNextAsync<T, TResult>(Func<T, Task<TResult>> onNextAsync)
    {
      Contract.Requires(onNextAsync != null);
      Contract.Ensures(Contract.Result<Action<IAsyncNotification<T, TResult>>>() != null);

      return async notification =>
      {
        Contract.Requires(notification != null);

        TResult result;
        try
        {
          result = await onNextAsync(notification.Value);
        }
        catch (OperationCanceledException)
        {
          notification.TrySetCanceled();
          return;
        }
        catch (Exception ex)
        {
          notification.TrySetException(ex);
          throw;
        }

        notification.TrySetCompleted(result);
      };
    }
  }
}