using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net.Sockets
{
  /// <summary>
  /// Provides <see langword="static" /> methods that return observable sequences for accepting and connecting sockets.
  /// </summary>
  public static partial class ObservableSocket
  {
    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <param name="addressFamily">One of the <see cref="AddressFamily"/> values.</param>
    /// <param name="socketType">One of the <see cref="SocketType"/> values.</param>
    /// <param name="protocolType">One of the <see cref="ProtocolType"/> values.</param>
    /// <param name="remoteEndPoint">An <see cref="EndPoint"/> that represents the remote host.</param>
    /// <returns>A singleton observable sequence containing the connected <see cref="Socket"/>.</returns>
    public static IObservable<Socket> Connect(
      AddressFamily addressFamily,
      SocketType socketType,
      ProtocolType protocolType,
      EndPoint remoteEndPoint)
    {
      Contract.Requires(remoteEndPoint != null);
      Contract.Ensures(Contract.Result<IObservable<Socket>>() != null);

      var socket = new Socket(addressFamily, socketType, protocolType);

      return socket.ConnectObservable(remoteEndPoint).Select(_ => socket);
    }

    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <param name="addressFamily">One of the <see cref="AddressFamily"/> values.</param>
    /// <param name="socketType">One of the <see cref="SocketType"/> values.</param>
    /// <param name="protocolType">One of the <see cref="ProtocolType"/> values.</param>
    /// <param name="address">The <see cref="IPAddress"/> of the remote host.</param>
    /// <param name="port">The port number of the remote host.</param>
    /// <returns>A singleton observable sequence containing the connected <see cref="Socket"/>.</returns>
    public static IObservable<Socket> Connect(
      AddressFamily addressFamily,
      SocketType socketType,
      ProtocolType protocolType,
      IPAddress address,
      int port)
    {
      Contract.Requires(address != null);
      Contract.Requires(port >= IPEndPoint.MinPort);
      Contract.Requires(port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<Socket>>() != null);

      var socket = new Socket(addressFamily, socketType, protocolType);

      return socket.ConnectObservable(address, port).Select(_ => socket);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <param name="addressFamily">One of the <see cref="AddressFamily"/> values.</param>
    /// <param name="socketType">One of the <see cref="SocketType"/> values.</param>
    /// <param name="protocolType">One of the <see cref="ProtocolType"/> values.</param>
    /// <param name="addresses">At least one <see cref="IPAddress"/>, designating the remote host.</param>
    /// <param name="port">The port number of the remote host.</param>
    /// <returns>A singleton observable sequence containing the connected <see cref="Socket"/>.</returns>
    public static IObservable<Socket> Connect(
      AddressFamily addressFamily,
      SocketType socketType,
      ProtocolType protocolType,
      IPAddress[] addresses,
      int port)
    {
      Contract.Requires(addresses != null);
      Contract.Requires(addresses.Length > 0);
      Contract.Requires(port >= IPEndPoint.MinPort);
      Contract.Requires(port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<Socket>>() != null);

      var socket = new Socket(addressFamily, socketType, protocolType);

      return socket.ConnectObservable(addresses, port).Select(_ => socket);
    }
#endif

    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <param name="addressFamily">One of the <see cref="AddressFamily"/> values.</param>
    /// <param name="socketType">One of the <see cref="SocketType"/> values.</param>
    /// <param name="protocolType">One of the <see cref="ProtocolType"/> values.</param>
    /// <param name="host">The name of the remote host.</param>
    /// <param name="port">The port number of the remote host.</param>
    /// <returns>A singleton observable sequence containing the connected <see cref="Socket"/>.</returns>
    public static IObservable<Socket> Connect(
      AddressFamily addressFamily,
      SocketType socketType,
      ProtocolType protocolType,
      string host,
      int port)
    {
      Contract.Requires(host != null);
      Contract.Requires(port >= IPEndPoint.MinPort);
      Contract.Requires(port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<Socket>>() != null);

      var socket = new Socket(addressFamily, socketType, protocolType);

      return socket.ConnectObservable(host, port).Select(_ => socket);
    }

    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <param name="socketType">One of the <see cref="SocketType"/> values.</param>
    /// <param name="protocolType">One of the <see cref="ProtocolType"/> values.</param>
    /// <param name="eventArgs">The <see cref="SocketAsyncEventArgs"/> object to use for this asynchronous socket operation.</param>
    /// <returns>A singleton observable sequence containing the result of the operation.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/cc422500.aspx">Socket.ConnectAsync</seealso>
    public static IObservable<SocketAsyncEventArgs> Connect(
      SocketType socketType,
      ProtocolType protocolType,
      SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      return eventArgs.InvokeAsync(e => Socket.ConnectAsync(socketType, protocolType, e));
    }
  }
}