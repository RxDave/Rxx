using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("CombineLatest (New)")]
  [Description("Combining the latest values from multiple observables in one operation.")]
  public sealed class CombineLatestLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);

      var queries = new Subject<IObservable<string>>();

      var zipped = queries
        .CombineLatest()
        .Select(list => list.Aggregate(string.Empty, (acc, cur) => acc += cur + " "))
        .Take(7);

      using (zipped.Subscribe(ConsoleOutput))
      {
        var xs = Observable.Interval(TimeSpan.FromSeconds(1));
        var ys = Observable.Interval(TimeSpan.FromSeconds(1.5));
        var zs = Observable.Interval(TimeSpan.FromSeconds(3.5));

        queries.OnNext(xs.Select(value => Text.First + ' ' + value));
        queries.OnNext(ys.Select(value => Text.Second + ' ' + value));
        queries.OnNext(zs.Select(value => Text.Third + ' ' + value));
        queries.OnCompleted();

        WaitForKey();
      }
    }
  }
}