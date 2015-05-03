using System.Diagnostics.Contracts;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace System.Net
{
#if !SILVERLIGHT && !PORT_45 && !PORT_40
  /// <summary>
  /// Provides <see langword="static"/> extension methods that return observable sequences for various asynchronous methods defined 
  /// by the <see cref="WebRequest"/>, <see cref="FtpWebRequest"/> and <see cref="HttpWebRequest"/> classes.
  /// </summary>
  public static class WebRequestExtensions
#else
  /// <summary>
  /// Provides <see langword="static"/> extension methods that return observable sequences for various asynchronous methods defined 
  /// by the <see cref="WebRequest"/> and <see cref="HttpWebRequest"/> classes.
  /// </summary>
  public static class WebRequestExtensions
#endif
  {
    /// <summary>
    /// Creates a <see cref="Stream"/> for writing data to the Internet resource.
    /// </summary>
    /// <param name="request">The <see cref="WebRequest"/> that provides the writable stream.</param>
    /// <returns>A singleton observable containing the writable <see cref="Stream"/>.</returns>
    public static IObservable<Stream> GetRequestStreamObservable(this WebRequest request)
    {
      Contract.Requires(request != null);
      Contract.Ensures(Contract.Result<IObservable<Stream>>() != null);

#if SILVERLIGHT || PORT_45 || PORT_40
      return Task.Factory.FromAsync<Stream>(
        request.BeginGetRequestStream,
        request.EndGetRequestStream,
        state: null)
        .ToObservable();
#else
      return Observable.StartAsync(request.GetRequestStreamAsync);
#endif
    }

#if !SILVERLIGHT && !PORT_45 && !PORT_40
    /// <summary>
    /// Creates a <see cref="Stream"/> for writing data to the Internet resource.
    /// </summary>
    /// <param name="request">The <see cref="HttpWebRequest"/> that provides the writable stream.</param>
    /// <returns>A singleton observable containing a tuple with the writable <see cref="Stream"/> as the first item
    /// and the <see cref="TransportContext"/> as the second item.</returns>
    public static IObservable<Tuple<Stream, TransportContext>> GetRequestStreamObservable(this HttpWebRequest request)
    {
      Contract.Requires(request != null);
      Contract.Ensures(Contract.Result<IObservable<Tuple<Stream, TransportContext>>>() != null);

      TransportContext context = null;

      return Task.Factory.FromAsync<Stream>(
        request.BeginGetRequestStream,
        asyncResult => request.EndGetRequestStream(asyncResult, out context),
        null)
        .ToObservable()
        .Select(stream => Tuple.Create(stream, context));
    }
#else
    /// <summary>
    /// Creates a <see cref="Stream"/> for writing data to the Internet resource.
    /// </summary>
    /// <param name="request">The <see cref="HttpWebRequest"/> that provides the writable stream.</param>
    /// <returns>A singleton observable containing the writable <see cref="Stream"/>.</returns>
    public static IObservable<Stream> GetRequestStreamObservable(this HttpWebRequest request)
    {
      Contract.Requires(request != null);
      Contract.Ensures(Contract.Result<IObservable<Stream>>() != null);

      return Task.Factory.FromAsync<Stream>(
        request.BeginGetRequestStream,
        asyncResult => request.EndGetRequestStream(asyncResult),
        null)
        .ToObservable();
    }
#endif

    /// <summary>
    /// Returns a response to an Internet request.
    /// </summary>
    /// <param name="request">The <see cref="WebRequest"/> that creates the response.</param>
    /// <returns>A singleton observable containing the response.</returns>
    public static IObservable<WebResponse> GetResponseObservable(this WebRequest request)
    {
      Contract.Requires(request != null);
      Contract.Ensures(Contract.Result<IObservable<WebResponse>>() != null);

#if SILVERLIGHT || PORT_45 || PORT_40
      return Task.Factory.FromAsync<WebResponse>(
        request.BeginGetResponse,
        request.EndGetResponse,
        state: null)
        .ToObservable();
#else
      return Observable.StartAsync(request.GetResponseAsync);
#endif
    }

    /// <summary>
    /// Returns a response to an Internet request.
    /// </summary>
    /// <param name="request">The <see cref="HttpWebRequest"/> that creates the response.</param>
    /// <returns>A singleton observable containing the response.</returns>
    public static IObservable<HttpWebResponse> GetResponseObservable(this HttpWebRequest request)
    {
      Contract.Requires(request != null);
      Contract.Ensures(Contract.Result<IObservable<HttpWebResponse>>() != null);

#if SILVERLIGHT || PORT_45 || PORT_40
      return Task.Factory.FromAsync<WebResponse>(
        request.BeginGetResponse,
        request.EndGetResponse,
        state: null)
        .ToObservable()
        .Cast<HttpWebResponse>();
#else
      return Observable.StartAsync(request.GetResponseAsync).Cast<HttpWebResponse>();
#endif
    }

#if !SILVERLIGHT && !PORT_45 && !PORT_40
    /// <summary>
    /// Returns the FTP server response.
    /// </summary>
    /// <param name="request">The <see cref="FtpWebRequest"/> that creates the response.</param>
    /// <returns>A singleton observable containing the response.</returns>
    public static IObservable<FtpWebResponse> GetResponseObservable(this FtpWebRequest request)
    {
      Contract.Requires(request != null);
      Contract.Ensures(Contract.Result<IObservable<FtpWebResponse>>() != null);

      return Observable.StartAsync(request.GetResponseAsync).Cast<FtpWebResponse>();
    }
#endif
  }
}