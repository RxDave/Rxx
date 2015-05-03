using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net
{
  public static partial class ObservableWebClient
  {
    /// <summary>
    /// Uploads a file to the specified resource.
    /// </summary>
    /// <param name="address">The URI of the resource to receive the file.</param>
    /// <param name="method">The HTTP method used to send data to the resource.  If <see langword="null"/>, the default is POST for HTTP and STOR for FTP.</param>
    /// <param name="fileName">The file to upload to the resource.</param>
    /// <returns>An observable that caches the response from the server and replays it to observers.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<byte[]> UploadFile(
      Uri address,
      string method,
      string fileName)
    {
      Contract.Requires(address != null);
      Contract.Requires(fileName != null);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return Observable2.UsingHot(new WebClient(), client => client.UploadFileObservable(address, method, fileName));
    }

    /// <summary>
    /// Uploads a file to the specified resource and includes a channel for progress notifications.
    /// </summary>
    /// <param name="address">The URI of the resource to receive the file.</param>
    /// <param name="method">The HTTP method used to send data to the resource.  If <see langword="null"/>, the default is POST for HTTP and STOR for FTP.</param>
    /// <param name="fileName">The file to upload to the resource.</param>
    /// <returns>An observable that contains progress notifications in the left channel and the server's response in the right channel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<Either<UploadProgressChangedEventArgs, byte[]>> UploadFileWithProgress(
      Uri address,
      string method,
      string fileName)
    {
      Contract.Requires(address != null);
      Contract.Requires(fileName != null);
      Contract.Ensures(Contract.Result<IObservable<Either<UploadProgressChangedEventArgs, byte[]>>>() != null);

      return Observable2.UsingHot(new WebClient(), client => client.UploadFileWithProgress(address, method, fileName));
    }
  }
}