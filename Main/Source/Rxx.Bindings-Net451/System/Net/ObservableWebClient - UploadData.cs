using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net
{
  public static partial class ObservableWebClient
  {
    /// <summary>
    /// Uploads a <see cref="byte"/> array to the specified resource.
    /// </summary>
    /// <param name="address">The URI of the resource to receive the data.</param>
    /// <param name="method">The HTTP method used to send data to the resource.  If <see langword="null"/>, the default is POST for HTTP and STOR for FTP.</param>
    /// <param name="data">The bytes to upload to the resource.</param>
    /// <returns>An observable that caches the response from the server and replays it to observers.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<byte[]> UploadData(
      Uri address,
      string method,
      byte[] data)
    {
      Contract.Requires(address != null);
      Contract.Requires(data != null);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return Observable2.UsingHot(new WebClient(), client => client.UploadDataObservable(address, method, data));
    }

    /// <summary>
    /// Uploads a <see cref="byte"/> array to the specified resource and includes a channel for progress notifications.
    /// </summary>
    /// <param name="address">The URI of the resource to receive the data.</param>
    /// <param name="method">The HTTP method used to send data to the resource.  If <see langword="null"/>, the default is POST for HTTP and STOR for FTP.</param>
    /// <param name="data">The bytes to upload to the resource.</param>
    /// <returns>An observable that contains progress notifications in the left channel and the server's response in the right channel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<Either<UploadProgressChangedEventArgs, byte[]>> UploadDataWithProgress(
      Uri address,
      string method,
      byte[] data)
    {
      Contract.Requires(address != null);
      Contract.Requires(data != null);
      Contract.Ensures(Contract.Result<IObservable<Either<UploadProgressChangedEventArgs, byte[]>>>() != null);

      return Observable2.UsingHot(new WebClient(), client => client.UploadDataWithProgress(address, method, data));
    }
  }
}