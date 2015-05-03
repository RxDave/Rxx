using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("Zip (New)")]
  [Description("Zipping multiple observables in one operation.")]
  public sealed class ZipLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);

      var queries = new Subject<IObservable<string>>();

      var zipped = queries
        .Zip()
        .Select(list => list.Aggregate(string.Empty, (acc, cur) => acc += cur + " "))
        .Take(3);

      using (zipped.Subscribe(ConsoleOutput))
      {
        var xs = Observable.Interval(TimeSpan.FromSeconds(.5));
        var ys = Observable.Interval(TimeSpan.FromSeconds(1));
        var zs = Observable.Interval(TimeSpan.FromSeconds(1.5));

        queries.OnNext(xs.Select(value => Text.First + ' ' + value));
        queries.OnNext(ys.Select(value => Text.Second + ' ' + value));
        queries.OnNext(zs.Select(value => Text.Third + ' ' + value));
        queries.OnCompleted();

        WaitForKey();
      }
    }
  }
}