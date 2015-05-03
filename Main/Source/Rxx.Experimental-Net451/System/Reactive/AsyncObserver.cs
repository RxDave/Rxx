using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace System.Reactive
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for observers of <see cref="IAsyncNotification{T}"/>.
  /// </summary>
  public static partial class AsyncObserver
  {
    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task OnNextAsync<TValue>(this IObserver<IAsyncNotification<TValue>> observer, TValue value)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue>(value));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="cancel">An object that is signaled when the operation has been cancelled.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task OnNextAsync<TValue>(this IObserver<IAsyncNotification<TValue>> observer, TValue value, CancellationToken cancel)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue>(value, cancel));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="state">An object that provides additional state for the operation.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task OnNextAsync<TValue>(this IObserver<IAsyncNotification<TValue>> observer, TValue value, object state)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue>(value, state));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="state">An object that provides additional state for the operation.</param>
    /// <param name="cancel">An object that is signaled when the operation has been cancelled.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task OnNextAsync<TValue>(this IObserver<IAsyncNotification<TValue>> observer, TValue value, object state, CancellationToken cancel)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue>(value, state, cancel));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="creationOptions">Options for the creation of the returned <see cref="Task{TResult}"/></param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task OnNextAsync<TValue>(this IObserver<IAsyncNotification<TValue>> observer, TValue value, TaskCreationOptions creationOptions)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue>(value, creationOptions));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="creationOptions">Options for the creation of the returned <see cref="Task{TResult}"/></param>
    /// <param name="cancel">An object that is signaled when the operation has been cancelled.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task OnNextAsync<TValue>(this IObserver<IAsyncNotification<TValue>> observer, TValue value, TaskCreationOptions creationOptions, CancellationToken cancel)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue>(value, creationOptions, cancel));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="state">An object that provides additional state for the operation.</param>
    /// <param name="creationOptions">Options for the creation of the returned <see cref="Task{TResult}"/></param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task OnNextAsync<TValue>(this IObserver<IAsyncNotification<TValue>> observer, TValue value, object state, TaskCreationOptions creationOptions)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue>(value, state, creationOptions));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="state">An object that provides additional state for the operation.</param>
    /// <param name="creationOptions">Options for the creation of the returned <see cref="Task{TResult}"/></param>
    /// <param name="cancel">An object that is signaled when the operation has been cancelled.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task OnNextAsync<TValue>(this IObserver<IAsyncNotification<TValue>> observer, TValue value, object state, TaskCreationOptions creationOptions, CancellationToken cancel)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue>(value, state, creationOptions, cancel));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current asynchronous notification information.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task OnNextAsync<TValue>(this IObserver<IAsyncNotification<TValue>> observer, IAsyncNotification<TValue> value)
    {
      Contract.Requires(observer != null);
      Contract.Requires(value != null);
      Contract.Ensures(Contract.Result<Task>() != null);

      observer.OnNext(value);

      return value.Task;
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the asynchronous result.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task<TResult> OnNextAsync<TValue, TResult>(this IObserver<IAsyncNotification<TValue, TResult>> observer, TValue value)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task<TResult>>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue, TResult>(value));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the asynchronous result.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="cancel">An object that is signaled when the operation has been cancelled.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task<TResult> OnNextAsync<TValue, TResult>(this IObserver<IAsyncNotification<TValue, TResult>> observer, TValue value, CancellationToken cancel)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task<TResult>>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue, TResult>(value, cancel));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the asynchronous result.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="state">An object that provides additional state for the operation.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task<TResult> OnNextAsync<TValue, TResult>(this IObserver<IAsyncNotification<TValue, TResult>> observer, TValue value, object state)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task<TResult>>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue, TResult>(value, state));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the asynchronous result.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="state">An object that provides additional state for the operation.</param>
    /// <param name="cancel">An object that is signaled when the operation has been cancelled.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task<TResult> OnNextAsync<TValue, TResult>(this IObserver<IAsyncNotification<TValue, TResult>> observer, TValue value, object state, CancellationToken cancel)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task<TResult>>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue, TResult>(value, state, cancel));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the asynchronous result.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="creationOptions">Options for the creation of the returned <see cref="Task{TResult}"/></param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task<TResult> OnNextAsync<TValue, TResult>(this IObserver<IAsyncNotification<TValue, TResult>> observer, TValue value, TaskCreationOptions creationOptions)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task<TResult>>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue, TResult>(value, creationOptions));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the asynchronous result.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="creationOptions">Options for the creation of the returned <see cref="Task{TResult}"/></param>
    /// <param name="cancel">An object that is signaled when the operation has been cancelled.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task<TResult> OnNextAsync<TValue, TResult>(this IObserver<IAsyncNotification<TValue, TResult>> observer, TValue value, TaskCreationOptions creationOptions, CancellationToken cancel)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task<TResult>>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue, TResult>(value, creationOptions, cancel));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the asynchronous result.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="state">An object that provides additional state for the operation.</param>
    /// <param name="creationOptions">Options for the creation of the returned <see cref="Task{TResult}"/></param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task<TResult> OnNextAsync<TValue, TResult>(this IObserver<IAsyncNotification<TValue, TResult>> observer, TValue value, object state, TaskCreationOptions creationOptions)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task<TResult>>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue, TResult>(value, state, creationOptions));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the asynchronous result.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current notification information.</param>
    /// <param name="state">An object that provides additional state for the operation.</param>
    /// <param name="creationOptions">Options for the creation of the returned <see cref="Task{TResult}"/></param>
    /// <param name="cancel">An object that is signaled when the operation has been cancelled.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task<TResult> OnNextAsync<TValue, TResult>(this IObserver<IAsyncNotification<TValue, TResult>> observer, TValue value, object state, TaskCreationOptions creationOptions, CancellationToken cancel)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<Task<TResult>>() != null);

      return OnNextAsync(observer, new AsyncNotification<TValue, TResult>(value, state, creationOptions, cancel));
    }

    /// <summary>
    /// Provides the asynchronous observer with new data.
    /// </summary>
    /// <typeparam name="TValue">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of the asynchronous result.</typeparam>
    /// <param name="observer">An asynchronous observer.</param>
    /// <param name="value">The current asynchronous notification information.</param>
    /// <returns>A <see cref="Task"/> that is signaled when the asynchronous observation completes.</returns>
    public static Task<TResult> OnNextAsync<TValue, TResult>(this IObserver<IAsyncNotification<TValue, TResult>> observer, IAsyncNotification<TValue, TResult> value)
    {
      Contract.Requires(observer != null);
      Contract.Requires(value != null);
      Contract.Ensures(Contract.Result<Task<TResult>>() != null);

      observer.OnNext(value);

      return value.Task;
    }
  }
}