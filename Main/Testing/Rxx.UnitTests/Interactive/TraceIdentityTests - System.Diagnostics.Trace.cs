using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Interactive
{
  [TestClass]
  public partial class TraceIdentityTests : RxxTraceTests
  {
    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceIdentity()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentity().ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Concat(
          Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(id, value)),
          TraceDefaults.DefaultOnCompleted(id)));

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceIdentityOnNext()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnNext().ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(id, value)));

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceIdentityOnNextFormat()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnNext("OnNext: {0}={1}").ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + id + "=" + value));

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceIdentityOnNextLazyMessage()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnNext((oId, value) => "OnNext: " + oId + "=" + value).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + id + "=" + value));

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceIdentityOnError()
    {
      AddTraceListener();

      var ex = new RxxMockException("Error");
      var xs = EnumerableEx.Throw<int>(ex);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnError().Catch<int, Exception>(_ => Enumerable.Empty<int>()).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, TraceDefaults.DefaultOnError(id, ex));

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceIdentityOnErrorFormat()
    {
      AddTraceListener();

      var ex = new RxxMockException("Error");
      var xs = EnumerableEx.Throw<int>(ex);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnError("OnError: {0}={1}").Catch<int, Exception>(_ => Enumerable.Empty<int>()).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnError: " + id + "=" + ex.ToString());

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceIdentityOnErrorLazyMessage()
    {
      AddTraceListener();

      var ex = new RxxMockException("Error");
      var xs = EnumerableEx.Throw<int>(ex);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnError((oId, error) => "OnError: " + oId + "=" + error.Message).Catch<int, Exception>(_ => Enumerable.Empty<int>()).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnError: " + id + "=" + ex.Message);

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceIdentityOnCompleted()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnCompleted().ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, TraceDefaults.DefaultOnCompleted(id));

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceIdentityOnCompletedFormat()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnCompleted("OnCompleted: {0}").ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnCompleted: " + id);

        Listener.Clear();
      }

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceIdentityOnCompletedLazyMessage()
    {
      AddTraceListener();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnCompleted(oId => "OnCompleted: " + oId).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnCompleted: " + id);

        Listener.Clear();
      }

      RemoveTraceListener();
    }
  }
}