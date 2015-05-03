using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace Rxx.Labs
{
  internal static class ObservableExtensions
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> Do<T>(this IObservable<T> source, Func<IObserver<object>> observerFactory)
    {
      Contract.Requires(observerFactory != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      Contract.Assume(source != null);

      return source.Do(new TypeCoercingObserver<T, object>(observerFactory()));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "Exception is passed to the observer.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static void ForEach<T>(this IObservable<T> source, Func<IObserver<object>> observerFactory)
    {
      Contract.Requires(observerFactory != null);

      Contract.Assume(source != null);

      var observer = new TypeCoercingObserver<T, object>(observerFactory());

      try
      {
#if !WINDOWS_PHONE
        source.ForEachAsync(observer.OnNext).Wait();
#else
				source.ForEach(observer.OnNext);
#endif
      }
      catch (Exception ex)
      {
        observer.OnError(ex);
        return;
      }

      observer.OnCompleted();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IDisposable Subscribe<T>(this IObservable<T> source, Func<IObserver<object>> observerFactory)
    {
      Contract.Requires(observerFactory != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      Contract.Assume(source != null);

      var observer = new TypeCoercingObserver<T, object>(observerFactory());

      return source.Subscribe(observer);
    }
  }
}