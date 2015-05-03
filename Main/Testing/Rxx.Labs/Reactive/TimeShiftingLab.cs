using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("Time Shifting")]
  [Description("Converting sequences of arbitrary data to Intervals, Timers and Pulses.")]
  public sealed class TimeShiftingLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);
      TraceLine();

      RunExperiments();
    }

    private IObservable<int> input = Observable.Range(1, 3)
      .Concat(Observable.Empty<int>().Delay(TimeSpan.FromSeconds(4.5)))
      .Concat(Observable.Range(4, 3));

    [Description("Converts a sequence of time-influenced values into a well-behaved interval.")]
    public void IntervalExperiment()
    {
      IObservable<int> xs = input.AsInterval(TimeSpan.FromSeconds(1));

      using (xs.Subscribe(ConsoleOutput))
      {
        WaitForKey();
      }
    }

    [Description("Converts a sequence of time-influenced values into a well-behaved timer.")]
    public void TimerExperiment()
    {
      IObservable<int> xs = input.AsTimer(TimeSpan.FromSeconds(1));

      using (xs.Subscribe(ConsoleOutput))
      {
        WaitForKey();
      }
    }

    [Description("Converts a sequence of time-influenced values into a well-behaved timer that "
               + "never goes silent.")]
    public void PulseExperiment()
    {
      IObservable<int> xs = input.Pulse(TimeSpan.FromSeconds(1));

      using (xs.Subscribe(ConsoleOutput))
      {
        WaitForKey();
      }
    }
  }
}