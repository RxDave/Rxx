using System.Diagnostics.Contracts;
using System.IO;
using System.Reactive.Linq;

namespace System.Net
{
  public static partial class ObservableWebClient
  {
    /// <summary>
    /// Opens a writeable stream to the specified resource.
    /// </summary>
    /// <param name="address">The URI of the resource to receive the stream.</param>
    /// <param name="method">The HTTP method used to send data to the resource.  If <see langword="null"/>, the default is POST for HTTP and STOR for FTP.</param>
    /// <returns>An observable containing the writeable stream that sends data to the resource.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "WebClient is composited by the UsingHot operator.")]
    public static IObservable<Stream> OpenWrite(
      Uri address,
      string method)
    {
      Contract.Requires(address != null);
      Contract.Requires(method != null);
      Contract.Ensures(Contract.Result<IObservable<Stream>>() != null);

#if !SILVERLIGHT
      return Observable2.UsingHot(new WebClient(), client => client.OpenWriteObservable(address, method));
#else
      return new WebClient().OpenWriteObservable(address, method);
#endif
    }
  }
}