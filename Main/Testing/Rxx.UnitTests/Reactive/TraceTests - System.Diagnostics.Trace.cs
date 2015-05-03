using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Reactive
{
  [TestClass]
  public partial class TraceTests : RxxTraceTests
  {
    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTrace()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      xs.Trace().Subscribe();

      AssertEqual(Listener.Messages, Concat(
        Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(value)),
        TraceDefaults.DefaultOnCompleted()));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceOnNext()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      xs.TraceOnNext().Subscribe();

      AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(value)));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceOnNextFormat()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      xs.TraceOnNext("OnNext: {0}").Subscribe();

      AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + value));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceOnNextLazyMessage()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      xs.TraceOnNext(value => "OnNext: " + value).Subscribe();

      AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + value));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceOnError()
    {
      AddTraceListener();

      var ex = new Exception("Error");
      var xs = Observable.Throw<int>(ex);

      xs.TraceOnError().Subscribe(_ => { }, __ => { });

      AssertEqual(Listener.Messages, TraceDefaults.DefaultOnError(ex));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceOnErrorFormat()
    {
      AddTraceListener();

      var ex = new Exception("Error");
      var xs = Observable.Throw<int>(ex);

      xs.TraceOnError("OnError: {0}").Subscribe(_ => { }, __ => { });

      AssertEqual(Listener.Messages, "OnError: " + ex.ToString());

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceOnErrorLazyMessage()
    {
      AddTraceListener();

      var ex = new Exception("Error");
      var xs = Observable.Throw<int>(ex);

      xs.TraceOnError(error => "OnError: " + error.Message).Subscribe(_ => { }, __ => { });

      AssertEqual(Listener.Messages, "OnError: " + ex.Message);

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceOnCompleted()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      xs.TraceOnCompleted().Subscribe();

      AssertEqual(Listener.Messages, TraceDefaults.DefaultOnCompleted());

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceOnCompletedFormat()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      xs.TraceOnCompleted("OnCompleted").Subscribe();

      AssertEqual(Listener.Messages, "OnCompleted");

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceOnCompletedLazyMessage()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      xs.TraceOnCompleted(() => "OnCompleted").Subscribe();

      AssertEqual(Listener.Messages, "OnCompleted");

      RemoveTraceListener();
    }
  }
}