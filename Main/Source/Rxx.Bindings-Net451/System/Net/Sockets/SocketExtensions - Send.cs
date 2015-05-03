using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;
#if !SILVERLIGHT
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
#endif

namespace System.Net.Sockets
{
  public static partial class SocketExtensions
  {
#if !SILVERLIGHT
    /// <summary>
    /// Sends the data in the specified <paramref name="buffers"/> to a connected <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="buffers">The data to send.</param>
    /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
    /// <returns>A singleton observable sequence containing the number of bytes that were sent.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<int> SendObservable(this Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
#else
    /// <summary>
    /// Sends the data in the specified <paramref name="buffers"/> to a connected <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="buffers">The data to send.</param>
    /// <returns>A singleton observable sequence containing the number of bytes that were sent.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<int> SendObservable(this Socket socket, IList<ArraySegment<byte>> buffers)
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
        .SendObservable(args)
        .Select(e => e.BytesTransferred)
        .Finally(args.Dispose);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Sends the data in the specified <paramref name="buffer"/> to a connected <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="buffer">The data to send.</param>
    /// <param name="offset">The zero-based position in the buffer parameter at which to begin sending data.</param>
    /// <param name="size">The number of bytes to send.</param>
    /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
    /// <returns>A singleton observable sequence containing the number of bytes that were sent.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<int> SendObservable(this Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags)
#else
    /// <summary>
    /// Sends the data in the specified <paramref name="buffer"/> to a connected <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="buffer">The data to send.</param>
    /// <param name="offset">The zero-based position in the buffer parameter at which to begin sending data.</param>
    /// <param name="size">The number of bytes to send.</param>
    /// <returns>A singleton observable sequence containing the number of bytes that were sent.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<int> SendObservable(this Socket socket, byte[] buffer, int offset, int size)
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
        .SendObservable(args)
        .Select(e => e.BytesTransferred)
        .Finally(args.Dispose);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Sends the data in the specified <paramref name="buffer"/> to a connected <see cref="Socket"/> until the specified 
    /// <paramref name="size"/> has been reached.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="buffer">The data to send.</param>
    /// <param name="offset">The zero-based position in the buffer parameter at which to begin sending data.</param>
    /// <param name="size">The number of bytes to send.</param>
    /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
    /// <returns>A singleton observable sequence that indicates when the data has been sent.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    public static IObservable<Unit> SendUntilCompleted(this Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags)
#else
    /// <summary>
    /// Sends the data in the specified <paramref name="buffer"/> to a connected <see cref="Socket"/> until the specified 
    /// <paramref name="size"/> has been reached.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="buffer">The data to send.</param>
    /// <param name="offset">The zero-based position in the buffer parameter at which to begin sending data.</param>
    /// <param name="size">The number of bytes to send.</param>
    /// <returns>A singleton observable sequence that indicates when the data has been sent.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    public static IObservable<Unit> SendUntilCompleted(this Socket socket, byte[] buffer, int offset, int size)
#endif
    {
      Contract.Requires(socket != null);
      Contract.Requires(buffer != null);
      Contract.Requires(offset >= 0);
      Contract.Requires(offset <= buffer.Length);
      Contract.Requires(size >= 0);
      Contract.Requires(size <= buffer.Length - offset);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      return Observable.Using(
        () => new SocketAsyncEventArgs()
          {
#if !SILVERLIGHT
            SocketFlags = socketFlags
#endif
          },
        eventArgs =>
        {
          int sent = 0;

          var deferredInvoke = Observable.Defer(() =>
            {
              eventArgs.SetBuffer(buffer, offset + sent, size - sent);

              return socket.SendObservable(eventArgs);
            });

          var loop = Observable.While(() => sent < size, deferredInvoke);

          return loop
            .Do(e => sent += e.BytesTransferred)
            .TakeLast(1)
            .Select(_ => Unit.Default);
        });
    }

#if !SILVERLIGHT
    /// <summary>
    /// Sends the data in the specified <paramref name="buffer"/> to a connected <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="buffer">The data to send.</param>
    /// <param name="offset">The zero-based position in the buffer parameter at which to begin sending data.</param>
    /// <param name="size">The number of bytes to send.</param>
    /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
    /// <param name="remoteEndPoint">An <see cref="EndPoint"/> that represents the remote device.</param>
    /// <returns>A singleton observable sequence containing the number of bytes that were sent.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<int> SendToObservable(this Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEndPoint)
#else
    /// <summary>
    /// Sends the data in the specified <paramref name="buffer"/> to a connected <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="buffer">The data to send.</param>
    /// <param name="offset">The zero-based position in the buffer parameter at which to begin sending data.</param>
    /// <param name="size">The number of bytes to send.</param>
    /// <param name="remoteEndPoint">An <see cref="EndPoint"/> that represents the remote device.</param>
    /// <returns>A singleton observable sequence containing the number of bytes that were sent.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "SocketAsyncEventArgs is disposed by the Finally operator.")]
    public static IObservable<int> SendToObservable(this Socket socket, byte[] buffer, int offset, int size, EndPoint remoteEndPoint)
#endif
    {
      Contract.Requires(socket != null);
      Contract.Requires(buffer != null);
      Contract.Requires(offset >= 0);
      Contract.Requires(offset <= buffer.Length);
      Contract.Requires(size >= 0);
      Contract.Requires(size <= buffer.Length - offset);
      Contract.Requires(remoteEndPoint != null);
      Contract.Ensures(Contract.Result<IObservable<int>>() != null);

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
        .SendToObservable(args)
        .Select(e => e.BytesTransferred)
        .Finally(args.Dispose);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Sends the data in the specified <paramref name="buffer"/> to a connected <see cref="Socket"/> until the specified 
    /// <paramref name="size"/> has been reached.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="buffer">The data to send.</param>
    /// <param name="offset">The zero-based position in the buffer parameter at which to begin sending data.</param>
    /// <param name="size">The number of bytes to send.</param>
    /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
    /// <param name="remoteEndPoint">An <see cref="EndPoint"/> that represents the remote device.</param>
    /// <returns>A singleton observable sequence that indicates when the data has been sent.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    public static IObservable<Unit> SendToUntilCompleted(this Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEndPoint)
#else
    /// <summary>
    /// Sends the data in the specified <paramref name="buffer"/> to a connected <see cref="Socket"/> until the specified 
    /// <paramref name="size"/> has been reached.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="buffer">The data to send.</param>
    /// <param name="offset">The zero-based position in the buffer parameter at which to begin sending data.</param>
    /// <param name="size">The number of bytes to send.</param>
    /// <param name="remoteEndPoint">An <see cref="EndPoint"/> that represents the remote device.</param>
    /// <returns>A singleton observable sequence that indicates when the data has been sent.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    public static IObservable<Unit> SendToUntilCompleted(this Socket socket, byte[] buffer, int offset, int size, EndPoint remoteEndPoint)
#endif
    {
      Contract.Requires(socket != null);
      Contract.Requires(buffer != null);
      Contract.Requires(offset >= 0);
      Contract.Requires(offset <= buffer.Length);
      Contract.Requires(size >= 0);
      Contract.Requires(size <= buffer.Length - offset);
      Contract.Requires(remoteEndPoint != null);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      return Observable.Using(
        () => new SocketAsyncEventArgs()
          {
#if !SILVERLIGHT
            SocketFlags = socketFlags,
#endif
            RemoteEndPoint = remoteEndPoint
          },
        eventArgs =>
        {
          int sent = 0;

          var deferredInvoke = Observable.Defer(() =>
          {
            eventArgs.SetBuffer(buffer, offset + sent, size - sent);

            return socket.SendToObservable(eventArgs);
          });

          var loop = Observable.While(() => sent < size, deferredInvoke);

          return loop
            .Do(e => sent += e.BytesTransferred)
            .TakeLast(1)
            .Select(_ => Unit.Default);
        });
    }

#if !SILVERLIGHT
    /// <summary>
    /// Sends the specified file to a connected <see cref="Socket"/> object using the 
    /// <see cref="TransmitFileOptions.UseDefaultWorkerThread"/> flag.
    /// </summary>
    /// <param name="socket">The socket that will send the file.</param>
    /// <param name="fileName">The path and name of the file to send.  This parameter can be <see langword="null" />.</param>
    /// <returns>A singleton observable sequence that indicates when the file has been sent.</returns>
    public static IObservable<Unit> SendFileObservable(this Socket socket, string fileName)
    {
      Contract.Requires(socket != null);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      return Task.Factory.FromAsync<string>(
        socket.BeginSendFile,
        socket.EndSendFile,
        fileName,
        state: null)
        .ToObservable();
    }

    /// <summary>
    /// Sends the specified file to a connected <see cref="Socket"/> object.
    /// </summary>
    /// <param name="socket">The socket that will send the file.</param>
    /// <param name="fileName">The path and name of the file to send.  This parameter can be <see langword="null" />.</param>
    /// <param name="preBuffer">The data to be sent before the file is sent.  This parameter can be <see langword="null"/>.</param>
    /// <param name="postBuffer">The data to be sent after the file is sent.  This parameter can be <see langword="null"/>.</param>
    /// <param name="flags">A bitwise combination of <see cref="TransmitFileOptions"/> values.</param>
    /// <returns>A singleton observable sequence that indicates when the file has been sent.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags",
      Justification = "Name is consistent with the corresponding BCL method.")]
    public static IObservable<Unit> SendFileObservable(this Socket socket, string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags)
    {
      Contract.Requires(socket != null);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

      return Task.Factory.FromAsync<string, Tuple<byte[], byte[]>, TransmitFileOptions>(
        (fn, buffers, fg, callback, state) => socket.BeginSendFile(fn, buffers.Item1, buffers.Item2, fg, callback, state),
        socket.EndSendFile,
        fileName,
        Tuple.Create(preBuffer, postBuffer),
        flags,
        state: null)
        .ToObservable();
    }
#endif

    /// <summary>
    /// Sends data asynchronously to a connected <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="eventArgs">The <see cref="SocketAsyncEventArgs"/> object to use for this asynchronous socket operation.</param>
    /// <returns>A singleton observable sequence containing the result of the operation.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.sendasync.aspx">Socket.SendAsync</seealso>
    public static IObservable<SocketAsyncEventArgs> SendObservable(this Socket socket, SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(socket != null);
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      return eventArgs.InvokeAsync(socket.SendAsync);
    }

    /// <summary>
    /// Sends data asynchronously to a connected <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="eventArgs">The <see cref="SocketAsyncEventArgs"/> object to use for this asynchronous socket operation.</param>
    /// <returns>A singleton observable sequence containing the result of the operation.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.sendtoasync.aspx">Socket.SendToAsync</seealso>
    public static IObservable<SocketAsyncEventArgs> SendToObservable(this Socket socket, SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(socket != null);
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      return eventArgs.InvokeAsync(socket.SendToAsync);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Sends a collection of files or in memory data buffers asynchronously to a connected <see cref="Socket"/>.
    /// </summary>
    /// <param name="socket">The socket that will send the data.</param>
    /// <param name="eventArgs">The <see cref="SocketAsyncEventArgs"/> object to use for this asynchronous socket operation.</param>
    /// <returns>A singleton observable sequence containing the result of the operation.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.sendpacketsasync.aspx">Socket.SendPacketsAsync</seealso>
    public static IObservable<SocketAsyncEventArgs> SendPacketsObservable(this Socket socket, SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(socket != null);
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      return eventArgs.InvokeAsync(socket.SendPacketsAsync);
    }
#endif
  }
}