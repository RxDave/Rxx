using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reactive;
using DaveSexton.Labs;
using Rxx.Labs.Properties;

namespace Rxx.Labs
{
  internal sealed partial class LabObserver<T> : ObserverBase<T>
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields",
      Justification = "Required for Silverlight and Windows Phone builds, yet must remain for the console build because the code is shared.")]
    private readonly LabTraceSource trace;
    private readonly bool showTimeOnNext, showValues;
    private readonly string namePrefix, valueFormat;
    private bool hasValue;

    public LabObserver(LabTraceSource trace)
      : this(trace, showTimeOnNext: true)
    {
      Contract.Requires(trace != null);
    }

    public LabObserver(LabTraceSource trace, bool showTimeOnNext)
    {
      Contract.Requires(trace != null);

      this.trace = trace;
      this.showTimeOnNext = showTimeOnNext;
      this.showValues = !(typeof(T) == typeof(Unit));
    }

    public LabObserver(LabTraceSource trace, string name)
      : this(trace, name, showTimeOnNext: true)
    {
      Contract.Requires(trace != null);
      Contract.Requires(!string.IsNullOrEmpty(name));
    }

    public LabObserver(LabTraceSource trace, string name, bool showTimeOnNext)
    {
      Contract.Requires(trace != null);
      Contract.Requires(!string.IsNullOrEmpty(name));

      this.trace = trace;
      this.namePrefix = name + ' ';
      this.showTimeOnNext = showTimeOnNext;
      this.showValues = !(typeof(T) == typeof(Unit));
    }

    internal LabObserver(LabTraceSource trace, string name, string valueFormat, bool showTimeOnNext)
    {
      Contract.Requires(trace != null);
      Contract.Requires(!string.IsNullOrEmpty(valueFormat));

      this.trace = trace;
      this.namePrefix = string.IsNullOrEmpty(name) ? null : name + ' ';
      this.valueFormat = valueFormat;
      this.showTimeOnNext = showTimeOnNext;
      this.showValues = !(typeof(T) == typeof(Unit));
    }

    [ContractInvariantMethod]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(trace != null);
    }

    public static LabObserver<T> Error(LabTraceSource trace)
    {
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<LabObserver<T>>() != null);

      return new LabObserver<T>(trace)
      {
        hasValue = true		// assumption
      };
    }

    public static LabObserver<T> Error(LabTraceSource trace, string name)
    {
      Contract.Requires(trace != null);
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Ensures(Contract.Result<LabObserver<T>>() != null);

      return new LabObserver<T>(trace, name)
      {
        hasValue = true		// assumption
      };
    }

    public static LabObserver<T> Completed(LabTraceSource trace)
    {
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<LabObserver<T>>() != null);

      return new LabObserver<T>(trace)
      {
        hasValue = true		// assumption
      };
    }

    public static LabObserver<T> Completed(LabTraceSource trace, string name)
    {
      Contract.Requires(trace != null);
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Ensures(Contract.Result<LabObserver<T>>() != null);

      return new LabObserver<T>(trace, name)
      {
        hasValue = true		// assumption
      };
    }

    public void StartTimer()
    {
      trace.ResetTime();
    }

    protected override void OnNextCore(T value)
    {
      hasValue = true;

      if (showValues)
      {
        // Do not call value.ToString() if a format is specified.  The format may require a particular type; e.g., {0:MM-dd-yyyy}
        var formattedValue = valueFormat == null
          ? value == null ? Text.Null : value.ToString()
          : string.Format(CultureInfo.CurrentCulture, valueFormat, value);

        if (showTimeOnNext)
        {
          trace.TraceTime(Text.OnNextTimeFormat, namePrefix, formattedValue);
        }
        else
        {
          trace.TraceLine(Text.OnNextFormat, namePrefix, formattedValue);
        }
      }
    }

    protected override void OnErrorCore(Exception error)
    {
      trace.TraceTime(Text.OnErrorTimeFormat, namePrefix, error.Message);
    }

    protected override void OnCompletedCore()
    {
      if (!hasValue)
      {
        trace.TraceWarning(Text.OnCompletedEmptyFormat, namePrefix);
      }

      trace.TraceTime(Text.OnCompletedTimeFormat, namePrefix);
    }
  }
}