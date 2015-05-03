using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using TraceSource = System.Diagnostics.TraceSource;

namespace System.Linq
{
  /// <summary>
  /// Provides extension methods that trace enumerables.
  /// </summary>
  public static partial class TraceEnumerable
  {
    #region System.Diagnostics.Trace
    /// <summary>
    /// Returns an enumerable that traces OnNext, OnError and OnCompleted calls from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <returns>An enumerable that traces all notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> Trace<T>(this IEnumerable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>());

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext calls from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <returns>An enumerable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnNext<T>(this IEnumerable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(TraceDefaults.DefaultOnNext));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext calls from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="format">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An enumerable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnNext<T>(this IEnumerable<T> source, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(TraceDefaults.GetFormatOnNext<T>(format)));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext calls from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for each notification.</param>
    /// <returns>An enumerable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnNext<T>(this IEnumerable<T> source, Func<T, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(messageSelector));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnError from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the error will be traced.</param>
    /// <returns>An enumerable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnError<T>(this IEnumerable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(_ => null, TraceDefaults.DefaultOnError));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnError from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the error will be traced.</param>
    /// <param name="format">The format in which the error will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An enumerable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnError<T>(this IEnumerable<T> source, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(_ => null, TraceDefaults.GetFormatOnError(format)));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnError from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the error will be traced.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the error.</param>
    /// <returns>An enumerable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnError<T>(this IEnumerable<T> source, Func<Exception, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(_ => null, messageSelector));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnCompleted from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the completed notification will be traced.</param>
    /// <returns>An enumerable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnCompleted<T>(this IEnumerable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(_ => null, TraceDefaults.DefaultOnCompleted));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnCompleted from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the completed notification will be traced.</param>
    /// <param name="message">The message to be traced for the completed notification.</param>
    /// <returns>An enumerable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnCompleted<T>(this IEnumerable<T> source, string message)
    {
      Contract.Requires(source != null);
      Contract.Requires(message != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(_ => null, TraceDefaults.GetMessageOnCompleted(message)));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnCompleted from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the completed notification will be traced.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the completed notification.</param>
    /// <returns>An enumerable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnCompleted<T>(this IEnumerable<T> source, Func<string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(_ => null, messageSelector));

      Contract.Assume(enumerable != null);

      return enumerable;
    }
    #endregion

    #region System.Diagnostics.TraceSource
    /// <summary>
    /// Returns an enumerable that traces OnNext, OnError and OnCompleted calls from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="traceSource">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An enumerable that traces all notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> Trace<T>(this IEnumerable<T> source, TraceSource traceSource)
    {
      Contract.Requires(source != null);
      Contract.Requires(traceSource != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(traceSource));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext calls from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An enumerable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnNext<T>(this IEnumerable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(trace, TraceDefaults.DefaultOnNext));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext calls from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="format">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An enumerable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnNext<T>(this IEnumerable<T> source, TraceSource trace, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(trace, TraceDefaults.GetFormatOnNext<T>(format)));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext calls from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for each notification.</param>
    /// <returns>An enumerable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnNext<T>(this IEnumerable<T> source, TraceSource trace, Func<T, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(trace, messageSelector));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnError from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the error will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An enumerable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnError<T>(this IEnumerable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(trace, _ => null, TraceDefaults.DefaultOnError));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnError from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the error will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="format">The format in which the error will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An enumerable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnError<T>(this IEnumerable<T> source, TraceSource trace, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(trace, _ => null, TraceDefaults.GetFormatOnError(format)));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnError from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the error will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the error.</param>
    /// <returns>An enumerable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnError<T>(this IEnumerable<T> source, TraceSource trace, Func<Exception, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(trace, _ => null, messageSelector));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnCompleted from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the completed notification will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An enumerable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnCompleted<T>(this IEnumerable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(trace, _ => null, TraceDefaults.DefaultOnCompleted));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnCompleted from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the completed notification will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="message">The message to be traced for the completed notification.</param>
    /// <returns>An enumerable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnCompleted<T>(this IEnumerable<T> source, TraceSource trace, string message)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(message != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(trace, _ => null, TraceDefaults.GetMessageOnCompleted(message)));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnCompleted from the specified enumerable.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the completed notification will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the completed notification.</param>
    /// <returns>An enumerable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceOnCompleted<T>(this IEnumerable<T> source, TraceSource trace, Func<string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new TraceObserver<T>(trace, _ => null, messageSelector));

      Contract.Assume(enumerable != null);

      return enumerable;
    }
    #endregion
  }
}