using System.Diagnostics.Contracts;
using System.Reactive.Linq;
#if WINDOWS_PHONE && !WINDOWS_PHONE_70 && !UNIVERSAL
using Microsoft.Phone.Net.NetworkInformation;
#endif

namespace System.Net.NetworkInformation
{
  /// <summary>
  /// Provides common properties for subscribing to changes in network status.
  /// </summary>
  public static class ObservableNetworkChange
  {
#if WINDOWS_PHONE && !WINDOWS_PHONE_70 && !UNIVERSAL
    /// <summary>
    /// Gets an observable that will notify the subscriber of any changes to network availability.
    /// </summary>
    public static IObservable<NetworkNotificationEventArgs> NetworkAvailability
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservable<NetworkNotificationEventArgs>>() != null);

        var changed = Observable.FromEventPattern<NetworkNotificationEventArgs>(
          eh => DeviceNetworkInformation.NetworkAvailabilityChanged += eh,
          eh => DeviceNetworkInformation.NetworkAvailabilityChanged -= eh);

        return changed.Select(e => e.EventArgs);
      }
    }
#else
    /// <summary>
    /// Gets an observable that will notify the subscriber of current network availability and then any changes to network availability.
    /// </summary>
    public static IObservable<bool> NetworkAvailability
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservable<bool>>() != null);

#if !SILVERLIGHT && !PORT_45 && !PORT_40
        var changed = Observable.FromEventPattern<NetworkAvailabilityChangedEventHandler, EventArgs>(
          eh => NetworkChange.NetworkAvailabilityChanged += eh,
          eh => NetworkChange.NetworkAvailabilityChanged -= eh);
#else
        var changed = Observable.FromEventPattern<NetworkAddressChangedEventHandler, EventArgs>(
          eh => NetworkChange.NetworkAddressChanged += eh,
          eh => NetworkChange.NetworkAddressChanged -= eh);
#endif

        return changed
          .Select(_ => NetworkInterface.GetIsNetworkAvailable())
          .Publish(NetworkInterface.GetIsNetworkAvailable())
          .RefCount()
          .DistinctUntilChanged();
      }
    }
#endif

#if WINDOWS_PHONE && !WINDOWS_PHONE_70 && !UNIVERSAL
    /// <summary>
    /// Gets an observable that will notify the subscriber of the current network address and then any changes to the network address.
    /// </summary>
    public static IObservable<NetworkInterfaceList> NetworkAddress
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservable<NetworkInterfaceList>>() != null);

        NetworkInterfaceList list = null;

        Func<NetworkInterfaceList> getList = () =>
          {
            if (list == null)
            {
              list = new NetworkInterfaceList();
            }

            return list;
          };

        return Observable.FromEventPattern<NetworkAddressChangedEventHandler, EventArgs>(
          eh => NetworkChange.NetworkAddressChanged += eh,
          eh => NetworkChange.NetworkAddressChanged -= eh)
          .Publish(initialValue: null)
          .RefCount()
          .Select(_ => getList())
          .Finally(() =>
          {
            if (list != null)
            {
              list.Dispose();
              list = null;
            }
          });
      }
    }
#elif !SILVERLIGHT && !PORT_45 && !PORT_40
    /// <summary>
    /// Gets an observable that will notify the subscriber of the current network address and then any changes to the network address.
    /// </summary>
    public static IObservable<NetworkInterface[]> NetworkAddress
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservable<NetworkInterface[]>>() != null);

        return Observable.FromEventPattern<NetworkAddressChangedEventHandler, EventArgs>(
          eh => NetworkChange.NetworkAddressChanged += eh,
          eh => NetworkChange.NetworkAddressChanged -= eh)
          .Select(_ => NetworkInterface.GetAllNetworkInterfaces())
          .Publish(NetworkInterface.GetAllNetworkInterfaces())
          .RefCount();
      }
    }
#endif
  }
}