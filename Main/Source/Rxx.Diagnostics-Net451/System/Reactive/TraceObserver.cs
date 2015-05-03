using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  /// <summary>
  /// Provides a mechanism for tracing push-based notifications.
  /// </summary>
  /// <typeparam name="T">Type of value notifications.</typeparam>
  public class TraceObserver<T> : ObserverBase<T>
  {
    private readonly Func<T, string> onNext;
    private readonly Func<Exception, string> onError;
    private readonly Func<string> onCompleted;
    private readonly TraceSource trace;

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class with default trace actions for all notification kinds.
    /// </summary>
    public TraceObserver()
      : this(TraceDefaults.DefaultOnNext, TraceDefaults.DefaultOnError, TraceDefaults.DefaultOnCompleted)
    {
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext calls.
    /// </summary>
    /// <param name="onNext">A function that returns the message to be traced for each notification.</param>
    public TraceObserver(Func<T, string> onNext)
    {
      Contract.Requires(onNext != null);

      this.onNext = onNext;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext and OnError calls.
    /// </summary>
    /// <param name="onNext">A function that returns the message to be traced for each notification.</param>
    /// <param name="onError">A function that returns the message to be traced for the error.</param>
    public TraceObserver(Func<T, string> onNext, Func<Exception, string> onError)
      : this(onNext)
    {
      Contract.Requires(onNext != null);
      Contract.Requires(onError != null);

      this.onError = onError;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext and OnCompleted calls.
    /// </summary>
    /// <param name="onNext">A function that returns the message to be traced for each notification.</param>
    /// <param name="onCompleted">A function that returns the message to be traced for the completed notification.</param>
    public TraceObserver(Func<T, string> onNext, Func<string> onCompleted)
      : this(onNext)
    {
      Contract.Requires(onNext != null);
      Contract.Requires(onCompleted != null);

      this.onCompleted = onCompleted;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext, OnError and OnCompleted calls.
    /// </summary>
    /// <param name="onNext">A function that returns the message to be traced for each notification.</param>
    /// <param name="onError">A function that returns the message to be traced for the error.</param>
    /// <param name="onCompleted">A function that returns the message to be traced for the completed notification.</param>
    public TraceObserver(Func<T, string> onNext, Func<Exception, string> onError, Func<string> onCompleted)
      : this(onNext, onError)
    {
      Contract.Requires(onNext != null);
      Contract.Requires(onError != null);
      Contract.Requires(onCompleted != null);

      this.onCompleted = onCompleted;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext calls.
    /// </summary>
    /// <param name="nextFormat">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    public TraceObserver(string nextFormat)
      : this(TraceDefaults.GetFormatOnNext<T>(nextFormat))
    {
      Contract.Requires(nextFormat != null);
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext and OnError calls.
    /// </summary>
    /// <param name="nextFormat">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    /// <param name="errorFormat">The format in which the error will be traced.  A single replacement token {0} is supported.</param>
    public TraceObserver(string nextFormat, string errorFormat)
      : this(TraceDefaults.GetFormatOnNext<T>(nextFormat), TraceDefaults.GetFormatOnError(errorFormat))
    {
      Contract.Requires(nextFormat != null);
      Contract.Requires(errorFormat != null);
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext, OnError and OnCompleted calls.
    /// </summary>
    /// <param name="nextFormat">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    /// <param name="errorFormat">The format in which the error will be traced.  A single replacement token {0} is supported.</param>
    /// <param name="completedMessage">The message to be traced for the completed notification.</param>
    public TraceObserver(string nextFormat, string errorFormat, string completedMessage)
      : this(TraceDefaults.GetFormatOnNext<T>(nextFormat), TraceDefaults.GetFormatOnError(errorFormat), TraceDefaults.GetMessageOnCompleted(completedMessage))
    {
      Contract.Requires(nextFormat != null);
      Contract.Requires(errorFormat != null);
      Contract.Requires(completedMessage != null);
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class with default trace actions for all notification kinds.
    /// </summary>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    public TraceObserver(TraceSource trace)
      : this(trace, TraceDefaults.DefaultOnNext, TraceDefaults.DefaultOnError, TraceDefaults.DefaultOnCompleted)
    {
      Contract.Requires(trace != null);
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext calls.
    /// </summary>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="onNext">A function that returns the message to be traced for each notification.</param>
    public TraceObserver(TraceSource trace, Func<T, string> onNext)
      : this(onNext)
    {
      Contract.Requires(trace != null);
      Contract.Requires(onNext != null);

      this.trace = trace;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext and OnError calls.
    /// </summary>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="onNext">A function that returns the message to be traced for each notification.</param>
    /// <param name="onError">A function that returns the message to be traced for the error.</param>
    public TraceObserver(TraceSource trace, Func<T, string> onNext, Func<Exception, string> onError)
      : this(onNext, onError)
    {
      Contract.Requires(trace != null);
      Contract.Requires(onNext != null);
      Contract.Requires(onError != null);

      this.trace = trace;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext and OnCompleted calls.
    /// </summary>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="onNext">A function that returns the message to be traced for each notification.</param>
    /// <param name="onCompleted">A function that returns the message to be traced for the completed notification.</param>
    public TraceObserver(TraceSource trace, Func<T, string> onNext, Func<string> onCompleted)
      : this(onNext, onCompleted)
    {
      Contract.Requires(trace != null);
      Contract.Requires(onNext != null);
      Contract.Requires(onCompleted != null);

      this.trace = trace;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext, OnError and OnCompleted calls.
    /// </summary>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="onNext">A function that returns the message to be traced for each notification.</param>
    /// <param name="onError">A function that returns the message to be traced for the error.</param>
    /// <param name="onCompleted">A function that returns the message to be traced for the completed notification.</param>
    public TraceObserver(TraceSource trace, Func<T, string> onNext, Func<Exception, string> onError, Func<string> onCompleted)
      : this(onNext, onError, onCompleted)
    {
      Contract.Requires(trace != null);
      Contract.Requires(onNext != null);
      Contract.Requires(onError != null);
      Contract.Requires(onCompleted != null);

      this.trace = trace;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext calls.
    /// </summary>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="nextFormat">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    public TraceObserver(TraceSource trace, string nextFormat)
      : this(nextFormat)
    {
      Contract.Requires(trace != null);
      Contract.Requires(nextFormat != null);

      this.trace = trace;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext and OnError calls.
    /// </summary>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="nextFormat">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    /// <param name="errorFormat">The format in which the error will be traced.  A single replacement token {0} is supported.</param>
    public TraceObserver(TraceSource trace, string nextFormat, string errorFormat)
      : this(nextFormat, errorFormat)
    {
      Contract.Requires(trace != null);
      Contract.Requires(nextFormat != null);
      Contract.Requires(errorFormat != null);

      this.trace = trace;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TraceObserver{T}"/> class for tracing OnNext, OnError and OnCompleted calls.
    /// </summary>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="nextFormat">The format in which values will be traced.  A single replacement token {0} is supported.</param>
    /// <param name="errorFormat">The format in which the error will be traced.  A single replacement token {0} is supported.</param>
    /// <param name="completedMessage">The message to be traced for the completed notification.</param>
    public TraceObserver(TraceSource trace, string nextFormat, string errorFormat, string completedMessage)
      : this(nextFormat, errorFormat, completedMessage)
    {
      Contract.Requires(trace != null);
      Contract.Requires(nextFormat != null);
      Contract.Requires(errorFormat != null);
      Contract.Requires(completedMessage != null);

      this.trace = trace;
    }

    /// <summary>
    /// Formats the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to be formatted.</param>
    /// <returns>The formatted value if calls to <see cref="IObserver{T}.OnNext"/> are supported by this instance; otherwise, <see langword="null" />.</returns>
    protected virtual string FormatOnNext(T value)
    {
      if (onNext != null)
        return onNext(value);
      else
        return null;
    }

    /// <summary>
    /// Formats the specified <paramref name="exception"/>.
    /// </summary>
    /// <param name="exception">The exception to be formatted.</param>
    /// <returns>The formatted exception if calls to <see cref="IObserver{T}.OnError"/> are supported by this instance and the specified <paramref name="exception"/> is 
    /// not <see langword="null" />; otherwise, <see langword="null" />.</returns>
    protected virtual string FormatOnError(Exception exception)
    {
      if (onError != null && exception != null)
        return onError(exception);
      else
        return null;
    }

    /// <summary>
    /// Returns a string for <see cref="IObserver{T}.OnCompleted"/>.
    /// </summary>
    /// <returns>The string to be traced if calls to <see cref="IObserver{T}.OnCompleted"/> are supported by this instance; otherwise, <see langword="null" />.</returns>
    protected virtual string FormatOnCompleted()
    {
      if (onCompleted != null)
        return onCompleted();
      else
        return null;
    }

    /// <inheritdoc />
    protected override void OnNextCore(T value)
    {
      var message = FormatOnNext(value);

      if (message != null)
      {
        if (trace != null)
        {
          trace.TraceInformation(message);
        }
        else
        {
          Trace.TraceInformation(message);
        }
      }
    }

    /// <inheritdoc />
    protected override void OnErrorCore(Exception error)
    {
      var message = FormatOnError(error);

      if (message != null)
      {
        if (trace != null)
        {
          trace.TraceEvent(TraceEventType.Error, 0, message);
        }
        else
        {
          Trace.TraceError(message);
        }
      }
    }

    /// <inheritdoc />
    protected override void OnCompletedCore()
    {
      var message = FormatOnCompleted();

      if (message != null)
      {
        if (trace != null)
        {
          trace.TraceInformation(message);
        }
        else
        {
          Trace.TraceInformation(message);
        }
      }
    }
  }
}