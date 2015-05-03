using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("Self-Observations")]
  [Description("Buffering or sampling while observing and also pairing an observable with introspection.")]
  public sealed class SelfObservingLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);
      TraceLine();

      RunExperiments();
    }

    [Description("Observes a timer and outputs the duration of each observation.")]
    public void DurationExperiment()
    {
      IObservable<Either<IObservable<long>, long>> xs = Observable
        .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
        .Take(4)
        .Introspect();

      using (xs.SubscribeEither(
        durationWindow =>
        {
          durationWindow.Subscribe(
            ConsoleOutputOnNext<long>(Text.Duration),
            ConsoleOutputOnError(Text.Duration),
            ConsoleOutputOnCompleted(Text.Duration));
        },
        ConsoleOutputOnNext<long>(
          value =>
          {
            Thread.Sleep(TimeSpan.FromSeconds(2));

            return value.ToString(CultureInfo.CurrentCulture);
          }),
        ConsoleOutputOnCompleted()))
      {
        WaitForKey();
      }
    }

    [Description("Observes a timer concurrently and outputs the values collected during each observation.")]
    public void BufferExperiment()
    {
      IObservable<IList<long>> xs = Observable
        .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
        .Take(10)
        .BufferIntrospective();

      using (xs.Subscribe(
        ConsoleOutputOnNext<IList<long>>(
          values =>
          {
            TraceLine(Text.ObservingFormat, values.Count);

            Thread.Sleep(TimeSpan.FromSeconds(2.5));

            return values.Aggregate(
              new System.Text.StringBuilder(),
              (acc, cur) => acc.Append(cur).Append(','),
              acc => acc.ToString(0, Math.Max(0, acc.Length - 1)));
          }),
        ConsoleOutputOnCompleted()))
      {
        WaitForKey();
      }
    }

    [Description("Observes a timer concurrently and outputs the last value collected during each observation.")]
    public void SampleExperiment()
    {
      IObservable<long> xs = Observable
        .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
        .Take(10)
        .SampleIntrospective();

      using (xs.Subscribe(
        ConsoleOutputOnNext<long>(
          value =>
          {
            Thread.Sleep(TimeSpan.FromSeconds(2.5));

            return value.ToString(CultureInfo.CurrentCulture);
          }),
        ConsoleOutputOnCompleted()))
      {
        WaitForKey();
      }
    }
  }
}