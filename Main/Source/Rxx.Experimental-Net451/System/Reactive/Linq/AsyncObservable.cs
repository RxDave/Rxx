using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace System.Reactive.Linq
{
  /// <summary>
  /// Provides <see langword="static"/> factory methods for creating asynchronous sequences of <see cref="IAsyncNotification{T}"/>.
  /// </summary>
  public static partial class AsyncObservable
  {
    /// <summary>
    /// Creates an observable sequence from a specified asynchronous <strong>Subscribe</strong> method using asynchronous calls to <strong>OnNext</strong>.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <param name="subscribeAsync">Asynchronous method used to produce elements.</param>
    /// <returns>The observable sequence surfacing the elements produced by the asynchronous method.</returns>
    public static IObservable<IAsyncNotification<TResult>> CreateAsync<TResult>(Func<IObserver<IAsyncNotification<TResult>>, Task> subscribeAsync)
    {
      Contract.Requires(subscribeAsync != null);
      Contract.Ensures(Contract.Result<IObservable<IAsyncNotification<TResult>>>() != null);

      return Observable.Create<IAsyncNotification<TResult>>(subscribeAsync);
    }

    /// <summary>
    /// Creates an observable sequence from a specified asynchronous <strong>Subscribe</strong> method using asynchronous calls to <strong>OnNext</strong>.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <param name="subscribeAsync">Asynchronous method used to produce elements.</param>
    /// <returns>The observable sequence surfacing the elements produced by the asynchronous method.</returns>
    public static IObservable<IAsyncNotification<TResult>> CreateAsync<TResult>(Func<IObserver<IAsyncNotification<TResult>>, Task<Action>> subscribeAsync)
    {
      Contract.Requires(subscribeAsync != null);
      Contract.Ensures(Contract.Result<IObservable<IAsyncNotification<TResult>>>() != null);

      return Observable.Create<IAsyncNotification<TResult>>(subscribeAsync);
    }

    /// <summary>
    /// Creates an observable sequence from a specified asynchronous <strong>Subscribe</strong> method using asynchronous calls to <strong>OnNext</strong>.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <param name="subscribeAsync">Asynchronous method used to produce elements.</param>
    /// <returns>The observable sequence surfacing the elements produced by the asynchronous method.</returns>
    public static IObservable<IAsyncNotification<TResult>> CreateAsync<TResult>(Func<IObserver<IAsyncNotification<TResult>>, Task<IDisposable>> subscribeAsync)
    {
      Contract.Requires(subscribeAsync != null);
      Contract.Ensures(Contract.Result<IObservable<IAsyncNotification<TResult>>>() != null);

      return Observable.Create<IAsyncNotification<TResult>>(subscribeAsync);
    }

    /// <summary>
    /// Creates an observable sequence from a specified cancellable asynchronous <strong>Subscribe</strong> method using asynchronous calls to <strong>OnNext</strong>.
    /// The <see cref="CancellationToken"/> passed to the asynchronous <strong>Subscribe</strong> method is tied to the returned disposable subscription, allowing best-effort cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <param name="subscribeAsync">Asynchronous method used to produce elements.</param>
    /// <returns>The observable sequence surfacing the elements produced by the asynchronous method.</returns>
    public static IObservable<IAsyncNotification<TResult>> CreateAsync<TResult>(Func<IObserver<IAsyncNotification<TResult>>, CancellationToken, Task> subscribeAsync)
    {
      Contract.Requires(subscribeAsync != null);
      Contract.Ensures(Contract.Result<IObservable<IAsyncNotification<TResult>>>() != null);

      return Observable.Create<IAsyncNotification<TResult>>(subscribeAsync);
    }

    /// <summary>
    /// Creates an observable sequence from a specified cancellable asynchronous <strong>Subscribe</strong> method using asynchronous calls to <strong>OnNext</strong>.
    /// The <see cref="CancellationToken"/> passed to the asynchronous <strong>Subscribe</strong> method is tied to the returned disposable subscription, allowing best-effort cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <param name="subscribeAsync">Asynchronous method used to produce elements.</param>
    /// <returns>The observable sequence surfacing the elements produced by the asynchronous method.</returns>
    public static IObservable<IAsyncNotification<TResult>> CreateAsync<TResult>(Func<IObserver<IAsyncNotification<TResult>>, CancellationToken, Task<Action>> subscribeAsync)
    {
      Contract.Requires(subscribeAsync != null);
      Contract.Ensures(Contract.Result<IObservable<IAsyncNotification<TResult>>>() != null);

      return Observable.Create<IAsyncNotification<TResult>>(subscribeAsync);
    }

    /// <summary>
    /// Creates an observable sequence from a specified cancellable asynchronous <strong>Subscribe</strong> method using asynchronous calls to <strong>OnNext</strong>.
    /// The <see cref="CancellationToken"/> passed to the asynchronous <strong>Subscribe</strong> method is tied to the returned disposable subscription, allowing best-effort cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <param name="subscribeAsync">Asynchronous method used to produce elements.</param>
    /// <returns>The observable sequence surfacing the elements produced by the asynchronous method.</returns>
    public static IObservable<IAsyncNotification<TResult>> CreateAsync<TResult>(Func<IObserver<IAsyncNotification<TResult>>, CancellationToken, Task<IDisposable>> subscribeAsync)
    {
      Contract.Requires(subscribeAsync != null);
      Contract.Ensures(Contract.Result<IObservable<IAsyncNotification<TResult>>>() != null);

      return Observable.Create<IAsyncNotification<TResult>>(subscribeAsync);
    }

    /// <summary>
    /// Creates an observable sequence from a specified asynchronous <strong>Subscribe</strong> method using asynchronous calls to <strong>OnNext</strong> with a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <typeparam name="TTaskResult">The type of the asynchronous results from calls to <strong>OnNext</strong>.</typeparam>
    /// <param name="subscribeAsync">Asynchronous method used to produce elements.</param>
    /// <returns>The observable sequence surfacing the elements produced by the asynchronous method.</returns>
    public static IObservable<IAsyncNotification<TResult, TTaskResult>> CreateAsync<TResult, TTaskResult>(Func<IObserver<IAsyncNotification<TResult, TTaskResult>>, Task> subscribeAsync)
    {
      Contract.Requires(subscribeAsync != null);
      Contract.Ensures(Contract.Result<IObservable<IAsyncNotification<TResult, TTaskResult>>>() != null);

      return Observable.Create<IAsyncNotification<TResult, TTaskResult>>(subscribeAsync);
    }

    /// <summary>
    /// Creates an observable sequence from a specified asynchronous <strong>Subscribe</strong> method using asynchronous calls to <strong>OnNext</strong> with a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <typeparam name="TTaskResult">The type of the asynchronous results from calls to <strong>OnNext</strong>.</typeparam>
    /// <param name="subscribeAsync">Asynchronous method used to produce elements.</param>
    /// <returns>The observable sequence surfacing the elements produced by the asynchronous method.</returns>
    public static IObservable<IAsyncNotification<TResult, TTaskResult>> CreateAsync<TResult, TTaskResult>(Func<IObserver<IAsyncNotification<TResult, TTaskResult>>, Task<Action>> subscribeAsync)
    {
      Contract.Requires(subscribeAsync != null);
      Contract.Ensures(Contract.Result<IObservable<IAsyncNotification<TResult, TTaskResult>>>() != null);

      return Observable.Create<IAsyncNotification<TResult, TTaskResult>>(subscribeAsync);
    }

    /// <summary>
    /// Creates an observable sequence from a specified asynchronous <strong>Subscribe</strong> method using asynchronous calls to <strong>OnNext</strong> with a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <typeparam name="TTaskResult">The type of the asynchronous results from calls to <strong>OnNext</strong>.</typeparam>
    /// <param name="subscribeAsync">Asynchronous method used to produce elements.</param>
    /// <returns>The observable sequence surfacing the elements produced by the asynchronous method.</returns>
    public static IObservable<IAsyncNotification<TResult, TTaskResult>> CreateAsync<TResult, TTaskResult>(Func<IObserver<IAsyncNotification<TResult, TTaskResult>>, Task<IDisposable>> subscribeAsync)
    {
      Contract.Requires(subscribeAsync != null);
      Contract.Ensures(Contract.Result<IObservable<IAsyncNotification<TResult, TTaskResult>>>() != null);

      return Observable.Create<IAsyncNotification<TResult, TTaskResult>>(subscribeAsync);
    }

    /// <summary>
    /// Creates an observable sequence from a specified cancellable asynchronous <strong>Subscribe</strong> method using asynchronous calls to <strong>OnNext</strong> with a result.
    /// The <see cref="CancellationToken"/> passed to the asynchronous <strong>Subscribe</strong> method is tied to the returned disposable subscription, allowing best-effort cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <typeparam name="TTaskResult">The type of the asynchronous results from calls to <strong>OnNext</strong>.</typeparam>
    /// <param name="subscribeAsync">Asynchronous method used to produce elements.</param>
    /// <returns>The observable sequence surfacing the elements produced by the asynchronous method.</returns>
    public static IObservable<IAsyncNotification<TResult, TTaskResult>> CreateAsync<TResult, TTaskResult>(Func<IObserver<IAsyncNotification<TResult, TTaskResult>>, CancellationToken, Task> subscribeAsync)
    {
      Contract.Requires(subscribeAsync != null);
      Contract.Ensures(Contract.Result<IObservable<IAsyncNotification<TResult, TTaskResult>>>() != null);

      return Observable.Create<IAsyncNotification<TResult, TTaskResult>>(subscribeAsync);
    }

    /// <summary>
    /// Creates an observable sequence from a specified cancellable asynchronous <strong>Subscribe</strong> method using asynchronous calls to <strong>OnNext</strong> with a result.
    /// The <see cref="CancellationToken"/> passed to the asynchronous <strong>Subscribe</strong> method is tied to the returned disposable subscription, allowing best-effort cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <typeparam name="TTaskResult">The type of the asynchronous results from calls to <strong>OnNext</strong>.</typeparam>
    /// <param name="subscribeAsync">Asynchronous method used to produce elements.</param>
    /// <returns>The observable sequence surfacing the elements produced by the asynchronous method.</returns>
    public static IObservable<IAsyncNotification<TResult, TTaskResult>> CreateAsync<TResult, TTaskResult>(Func<IObserver<IAsyncNotification<TResult, TTaskResult>>, CancellationToken, Task<Action>> subscribeAsync)
    {
      Contract.Requires(subscribeAsync != null);
      Contract.Ensures(Contract.Result<IObservable<IAsyncNotification<TResult, TTaskResult>>>() != null);

      return Observable.Create<IAsyncNotification<TResult, TTaskResult>>(subscribeAsync);
    }

    /// <summary>
    /// Creates an observable sequence from a specified cancellable asynchronous <strong>Subscribe</strong> method using asynchronous calls to <strong>OnNext</strong> with a result.
    /// The <see cref="CancellationToken"/> passed to the asynchronous <strong>Subscribe</strong> method is tied to the returned disposable subscription, allowing best-effort cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <typeparam name="TTaskResult">The type of the asynchronous results from calls to <strong>OnNext</strong>.</typeparam>
    /// <param name="subscribeAsync">Asynchronous method used to produce elements.</param>
    /// <returns>The observable sequence surfacing the elements produced by the asynchronous method.</returns>
    public static IObservable<IAsyncNotification<TResult, TTaskResult>> CreateAsync<TResult, TTaskResult>(Func<IObserver<IAsyncNotification<TResult, TTaskResult>>, CancellationToken, Task<IDisposable>> subscribeAsync)
    {
      Contract.Requires(subscribeAsync != null);
      Contract.Ensures(Contract.Result<IObservable<IAsyncNotification<TResult, TTaskResult>>>() != null);

      return Observable.Create<IAsyncNotification<TResult, TTaskResult>>(subscribeAsync);
    }
  }
}