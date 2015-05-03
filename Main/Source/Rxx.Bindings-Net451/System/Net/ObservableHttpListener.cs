using System.Diagnostics.Contracts;
#if NET_45
using System.Net.WebSockets;
#endif
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace System.Net
{
  /// <summary>
  /// Provides <see langword="static"/> methods for creating HTTP request observables.
  /// </summary>
  public static class ObservableHttpListener
  {
    /// <summary>
    /// Returns an observable of concurrent HTTP requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(IPEndPoint)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(IPEndPoint)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for requests.</param>
    /// <returns>An observable of concurrent HTTP requests.</returns>
    public static IObservable<HttpListenerContext> Start(IPEndPoint endPoint)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerContext>>() != null);

      return Start(endPoint, null);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(IPEndPoint,int)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(IPEndPoint,int)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for requests.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP requests.</returns>
    public static IObservable<HttpListenerContext> Start(IPEndPoint endPoint, int maxConcurrent)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerContext>>() != null);

      return Start(endPoint, null, maxConcurrent);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(IPEndPoint,string)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(IPEndPoint,string)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for requests.</param>
    /// <param name="path">The path at which requests will be served.</param>
    /// <returns>An observable of concurrent HTTP requests.</returns>
    public static IObservable<HttpListenerContext> Start(IPEndPoint endPoint, string path)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerContext>>() != null);

      var address = endPoint.Address == IPAddress.Any ? "+" : endPoint.Address.ToString();

      Contract.Assume(!string.IsNullOrEmpty(address));

      return Start(address, endPoint.Port, path);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(IPEndPoint,string,int)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(IPEndPoint,string,int)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for requests.</param>
    /// <param name="path">The path at which requests will be served.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP requests.</returns>
    public static IObservable<HttpListenerContext> Start(IPEndPoint endPoint, string path, int maxConcurrent)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerContext>>() != null);

      var address = endPoint.Address == IPAddress.Any ? "+" : endPoint.Address.ToString();

      Contract.Assume(!string.IsNullOrEmpty(address));

      return Start(address, endPoint.Port, path, maxConcurrent);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(string)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(string)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <returns>An observable of concurrent HTTP requests.</returns>
    public static IObservable<HttpListenerContext> Start(string domain)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Ensures(Contract.Result<IObservable<HttpListenerContext>>() != null);

      return Start(domain, 80, null);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(string,int)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(string,int)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="port">The port number on which to listen for requests.</param>
    /// <returns>An observable of concurrent HTTP requests.</returns>
    public static IObservable<HttpListenerContext> Start(string domain, int port)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerContext>>() != null);

      return Start(domain, port, null);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(string,int,int)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(string,int,int)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="port">The port number on which to listen for requests.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP requests.</returns>
    public static IObservable<HttpListenerContext> Start(string domain, int port, int maxConcurrent)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerContext>>() != null);

      return Start(domain, port, null, maxConcurrent);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(string,int,string)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(string,int,string)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="port">The port number on which to listen for requests.</param>
    /// <param name="path">The path at which requests will be served.</param>
    /// <returns>An observable of concurrent HTTP requests.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The listener variable is disposed by Observable.Using.")]
    public static IObservable<HttpListenerContext> Start(string domain, int port, string path)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerContext>>() != null);

      Contract.Assume(ConcurrencyEnvironment.DefaultMaximumConcurrency > 0);

      return Start(domain, port, path, ConcurrencyEnvironment.DefaultMaximumConcurrency);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(string,int,string,int)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(string,int,string,int)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="port">The port number on which to listen for requests.</param>
    /// <param name="path">The path at which requests will be served.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP requests.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
          Justification = "The listener variable is disposed by Observable.Using.")]
    public static IObservable<HttpListenerContext> Start(string domain, int port, string path, int maxConcurrent)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerContext>>() != null);

      var url = CreatePrefixUrl(domain, port, path);

      return Observable.Using(
        () =>
        {
          var listener = new HttpListener();

          listener.Prefixes.Add(url);

          return listener;
        },
        listener => listener.StartObservable(maxConcurrent));
    }

#if NET_45
    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      IPEndPoint endPoint,
      string subProtocol)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      return StartWebSockets(endPoint, null, subProtocol);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="receiveBufferSize">The size of the buffer for receiving data.</param>
    /// <param name="keepAliveInterval">The keep-alive interval for the socket connection.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      IPEndPoint endPoint,
      string subProtocol,
      int receiveBufferSize,
      TimeSpan keepAliveInterval)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Requires(receiveBufferSize >= 256);
      Contract.Requires(receiveBufferSize <= 67108864);
      Contract.Requires(keepAliveInterval >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      return StartWebSockets(endPoint, null, subProtocol, receiveBufferSize, keepAliveInterval);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      IPEndPoint endPoint,
      string subProtocol,
      int maxConcurrent)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      return StartWebSockets(endPoint, null, subProtocol, maxConcurrent);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="receiveBufferSize">The size of the buffer for receiving data.</param>
    /// <param name="keepAliveInterval">The keep-alive interval for the socket connection.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      IPEndPoint endPoint,
      string subProtocol,
      int receiveBufferSize,
      TimeSpan keepAliveInterval,
      int maxConcurrent)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Requires(receiveBufferSize >= 256);
      Contract.Requires(receiveBufferSize <= 67108864);
      Contract.Requires(keepAliveInterval >= TimeSpan.Zero);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      return StartWebSockets(endPoint, null, subProtocol, receiveBufferSize, keepAliveInterval, maxConcurrent);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for requests.</param>
    /// <param name="path">The path at which requests will be served.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      IPEndPoint endPoint,
      string path,
      string subProtocol)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      var address = endPoint.Address == IPAddress.Any ? "+" : endPoint.Address.ToString();

      Contract.Assume(!string.IsNullOrEmpty(address));

      return StartWebSockets(address, endPoint.Port, path, subProtocol);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for requests.</param>
    /// <param name="path">The path at which requests will be served.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="receiveBufferSize">The size of the buffer for receiving data.</param>
    /// <param name="keepAliveInterval">The keep-alive interval for the socket connection.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      IPEndPoint endPoint,
      string path,
      string subProtocol,
      int receiveBufferSize,
      TimeSpan keepAliveInterval)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Requires(receiveBufferSize >= 256);
      Contract.Requires(receiveBufferSize <= 67108864);
      Contract.Requires(keepAliveInterval >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      var address = endPoint.Address == IPAddress.Any ? "+" : endPoint.Address.ToString();

      Contract.Assume(!string.IsNullOrEmpty(address));

      return StartWebSockets(address, endPoint.Port, path, subProtocol, receiveBufferSize, keepAliveInterval);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for requests.</param>
    /// <param name="path">The path at which requests will be served.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      IPEndPoint endPoint,
      string path,
      string subProtocol,
      int maxConcurrent)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      var address = endPoint.Address == IPAddress.Any ? "+" : endPoint.Address.ToString();

      Contract.Assume(!string.IsNullOrEmpty(address));

      return StartWebSockets(address, endPoint.Port, path, subProtocol, maxConcurrent);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for requests.</param>
    /// <param name="path">The path at which requests will be served.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="receiveBufferSize">The size of the buffer for receiving data.</param>
    /// <param name="keepAliveInterval">The keep-alive interval for the socket connection.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      IPEndPoint endPoint,
      string path,
      string subProtocol,
      int receiveBufferSize,
      TimeSpan keepAliveInterval,
      int maxConcurrent)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Requires(receiveBufferSize >= 256);
      Contract.Requires(receiveBufferSize <= 67108864);
      Contract.Requires(keepAliveInterval >= TimeSpan.Zero);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      var address = endPoint.Address == IPAddress.Any ? "+" : endPoint.Address.ToString();

      Contract.Assume(!string.IsNullOrEmpty(address));

      return StartWebSockets(address, endPoint.Port, path, subProtocol, receiveBufferSize, keepAliveInterval, maxConcurrent);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      string domain,
      string subProtocol)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      return StartWebSockets(domain, 80, null, subProtocol);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="receiveBufferSize">The size of the buffer for receiving data.</param>
    /// <param name="keepAliveInterval">The keep-alive interval for the socket connection.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      string domain,
      string subProtocol,
      int receiveBufferSize,
      TimeSpan keepAliveInterval)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(receiveBufferSize >= 256);
      Contract.Requires(receiveBufferSize <= 67108864);
      Contract.Requires(keepAliveInterval >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      return StartWebSockets(domain, 80, null, subProtocol, receiveBufferSize, keepAliveInterval);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="port">The port number on which to listen for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      string domain,
      int port,
      string subProtocol)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      return StartWebSockets(domain, port, null, subProtocol);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="port">The port number on which to listen for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="receiveBufferSize">The size of the buffer for receiving data.</param>
    /// <param name="keepAliveInterval">The keep-alive interval for the socket connection.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      string domain,
      int port,
      string subProtocol,
      int receiveBufferSize,
      TimeSpan keepAliveInterval)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Requires(receiveBufferSize >= 256);
      Contract.Requires(receiveBufferSize <= 67108864);
      Contract.Requires(keepAliveInterval >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      return StartWebSockets(domain, port, null, subProtocol, receiveBufferSize, keepAliveInterval);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="port">The port number on which to listen for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      string domain,
      int port,
      string subProtocol,
      int maxConcurrent)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      return StartWebSockets(domain, port, null, subProtocol, maxConcurrent);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="port">The port number on which to listen for requests.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="receiveBufferSize">The size of the buffer for receiving data.</param>
    /// <param name="keepAliveInterval">The keep-alive interval for the socket connection.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      string domain,
      int port,
      string subProtocol,
      int receiveBufferSize,
      TimeSpan keepAliveInterval,
      int maxConcurrent)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Requires(receiveBufferSize >= 256);
      Contract.Requires(receiveBufferSize <= 67108864);
      Contract.Requires(keepAliveInterval >= TimeSpan.Zero);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      return StartWebSockets(domain, port, null, subProtocol, receiveBufferSize, keepAliveInterval, maxConcurrent);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="port">The port number on which to listen for requests.</param>
    /// <param name="path">The path at which requests will be served.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The listener variable is disposed by Observable.Using.")]
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      string domain,
      int port,
      string path,
      string subProtocol)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      Contract.Assume(ConcurrencyEnvironment.DefaultMaximumConcurrency > 0);

      return StartWebSockets(domain, port, path, subProtocol, ConcurrencyEnvironment.DefaultMaximumConcurrency);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="port">The port number on which to listen for requests.</param>
    /// <param name="path">The path at which requests will be served.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="receiveBufferSize">The size of the buffer for receiving data.</param>
    /// <param name="keepAliveInterval">The keep-alive interval for the socket connection.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The listener variable is disposed by Observable.Using.")]
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      string domain,
      int port,
      string path,
      string subProtocol,
      int receiveBufferSize,
      TimeSpan keepAliveInterval)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Requires(receiveBufferSize >= 256);
      Contract.Requires(receiveBufferSize <= 67108864);
      Contract.Requires(keepAliveInterval >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      Contract.Assume(ConcurrencyEnvironment.DefaultMaximumConcurrency > 0);

      return StartWebSockets(domain, port, path, subProtocol, receiveBufferSize, keepAliveInterval, ConcurrencyEnvironment.DefaultMaximumConcurrency);
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="port">The port number on which to listen for requests.</param>
    /// <param name="path">The path at which requests will be served.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The listener variable is disposed by Observable.Using.")]
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      string domain,
      int port,
      string path,
      string subProtocol,
      int maxConcurrent)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      var url = CreatePrefixUrl(domain, port, path);

      return Observable.Using(
        () =>
        {
          var listener = new HttpListener();

          listener.Prefixes.Add(url);

          return listener;
        },
        listener => listener.StartObservableWebSockets(subProtocol, maxConcurrent));
    }

    /// <summary>
    /// Returns an observable of concurrent HTTP web socket requests.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <strong>StartWebSockets</strong> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple requests concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <strong>StartWebSockets</strong> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="domain">The domain name on which to listen for requests.</param>
    /// <param name="port">The port number on which to listen for requests.</param>
    /// <param name="path">The path at which requests will be served.</param>
    /// <param name="subProtocol">The sub-protocol in which the web socket communicates.</param>
    /// <param name="receiveBufferSize">The size of the buffer for receiving data.</param>
    /// <param name="keepAliveInterval">The keep-alive interval for the socket connection.</param>
    /// <param name="maxConcurrent">The maximum number of requests that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent HTTP web socket requests.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
          Justification = "The listener variable is disposed by Observable.Using.")]
    public static IObservable<HttpListenerWebSocketContext> StartWebSockets(
      string domain,
      int port,
      string path,
      string subProtocol,
      int receiveBufferSize,
      TimeSpan keepAliveInterval,
      int maxConcurrent)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Requires(receiveBufferSize >= 256);
      Contract.Requires(receiveBufferSize <= 67108864);
      Contract.Requires(keepAliveInterval >= TimeSpan.Zero);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<HttpListenerWebSocketContext>>() != null);

      var url = CreatePrefixUrl(domain, port, path);

      return Observable.Using(
        () =>
        {
          var listener = new HttpListener();

          listener.Prefixes.Add(url);

          return listener;
        },
        listener => listener.StartObservableWebSockets(subProtocol, receiveBufferSize, keepAliveInterval, maxConcurrent));
    }
#endif

    private static string CreatePrefixUrl(string domain, int port, string path)
    {
      Contract.Requires(!string.IsNullOrEmpty(domain));
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

      const string wildcardPlaceholder = "w";

      var wildcard = domain == "*" || domain == "+";

      var url = new Uri(Uri.UriSchemeHttp + Uri.SchemeDelimiter + (wildcard ? wildcardPlaceholder : domain) + ":" + port + "/" + path);

      return wildcard
           ? url.ToString().Replace(Uri.SchemeDelimiter + wildcardPlaceholder, Uri.SchemeDelimiter + domain)
           : url.ToString();
    }
  }
}