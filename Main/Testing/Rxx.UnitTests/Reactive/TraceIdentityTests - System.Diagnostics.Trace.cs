using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Reactive
{
  [TestClass]
  public partial class TraceIdentityTests : RxxTraceTests
  {
    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceIdentity()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentity().Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Concat(
          Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(id, value)),
          TraceDefaults.DefaultOnCompleted(id)));

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceIdentityOnNext()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnNext().Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(id, value)));

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceIdentityOnNextFormat()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnNext("OnNext: {0}={1}").Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + id + "=" + value));

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceIdentityOnNextLazyMessage()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnNext((oId, value) => "OnNext: " + oId + "=" + value).Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + id + "=" + value));

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceIdentityOnError()
    {
      AddTraceListener();

      var ex = new Exception("Error");
      var xs = Observable.Throw<int>(ex);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnError().Subscribe(_ => { }, __ => { });

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, TraceDefaults.DefaultOnError(id, ex));

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceIdentityOnErrorFormat()
    {
      AddTraceListener();

      var ex = new Exception("Error");
      var xs = Observable.Throw<int>(ex);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnError("OnError: {0}={1}").Subscribe(_ => { }, __ => { });

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnError: " + id + "=" + ex.ToString());

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceIdentityOnErrorLazyMessage()
    {
      AddTraceListener();

      var ex = new Exception("Error");
      var xs = Observable.Throw<int>(ex);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnError((oId, error) => "OnError: " + oId + "=" + error.Message).Subscribe(_ => { }, __ => { });

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnError: " + id + "=" + ex.Message);

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceIdentityOnCompleted()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnCompleted().Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, TraceDefaults.DefaultOnCompleted(id));

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceIdentityOnCompletedFormat()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnCompleted("OnCompleted: {0}").Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnCompleted: " + id);

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceIdentityOnCompletedLazyMessage()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnCompleted(oId => "OnCompleted: " + oId).Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnCompleted: " + id);

        Listener.Clear();
      }

      RemoveTraceListener();
    }
  }
}