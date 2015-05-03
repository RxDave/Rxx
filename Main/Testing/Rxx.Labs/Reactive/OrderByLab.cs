using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using DaveSexton.Labs;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("OrderBy (New)")]
  [Description("Sorting the elements of an observable sequence by keys or time.")]
  public sealed class OrderByLab : BaseConsoleLab
  {
    protected override void Main()
    {
      RunExperiments();
    }

    [Experiment("Value-Dependent")]
    [Description("Orders a sequence by buffering all elements and sorting by key.")]
    public void ValueDependentExperiment()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);
      TraceLine();

      var xs = Observable
        .Interval(TimeSpan.FromSeconds(1))
        .Take(5)
        .SelectMany(x => "ABC".Select(c => new { Index = x, Character = c }))
        .Do(ConsoleOutput(Text.Generated));

      var ordered =
        from x in xs
        orderby x.Index descending, x.Character descending
        select x;

      using (ordered.Subscribe(ConsoleOutput))
      {
        WaitForKey();
      }
    }

    [Experiment("Time-Dependent")]
    [Description("Orders a sequence by correlating elements with the times at which other sequences complete.")]
    public void TimeDependentExperiment()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);
      TraceLine();

      IObservable<int> xs =
        from x in Observable.Range(1, 5)
        orderby Observable.Timer(TimeSpan.FromSeconds(5 - x))
        select x;

      using (xs.Subscribe(ConsoleOutput))
      {
        WaitForKey();
      }
    }
  }
}