using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DaveSexton.Labs;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("Paired Observables")]
  [Description("Tracking progress with the Pair operator.")]
  public sealed class PairingLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);
      TraceLine();

      RunExperiments();
    }

    [Experiment("Value-Dependent")]
    [Description("Pairs an interval with the projection of each value's progress.")]
    public void ValueDependentExperiment()
    {
      IObservable<double> source = Observable.Interval(TimeSpan.FromSeconds(.5))
        .Select(v => (double)++v)
        .Take(10);

      IConnectableObservable<Either<double, double>> sourceWithProgress =
        source
          .Pair(value => value * 10 / 100)
          .Publish();

      IObservable<double> progressOnly = sourceWithProgress.TakeRight();
      IObservable<double> resultOnly = sourceWithProgress.TakeLeft().TakeLast(1);

      using (progressOnly.Subscribe(ConsoleOutputFormat(Text.Progress, "{0,5:P0}")))
      using (resultOnly.Subscribe(ConsoleOutput(Text.Result)))
      using (sourceWithProgress.Connect())
      {
        WaitForKey();
      }
    }

    [Experiment("Time-Dependent")]
    [Description("Pairs an interval with an out-of-band progress sequence.")]
    public void TimeDependentExperiment()
    {
      IObservable<double> progressByTime = Observable.Interval(TimeSpan.FromSeconds(.5))
        .Select(v => (double)++v * 10 / 100)
        .Take(10)
        .Publish()
        .Prime();

      IObservable<long> source = Observable.Interval(TimeSpan.FromSeconds(.2))
        .TakeUntil(progressByTime.Where(progress => progress == 1));

      IConnectableObservable<Either<long, double>> sourceWithProgress =
        source
          .Pair(progressByTime)
          .Publish();

      IObservable<double> progressOnly = sourceWithProgress.TakeRight().DistinctUntilChanged();
      IObservable<long> resultOnly = sourceWithProgress.TakeLeft().TakeLast(1);

      using (progressOnly.Subscribe(ConsoleOutputFormat(Text.Progress, "{0,5:P0}")))
      using (resultOnly.Subscribe(ConsoleOutput(Text.Result)))
      using (sourceWithProgress.Connect())
      {
        WaitForKey();
      }
    }
  }
}