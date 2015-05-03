using System.Diagnostics.Contracts;
#if !PORT_45 && !PORT_40 && !UNIVERSAL
using System.Threading;
#endif

namespace System.Reactive.Concurrency
{
  /// <summary>
  /// Provides <see langword="static"/> members that provide information about the runtime environment with regard to concurrency.
  /// </summary>
  public static class ConcurrencyEnvironment
  {
    /// <summary>
    /// Stores the calculated default maximum concurrency for processing CPU-bound tasks in parallel.
    /// </summary>
    public static readonly int DefaultMaximumConcurrency = GetDefaultMaximumConcurrency();

    private static int GetDefaultMaximumConcurrency()
    {
      Contract.Ensures(Contract.Result<int>() > 0);

#if !PORT_45 && !PORT_40 && !UNIVERSAL
      int worker, io;
      ThreadPool.GetMaxThreads(out worker, out io);
#else
      const int io = 1000;
#endif

      // TODO: This is an arbitrary formula.  Do some research to find a better solution.
      int maxConcurrent = (io / 8) * Environment.ProcessorCount;

      Contract.Assume(maxConcurrent > 0);

      return maxConcurrent;
    }
  }
}