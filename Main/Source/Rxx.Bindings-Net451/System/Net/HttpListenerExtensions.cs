using System.Diagnostics.Contracts;
#if NET_45
using System.Net.WebSockets;
#endif
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace System.Net
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for creating HTTP request observables from <see cref="HttpListener"/> objects.
  /// </summary>
  public static class HttpListenerExtensions
  {
    /// <summary>
    /// Returns an observable of concurrent HTTP requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="StartObservable(HttpListener)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="StartObservable(HttpListener)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="listener">The object that listens for requests.</param>
    /// <returns>An observable of concurrent HTTP requests.</returns>
    public static IObservable<HttpListenerContext> StartObservable(this HttpListener listener)
    {
      Contract.Requires(listener != null);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerContext>>() != null);

      Contract.Assume(ConcurrencyEnvironment.DefaultMaximumConcurrency > 0);

      return StartObservable(listener, ConcurrencyEnvironment.DefaultMaximumConcurrency);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="StartObservable(HttpListener,int)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="StartObservable(HttpListener,int)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="listener">The object that listens for requests.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP requests.</returns>
    public static IObservable<HttpListenerContext> StartObservable(this HttpListener listener, int maxConcurrent)
    {
      Contract.Requires(listener != null);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerContext>>() != null);

      return listener.StartObservable(maxConcurrent, () => Observable.StartAsync(listener.GetContextAsync));
    }

#if NET_45
    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartObservableWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartObservableWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="listener">The object that listens for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartObservableWebSockets(
      this HttpListener listener,
      string subProtocol)
    {
      Contract.Requires(listener != null);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      Contract.Assume(ConcurrencyEnvironment.DefaultMaximumConcurrency > 0);

      return StartObservableWebSockets(listener, subProtocol, ConcurrencyEnvironment.DefaultMaximumConcurrency);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartObservableWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartObservableWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="listener">The object that listens for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="receiveBufferSize">The size of the buffer for receiving data.</param>
    /// <param name="keepAliveInterval">The keep-alive interval for the socket connection.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartObservableWebSockets(
      this HttpListener listener,
      string subProtocol,
      int receiveBufferSize,
      TimeSpan keepAliveInterval)
    {
      Contract.Requires(listener != null);
      Contract.Requires(receiveBufferSize >= 256);
      Contract.Requires(receiveBufferSize <= 67108864);
      Contract.Requires(keepAliveInterval >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      Contract.Assume(ConcurrencyEnvironment.DefaultMaximumConcurrency > 0);

      return StartObservableWebSockets(listener, subProtocol, receiveBufferSize, keepAliveInterval, ConcurrencyEnvironment.DefaultMaximumConcurrency);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartObservableWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartObservableWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="listener">The object that listens for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartObservableWebSockets(
      this HttpListener listener,
      string subProtocol,
      int maxConcurrent)
    {
      Contract.Requires(listener != null);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      return listener.StartObservable(
        maxConcurrent,
        () => from context in Observable.StartAsync(listener.GetContextAsync)
              from webSocketContext in context.AcceptWebSocketAsync(subProtocol)
              select webSocketContext);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartObservableWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartObservableWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="listener">The object that listens for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="receiveBufferSize">The size of the buffer for receiving data.</param>
    /// <param name="keepAliveInterval">The keep-alive interval for the socket connection.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartObservableWebSockets(
      this HttpListener listener,
      string subProtocol,
      int receiveBufferSize,
      TimeSpan keepAliveInterval,
      int maxConcurrent)
    {
      Contract.Requires(listener != null);
      Contract.Requires(receiveBufferSize >= 256);
      Contract.Requires(receiveBufferSize <= 67108864);
      Contract.Requires(keepAliveInterval >= TimeSpan.Zero);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      return listener.StartObservable(
        maxConcurrent,
        () => from context in Observable.StartAsync(listener.GetContextAsync)
              from webSocketContext in context.AcceptWebSocketAsync(subProtocol, receiveBufferSize, keepAliveInterval)
              select webSocketContext);
    }
#endif

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "Exception is passed to observers.")]
    private static IObservable<TContext> StartObservable<TContext>(
      this HttpListener listener,
      int maxConcurrent,
      Func<IObservable<TContext>> getRequest)
    {
      Contract.Requires(listener != null);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<TContext>>() != null);

      return Observable.Create<TContext>(
        observer =>
        {
          try
          {
            if (!listener.IsListening)
            {
              listener.Start();
            }
          }
          catch (Exception ex)
          {
            observer.OnError(ex);

            return Disposable.Empty;
          }

          return getRequest
            .Serve(maxConcurrent)
            .Finally(() =>
            {
              try
              {
                listener.Stop();
              }
              catch (ObjectDisposedException)
              {
              }
            })
            .SubscribeSafe(observer);
        });
    }
  }
}