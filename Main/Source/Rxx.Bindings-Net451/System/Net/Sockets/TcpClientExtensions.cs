using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Threading.Tasks;

namespace System.Net.Sockets
{
  /// <summary>
  /// Provides <see langword="static"/> methods for asynchronously connecting a <see cref="TcpClient"/> object to a remote host.
  /// </summary>
  public static partial class TcpClientExtensions
  {
    /// <summary>
    /// Requests a remote host connection asynchronously.
    /// </summary>
    /// <param name="client">The TCP client to be connected.</param>
    /// <param name="address">The IP address of the remote host.</param>
    /// <param name="port">The port number of the remote host.</param>
    /// <returns>An observable containing a single notification when <paramref name="client"/> is connected.</returns>
    public static IObservable<Unit> ConnectObservable(this TcpClient client, IPAddress address, int port)
    {
      Contract.Requires(client != null);
      Contract.Requires(address != null);
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      return client.ConnectAsync(address, port).ToObservable();
    }

    /// <summary>
    /// Requests a remote host connection asynchronously.
    /// </summary>
    /// <param name="client">The TCP client to be connected.</param>
    /// <param name="addresses">At least one IP address that designates the remote host.</param>
    /// <param name="port">The port number of the remote host.</param>
    /// <returns>An observable containing a single notification when <paramref name="client"/> is connected.</returns>
    public static IObservable<Unit> ConnectObservable(this TcpClient client, IPAddress[] addresses, int port)
    {
      Contract.Requires(client != null);
      Contract.Requires(addresses != null);
      Contract.Requires(addresses.Length > 0);
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      return client.ConnectAsync(addresses, port).ToObservable();
    }

    /// <summary>
    /// Requests a remote host connection asynchronously.
    /// </summary>
    /// <param name="client">The TCP client to be connected.</param>
    /// <param name="host">The name of the remote host.</param>
    /// <param name="port">The port number of the remote host.</param>
    /// <returns>An observable containing a single notification when <paramref name="client"/> is connected.</returns>
    public static IObservable<Unit> ConnectObservable(this TcpClient client, string host, int port)
    {
      Contract.Requires(client != null);
      Contract.Requires(host != null);
      Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      return client.ConnectAsync(host, port).ToObservable();
    }
  }
}