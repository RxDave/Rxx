using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Interactive
{
  public partial class TraceTests : RxxTraceTests
  {
    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSource()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      xs.Trace(source).ForEach();

      AssertEqual(Listener.Messages, Concat(
        Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(value)),
        TraceDefaults.DefaultOnCompleted()));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceOnNext()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      xs.TraceOnNext(source).ForEach();

      AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(value)));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceOnNextFormat()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      xs.TraceOnNext(source, "OnNext: {0}").ForEach();

      AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + value));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceOnNextLazyMessage()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      xs.TraceOnNext(source, value => "OnNext: " + value).ForEach();

      AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + value));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceOnError()
    {
      var source = CreateTraceSource();

      var ex = new RxxMockException("Error");
      var xs = EnumerableEx.Throw<int>(ex);

      xs.TraceOnError(source).Catch<int, Exception>(_ => Enumerable.Empty<int>()).ForEach();

      AssertEqual(Listener.Messages, TraceDefaults.DefaultOnError(ex));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceOnErrorFormat()
    {
      var source = CreateTraceSource();

      var ex = new RxxMockException("Error");
      var xs = EnumerableEx.Throw<int>(ex);

      xs.TraceOnError(source, "OnError: {0}").Catch<int, Exception>(_ => Enumerable.Empty<int>()).ForEach();

      AssertEqual(Listener.Messages, "OnError: " + ex.ToString());
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceOnErrorLazyMessage()
    {
      var source = CreateTraceSource();

      var ex = new RxxMockException("Error");
      var xs = EnumerableEx.Throw<int>(ex);

      xs.TraceOnError(source, error => "OnError: " + error.Message).Catch<int, Exception>(_ => Enumerable.Empty<int>()).ForEach();

      AssertEqual(Listener.Messages, "OnError: " + ex.Message);
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceOnCompleted()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      xs.TraceOnCompleted(source).ForEach();

      AssertEqual(Listener.Messages, TraceDefaults.DefaultOnCompleted());
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceOnCompletedFormat()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      xs.TraceOnCompleted(source, "OnCompleted").ForEach();

      AssertEqual(Listener.Messages, "OnCompleted");
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceOnCompletedLazyMessage()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      xs.TraceOnCompleted(source, () => "OnCompleted").ForEach();

      AssertEqual(Listener.Messages, "OnCompleted");
    }
  }
}