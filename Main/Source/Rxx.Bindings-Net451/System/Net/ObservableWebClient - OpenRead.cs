using System.Diagnostics.Contracts;
using System.IO;
using System.Reactive.Linq;

namespace System.Net
{
  public static partial class ObservableWebClient
  {
    /// <summary>
    /// Downloads the specified resource as a <see cref="Stream"/>.
    /// </summary>
    /// <param name="address">A <see cref="Uri"/> containing the URI to download.</param>
    /// <returns>An observable containing the readable stream that reads data from the resource.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<Stream> OpenRead(
      Uri address)
    {
      Contract.Requires(address != null);
      Contract.Ensures(Contract.Result<IObservable<Stream>>() != null);

#if !SILVERLIGHT
      return Observable2.UsingHot(new WebClient(), client => client.OpenReadObservable(address));
#else
      return new WebClient().OpenReadObservable(address);
#endif
    }

    /// <summary>
    /// Downloads the specified resource as a <see cref="Stream"/> and includes a channel for progress notifications.
    /// </summary>
    /// <param name="address">A <see cref="Uri"/> containing the URI to download.</param>
    /// <returns>An observable that contains progress notifications in the left channel and the stream that 
    /// reads data from the resource in the right channel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<Either<DownloadProgressChangedEventArgs, Stream>> OpenReadWithProgress(
      Uri address)
    {
      Contract.Requires(address != null);
      Contract.Ensures(Contract.Result<IObservable<Either<DownloadProgressChangedEventArgs, Stream>>>() != null);

#if !SILVERLIGHT
      return Observable2.UsingHot(new WebClient(), client => client.OpenReadWithProgress(address));
#else
      return new WebClient().OpenReadWithProgress(address);
#endif
    }
  }
}