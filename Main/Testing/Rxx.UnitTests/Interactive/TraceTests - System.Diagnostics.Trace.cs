using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Interactive
{
  [TestClass]
  public partial class TraceTests : RxxTraceTests
  {
    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTrace()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      xs.Trace().ForEach();

      AssertEqual(Listener.Messages, Concat(
        Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(value)),
        TraceDefaults.DefaultOnCompleted()));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceOnNext()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      xs.TraceOnNext().ForEach();

      AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(value)));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceOnNextFormat()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      xs.TraceOnNext("OnNext: {0}").ForEach();

      AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + value));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceOnNextLazyMessage()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      xs.TraceOnNext(value => "OnNext: " + value).ForEach();

      AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + value));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceOnError()
    {
      AddTraceListener();

      var ex = new RxxMockException("Error");
      var xs = EnumerableEx.Throw<int>(ex);

      xs.TraceOnError().Catch<int, Exception>(_ => Enumerable.Empty<int>()).ForEach();

      AssertEqual(Listener.Messages, TraceDefaults.DefaultOnError(ex));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceOnErrorFormat()
    {
      AddTraceListener();

      var ex = new RxxMockException("Error");
      var xs = EnumerableEx.Throw<int>(ex);

      xs.TraceOnError("OnError: {0}").Catch<int, Exception>(_ => Enumerable.Empty<int>()).ForEach();

      AssertEqual(Listener.Messages, "OnError: " + ex.ToString());

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceOnErrorLazyMessage()
    {
      AddTraceListener();

      var ex = new RxxMockException("Error");
      var xs = EnumerableEx.Throw<int>(ex);

      xs.TraceOnError(error => "OnError: " + error.Message).Catch<int, Exception>(_ => Enumerable.Empty<int>()).ForEach();

      AssertEqual(Listener.Messages, "OnError: " + ex.Message);

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceOnCompleted()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      xs.TraceOnCompleted().ForEach();

      AssertEqual(Listener.Messages, TraceDefaults.DefaultOnCompleted());

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceOnCompletedFormat()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      xs.TraceOnCompleted("OnCompleted").ForEach();

      AssertEqual(Listener.Messages, "OnCompleted");

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceOnCompletedLazyMessage()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      xs.TraceOnCompleted(() => "OnCompleted").ForEach();

      AssertEqual(Listener.Messages, "OnCompleted");

      RemoveTraceListener();
    }
  }
}