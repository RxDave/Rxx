using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace System.Net.Sockets
{
  /// <summary>
  /// Provides <see langword="static"/> methods for creating TCP connection observables.
  /// </summary>
  public static class ObservableTcpListener
  {
    /// <summary>
    /// Returns an observable of concurrent TCP connections.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(IPEndPoint)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple clients concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(IPEndPoint)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="address">The local IP address on which to listen for clients.</param>
    /// <param name="port">The local port number on which to listen for clients.</param>
    /// <returns>An observable of concurrent TCP connections.</returns>
    public static IObservable<TcpClient> Start(IPAddress address, int port)
    {
      Contract.Requires(address != null);
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<TcpClient>>() != null);

      return Start(address, port, ConcurrencyEnvironment.DefaultMaximumConcurrency);
    }

    /// <summary>
    /// Returns an observable of concurrent TCP connections.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(IPEndPoint)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple clients concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(IPEndPoint)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for clients.</param>
    /// <returns>An observable of concurrent TCP connections.</returns>
    public static IObservable<TcpClient> Start(IPEndPoint endPoint)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<TcpClient>>() != null);

      var maxConcurrent = ConcurrencyEnvironment.DefaultMaximumConcurrency;

      Contract.Assume(maxConcurrent > 0);

      return Start(endPoint, maxConcurrent);
    }

    /// <summary>
    /// Returns an observable of concurrent TCP connections.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(IPEndPoint)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple clients concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(IPEndPoint)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="address">The local IP address on which to listen for clients.</param>
    /// <param name="port">The local port number on which to listen for clients.</param>
    /// <param name="maxConcurrent">The maximum number of clients that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent TCP connections.</returns>
    public static IObservable<TcpClient> Start(IPAddress address, int port, int maxConcurrent)
    {
      Contract.Requires(address != null);
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<TcpClient>>() != null);

      return Observable.Defer(() =>
        {
          var listener = new TcpListener(address, port);

          return listener.StartObservable(maxConcurrent).Finally(listener.Stop);
        });
    }

    /// <summary>
    /// Returns an observable of concurrent TCP connections.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="Start(IPEndPoint,int)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple clients concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="Start(IPEndPoint,int)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="endPoint">The local end point on which to listen for clients.</param>
    /// <param name="maxConcurrent">The maximum number of clients that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent TCP connections.</returns>
    public static IObservable<TcpClient> Start(IPEndPoint endPoint, int maxConcurrent)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(endPoint.Address != null);
      Contract.Requires(endPoint.Port >= IPEndPoint.MinPort && endPoint.Port <= IPEndPoint.MaxPort);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<TcpClient>>() != null);

      return Observable.Defer(() =>
        {
          var listener = new TcpListener(endPoint);

          return listener.StartObservable(maxConcurrent).Finally(listener.Stop);
        });
    }
  }
}