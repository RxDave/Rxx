using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net
{
  public static partial class WebClientExtensions
  {
    /// <summary>
    /// Uploads a string to the specified resource.
    /// </summary>
    /// <param name="client">The object that uploads to the resource.</param>
    /// <param name="address">The URI of the resource to receive the string.</param>
    /// <param name="method">The HTTP method used to send data to the resource.  If <see langword="null"/>, the default is POST for HTTP and STOR for FTP.</param>
    /// <param name="data">The string to send to the resource.</param>
    /// <returns>An observable that caches the response from the server and replays it to observers.</returns>
    public static IObservable<string> UploadStringObservable(
      this WebClient client,
      Uri address,
      string method,
      string data)
    {
      Contract.Requires(client != null);
      Contract.Requires(address != null);
      Contract.Requires(method != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return Observable2.FromEventBasedAsyncPattern<UploadStringCompletedEventHandler, UploadStringCompletedEventArgs>(
        handler => handler.Invoke,
        handler => client.UploadStringCompleted += handler,
        handler => client.UploadStringCompleted -= handler,
        token => client.UploadStringAsync(address, method, data, token),
        client.CancelAsync)
        .Select(e => e.EventArgs.Result);
    }
  }
}