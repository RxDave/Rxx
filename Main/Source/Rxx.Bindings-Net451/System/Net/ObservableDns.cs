using System.Diagnostics.Contracts;
#if WINDOWS_PHONE
using System.Reactive.Linq;
using System.Reactive.Subjects;
#else
using System.Reactive.Threading.Tasks;
#endif
#if WINDOWS_PHONE
using Microsoft.Phone.Net.NetworkInformation;
#endif

namespace System.Net
{
  /// <summary>
  /// Provides <see langword="static"/> methods for asynchronously resolving domain names.
  /// </summary>
  public static class ObservableDns
  {
#if !WINDOWS_PHONE
    /// <summary>
    /// Resolves an IP address to an <see cref="IPHostEntry"/> instance asynchronously.
    /// </summary>
    /// <param name="address">An IP address.</param>
    /// <returns>A singleton observable containing an <see cref="IPHostEntry"/> instance that contains address information about
    /// the host specified in <paramref name="address"/>.</returns>
    public static IObservable<IPHostEntry> GetHostEntry(IPAddress address)
    {
      Contract.Requires(address != null);
      Contract.Ensures(Contract.Result<IObservable<IPHostEntry>>() != null);

#if !WINDOWS_PHONE
#if NET_45
      return Dns.GetHostEntryAsync(address).ToObservable();
#else
      return DnsEx.GetHostEntryAsync(address).ToObservable();
#endif
#else
      var invoke = Observable.FromAsyncPattern<IPAddress, IPHostEntry>(Dns.BeginGetHostEntry, Dns.EndGetHostEntry);

      var observable = invoke(address);

      Contract.Assume(observable != null);

      return observable;
#endif
    }

    /// <summary>
    /// Resolves a host name or IP address to an <see cref="IPHostEntry"/> instance asynchronously.
    /// </summary>
    /// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
    /// <returns>A singleton observable containing an <see cref="IPHostEntry"/> instance that contains address information about
    /// the host specified in <paramref name="hostNameOrAddress"/>.</returns>
    public static IObservable<IPHostEntry> GetHostEntry(string hostNameOrAddress)
    {
      Contract.Requires(hostNameOrAddress != null);
      Contract.Requires(hostNameOrAddress.Length < 256);
      Contract.Ensures(Contract.Result<IObservable<IPHostEntry>>() != null);

#if !WINDOWS_PHONE
#if NET_45
      return Dns.GetHostEntryAsync(hostNameOrAddress).ToObservable();
#else
      return DnsEx.GetHostEntryAsync(hostNameOrAddress).ToObservable();
#endif
#else
      var invoke = Observable.FromAsyncPattern<string, IPHostEntry>(Dns.BeginGetHostEntry, Dns.EndGetHostEntry);

      var observable = invoke(hostNameOrAddress);

      Contract.Assume(observable != null);

      return observable;
#endif
    }

    /// <summary>
    /// Returns a singleton observable containing the Internet Protocol (IP) addresses for the specified host.
    /// </summary>
    /// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
    /// <returns>A singleton observable containing an array of type <see cref="IPAddress"/> that holds the IP addresses for the
    /// host that is specified by the <paramref name="hostNameOrAddress"/> parameter.</returns>
    public static IObservable<IPAddress[]> GetHostAddresses(string hostNameOrAddress)
    {
      Contract.Requires(hostNameOrAddress != null);
      Contract.Requires(hostNameOrAddress.Length < 256);
      Contract.Ensures(Contract.Result<IObservable<IPAddress[]>>() != null);

#if !WINDOWS_PHONE
#if NET_45
      return Dns.GetHostAddressesAsync(hostNameOrAddress).ToObservable();
#else
      return DnsEx.GetHostAddressesAsync(hostNameOrAddress).ToObservable();
#endif
#else
      var invoke = Observable.FromAsyncPattern<string, IPAddress[]>(Dns.BeginGetHostAddresses, Dns.EndGetHostAddresses);

      var observable = invoke(hostNameOrAddress);

      Contract.Assume(observable != null);

      return observable;
#endif
    }
#else
    /// <summary>
    /// Asynchronously resolves a host name on the first available network interface.
    /// </summary>
    /// <param name="endPoint">A host name or IP address.</param>
    /// <returns>A singleton observable containing a <see cref="NameResolutionResult"/> instance that contains 
    /// address information about the host specified in <paramref name="endPoint"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "The Exception is passed to the subject.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "AsyncSubject does not require disposal here.")]
    public static IObservable<NameResolutionResult> ResolveHostName(DnsEndPoint endPoint)
    {
      Contract.Requires(endPoint != null);
      Contract.Ensures(Contract.Result<IObservable<NameResolutionResult>>() != null);

      var subject = new AsyncSubject<NameResolutionResult>();

      try
      {
        DeviceNetworkInformation.ResolveHostNameAsync(
          endPoint,
          result =>
          {
            var s = (AsyncSubject<NameResolutionResult>) result.AsyncState;

            s.OnNext(result);
            s.OnCompleted();
          },
          subject);
      }
      catch (Exception ex)
      {
        subject.OnError(ex);
      }

      return subject.AsObservable();
    }

    /// <summary>
    /// Asynchronously resolves a host name on the first available network interface.
    /// </summary>
    /// <param name="endPoint">A host name or IP address.</param>
    /// <param name="networkInterface">The network interface on which to resolve the host name.</param>
    /// <returns>A singleton observable containing a <see cref="NameResolutionResult"/> instance that contains 
    /// address information about the host specified in <paramref name="endPoint"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "The Exception is passed to the subject.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "AsyncSubject does not require disposal here.")]
    public static IObservable<NameResolutionResult> ResolveHostName(DnsEndPoint endPoint, NetworkInterfaceInfo networkInterface)
    {
      Contract.Requires(endPoint != null);
      Contract.Requires(networkInterface != null);
      Contract.Ensures(Contract.Result<IObservable<NameResolutionResult>>() != null);

      var subject = new AsyncSubject<NameResolutionResult>();

      try
      {
        DeviceNetworkInformation.ResolveHostNameAsync(
          endPoint,
          networkInterface,
          result =>
          {
            var s = (AsyncSubject<NameResolutionResult>) result.AsyncState;

            s.OnNext(result);
            s.OnCompleted();
          },
          subject);
      }
      catch (Exception ex)
      {
        subject.OnError(ex);
      }

      return subject.AsObservable();
    }
#endif
  }
}