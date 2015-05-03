using System.Diagnostics.Contracts;
using System.IO;
using System.Reactive.Linq;

namespace System.Net
{
  public static partial class WebClientExtensions
  {
    /// <summary>
    /// Opens a writeable stream to the specified resource.
    /// </summary>
    /// <param name="client">The object that uploads to the resource.</param>
    /// <param name="address">The URI of the resource to receive the stream.</param>
    /// <param name="method">The HTTP method used to send data to the resource.  If <see langword="null"/>, the default is POST for HTTP and STOR for FTP.</param>
    /// <returns>An observable containing the writeable stream that sends data to the resource.</returns>
    public static IObservable<Stream> OpenWriteObservable(
      this WebClient client,
      Uri address,
      string method)
    {
      Contract.Requires(client != null);
      Contract.Requires(address != null);
      Contract.Requires(method != null);
      Contract.Ensures(Contract.Result<IObservable<Stream>>() != null);

      return Observable2.FromEventBasedAsyncPattern<OpenWriteCompletedEventHandler, OpenWriteCompletedEventArgs>(
        handler => handler.Invoke,
        handler => client.OpenWriteCompleted += handler,
        handler => client.OpenWriteCompleted -= handler,
        token => client.OpenWriteAsync(address, method, token),
        client.CancelAsync)
        .Select(e => e.EventArgs.Result);
    }
  }
}