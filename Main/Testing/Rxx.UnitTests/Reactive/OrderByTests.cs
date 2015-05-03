using System;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Reactive
{
  [TestClass]
  public class OrderByTests : RxxTests
  {
    private static IObservable<int> RangeOneToThree(TestScheduler scheduler)
    {
      return scheduler.CreateColdObservable(
        OnNext(100, 1),
        OnNext(100, 2),
        OnNext(100, 3),
        OnCompleted<int>(100));
    }

    private static IObservable<int> IntervalZeroToTwo(TestScheduler scheduler)
    {
      return scheduler.CreateColdObservable(
        OnNext(100, 0),
        OnNext(200, 1),
        OnNext(300, 2),
        OnCompleted<int>(300));
    }

    [TestMethod, TestCategory("OrderBy"), TestCategory("Rx")]
    public void KeyAscending()
    {
      var scheduler = new TestScheduler();

      var xs =
        from x in IntervalZeroToTwo(scheduler)
        orderby x
        select x;

      AssertEqual(
        scheduler.Start(() => xs, 0, 0, 1000).Messages,
        OnNext(301, 0),
        OnNext(301, 1),
        OnNext(301, 2),
        OnCompleted<int>(301));
    }

    [TestMethod, TestCategory("OrderBy"), TestCategory("Rx")]
    public void KeyDescending()
    {
      var scheduler = new TestScheduler();

      var xs =
        from x in IntervalZeroToTwo(scheduler)
        orderby x descending
        select x;

      AssertEqual(
        scheduler.Start(() => xs, 0, 0, 1000).Messages,
        OnNext(301, 2),
        OnNext(301, 1),
        OnNext(301, 0),
        OnCompleted<int>(301));
    }

    [TestMethod, TestCategory("OrderBy"), TestCategory("Rx")]
    public void TwoKeysAscending()
    {
      var scheduler = new TestScheduler();

      var xs =
        from x in IntervalZeroToTwo(scheduler)
        from y in "ABC"
        orderby x, y
        select x + y.ToString();

      AssertEqual(
        scheduler.Start(() => xs, 0, 0, 1000).Messages,
        OnNext(301, "0A"),
        OnNext(301, "0B"),
        OnNext(301, "0C"),
        OnNext(301, "1A"),
        OnNext(301, "1B"),
        OnNext(301, "1C"),
        OnNext(301, "2A"),
        OnNext(301, "2B"),
        OnNext(301, "2C"),
        OnCompleted<string>(301));
    }

    [TestMethod, TestCategory("OrderBy"), TestCategory("Rx")]
    public void TwoKeysAscendingThenDescending()
    {
      var scheduler = new TestScheduler();

      var xs =
        from x in IntervalZeroToTwo(scheduler)
        from y in "ABC"
        orderby x, y descending
        select x + y.ToString();

      AssertEqual(
        scheduler.Start(() => xs, 0, 0, 1000).Messages,
        OnNext(301, "0C"),
        OnNext(301, "0B"),
        OnNext(301, "0A"),
        OnNext(301, "1C"),
        OnNext(301, "1B"),
        OnNext(301, "1A"),
        OnNext(301, "2C"),
        OnNext(301, "2B"),
        OnNext(301, "2A"),
        OnCompleted<string>(301));
    }

    [TestMethod, TestCategory("OrderBy"), TestCategory("Rx")]
    public void TwoKeysDescending()
    {
      var scheduler = new TestScheduler();

      var xs =
        from x in IntervalZeroToTwo(scheduler)
        from y in "ABC"
        orderby x descending, y descending
        select x + y.ToString();

      AssertEqual(
        scheduler.Start(() => xs, 0, 0, 1000).Messages,
        OnNext(301, "2C"),
        OnNext(301, "2B"),
        OnNext(301, "2A"),
        OnNext(301, "1C"),
        OnNext(301, "1B"),
        OnNext(301, "1A"),
        OnNext(301, "0C"),
        OnNext(301, "0B"),
        OnNext(301, "0A"),
        OnCompleted<string>(301));
    }

    [TestMethod, TestCategory("OrderBy"), TestCategory("Rx")]
    public void TimeAscending()
    {
      var scheduler = new TestScheduler();

      var xs =
        from x in RangeOneToThree(scheduler)
        orderby Observable.Timer(TimeSpan.FromTicks(100 * (3 - x)), scheduler)
        select x;

      AssertEqual(
        scheduler.Start(() => xs, 0, 0, 1000).Messages,
        OnNext(102, 3),
        OnNext(201, 2),
        OnNext(301, 1),
        OnCompleted<int>(301));
    }

    [TestMethod, TestCategory("OrderBy"), TestCategory("Rx")]
    public void TimeDescending()
    {
      var scheduler = new TestScheduler();

      var xs =
        from x in RangeOneToThree(scheduler)
        orderby Observable.Timer(TimeSpan.FromTicks(100 * (3 - x)), scheduler) descending
        select x;

      AssertEqual(
        scheduler.Start(() => xs, 0, 0, 1000).Messages,
        OnNext(301, 1),
        OnNext(301, 2),
        OnNext(301, 3),
        OnCompleted<int>(301));
    }

    [TestMethod, TestCategory("OrderBy"), TestCategory("Rx")]
    public void TwoTimesAscending()
    {
      var scheduler = new TestScheduler();

      var xs =
        from x in RangeOneToThree(scheduler)
        orderby Observable.Timer(TimeSpan.FromTicks(100 * (3 - x)), scheduler),
                Observable.Timer(TimeSpan.FromTicks(100), scheduler)
        select x;

      AssertEqual(
        scheduler.Start(() => xs, 0, 0, 1000).Messages,
        OnNext(202, 3),
        OnNext(301, 2),
        OnNext(401, 1),
        OnCompleted<int>(401));
    }

    [TestMethod, TestCategory("OrderBy"), TestCategory("Rx")]
    public void TwoTimesAscendingThenDescending()
    {
      var scheduler = new TestScheduler();

      var xs =
        from x in RangeOneToThree(scheduler)
        orderby Observable.Timer(TimeSpan.FromTicks(100 * (3 - x)), scheduler),
                Observable.Timer(TimeSpan.FromTicks(100), scheduler) descending
        select x;

      AssertEqual(
        scheduler.Start(() => xs, 0, 0, 1000).Messages,
        OnNext(401, 1),
        OnNext(401, 2),
        OnNext(401, 3),
        OnCompleted<int>(401));
    }

    [TestMethod, TestCategory("OrderBy"), TestCategory("Rx")]
    public void TwoTimesDescending()
    {
      var scheduler = new TestScheduler();

      var xs =
        from x in RangeOneToThree(scheduler)
        orderby Observable.Timer(TimeSpan.FromTicks(100 * (3 - x)), scheduler) descending,
                Observable.Timer(TimeSpan.FromTicks(100), scheduler) descending
        select x;

      AssertEqual(
        scheduler.Start(() => xs, 0, 0, 1000).Messages,
        OnNext(401, 3),
        OnNext(401, 2),
        OnNext(401, 1),
        OnCompleted<int>(401));
    }

    [TestMethod, TestCategory("OrderBy"), TestCategory("Rx")]
    public void KeyDescendingThenTimeAscending()
    {
      var scheduler = new TestScheduler();

      var xs =
        from x in IntervalZeroToTwo(scheduler)
        orderby x descending,
                Observable.Timer(TimeSpan.FromTicks(100 * x), scheduler)
        select x;

      AssertEqual(
        scheduler.Start(() => xs, 0, 0, 1000).Messages,
        OnNext(302, 0),
        OnNext(401, 1),
        OnNext(501, 2),
        OnCompleted<int>(501));
    }

    [TestMethod, TestCategory("OrderBy"), TestCategory("Rx")]
    public void TimeAscendingThenKeyDescending()
    {
      var scheduler = new TestScheduler();

      var xs =
        from x in RangeOneToThree(scheduler)
        orderby Observable.Timer(TimeSpan.FromTicks(100 * (3 - x)), scheduler),
                x descending
        select x;

      AssertEqual(
        scheduler.Start(() => xs, 0, 0, 1000).Messages,
        OnNext(301, 3),
        OnNext(301, 2),
        OnNext(301, 1),
        OnCompleted<int>(301));
    }
  }
}