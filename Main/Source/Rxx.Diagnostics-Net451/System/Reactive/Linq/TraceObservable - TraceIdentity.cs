using System.Diagnostics.Contracts;
using TraceSource = System.Diagnostics.TraceSource;

namespace System.Reactive.Linq
{
  public static partial class TraceObservable
  {
    #region System.Diagnostics.Trace
    /// <summary>
    /// Returns an observable that traces OnNext, OnError and OnCompleted calls from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <returns>An observable that traces all notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentity<T>(this IObservable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>());
    }

    /// <summary>
    /// Returns an observable that traces OnNext, OnError and OnCompleted calls from the specified observable
    /// and includes the specified <paramref name="identity"/> in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="identity">Identifies the observer in the trace output.</param>
    /// <returns>An observable that traces all notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentity<T>(this IObservable<T> source, string identity)
    {
      Contract.Requires(source != null);
      Contract.Requires(!string.IsNullOrWhiteSpace(identity));
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>()
        {
          Identity = identity
        });
    }

    /// <summary>
    /// Returns an observable that traces OnNext calls from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <returns>An observable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnNext<T>(this IObservable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(TraceDefaults.DefaultOnNext));
    }

    /// <summary>
    /// Returns an observable that traces OnNext calls from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="format">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An observable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnNext<T>(this IObservable<T> source, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(TraceDefaults.GetIdentityFormatOnNext<T>(format)));
    }

    /// <summary>
    /// Returns an observable that traces OnNext calls from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for each notification.</param>
    /// <returns>An observable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnNext<T>(this IObservable<T> source, Func<string, T, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(messageSelector));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnError from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the error will be traced.</param>
    /// <returns>An observable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnError<T>(this IObservable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>((a, b) => null, TraceDefaults.DefaultOnError));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnError from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the error will be traced.</param>
    /// <param name="format">The format in which the error will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An observable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnError<T>(this IObservable<T> source, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>((a, b) => null, TraceDefaults.GetIdentityFormatOnError(format)));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnError from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the error will be traced.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the error.</param>
    /// <returns>An observable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnError<T>(this IObservable<T> source, Func<string, Exception, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>((a, b) => null, messageSelector));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnCompleted from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the completed notification will be traced.</param>
    /// <returns>An observable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnCompleted<T>(this IObservable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>((a, b) => null, TraceDefaults.DefaultOnCompleted));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnCompleted from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the completed notification will be traced.</param>
    /// <param name="message">The message to be traced for the completed notification.</param>
    /// <returns>An observable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnCompleted<T>(this IObservable<T> source, string message)
    {
      Contract.Requires(source != null);
      Contract.Requires(message != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>((a, b) => null, TraceDefaults.GetIdentityMessageOnCompleted(message)));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnCompleted from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the completed notification will be traced.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the completed notification.</param>
    /// <returns>An observable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnCompleted<T>(this IObservable<T> source, Func<string, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>((a, b) => null, messageSelector));
    }
    #endregion

    #region System.Diagnostics.TraceSource
    /// <summary>
    /// Returns an observable that traces OnNext, OnError and OnCompleted calls from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An observable that traces all notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentity<T>(this IObservable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(trace));
    }

    /// <summary>
    /// Returns an observable that traces OnNext, OnError and OnCompleted calls from the specified observable
    /// and includes the specified <paramref name="identity"/> in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="identity">Identifies the observer in the trace output.</param>
    /// <returns>An observable that traces all notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentity<T>(this IObservable<T> source, TraceSource trace, string identity)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(!string.IsNullOrWhiteSpace(identity));
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(trace)
      {
        Identity = identity
      });
    }

    /// <summary>
    /// Returns an observable that traces OnNext calls from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An observable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnNext<T>(this IObservable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(trace, TraceDefaults.DefaultOnNext));
    }

    /// <summary>
    /// Returns an observable that traces OnNext calls from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="format">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An observable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnNext<T>(this IObservable<T> source, TraceSource trace, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(trace, TraceDefaults.GetIdentityFormatOnNext<T>(format)));
    }

    /// <summary>
    /// Returns an observable that traces OnNext calls from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which notifications will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for each notification.</param>
    /// <returns>An observable that traces OnNext notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnNext<T>(this IObservable<T> source, TraceSource trace, Func<string, T, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(trace, messageSelector));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnError from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the error will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An observable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnError<T>(this IObservable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(trace, (a, b) => null, TraceDefaults.DefaultOnError));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnError from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the error will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="format">The format in which the error will be traced.  A single replacement token {0} is supported.</param>
    /// <returns>An observable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnError<T>(this IObservable<T> source, TraceSource trace, string format)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(trace, (a, b) => null, TraceDefaults.GetIdentityFormatOnError(format)));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnError from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the error will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the error.</param>
    /// <returns>An observable that traces a call to OnError.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnError<T>(this IObservable<T> source, TraceSource trace, Func<string, Exception, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(trace, (a, b) => null, messageSelector));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnCompleted from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the completed notification will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An observable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnCompleted<T>(this IObservable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(trace, (a, b) => null, TraceDefaults.DefaultOnCompleted));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnCompleted from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the completed notification will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="message">The message to be traced for the completed notification.</param>
    /// <returns>An observable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnCompleted<T>(this IObservable<T> source, TraceSource trace, string message)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(message != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(trace, (a, b) => null, TraceDefaults.GetIdentityMessageOnCompleted(message)));
    }

    /// <summary>
    /// Returns an observable that traces a call to OnCompleted from the specified observable
    /// and includes an auto-generated identifier in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable from which the completed notification will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="messageSelector">A function that returns the message to be traced for the completed notification.</param>
    /// <returns>An observable that traces a call to OnCompleted.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Internal object that doesn't need disposal.")]
    public static IObservable<T> TraceIdentityOnCompleted<T>(this IObservable<T> source, TraceSource trace, Func<string, string> messageSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(messageSelector != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.Do(new IdentifiedTraceObserver<T>(trace, (a, b) => null, messageSelector));
    }
    #endregion
  }
}