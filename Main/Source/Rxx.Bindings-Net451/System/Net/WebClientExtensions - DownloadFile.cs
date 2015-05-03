using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net
{
  public static partial class WebClientExtensions
  {
    /// <summary>
    /// Downloads the specified resource as a file.
    /// </summary>
    /// <param name="client">The object that downloads the resource.</param>
    /// <param name="address">A <see cref="Uri"/> containing the URI to download.</param>
    /// <param name="fileName">The file to create or overwrite with the resource.</param>
    /// <returns>An observable that caches the result of the download and replays it to observers.</returns>
    public static IObservable<string> DownloadFileObservable(
      this WebClient client,
      Uri address,
      string fileName)
    {
      Contract.Requires(client != null);
      Contract.Requires(address != null);
      Contract.Requires(fileName != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return Observable2.FromEventBasedAsyncPattern<AsyncCompletedEventHandler, AsyncCompletedEventArgs>(
        handler => handler.Invoke,
        handler => client.DownloadFileCompleted += handler,
        handler => client.DownloadFileCompleted -= handler,
        token => client.DownloadFileAsync(address, fileName, token),
        client.CancelAsync)
        .Select(e => fileName);
    }

    /// <summary>
    /// Downloads the specified resource as a file and includes a channel for progress notifications.
    /// </summary>
    /// <param name="client">The object that downloads the resource.</param>
    /// <param name="address">A <see cref="Uri"/> containing the URI to download.</param>
    /// <param name="fileName">The file to create or overwrite with the resource.</param>
    /// <returns>An observable that contains progress notifications in the left channel and the file name in the right channel.</returns>
    public static IObservable<Either<DownloadProgressChangedEventArgs, string>> DownloadFileWithProgress(
      this WebClient client,
      Uri address,
      string fileName)
    {
      Contract.Requires(client != null);
      Contract.Requires(address != null);
      Contract.Requires(fileName != null);
      Contract.Ensures(Contract.Result<IObservable<Either<DownloadProgressChangedEventArgs, string>>>() != null);

      return Observable2.FromEventBasedAsyncPattern<AsyncCompletedEventHandler, AsyncCompletedEventArgs, DownloadProgressChangedEventHandler, DownloadProgressChangedEventArgs>(
        handler => handler.Invoke,
        handler => client.DownloadFileCompleted += handler,
        handler => client.DownloadFileCompleted -= handler,
        handler => handler.Invoke,
        handler => client.DownloadProgressChanged += handler,
        handler => client.DownloadProgressChanged -= handler,
        token => client.DownloadFileAsync(address, fileName, token),
        client.CancelAsync)
        .Select(
          left => left.EventArgs,
          right => fileName);
    }
  }
}