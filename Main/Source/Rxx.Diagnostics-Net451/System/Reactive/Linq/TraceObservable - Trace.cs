using System.Diagnostics.Contracts;
using TraceSource = System.Diagnostics.TraceSource;

namespace System.Reactive.Linq
{
  /// <summary>
  /// Provides extension methods that trace observables.
  /// </summary>
  public static partial class TraceObservable
  {
    #region System.Diagnostics.Trace
    /// <summary>
    /// Returns an observable that traces OnNext, OnError and OnCompleted calls from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <returns>An observable that traces all notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> Trace<T>(this IObservable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>());
    }

    /// <summary>
    /// Returns an observable that traces OnNext calls from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <returns>An observable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnNext<T>(this IObservable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(TraceDefaults.DefaultOnNext));
    }

    /// <summary>
    /// Returns an observable that traces OnNext calls from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="format">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An observable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnNext<T>(this IObservable<T> source, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(TraceDefaults.GetFormatOnNext<T>(format)));
    }

    /// <summary>
    /// Returns an observable that traces OnNext calls from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for each notification.</param>
    /// <returns>An observable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnNext<T>(this IObservable<T> source, Func<T, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(messageSelector));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnError from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the error will be traced.</param>
    /// <returns>An observable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnError<T>(this IObservable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(_ => null, TraceDefaults.DefaultOnError));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnError from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the error will be traced.</param>
    /// <param name="format">The format in which the error will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An observable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnError<T>(this IObservable<T> source, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(_ => null, TraceDefaults.GetFormatOnError(format)));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnError from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the error will be traced.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the error.</param>
    /// <returns>An observable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnError<T>(this IObservable<T> source, Func<Exception, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(_ => null, messageSelector));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnCompleted from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the completed notification will be traced.</param>
    /// <returns>An observable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnCompleted<T>(this IObservable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(_ => null, TraceDefaults.DefaultOnCompleted));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnCompleted from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the completed notification will be traced.</param>
    /// <param name="message">The message to be traced for the completed notification.</param>
    /// <returns>An observable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnCompleted<T>(this IObservable<T> source, string message)
    {
      Contract.Requires(source != null);
      Contract.Requires(message != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(_ => null, TraceDefaults.GetMessageOnCompleted(message)));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnCompleted from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the completed notification will be traced.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the completed notification.</param>
    /// <returns>An observable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnCompleted<T>(this IObservable<T> source, Func<string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(_ => null, messageSelector));
    }
    #endregion

    #region System.Diagnostics.TraceSource
    /// <summary>
    /// Returns an observable that traces OnNext, OnError and OnCompleted calls from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="traceSource">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An observable that traces all notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> Trace<T>(this IObservable<T> source, TraceSource traceSource)
    {
      Contract.Requires(source != null);
      Contract.Requires(traceSource != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(traceSource));
    }

    /// <summary>
    /// Returns an observable that traces OnNext calls from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An observable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnNext<T>(this IObservable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(trace, TraceDefaults.DefaultOnNext));
    }

    /// <summary>
    /// Returns an observable that traces OnNext calls from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="format">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An observable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnNext<T>(this IObservable<T> source, TraceSource trace, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(trace, TraceDefaults.GetFormatOnNext<T>(format)));
    }

    /// <summary>
    /// Returns an observable that traces OnNext calls from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for each notification.</param>
    /// <returns>An observable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnNext<T>(this IObservable<T> source, TraceSource trace, Func<T, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(trace, messageSelector));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnError from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the error will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An observable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnError<T>(this IObservable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(trace, _ => null, TraceDefaults.DefaultOnError));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnError from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the error will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="format">The format in which the error will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An observable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnError<T>(this IObservable<T> source, TraceSource trace, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(trace, _ => null, TraceDefaults.GetFormatOnError(format)));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnError from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the error will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the error.</param>
    /// <returns>An observable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnError<T>(this IObservable<T> source, TraceSource trace, Func<Exception, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(trace, _ => null, messageSelector));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnCompleted from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the completed notification will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An observable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnCompleted<T>(this IObservable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(trace, _ => null, TraceDefaults.DefaultOnCompleted));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnCompleted from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the completed notification will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="message">The message to be traced for the completed notification.</param>
    /// <returns>An observable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnCompleted<T>(this IObservable<T> source, TraceSource trace, string message)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(message != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(trace, _ => null, TraceDefaults.GetMessageOnCompleted(message)));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnCompleted from the specified observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the completed notification will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the completed notification.</param>
    /// <returns>An observable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceOnCompleted<T>(this IObservable<T> source, TraceSource trace, Func<string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new TraceObserver<T>(trace, _ => null, messageSelector));
    }
    #endregion
  }
}