using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Interactive
{
  public partial class TraceIdentityTests : RxxTraceTests
  {
    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceIdentity()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentity(source).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Concat(
          Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(id, value)),
          TraceDefaults.DefaultOnCompleted(id)));

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceIdentityOnNext()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnNext(source).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => TraceDefaults.DefaultOnNext(id, value)));

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceIdentityOnNextFormat()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnNext(source, "OnNext: {0}={1}").ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + id + "=" + value));

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceIdentityOnNextLazyMessage()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnNext(source, (oId, value) => "OnNext: " + oId + "=" + value).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, Enumerable.Range(0, 5).Select(value => "OnNext: " + id + "=" + value));

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceIdentityOnError()
    {
      var source = CreateTraceSource();

      var ex = new RxxMockException("Error");
      var xs = EnumerableEx.Throw<int>(ex);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnError(source).Catch<int, Exception>(_ => Enumerable.Empty<int>()).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, TraceDefaults.DefaultOnError(id, ex));

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceIdentityOnErrorFormat()
    {
      var source = CreateTraceSource();

      var ex = new RxxMockException("Error");
      var xs = EnumerableEx.Throw<int>(ex);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnError(source, "OnError: {0}={1}").Catch<int, Exception>(_ => Enumerable.Empty<int>()).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnError: " + id + "=" + ex.ToString());

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceIdentityOnErrorLazyMessage()
    {
      var source = CreateTraceSource();

      var ex = new RxxMockException("Error");
      var xs = EnumerableEx.Throw<int>(ex);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnError(source, (oId, error) => "OnError: " + oId + "=" + error.Message).Catch<int, Exception>(_ => Enumerable.Empty<int>()).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnError: " + id + "=" + ex.Message);

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceIdentityOnCompleted()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnCompleted(source).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, TraceDefaults.DefaultOnCompleted(id));

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceIdentityOnCompletedFormat()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnCompleted(source, "OnCompleted: {0}").ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnCompleted: " + id);

        Listener.Clear();
      }
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Ix")]
    public void IxTraceSourceIdentityOnCompletedLazyMessage()
    {
      var source = CreateTraceSource();

      var xs = Enumerable.Range(0, 5);

      for (int i = 0; i < 3; i++)
      {
        xs.TraceIdentityOnCompleted(source, oId => "OnCompleted: " + oId).ForEach();

        string id = GetCurrentId();

        AssertEqual(Listener.Messages, "OnCompleted: " + id);

        Listener.Clear();
      }
    }
  }
}