using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net.Sockets
{
  public static partial class SocketExtensions
  {
    /// <summary>
    /// Accepts an incoming connection attempt and creates a new <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will accept a new connection.</param>
    /// <returns>A singleton observable sequence containing the connected <see cref="Socket"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<Socket> AcceptObservable(this Socket socket)
    {
      Contract.Requires(socket != null);
      Contract.Ensures(Contract.Result<IObservable<Socket>>() != null);

      var args = new SocketAsyncEventArgs();

      return socket
        .AcceptObservable(args)
        .Select(e => e.AcceptSocket)
        .Finally(args.Dispose);
    }

    /// <summary>
    /// Accepts an incoming connection attempt, creates a new <see cref="Socket"/> and receives the first 
    /// block of data sent by the client application.
    /// </summary>
    /// <param name="socket">The socket that will accept a new connection.</param>
    /// <param name="receiveSize">The number of bytes to accept from the sender.</param>
    /// <returns>A singleton observable sequence containing the connected <see cref="Socket"/> in the left channel
    /// and the first block of data in the right channel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<Either<Socket, byte[]>> AcceptObservable(this Socket socket, int receiveSize)
    {
      Contract.Requires(socket != null);
      Contract.Requires(receiveSize >= 0);
      Contract.Ensures(Contract.Result<IObservable<Either<Socket, byte[]>>>() != null);

      var args = new SocketAsyncEventArgs();

      args.SetBuffer(0, receiveSize);

      return socket
        .AcceptObservableWithBuffer(args)
        .Finally(args.Dispose);
    }

    /// <summary>
    /// Accepts an incoming connection attempt, creates a new <see cref="Socket"/> and receives the first 
    /// block of data sent by the client application.
    /// </summary>
    /// <param name="socket">The socket that will accept a new connection.</param>
    /// <param name="acceptSocket">The accepted <see cref="Socket"/> object.  This value may be <see langword="null"/>.</param>
    /// <param name="receiveSize">The maximum number of bytes to receive.</param>
    /// <returns>A singleton observable sequence containing the connected <see cref="Socket"/> in the left channel
    /// and the first block of data in the right channel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<Either<Socket, byte[]>> AcceptObservable(this Socket socket, Socket acceptSocket, int receiveSize)
    {
      Contract.Requires(socket != null);
      Contract.Requires(receiveSize >= 0);
      Contract.Ensures(Contract.Result<IObservable<Either<Socket, byte[]>>>() != null);

      var args = new SocketAsyncEventArgs()
      {
        AcceptSocket = acceptSocket
      };

      args.SetBuffer(0, receiveSize);

      return socket
        .AcceptObservableWithBuffer(args)
        .Finally(args.Dispose);
    }

    private static IObservable<Either<Socket, byte[]>> AcceptObservableWithBuffer(this Socket socket, SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(socket != null);
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<Either<Socket, byte[]>>>() != null);

      return socket
        .AcceptObservable(eventArgs)
        .Pair(
          args => args.AcceptSocket,
          args =>
          {
            byte[] buffer = args.Buffer;

            if (buffer.Length > args.BytesTransferred)
            {
              var copy = new byte[args.BytesTransferred];

              Array.Copy(buffer, copy, args.BytesTransferred);

              buffer = copy;
            }

            return buffer;
          });
    }

    /// <summary>
    /// Accepts an incoming connection attempt.
    /// </summary>
    /// <param name="socket">The socket that will accept a new connection.</param>
    /// <param name="eventArgs">The <see cref="SocketAsyncEventArgs"/> object to use for this asynchronous socket operation.</param>
    /// <returns>A singleton observable sequence containing the result of the operation.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.acceptasync.aspx">Socket.AcceptAsync</seealso>
    public static IObservable<SocketAsyncEventArgs> AcceptObservable(this Socket socket, SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(socket != null);
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      return eventArgs.InvokeAsync(socket.AcceptAsync);
    }
  }
}