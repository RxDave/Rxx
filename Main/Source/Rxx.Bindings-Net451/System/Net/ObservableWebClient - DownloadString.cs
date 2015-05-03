using System.Diagnostics.Contracts;
#if !SILVERLIGHT
using System.Reactive.Linq;
#endif

namespace System.Net
{
  /// <summary>
  /// Provides <see langword="static"/> methods for asynchronously sending data to and observing data from a resource identified by a URI.
  /// </summary>
  public static partial class ObservableWebClient
  {
    /// <summary>
    /// Downloads the specified resource as a <see cref="string"/>.
    /// </summary>
    /// <param name="address">A <see cref="Uri"/> containing the URI to download.</param>
    /// <returns>An observable that caches the result of the download and replays it to observers.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<string> DownloadString(
      Uri address)
    {
      Contract.Requires(address != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

#if !SILVERLIGHT
      return Observable2.UsingHot(new WebClient(), client => client.DownloadStringObservable(address));
#else
      return new WebClient().DownloadStringObservable(address);
#endif
    }
  }
}