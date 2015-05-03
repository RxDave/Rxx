using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Reactive
{
  public partial class TraceIdentityTests : RxxTraceTests
  {
    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceIdentity()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentity(source).Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Concat(
          Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(id, value)),
          TraceDefaults.DefaultOnCompleted(id)));

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceIdentityOnNext()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnNext(source).Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(id, value)));

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceIdentityOnNextFormat()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnNext(source, "OnNext: {0}={1}").Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + id + "=" + value));

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceIdentityOnNextLazyMessage()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnNext(source, (oId, value) => "OnNext: " + oId + "=" + value).Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + id + "=" + value));

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceIdentityOnError()
    {
      var source = CreateTraceSource();

      var ex = new Exception("Error");
      var xs = Observable.Throw<int>(ex);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnError(source).Subscribe(_ => { }, __ => { });

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, TraceDefaults.DefaultOnError(id, ex));

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceIdentityOnErrorFormat()
    {
      var source = CreateTraceSource();

      var ex = new Exception("Error");
      var xs = Observable.Throw<int>(ex);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnError(source, "OnError: {0}={1}").Subscribe(_ => { }, __ => { });

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnError: " + id + "=" + ex.ToString());

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceIdentityOnErrorLazyMessage()
    {
      var source = CreateTraceSource();

      var ex = new Exception("Error");
      var xs = Observable.Throw<int>(ex);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnError(source, (oId, error) => "OnError: " + oId + "=" + error.Message).Subscribe(_ => { }, __ => { });

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnError: " + id + "=" + ex.Message);

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceIdentityOnCompleted()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnCompleted(source).Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, TraceDefaults.DefaultOnCompleted(id));

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceIdentityOnCompletedFormat()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnCompleted(source, "OnCompleted: {0}").Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnCompleted: " + id);

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceIdentityOnCompletedLazyMessage()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnCompleted(source, oId => "OnCompleted: " + oId).Subscribe();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnCompleted: " + id);

        Listener.Clear();
      }
    }
  }
}