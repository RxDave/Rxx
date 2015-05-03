using System;
using System.Diagnostics;
using System.Globalization;

namespace Rxx.UnitTests
{
  public abstract class RxxTraceTests : RxxTests
  {
    #region Public Properties
    public TestTraceListener Listener
    {
      get
      {
        return testListener;
      }
    }
    #endregion

    #region Private / Protected
    protected static string GetCurrentId()
    {
      return TraceDefaults.IdentityCounter.ToString(CultureInfo.InvariantCulture);
    }

    private readonly TestTraceListener testListener = new TestTraceListener();
    #endregion

    #region Constructors
    protected RxxTraceTests()
    {
    }
    #endregion

    #region Methods
    protected void AddTraceListener()
    {
      testListener.Clear();

      Trace.Listeners.Add(testListener);
    }

    protected void RemoveTraceListener()
    {
      Trace.Listeners.Remove(testListener);
    }

    protected TestTraceSource CreateTraceSource()
    {
      testListener.Clear();

      var source = new TestTraceSource();

      source.Listeners.Add(testListener);

      return source;
    }
    #endregion
  }
}
