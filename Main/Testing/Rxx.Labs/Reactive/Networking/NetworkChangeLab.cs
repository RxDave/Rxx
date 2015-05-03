using System;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
#if SILVERLIGHT
using System.Reactive.Concurrency;
using System.Reactive.Linq;
#endif
#if WINDOWS_PHONE && !WINDOWS_PHONE_70
using System.Reactive.Subjects;
using Microsoft.Phone.Net.NetworkInformation;
#endif
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive.Networking
{
  [DisplayName("Network Connectivity")]
  [Description("Observing changes to network addresses and availability.")]
  public sealed class NetworkChangeLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);

#if WINDOWS_PHONE && !WINDOWS_PHONE_70
			IObservable<string> monitor = ObservableNetworkChange.NetworkAvailability
				.Select(e => e.NetworkInterface.InterfaceName + " -> " + e.NotificationType.ToString());
#else
      IObservable<bool> monitor = ObservableNetworkChange.NetworkAvailability;
#endif

#if SILVERLIGHT
			monitor = monitor.SubscribeOn(new DispatcherScheduler(Dispatcher));
#endif

#if WINDOWS_PHONE && !WINDOWS_PHONE_70
			var addresses = new Subject<NetworkInterfaceList>();

			Dispatcher.BeginInvoke(() => ObservableNetworkChange.NetworkAddress.Subscribe(addresses));

			using (addresses)
			using (addresses.Subscribe(
				ConsoleOutputOnNext<NetworkInterfaceList>(
					Text.NetworkAddresses,
					a => a.Aggregate(string.Empty, (acc, addr) => acc + Environment.NewLine + addr.InterfaceName)),
				ConsoleOutputOnCompleted()))
#elif !SILVERLIGHT
      IObservable<NetworkInterface[]> addresses = ObservableNetworkChange.NetworkAddress;

      using (addresses.Subscribe(
        ConsoleOutputOnNext<NetworkInterface[]>(
          Text.NetworkAddresses,
          a => a.Aggregate(string.Empty, (acc, addr) => acc + Environment.NewLine + addr.Name)),
        ConsoleOutputOnCompleted()))
#endif
      using (monitor.Subscribe(ConsoleOutput(Text.NetworkAvailability)))
      {
        TraceLine();
        TraceDescription(Instructions.DisableYourNetwork);

        WaitForKey();
      }
    }
  }
}