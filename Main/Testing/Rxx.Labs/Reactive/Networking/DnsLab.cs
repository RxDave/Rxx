using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive.Networking
{
  [DisplayName("DNS")]
  [Description("Resolving DNS information by host name or IP address.")]
  public sealed class DnsLab : BaseConsoleLab
  {
    protected override void Main()
    {
      var target = UserInput(Text.PromptFormat, Text.HostNameOrAddress);

      TraceLine(Instructions.PressAnyKeyToCancel);

#if WINDOWS_PHONE && !WINDOWS_PHONE_70
			IObservable<string> host = ObservableDns.ResolveHostName(new DnsEndPoint(target, 0))
				.Select(entry => entry.HostName
					+ Environment.NewLine
					+ entry.IPEndPoints.Aggregate("IP Addresses: ", (acc, cur) => acc + Environment.NewLine + cur)
					+ Environment.NewLine
					+ "Interface Name: " + entry.NetworkInterface.InterfaceName);
#else
      IObservable<string> host = ObservableDns.GetHostEntry(target)
        .Select(entry => entry.HostName
          + Environment.NewLine
          + entry.AddressList.Aggregate("IP Addresses: ", (acc, cur) => acc + Environment.NewLine + cur)
          + Environment.NewLine
          + entry.Aliases.Aggregate("Aliases: ", (acc, cur) => acc + Environment.NewLine + cur));
#endif

      using (host.Subscribe(ConsoleOutput))
      {
        WaitForKey();
      }
    }
  }
}