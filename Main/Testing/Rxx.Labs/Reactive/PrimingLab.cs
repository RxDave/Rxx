using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("Primed Observables")]
  [Description("Using the Prime and PrimeStart operators.")]
  public sealed class PrimingLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForCompletion);
      TraceLine();

      RunExperiments();
    }

    [Description("The Prime operator connects a connectable observable automatically on the first "
               + "subscription and never disconnects.")]
    public void PrimeExperiment()
    {
      IObservable<long> xs = Observable.Interval(TimeSpan.FromSeconds(1))
        .Do(ConsoleOutput(Text.Generated))
        .Take(3);

      IObservable<long> pruned = xs.PublishLast().Prime();

      for (int r = 0; r < 2; r++)
      {
        using (var subscriptions = new CompositeDisposable())
        {
          for (int s = 0; s < 5; s++)
          {
            Thread.Sleep(TimeSpan.FromSeconds(.4));

            TraceLine(Text.SubscribingFormat, s);

            subscriptions.Add(
              pruned.Subscribe(ConsoleOutput(Text.NamedObserverFormat, s)));
          }

          Thread.Sleep(TimeSpan.FromSeconds(2));
        }
      }
    }

    [Description("StartPrimed creates a warm observable for the specified action or function.  "
               + "Essentially, it works like a lazy Start, invoking the action or function upon "
               + "the first subscription only.")]
    public void StartPrimedExperiment()
    {
      int count = 0;
      IObservable<int> ys = Observable2.StartPrimed(() => count++);

      Thread.Sleep(TimeSpan.FromSeconds(1));

      TraceLine(Text.StartedFormat, count > 0);
      TraceLine(Text.Subscribing);

      ys.Subscribe(ConsoleOutput);

      Thread.Sleep(TimeSpan.FromSeconds(1));

      TraceLine(Text.StartedFormat, count > 0);

      ys.Subscribe(ConsoleOutput);

      Thread.Sleep(TimeSpan.FromSeconds(1));

      TraceLine(Text.GeneratedFormat, count > 1);
    }
  }
}