using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net
{
  public static partial class WebClientExtensions
  {
    /// <summary>
    /// Downloads the specified resource as a <see cref="Byte"/> array.
    /// </summary>
    /// <param name="client">The object that downloads the resource.</param>
    /// <param name="address">A <see cref="Uri"/> containing the URI to download.</param>
    /// <returns>An observable that caches the result of the download and replays it to observers.</returns>
    public static IObservable<byte[]> DownloadDataObservable(
      this WebClient client,
      Uri address)
    {
      Contract.Requires(client != null);
      Contract.Requires(address != null);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return Observable2.FromEventBasedAsyncPattern<DownloadDataCompletedEventHandler, DownloadDataCompletedEventArgs>(
        handler => handler.Invoke,
        handler => client.DownloadDataCompleted += handler,
        handler => client.DownloadDataCompleted -= handler,
        token => client.DownloadDataAsync(address, token),
        client.CancelAsync)
        .Select(e => e.EventArgs.Result);
    }

    /// <summary>
    /// Downloads the specified resource as a <see cref="Byte"/> array and includes a channel for progress notifications.
    /// </summary>
    /// <param name="client">The object that downloads the resource.</param>
    /// <param name="address">A <see cref="Uri"/> containing the URI to download.</param>
    /// <returns>An observable that contains progress notifications in the left channel and the data in the right channel.</returns>
    public static IObservable<Either<DownloadProgressChangedEventArgs, byte[]>> DownloadDataWithProgress(
      this WebClient client,
      Uri address)
    {
      Contract.Requires(client != null);
      Contract.Requires(address != null);
      Contract.Ensures(Contract.Result<IObservable<Either<DownloadProgressChangedEventArgs, byte[]>>>() != null);

      return Observable2.FromEventBasedAsyncPattern<DownloadDataCompletedEventHandler, DownloadDataCompletedEventArgs, DownloadProgressChangedEventHandler, DownloadProgressChangedEventArgs>(
        handler => handler.Invoke,
        handler => client.DownloadDataCompleted += handler,
        handler => client.DownloadDataCompleted -= handler,
        handler => handler.Invoke,
        handler => client.DownloadProgressChanged += handler,
        handler => client.DownloadProgressChanged -= handler,
        token => client.DownloadDataAsync(address, token),
        client.CancelAsync)
        .Select(
          left => left.EventArgs,
          right => right.EventArgs.Result);
    }
  }
}