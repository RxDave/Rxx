using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net
{
  public static partial class ObservableWebClient
  {
    /// <summary>
    /// Uploads a string to the specified resource.
    /// </summary>
    /// <param name="address">The URI of the resource to receive the string.</param>
    /// <param name="method">The HTTP method used to send data to the resource.  If <see langword="null"/>, the default is POST for HTTP and STOR for FTP.</param>
    /// <param name="data">The string to send to the resource.</param>
    /// <returns>An observable that caches the response from the server and replays it to observers.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<string> UploadString(
      Uri address,
      string method,
      string data)
    {
      Contract.Requires(address != null);
      Contract.Requires(method != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

#if !SILVERLIGHT
      return Observable2.UsingHot(new WebClient(), client => client.UploadStringObservable(address, method, data));
#else
      return new WebClient().UploadStringObservable(address, method, data);
#endif
    }
  }
}