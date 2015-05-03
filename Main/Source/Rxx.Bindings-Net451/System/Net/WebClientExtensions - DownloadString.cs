using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for asynchronously sending and receiving data from <see cref="WebClient"/> objects.
  /// </summary>
  public static partial class WebClientExtensions
  {
    /// <summary>
    /// Downloads the specified resource as a <see cref="string"/>.
    /// </summary>
    /// <param name="client">The object that downloads the resource.</param>
    /// <param name="address">A <see cref="Uri"/> containing the URI to download.</param>
    /// <returns>An observable that caches the result of the download and replays it to observers.</returns>
    public static IObservable<string> DownloadStringObservable(
      this WebClient client,
      Uri address)
    {
      Contract.Requires(client != null);
      Contract.Requires(address != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return Observable2.FromEventBasedAsyncPattern<DownloadStringCompletedEventHandler, DownloadStringCompletedEventArgs>(
        handler => handler.Invoke,
        handler => client.DownloadStringCompleted += handler,
        handler => client.DownloadStringCompleted -= handler,
        token => client.DownloadStringAsync(address, token),
        client.CancelAsync)
        .Select(e => e.EventArgs.Result);
    }
  }
}