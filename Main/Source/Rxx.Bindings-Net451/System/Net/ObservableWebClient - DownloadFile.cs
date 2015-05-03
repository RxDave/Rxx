using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net
{
  public static partial class ObservableWebClient
  {
    /// <summary>
    /// Downloads the specified resource as a file.
    /// </summary>
    /// <param name="address">A <see cref="Uri"/> containing the URI to download.</param>
    /// <param name="fileName">The file to create or overwrite with the resource.</param>
    /// <returns>An observable that caches the result of the download and replays it to observers.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<string> DownloadFile(
      Uri address,
      string fileName)
    {
      Contract.Requires(address != null);
      Contract.Requires(fileName != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return Observable2.UsingHot(new WebClient(), client => client.DownloadFileObservable(address, fileName));
    }

    /// <summary>
    /// Downloads the specified resource as a file and includes a channel for progress notifications.
    /// </summary>
    /// <param name="address">A <see cref="Uri"/> containing the URI to download.</param>
    /// <param name="fileName">The file to create or overwrite with the resource.</param>
    /// <returns>An observable that contains progress notifications in the left channel and the file name in the right channel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<Either<DownloadProgressChangedEventArgs, string>> DownloadFileWithProgress(
      Uri address,
      string fileName)
    {
      Contract.Requires(address != null);
      Contract.Requires(fileName != null);
      Contract.Ensures(Contract.Result<IObservable<Either<DownloadProgressChangedEventArgs, string>>>() != null);

      return Observable2.UsingHot(new WebClient(), client => client.DownloadFileWithProgress(address, fileName));
    }
  }
}