using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Reactive
{
  [TestClass]
  public class IntrospectiveTests : RxxTests
  {
    /// <summary>
    /// Origin of unit test: 
    /// https://social.msdn.microsoft.com/Forums/en-US/89f3d9c7-654f-41da-96e5-9bf9f5f97f2e/rxx-bufferintrospective-does-not-immediately-subscribe-to-underlying-observable?forum=rx
    /// </summary>
    [TestMethod, TestCategory("Rx")]
    public void BufferIntrospective()
    {
      var scheduler = new TestScheduler();
      var buffers = new List<IList<int>>();

      var source = scheduler.CreateHotObservable(
        OnNext(0, 42),
        OnCompleted(1, 0));

      using (source.BufferIntrospective(scheduler).Subscribe(buffers.Add))
      {
        scheduler.Start();

        Assert.AreEqual(1, buffers.Count);
        Assert.AreEqual(1, buffers[0].Count);
      }
    }

    /// <summary>
    /// Origin of unit test: 
    /// http://social.msdn.microsoft.com/Forums/en-US/rx/thread/87153e99-3a8b-4515-b743-33983ccb9138/
    /// </summary>
    [TestMethod, TestCategory("Rx")]
    public void SampleIntrospective()
    {
      var scheduler = new TestScheduler();

      var source = scheduler.CreateHotObservable(
        OnNext(110, "A"),
        OnNext(120, "B"),
        OnNext(130, "C"),
        OnNext(140, "D"),
        // ... delay ...
        OnNext(180, "E"),
        OnNext(190, "F"),
        OnNext(200, "G"), OnNext(201, "H"), OnNext(202, "I"), // burst
        // ... delay ...
        OnNext(250, "J"),
        // ... delay ...
        OnNext(300, "K"), OnNext(302, "L"), OnNext(304, "M"), // burst
        // complete
        OnCompleted<string>(304),
        OnNext(305, "N")
      );

      var sampledSource = source.SampleIntrospective(scheduler)
                                .Do(_ => scheduler.Sleep(15));

      AssertEqual(
        scheduler.Start(() => sampledSource, created: 0, subscribed: 0, disposed: 10000).Messages,
        OnNext(126, "A"),
        OnNext(142, "B"),
        // skipping "C"
        OnNext(158, "D"),
        OnNext(196, "E"),
        OnNext(212, "F"),
        // skipping "G"
        // skipping "H"
        OnNext(228, "I"),
        OnNext(266, "J"),
        OnNext(316, "K"),
        // skipping "L", "M"
        OnCompleted<string>(317));
    }
  }
}