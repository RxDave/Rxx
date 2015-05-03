using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Reactive
{
  public partial class TraceTests : RxxTraceTests
  {
    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSource()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      xs.Trace(source).Subscribe();

      AssertEqual(Listener.Messages, Concat(
        Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(value)),
        TraceDefaults.DefaultOnCompleted()));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceOnNext()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      xs.TraceOnNext(source).Subscribe();

      AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(value)));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceOnNextFormat()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      xs.TraceOnNext(source, "OnNext: {0}").Subscribe();

      AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + value));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceOnNextLazyMessage()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      xs.TraceOnNext(source, value => "OnNext: " + value).Subscribe();

      AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + value));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceOnError()
    {
      var source = CreateTraceSource();

      var ex = new Exception("Error");
      var xs = Observable.Throw<int>(ex);

      xs.TraceOnError(source).Subscribe(_ => { }, __ => { });

      AssertEqual(Listener.Messages, TraceDefaults.DefaultOnError(ex));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceOnErrorFormat()
    {
      var source = CreateTraceSource();

      var ex = new Exception("Error");
      var xs = Observable.Throw<int>(ex);

      xs.TraceOnError(source, "OnError: {0}").Subscribe(_ => { }, __ => { });

      AssertEqual(Listener.Messages, "OnError: " + ex.ToString());
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceOnErrorLazyMessage()
    {
      var source = CreateTraceSource();

      var ex = new Exception("Error");
      var xs = Observable.Throw<int>(ex);

      xs.TraceOnError(source, error => "OnError: " + error.Message).Subscribe(_ => { }, __ => { });

      AssertEqual(Listener.Messages, "OnError: " + ex.Message);
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceOnCompleted()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      xs.TraceOnCompleted(source).Subscribe();

      AssertEqual(Listener.Messages, TraceDefaults.DefaultOnCompleted());
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceOnCompletedFormat()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      xs.TraceOnCompleted(source, "OnCompleted").Subscribe();

      AssertEqual(Listener.Messages, "OnCompleted");
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceOnCompletedLazyMessage()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      xs.TraceOnCompleted(source, () => "OnCompleted").Subscribe();

      AssertEqual(Listener.Messages, "OnCompleted");
    }
  }
}