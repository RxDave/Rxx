using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;
#if !WINDOWS_PHONE
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
#endif

namespace System.Net.Sockets
{
  /// <summary>
  /// Provides <see langword="static" /> extension methods that return observable sequences for accepting, connecting, disconnecting, 
  /// receiving data and sending data via <see cref="Socket"/> objects.
  /// </summary>
  public static partial class SocketExtensions
  {
    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <param name="socket">The socket that will create the connection.</param>
    /// <param name="remoteEndPoint">An <see cref="EndPoint"/> that represents the remote host.</param>
    /// <returns>A singleton observable sequence that indicates when the connection has been established.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<Unit> ConnectObservable(this Socket socket, EndPoint remoteEndPoint)
    {
      Contract.Requires(socket != null);
      Contract.Requires(remoteEndPoint != null);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      var args = new SocketAsyncEventArgs()
      {
        RemoteEndPoint = remoteEndPoint
      };

      return socket
        .ConnectObservable(args)
        .Select(_ => Unit.Default)
        .Finally(args.Dispose);
    }

    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <param name="socket">The socket that will create the connection.</param>
    /// <param name="address">The <see cref="IPAddress"/> of the remote host.</param>
    /// <param name="port">The port number of the remote host.</param>
    /// <returns>A singleton observable sequence that indicates when the connection has been established.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<Unit> ConnectObservable(this Socket socket, IPAddress address, int port)
    {
      Contract.Requires(socket != null);
      Contract.Requires(address != null);
      Contract.Requires(port >= IPEndPoint.MinPort);
      Contract.Requires(port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      var args = new SocketAsyncEventArgs()
      {
        RemoteEndPoint = new IPEndPoint(address, port)
      };

      return socket
        .ConnectObservable(args)
        .Select(_ => Unit.Default)
        .Finally(args.Dispose);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <param name="socket">The socket that will create the connection.</param>
    /// <param name="addresses">At least one <see cref="IPAddress"/>, designating the remote host.</param>
    /// <param name="port">The port number of the remote host.</param>
    /// <returns>A singleton observable sequence that indicates when the connection has been established.</returns>
    public static IObservable<Unit> ConnectObservable(this Socket socket, IPAddress[] addresses, int port)
    {
      Contract.Requires(socket != null);
      Contract.Requires(addresses != null);
      Contract.Requires(addresses.Length > 0);
      Contract.Requires(port >= IPEndPoint.MinPort);
      Contract.Requires(port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      return Task.Factory.FromAsync<IPAddress[], int>(
        socket.BeginConnect,
        socket.EndConnect,
        addresses,
        port,
        state: null)
        .ToObservable();
    }
#endif

    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <param name="socket">The socket that will create the connection.</param>
    /// <param name="host">The name of the remote host.</param>
    /// <param name="port">The port number of the remote host.</param>
    /// <returns>A singleton observable sequence that indicates when the connection has been established.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<Unit> ConnectObservable(this Socket socket, string host, int port)
    {
      Contract.Requires(socket != null);
      Contract.Requires(host != null);
      Contract.Requires(port >= IPEndPoint.MinPort);
      Contract.Requires(port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      var args = new SocketAsyncEventArgs()
      {
        RemoteEndPoint = new DnsEndPoint(host, port)
      };

      return socket
        .ConnectObservable(args)
        .Select(_ => Unit.Default)
        .Finally(args.Dispose);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Closes the socket connection and allows reuse of the <paramref name="socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will be disconnected.</param>
    /// <param name="reuseSocket"><see langword="true"/> if this <paramref name="socket"/> can be reused after the 
    /// connection is closed; otherwise, <see langword="false" />.</param>
    /// <returns>A singleton observable sequence that indicates when the socket has been disconnected.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<Unit> DisconnectObservable(this Socket socket, bool reuseSocket)
    {
      Contract.Requires(socket != null);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      var args = new SocketAsyncEventArgs()
      {
        DisconnectReuseSocket = reuseSocket
      };

      return socket
        .ConnectObservable(args)
        .Select(_ => Unit.Default)
        .Finally(args.Dispose);
    }
#endif

    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <param name="socket">The socket that will create the connection.</param>
    /// <param name="eventArgs">The <see cref="SocketAsyncEventArgs"/> object to use for this asynchronous socket operation.</param>
    /// <returns>A singleton observable sequence containing the result of the operation.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/bb538102.aspx">Socket.ConnectAsync</seealso>
    public static IObservable<SocketAsyncEventArgs> ConnectObservable(this Socket socket, SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(socket != null);
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      return eventArgs.InvokeAsync(socket.ConnectAsync);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Closes the socket connection and allows reuse of the <paramref name="socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will be disconnected.</param>
    /// <param name="eventArgs">The <see cref="SocketAsyncEventArgs"/> object to use for this asynchronous socket operation.</param>
    /// <returns>A singleton observable sequence containing the result of the operation.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.disconnectasync.aspx">Socket.DisconnectAsync</seealso>
    public static IObservable<SocketAsyncEventArgs> DisconnectObservable(this Socket socket, SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(socket != null);
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      return eventArgs.InvokeAsync(socket.DisconnectAsync);
    }
#endif
  }
}