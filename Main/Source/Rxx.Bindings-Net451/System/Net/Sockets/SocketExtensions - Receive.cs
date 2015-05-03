using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net.Sockets
{
  public static partial class SocketExtensions
  {
#if !SILVERLIGHT
    /// <summary>
    /// Receives data from a bound <see cref="Socket"/> into the specified <paramref name="buffers"/>.
    /// </summary>
    /// <param name="socket">The socket that will receive the data.</param>
    /// <param name="buffers">The storage location for the received data.</param>
    /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
    /// <returns>A singleton observable sequence containing the number of bytes that were received.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<int> ReceiveObservable(this Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
#else
    /// <summary>
    /// Receives data from a bound <see cref="Socket"/> into the specified <paramref name="buffers"/>.
    /// </summary>
    /// <param name="socket">The socket that will receive the data.</param>
    /// <param name="buffers">The storage location for the received data.</param>
    /// <returns>A singleton observable sequence containing the number of bytes that were received.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<int> ReceiveObservable(this Socket socket, IList<ArraySegment<byte>> buffers)
#endif
    {
      Contract.Requires(socket != null);
      Contract.Requires(buffers != null);
      Contract.Ensures(Contract.Result<IObservable<int>>() != null);

#if !SILVERLIGHT
      var args = new SocketAsyncEventArgs()
      {
        BufferList = buffers,
        SocketFlags = socketFlags
      };
#else
      var args = new SocketAsyncEventArgs()
      {
        BufferList = buffers
      };
#endif

      return socket
        .ReceiveObservable(args)
        .Select(e => e.BytesTransferred)
        .Finally(args.Dispose);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Receives data from a bound <see cref="Socket"/> into the specified <paramref name="buffer"/>.
    /// </summary>
    /// <param name="socket">The socket that will receive the data.</param>
    /// <param name="buffer">The storage location for the received data.</param>
    /// <param name="offset">The zero-based position in the <paramref name="buffer"/> at which to store the received data.</param>
    /// <param name="size">The number of bytes to receive.</param>
    /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
    /// <returns>A singleton observable sequence containing the number of bytes that were received.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<int> ReceiveObservable(this Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags)
#else
    /// <summary>
    /// Receives data from a bound <see cref="Socket"/> into the specified <paramref name="buffer"/>.
    /// </summary>
    /// <param name="socket">The socket that will receive the data.</param>
    /// <param name="buffer">The storage location for the received data.</param>
    /// <param name="offset">The zero-based position in the <paramref name="buffer"/> at which to store the received data.</param>
    /// <param name="size">The number of bytes to receive.</param>
    /// <returns>A singleton observable sequence containing the number of bytes that were received.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<int> ReceiveObservable(this Socket socket, byte[] buffer, int offset, int size)
#endif
    {
      Contract.Requires(socket != null);
      Contract.Requires(buffer != null);
      Contract.Requires(offset >= 0);
      Contract.Requires(offset <= buffer.Length);
      Contract.Requires(size >= 0);
      Contract.Requires(size <= buffer.Length - offset);
      Contract.Ensures(Contract.Result<IObservable<int>>() != null);

#if !SILVERLIGHT
      var args = new SocketAsyncEventArgs()
      {
        SocketFlags = socketFlags
      };
#else
      var args = new SocketAsyncEventArgs();
#endif

      args.SetBuffer(buffer, offset, size);

      return socket
        .ReceiveObservable(args)
        .Select(e => e.BytesTransferred)
        .Finally(args.Dispose);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Receives data from a bound <see cref="Socket"/> into a sequence of <see cref="byte"/> arrays until the host shuts down 
    /// the connection.
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// Unlike many of the other <see cref="ObservableSocket"/> methods, 	<see cref="ReceiveUntilCompleted(Socket,SocketFlags)"/>
    /// returns a <strong>cold</strong> observable.  This is to ensure that observers do not miss any of the byte arrays in the sequence; otherwise, 
    /// buffering would be required for the entire sequence, similar to how <see cref="Observable.FromAsyncPattern(Func{AsyncCallback,object,IAsyncResult},Action{IAsyncResult})"/> 
    /// buffers the first result and then replays it to all subscribers.  Another consequence of the <strong>cold</strong> behavior is that 
    /// subscriptions are not shared, thus every new subscription will perform an independent receive operation on the <paramref name="socket"/>.
    /// </alert>
    /// </remarks>
    /// <param name="socket">The socket that will receive the data.</param>
    /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
    /// <returns>An observable sequence of <see cref="byte"/> arrays containing the data that was received.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the other methods.")]
    public static IObservable<byte[]> ReceiveUntilCompleted(this Socket socket, SocketFlags socketFlags)
#else
    /// <summary>
    /// Receives data from a bound <see cref="Socket"/> into a sequence of <see cref="byte"/> arrays until the host shuts down 
    /// the connection.
    /// </summary>
    /// <remarks>
    /// <alert type="warning">
    /// Unlike many of the other <see cref="ObservableSocket"/> methods, 	<see cref="ReceiveUntilCompleted(Socket)"/>
    /// returns a <strong>cold</strong> observable.  This is to ensure that observers do not miss any of the byte arrays in the sequence; otherwise, 
    /// buffering would be required for the entire sequence, similar to how <see cref="Observable.FromAsyncPattern(Func{AsyncCallback,object,IAsyncResult},Action{IAsyncResult})"/> 
    /// buffers the first result and then replays it to all subscribers.  Another consequence of the <strong>cold</strong> behavior is that 
    /// subscriptions are not shared, thus every new subscription will perform an independent receive operation on the <paramref name="socket"/>.
    /// </alert>
    /// </remarks>
    /// <param name="socket">The socket that will receive the data.</param>
    /// <returns>An observable sequence of <see cref="byte"/> arrays containing the data that was received.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the other methods.")]
    public static IObservable<byte[]> ReceiveUntilCompleted(this Socket socket)
#endif
    {
      Contract.Requires(socket != null);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return Observable.Using(
        () => new SocketAsyncEventArgs()
          {
#if !SILVERLIGHT
            SocketFlags = socketFlags
#endif
          },
        eventArgs =>
        {
          Contract.Assume(socket.ReceiveBufferSize >= 0);

          var buffer = new byte[socket.ReceiveBufferSize];

          eventArgs.SetBuffer(buffer, 0, buffer.Length);

          int received = -1;

          var deferredInvoke = Observable.Defer(() => socket.ReceiveObservable(eventArgs));

          var loop = Observable.While(() => received != 0, deferredInvoke);

          return loop
            .Do(e => received = e.BytesTransferred)
            .Where(e => e.BytesTransferred > 0)
            .Select(_ =>
              {
                if (received < buffer.Length)
                {
                  var copy = new byte[received];

                  Array.Copy(buffer, copy, received);

                  return copy;
                }
                else
                {
                  return buffer;
                }
              });
        });
    }

#if !SILVERLIGHT
    /// <summary>
    /// Receives data from a bound <see cref="Socket"/> into the specified <paramref name="buffer"/> and stores the 
    /// actual <see cref="EndPoint"/>.
    /// </summary>
    /// <param name="socket">The socket that will receive the data.</param>
    /// <param name="buffer">The storage location for the received data.</param>
    /// <param name="offset">The zero-based position in the <paramref name="buffer"/> at which to store the received data.</param>
    /// <param name="size">The number of bytes to receive.</param>
    /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
    /// <param name="remoteEndPoint">An <see cref="EndPoint"/> that represents the source of the data.</param>
    /// <returns>A singleton observable sequence containing a tuple that indicates the number of bytes that were received
    /// and the actual <see cref="EndPoint"/> from which the data was received.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<Tuple<int, EndPoint>> ReceiveFromObservable(this Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEndPoint)
#else
    /// <summary>
    /// Receives data from a bound <see cref="Socket"/> into the specified <paramref name="buffer"/> and stores the 
    /// actual <see cref="EndPoint"/>.
    /// </summary>
    /// <param name="socket">The socket that will receive the data.</param>
    /// <param name="buffer">The storage location for the received data.</param>
    /// <param name="offset">The zero-based position in the <paramref name="buffer"/> at which to store the received data.</param>
    /// <param name="size">The number of bytes to receive.</param>
    /// <param name="remoteEndPoint">An <see cref="EndPoint"/> that represents the source of the data.</param>
    /// <returns>A singleton observable sequence containing a tuple that indicates the number of bytes that were received
    /// and the actual <see cref="EndPoint"/> from which the data was received.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<Tuple<int, EndPoint>> ReceiveFromObservable(this Socket socket, byte[] buffer, int offset, int size, EndPoint remoteEndPoint)
#endif
    {
      Contract.Requires(socket != null);
      Contract.Requires(buffer != null);
      Contract.Requires(offset >= 0);
      Contract.Requires(offset <= buffer.Length);
      Contract.Requires(size >= 0);
      Contract.Requires(size <= buffer.Length - offset);
      Contract.Requires(remoteEndPoint != null);
      Contract.Ensures(Contract.Result<IObservable<Tuple<int, EndPoint>>>() != null);

#if !SILVERLIGHT
      var args = new SocketAsyncEventArgs()
      {
        RemoteEndPoint = remoteEndPoint,
        SocketFlags = socketFlags
      };
#else
      var args = new SocketAsyncEventArgs()
      {
        RemoteEndPoint = remoteEndPoint
      };
#endif

      args.SetBuffer(buffer, offset, size);

      return socket
        .ReceiveFromObservable(args)
        .Select(e => Tuple.Create(e.BytesTransferred, e.RemoteEndPoint))
        .Finally(args.Dispose);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Receives data from a bound <see cref="Socket"/> into the specified <paramref name="buffer"/> and stores the 
    /// actual <see cref="SocketFlags"/>, <see cref="EndPoint"/> and packet information.
    /// </summary>
    /// <param name="socket">The socket that will receive the data.</param>
    /// <param name="buffer">The storage location for the received data.</param>
    /// <param name="offset">The zero-based position in the <paramref name="buffer"/> at which to store the received data.</param>
    /// <param name="size">The number of bytes to receive.</param>
    /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
    /// <param name="remoteEndPoint">An <see cref="EndPoint"/> that represents the source of the data.</param>
    /// <returns>A singleton observable sequence containing a tuple that indicates the number of bytes that were received
    /// and the actual <see cref="EndPoint"/> from which the data was received.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<Tuple<int, SocketFlags, EndPoint, IPPacketInformation>> ReceiveMessageFromObservable(this Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEndPoint)
    {
      Contract.Requires(socket != null);
      Contract.Requires(buffer != null);
      Contract.Requires(offset >= 0);
      Contract.Requires(offset <= buffer.Length);
      Contract.Requires(size >= 0);
      Contract.Requires(size <= buffer.Length - offset);
      Contract.Requires(remoteEndPoint != null);
      Contract.Ensures(Contract.Result<IObservable<Tuple<int, SocketFlags, EndPoint, IPPacketInformation>>>() != null);

      var args = new SocketAsyncEventArgs()
      {
        RemoteEndPoint = remoteEndPoint,
        SocketFlags = socketFlags
      };

      args.SetBuffer(buffer, offset, size);

      return socket
        .ReceiveMessageFromObservable(args)
        .Select(e => Tuple.Create(e.BytesTransferred, e.SocketFlags, e.RemoteEndPoint, e.ReceiveMessageFromPacketInfo))
        .Finally(args.Dispose);
    }
#endif

    /// <summary>
    /// Receives data from a bound <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will receive the data.</param>
    /// <param name="eventArgs">The <see cref="SocketAsyncEventArgs"/> object to use for this asynchronous socket operation.</param>
    /// <returns>A singleton observable sequence containing the result of the operation.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.receiveasync.aspx">Socket.ReceiveAsync</seealso>
    public static IObservable<SocketAsyncEventArgs> ReceiveObservable(this Socket socket, SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(socket != null);
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      return eventArgs.InvokeAsync(socket.ReceiveAsync);
    }

    /// <summary>
    /// Receives data from a bound <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will receive the data.</param>
    /// <param name="eventArgs">The <see cref="SocketAsyncEventArgs"/> object to use for this asynchronous socket operation.</param>
    /// <returns>A singleton observable sequence containing the result of the operation.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.receivefromasync.aspx">Socket.ReceiveFromAsync</seealso>
    public static IObservable<SocketAsyncEventArgs> ReceiveFromObservable(this Socket socket, SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(socket != null);
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      return eventArgs.InvokeAsync(socket.ReceiveFromAsync);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Receives data from a bound <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will receive the data.</param>
    /// <param name="eventArgs">The <see cref="SocketAsyncEventArgs"/> object to use for this asynchronous socket operation.</param>
    /// <returns>A singleton observable sequence containing the result of the operation.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.receivemessagefromasync.aspx">Socket.ReceiveMessageFromAsync</seealso>
    public static IObservable<SocketAsyncEventArgs> ReceiveMessageFromObservable(this Socket socket, SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(socket != null);
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      return eventArgs.InvokeAsync(socket.ReceiveMessageFromAsync);
    }
#endif
  }
}