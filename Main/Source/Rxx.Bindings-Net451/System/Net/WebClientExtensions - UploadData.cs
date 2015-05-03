using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net
{
  public static partial class WebClientExtensions
  {
    /// <summary>
    /// Uploads a <see cref="byte"/> array to the specified resource.
    /// </summary>
    /// <param name="client">The object that uploads to the resource.</param>
    /// <param name="address">The URI of the resource to receive the data.</param>
    /// <param name="method">The HTTP method used to send data to the resource.  If <see langword="null"/>, the default is POST for HTTP and STOR for FTP.</param>
    /// <param name="data">The bytes to upload to the resource.</param>
    /// <returns>An observable that caches the response from the server and replays it to observers.</returns>
    public static IObservable<byte[]> UploadDataObservable(
      this WebClient client,
      Uri address,
      string method,
      byte[] data)
    {
      Contract.Requires(client != null);
      Contract.Requires(address != null);
      Contract.Requires(data != null);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return Observable2.FromEventBasedAsyncPattern<UploadDataCompletedEventHandler, UploadDataCompletedEventArgs>(
        handler => handler.Invoke,
        handler => client.UploadDataCompleted += handler,
        handler => client.UploadDataCompleted -= handler,
        token => client.UploadDataAsync(address, method, data, token),
        client.CancelAsync)
        .Select(e => e.EventArgs.Result);
    }

    /// <summary>
    /// Uploads a <see cref="byte"/> array to the specified resource and includes a channel for progress notifications.
    /// </summary>
    /// <param name="client">The object that uploads to the resource.</param>
    /// <param name="address">The URI of the resource to receive the data.</param>
    /// <param name="method">The HTTP method used to send data to the resource.  If <see langword="null"/>, the default is POST for HTTP and STOR for FTP.</param>
    /// <param name="data">The bytes to upload to the resource.</param>
    /// <returns>An observable that contains progress notifications in the left channel and the server's response in the right channel.</returns>
    public static IObservable<Either<UploadProgressChangedEventArgs, byte[]>> UploadDataWithProgress(
      this WebClient client,
      Uri address,
      string method,
      byte[] data)
    {
      Contract.Requires(client != null);
      Contract.Requires(address != null);
      Contract.Requires(data != null);
      Contract.Ensures(Contract.Result<IObservable<Either<UploadProgressChangedEventArgs, byte[]>>>() != null);

      return Observable2.FromEventBasedAsyncPattern<UploadDataCompletedEventHandler, UploadDataCompletedEventArgs, UploadProgressChangedEventHandler, UploadProgressChangedEventArgs>(
        handler => handler.Invoke,
        handler => client.UploadDataCompleted += handler,
        handler => client.UploadDataCompleted -= handler,
        handler => handler.Invoke,
        handler => client.UploadProgressChanged += handler,
        handler => client.UploadProgressChanged -= handler,
        token => client.UploadDataAsync(address, method, data, token),
        client.CancelAsync)
        .Select(
          left => left.EventArgs,
          right => right.EventArgs.Result);
    }
  }
}