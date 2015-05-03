using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Rxx.UnitTests
{
  public sealed class TestTraceListener : TraceListener
  {
    #region Public Properties
    public IList<string> Messages
    {
      get
      {
        return messages.AsReadOnly();
      }
    }

    public IList<string> Events
    {
      get
      {
        return events.AsReadOnly();
      }
    }
    #endregion

    #region Private / Protected
    private readonly List<string> messages = new List<string>();
    private readonly List<string> events = new List<string>();
    private int line;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="TestTraceListener" /> class.
    /// </summary>
    public TestTraceListener()
    {
    }
    #endregion

    #region Methods
    public void Clear()
    {
      messages.Clear();

      line = 0;
    }

    public static bool EventEquals(string eventMessage, TraceEventType eventType, string message)
    {
      return EventEquals(eventMessage, eventType, message, StringComparison.Ordinal);
    }

    public static bool EventEquals(string eventMessage, TraceEventType eventType, string message, StringComparison comparison)
    {
      return string.Equals(eventMessage, FormatEvent(eventType, message), comparison);
    }

    private static string FormatEvent(TraceEventType eventType, object data)
    {
      return eventType.ToString() + ": " + (data == null ? string.Empty : data.ToString());
    }

    private static string FormatEvent(TraceEventType eventType, string message)
    {
      return eventType.ToString() + ": " + message;
    }

    public override void Write(string message)
    {
      if (line >= messages.Count)
        messages.Add(string.Empty);

      messages[line] += message;
    }

    public override void WriteLine(string message)
    {
      Write(message);
      line++;
    }

    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
    {
      events.Add(FormatEvent(eventType, data));

      WriteLine(data);
    }

    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
    {
      string message = data == null ? string.Empty : string.Join(",", data);

      events.Add(FormatEvent(eventType, message));

      WriteLine(message);
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
    {
      events.Add(FormatEvent(eventType, string.Empty));

      WriteLine(string.Empty);
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
    {
      string message = args == null
        ? format
        : string.Format(CultureInfo.CurrentCulture, format, args);

      events.Add(FormatEvent(eventType, message));

      WriteLine(message);
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
    {
      events.Add(FormatEvent(eventType, message));

      WriteLine(message);
    }

    public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
    {
      events.Add(FormatEvent(TraceEventType.Transfer, message));

      WriteLine(message);
    }
    #endregion
  }
}
