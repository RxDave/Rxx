using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace System.Net.Sockets
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for creating TCP connection observables from <see cref="TcpListener"/> objects.
  /// </summary>
  public static class TcpListenerExtensions
  {
    /// <summary>
    /// Returns an observable of concurrent TCP connections.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="StartObservable(TcpListener)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple clients concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="StartObservable(TcpListener)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="listener">The object that listens for incoming TCP connections.</param>
    /// <returns>An observable of concurrent TCP connections.</returns>
    public static IObservable<TcpClient> StartObservable(this TcpListener listener)
    {
      Contract.Requires(listener != null);
      Contract.Ensures(Contract.Result<IObservable<TcpClient>>() != null);

      Contract.Assume(ConcurrencyEnvironment.DefaultMaximumConcurrency > 0);

      return StartObservable(listener, ConcurrencyEnvironment.DefaultMaximumConcurrency);
    }

    /// <summary>
    /// Returns an observable of concurrent TCP connections.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="StartObservable(TcpListener,int)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple clients concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="StartObservable(TcpListener,int)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="listener">The object that listens for incoming TCP connections.</param>
    /// <param name="maxConcurrent">The maximum number of connections that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent TCP connections.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "Exception is passed to observers.")]
    public static IObservable<TcpClient> StartObservable(this TcpListener listener, int maxConcurrent)
    {
      Contract.Requires(listener != null);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<TcpClient>>() != null);

      return listener.StartObservable(
        () => Observable.StartAsync(listener.AcceptTcpClientAsync),
        maxConcurrent);
    }

    /// <summary>
    /// Returns an observable of concurrent TCP socket connections.  (See the remarks section for important details about the concurrent behavior.)
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// <see cref="StartSocketObservable(TcpListener,int)"/> returns an observable that may send notifications concurrently.  This behavior allows 
    /// observers to receive multiple sockets concurrently, as is common in hosting scenarios; however, it violates an important contract 
    /// in Rx.  The Rx Design Guidelines document states that Rx operators assume notifications are sent in a serialized fashion.  The only 
    /// methods that do not conflict with this guideline and are safe to call on the observable that is returned by 
    /// <see cref="StartSocketObservable(TcpListener,int)"/> are <strong>Subscribe</strong> and <strong>Synchronize</strong>.  Do not attempt to use 
    /// any other Rx operators unless the observable is synchronized first.
    /// </alert>
    /// </remarks>
    /// <param name="listener">The object that listens for incoming TCP socket connections.</param>
    /// <param name="maxConcurrent">The maximum number of sockets that can be pushed through the observable simultaneously.</param>
    /// <returns>An observable of concurrent TCP socket connections.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "Exception is passed to observers.")]
    public static IObservable<Socket> StartSocketObservable(this TcpListener listener, int maxConcurrent)
    {
      Contract.Requires(listener != null);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<Socket>>() != null);

      return listener.StartObservable(
        () => Observable.StartAsync(listener.AcceptSocketAsync),
        maxConcurrent);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "Exception is passed to observers.")]
    private static IObservable<TResult> StartObservable<TResult>(this TcpListener listener, Func<IObservable<TResult>> getRequest, int maxConcurrent)
    {
      Contract.Requires(listener != null);
      Contract.Requires(getRequest != null);
      Contract.Requires(maxConcurrent > 0);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      return Observable.Create<TResult>(
        observer =>
        {
          try
          {
            if (!listener.Server.IsBound)
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