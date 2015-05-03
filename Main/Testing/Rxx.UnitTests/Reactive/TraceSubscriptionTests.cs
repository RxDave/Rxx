using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rxx.Diagnostics.Properties;

namespace Rxx.UnitTests.Reactive
{
  [TestClass]
  public class TraceSubscriptionTests : RxxTraceTests
  {
    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSubscriptions()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5)
        .TraceSubscriptions();

      const int iterations = 3;

      xs.Repeat(iterations).Subscribe();

      AssertEqual(Listener.Messages, new[]
        {
          Text.DefaultSubscribingMessage, 
          Text.DefaultSubscribedMessage, 
          Text.DefaultDisposingSubscriptionMessage, 
          Text.DefaultDisposedSubscriptionMessage
        }
        .Repeat(iterations));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSubscriptionsNamed()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5)
        .TraceSubscriptions("Test Sequence");

      const int iterations = 3;

      xs.Repeat(iterations).Subscribe();

      AssertEqual(Listener.Messages, new[]
        {
          string.Format(Text.SubscribingFormat, "Test Sequence"),
          string.Format(Text.SubscribedFormat, "Test Sequence"),
          string.Format(Text.DisposingSubscriptionFormat, "Test Sequence"),
          string.Format(Text.DisposedSubscriptionFormat, "Test Sequence")
        }
        .Repeat(iterations));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSubscriptionsConnectMessage()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5)
        .TraceSubscriptions("First", "Second");

      const int iterations = 3;

      xs.Repeat(iterations).Subscribe();

      AssertEqual(Listener.Messages, new[]
        {
          "First", 
          "Second", 
          Text.DefaultDisposingSubscriptionMessage, 
          Text.DefaultDisposedSubscriptionMessage
        }
        .Repeat(iterations));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSubscriptionsLifetimeMessages()
    {
      AddTraceListener();

      var xs = Observable.Range(0, 5)
        .TraceSubscriptions("First", "Second", "Third", "Fourth");

      const int iterations = 3;

      xs.Repeat(iterations).Subscribe();

      AssertEqual(Listener.Messages, new[]
        {
          "First", 
          "Second", 
          "Third", 
          "Fourth"
        }
        .Repeat(iterations));

      RemoveTraceListener();
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceSubscriptions()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5)
        .TraceSubscriptions(source);

      const int iterations = 3;

      xs.Repeat(iterations).Subscribe();

      AssertEqual(Listener.Messages, new[]
        {
          Text.DefaultSubscribingMessage, 
          Text.DefaultSubscribedMessage, 
          Text.DefaultDisposingSubscriptionMessage, 
          Text.DefaultDisposedSubscriptionMessage
        }
        .Repeat(iterations));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceSubscriptionsNamed()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5)
        .TraceSubscriptions(source, "Test Sequence");

      const int iterations = 3;

      xs.Repeat(iterations).Subscribe();

      AssertEqual(Listener.Messages, new[]
        {
          string.Format(Text.SubscribingFormat, "Test Sequence"),
          string.Format(Text.SubscribedFormat, "Test Sequence"),
          string.Format(Text.DisposingSubscriptionFormat, "Test Sequence"),
          string.Format(Text.DisposedSubscriptionFormat, "Test Sequence")
        }
        .Repeat(iterations));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceSubscriptionsConnectMessage()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5)
        .TraceSubscriptions(source, "First", "Second");

      const int iterations = 3;

      xs.Repeat(iterations).Subscribe();

      AssertEqual(Listener.Messages, new[]
        {
          "First", 
          "Second", 
          Text.DefaultDisposingSubscriptionMessage, 
          Text.DefaultDisposedSubscriptionMessage
        }
        .Repeat(iterations));
    }

    [TestMethod, TestCategory("Trace"), TestCategory("Rx")]
    public void RxTraceSourceSubscriptionsLifetimeMessages()
    {
      var source = CreateTraceSource();

      var xs = Observable.Range(0, 5)
        .TraceSubscriptions(source, "First", "Second", "Third", "Fourth");

      const int iterations = 3;

      xs.Repeat(iterations).Subscribe();

      AssertEqual(Listener.Messages, new[]
        {
          "First", 
          "Second", 
          "Third", 
          "Fourth"
        }
        .Repeat(iterations));
    }
  }
}