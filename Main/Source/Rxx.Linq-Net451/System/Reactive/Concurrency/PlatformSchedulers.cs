using System.Diagnostics.Contracts;

namespace System.Reactive.Concurrency
{
  internal static class PlatformSchedulers
  {
    public static IScheduler Concurrent
    {
      get
      {
        Contract.Ensures(Contract.Result<IScheduler>() != null);

        return Scheduler.Default;
      }
    }
  }
}