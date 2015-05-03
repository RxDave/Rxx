using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Labs
{
  internal static class EnumerableExtensions
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> Do<T>(this IEnumerable<T> source, Func<IObserver<object>> observerFactory)
    {
      Contract.Requires(observerFactory != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      Contract.Assume(source != null);

      var result = source.Do(new TypeCoercingObserver<T, object>(observerFactory()));

      Contract.Assume(result != null);

      return result;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "Exception is passed to the observer.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static void ForEach<T>(this IEnumerable<T> source, Func<IObserver<object>> observerFactory)
    {
      Contract.Requires(source != null);
      Contract.Requires(observerFactory != null);

      var observer = new TypeCoercingObserver<T, object>(observerFactory());

      try
      {
        source.ForEach(observer.OnNext);
      }
      catch (Exception ex)
      {
        observer.OnError(ex);
      }

      observer.OnCompleted();
    }
  }
}
