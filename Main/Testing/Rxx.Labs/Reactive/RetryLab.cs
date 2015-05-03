using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using DaveSexton.Labs;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("Retry")]
  [Description("Pairing a faulty sequence with an OnError sequence.")]
  public sealed class RetryLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForError);
      TraceLine();

      RunExperiments();
    }

    private static int e;

    private IObservable<int> xs = Observable.Defer(
      () => e < 6
      ? Observable.Return(e).Concat(Observable.Throw<int>(new Exception("Error " + e++)))
      : Observable.Throw<int>(new Exception("Consecutive Error " + e++)));

    [Description("Retries a faulty sequence at most 5 times.")]
    public void RetryExperiment()
    {
      e = 1;
      IObservable<Either<int, Exception>> paired = xs.Retry<int, Exception>(5);

      using (paired.SubscribeEither(
        ConsoleOutputOnNext<int>(),
        ConsoleOutputOnNext<Exception>(ex => ex.Message),
        ConsoleOutputOnError()))
      {
        WaitForKey();
      }
    }

    [Description("Retries a faulty sequence an unlimited number of times, but at most 3 "
               + "consecutive errors.")]
    public void RetryConsecutiveExperiment()
    {
      e = 1;
      IObservable<Either<int, Exception>> consecutive = xs.RetryConsecutive(3);

      using (consecutive.SubscribeEither(
        ConsoleOutputOnNext<int>(),
        ConsoleOutputOnNext<Exception>(ex => ex.Message),
        ConsoleOutputOnError()))
      {
        WaitForKey();
      }
    }

    [Experiment("Retry with Linear Back-off")]
    [Description("Retries a faulty sequence at most 5 times, with a delay between retries "
               + "that increases in duration linearly.")]
    public void RetryLinearBackOffExperiment()
    {
      e = 1;
      IObservable<Either<int, Exception>> linearBackOff = xs.Retry(
        retryCount: 5,
        backOffSelector: (ex, attemptCount) => TimeSpan.FromSeconds(.5 * attemptCount));

      using (linearBackOff.SubscribeEither(
        ConsoleOutputOnNext<int>(),
        ConsoleOutputOnNext<Exception>(ex => ex.Message),
        ConsoleOutputOnError()))
      {
        WaitForKey();
      }
    }

    [Experiment("Retry with Exponential Back-off")]
    [Description("Retries a faulty sequence at most 5 times, with a delay between retries "
               + "that increases in duration exponentially.")]
    public void RetryExponentialBackOffExperiment()
    {
      e = 1;
      IObservable<Either<int, Exception>> exponentialBackOff = xs.Retry(
        retryCount: 5,
        backOffSelector: (ex, attemptCount) => TimeSpan.FromSeconds(.5 * Math.Pow(2, attemptCount - 1)));

      using (exponentialBackOff.SubscribeEither(
        ConsoleOutputOnNext<int>(),
        ConsoleOutputOnNext<Exception>(ex => ex.Message),
        ConsoleOutputOnError()))
      {
        WaitForKey();
      }
    }
  }
}