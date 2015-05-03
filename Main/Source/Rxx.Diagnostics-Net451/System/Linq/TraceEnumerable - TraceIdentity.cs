using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using TraceSource = System.Diagnostics.TraceSource;

namespace System.Linq
{
  public static partial class TraceEnumerable
  {
    #region System.Diagnostics.Trace
    /// <summary>
    /// Returns an enumerable that traces OnNext, OnError and OnCompleted calls from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <returns>An enumerable that traces all notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentity<T>(this IEnumerable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>());

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext, OnError and OnCompleted calls from the specified enumerable
    /// and includes the specified <paramref name="identity"/> in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="identity">Identifies the observer in the trace output.</param>
    /// <returns>An enumerable that traces all notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentity<T>(this IEnumerable<T> source, string identity)
    {
      Contract.Requires(source != null);
      Contract.Requires(!string.IsNullOrWhiteSpace(identity));
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>()
        {
          Identity = identity
        });

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext calls from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <returns>An enumerable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnNext<T>(this IEnumerable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(TraceDefaults.DefaultOnNext));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext calls from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="format">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An enumerable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnNext<T>(this IEnumerable<T> source, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(TraceDefaults.GetIdentityFormatOnNext<T>(format)));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext calls from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for each notification.</param>
    /// <returns>An enumerable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnNext<T>(this IEnumerable<T> source, Func<string, T, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(messageSelector));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnError from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the error will be traced.</param>
    /// <returns>An enumerable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnError<T>(this IEnumerable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>((a, b) => null, TraceDefaults.DefaultOnError));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnError from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the error will be traced.</param>
    /// <param name="format">The format in which the error will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An enumerable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnError<T>(this IEnumerable<T> source, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>((a, b) => null, TraceDefaults.GetIdentityFormatOnError(format)));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnError from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the error will be traced.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the error.</param>
    /// <returns>An enumerable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnError<T>(this IEnumerable<T> source, Func<string, Exception, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>((a, b) => null, messageSelector));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnCompleted from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the completed notification will be traced.</param>
    /// <returns>An enumerable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnCompleted<T>(this IEnumerable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>((a, b) => null, TraceDefaults.DefaultOnCompleted));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnCompleted from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the completed notification will be traced.</param>
    /// <param name="message">The message to be traced for the completed notification.</param>
    /// <returns>An enumerable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnCompleted<T>(this IEnumerable<T> source, string message)
    {
      Contract.Requires(source != null);
      Contract.Requires(message != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>((a, b) => null, TraceDefaults.GetIdentityMessageOnCompleted(message)));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnCompleted from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the completed notification will be traced.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the completed notification.</param>
    /// <returns>An enumerable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnCompleted<T>(this IEnumerable<T> source, Func<string, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>((a, b) => null, messageSelector));

      Contract.Assume(enumerable != null);

      return enumerable;
    }
    #endregion

    #region System.Diagnostics.TraceSource
    /// <summary>
    /// Returns an enumerable that traces OnNext, OnError and OnCompleted calls from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An enumerable that traces all notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentity<T>(this IEnumerable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(trace));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext, OnError and OnCompleted calls from the specified enumerable
    /// and includes the specified <paramref name="identity"/> in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="identity">Identifies the observer in the trace output.</param>
    /// <returns>An enumerable that traces all notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentity<T>(this IEnumerable<T> source, TraceSource trace, string identity)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(!string.IsNullOrWhiteSpace(identity));
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(trace)
      {
        Identity = identity
      });

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext calls from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An enumerable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnNext<T>(this IEnumerable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(trace, TraceDefaults.DefaultOnNext));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext calls from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="format">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An enumerable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnNext<T>(this IEnumerable<T> source, TraceSource trace, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(trace, TraceDefaults.GetIdentityFormatOnNext<T>(format)));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces OnNext calls from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for each notification.</param>
    /// <returns>An enumerable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnNext<T>(this IEnumerable<T> source, TraceSource trace, Func<string, T, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(trace, messageSelector));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnError from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the error will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An enumerable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnError<T>(this IEnumerable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(trace, (a, b) => null, TraceDefaults.DefaultOnError));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnError from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the error will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="format">The format in which the error will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An enumerable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnError<T>(this IEnumerable<T> source, TraceSource trace, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(trace, (a, b) => null, TraceDefaults.GetIdentityFormatOnError(format)));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnError from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the error will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the error.</param>
    /// <returns>An enumerable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnError<T>(this IEnumerable<T> source, TraceSource trace, Func<string, Exception, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(trace, (a, b) => null, messageSelector));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnCompleted from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the completed notification will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An enumerable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnCompleted<T>(this IEnumerable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(trace, (a, b) => null, TraceDefaults.DefaultOnCompleted));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnCompleted from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the completed notification will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="message">The message to be traced for the completed notification.</param>
    /// <returns>An enumerable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnCompleted<T>(this IEnumerable<T> source, TraceSource trace, string message)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(message != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(trace, (a, b) => null, TraceDefaults.GetIdentityMessageOnCompleted(message)));

      Contract.Assume(enumerable != null);

      return enumerable;
    }

    /// <summary>
    /// Returns an enumerable that traces a call to OnCompleted from the specified enumerable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">Type of object to be enumerated.</typeparam>
    /// <param name="source">The enumerable from which the completed notification will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the completed notification.</param>
    /// <returns>An enumerable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IEnumerable<T> TraceIdentityOnCompleted<T>(this IEnumerable<T> source, TraceSource trace, Func<string, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      var enumerable = source.Do(new IdentifiedTraceObserver<T>(trace, (a, b) => null, messageSelector));

      Contract.Assume(enumerable != null);

      return enumerable;
    }
    #endregion
  }
}