using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net.Sockets
{
  public static partial class ObservableSocket
  {
    /// <summary>
    /// Accepts the specified number of incoming connection attempts and creates new <see cref="Socket"/> objects.
    /// </summary>
    /// <param name="addressFamily">One of the <see cref="AddressFamily"/> values.</param>
    /// <param name="socketType">One of the <see cref="SocketType"/> values.</param>
    /// <param name="protocolType">One of the <see cref="ProtocolType"/> values.</param>
    /// <param name="localEndPoint">The local <see cref="EndPoint"/> to associate with the <see cref="Socket"/> that 
    /// will be accepting connections.</param>
    /// <param name="count">The maximum number of connections to accept.</param>
    /// <returns>An observable sequence containing the connected <see cref="Socket"/> objects.</returns>
    public static IObservable<Socket> Accept(
      AddressFamily addressFamily,
      SocketType socketType,
      ProtocolType protocolType,
      IPEndPoint localEndPoint,
      int count)
    {
      Contract.Requires(localEndPoint != null);
      Contract.Requires(count > 0);
      Contract.Ensures(Contract.Result<IObservable<Socket>>() != null);

      return Accept(addressFamily, socketType, protocolType, localEndPoint, count, () => null);
    }

    /// <summary>
    /// Accepts the specified number of incoming connection attempts and creates new <see cref="Socket"/> objects.
    /// </summary>
    /// <param name="addressFamily">One of the <see cref="AddressFamily"/> values.</param>
    /// <param name="socketType">One of the <see cref="SocketType"/> values.</param>
    /// <param name="protocolType">One of the <see cref="ProtocolType"/> values.</param>
    /// <param name="localEndPoint">The local <see cref="EndPoint"/> to associate with the <see cref="Socket"/> that 
    /// will be accepting connections.</param>
    /// <param name="count">The maximum number of connections to accept.</param>
    /// <param name="acceptSocketFactory">Called when ready to accept a new <see cref="Socket"/>, up to the specified <paramref name="count"/>.
    /// This function may return <see langword="null"/> to create new sockets automatically.</param>
    /// <returns>An observable sequence containing the connected <see cref="Socket"/> objects.</returns>
    public static IObservable<Socket> Accept(
      AddressFamily addressFamily,
      SocketType socketType,
      ProtocolType protocolType,
      IPEndPoint localEndPoint,
      int count,
      Func<Socket> acceptSocketFactory)
    {
      Contract.Requires(localEndPoint != null);
      Contract.Requires(count > 0);
      Contract.Requires(acceptSocketFactory != null);
      Contract.Ensures(Contract.Result<IObservable<Socket>>() != null);

      return Observable.Using(
        () => new Socket(addressFamily, socketType, protocolType),
        socket =>
        {
          socket.Bind(localEndPoint);
          socket.Listen(count);

          var e = new SocketAsyncEventArgs();

          return Observable
            .Defer(() =>
            {
              e.AcceptSocket = acceptSocketFactory();		// Work item 23871

              return socket.AcceptObservable(e);
            })
            .Select(r => r.AcceptSocket)
            .Repeat(count);
        });
    }
  }
}