using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("Tracing")]
  [Description("One example of the many reactive Trace extensions.")]
  public sealed class TraceLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);

      IObservable<long> xs = Observable
        .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
        .TraceOnNext(value => "OnNext: " + value)
        .TraceSubscriptions();

      IObservable<long> query = Observable
        .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(3))
        .Select(_ => xs)
        .Switch();

      using (query.Subscribe())
      {
        WaitForKey();
      }
    }
  }
}