using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net
{
  public static partial class ObservableWebClient
  {
    /// <summary>
    /// Downloads the specified resource as a <see cref="Byte"/> array.
    /// </summary>
    /// <param name="address">A <see cref="Uri"/> containing the URI to download.</param>
    /// <returns>An observable that caches the result of the download and replays it to observers.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<byte[]> DownloadData(
      Uri address)
    {
      Contract.Requires(address != null);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return Observable2.UsingHot(new WebClient(), client => client.DownloadDataObservable(address));
    }

    /// <summary>
    /// Downloads the specified resource as a <see cref="Byte"/> array and includes a channel for progress notifications.
    /// </summary>
    /// <param name="address">A <see cref="Uri"/> containing the URI to download.</param>
    /// <returns>An observable that contains progress notifications in the left channel and the data in the right channel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<Either<DownloadProgressChangedEventArgs, byte[]>> DownloadDataWithProgress(
      Uri address)
    {
      Contract.Requires(address != null);
      Contract.Ensures(Contract.Result<IObservable<Either<DownloadProgressChangedEventArgs, byte[]>>>() != null);

      return Observable2.UsingHot(new WebClient(), client => client.DownloadDataWithProgress(address));
    }
  }
}