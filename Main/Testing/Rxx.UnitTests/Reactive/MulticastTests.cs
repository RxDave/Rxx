using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Reactive
{
  [TestClass]
  public class MulticastTests : RxxTests
  {
    [TestMethod, TestCategory("Rx")]
    public void RxNotificationsArePushedIntoSubject()
    {
      var source = new Subject<int>();
      var connectable = source.Multicast(() => new ReplaySubject<int>(1));
      connectable.Connect();
      source.OnNext(42);

      int actual = 0;
      connectable.Subscribe(x => actual = x);

      Assert.AreEqual(42, actual);
    }

    [TestMethod, TestCategory("Rx")]
    public void RxEarlySubscribesReceiveNotifications()
    {
      var source = new Subject<int>();
      var connectable = source.Multicast(() => new ReplaySubject<int>(1));
      int actual = 0;
      connectable.Subscribe(x => actual = x);

      connectable.Connect();
      source.OnNext(42);

      Assert.AreEqual(42, actual);
    }

    [TestMethod, TestCategory("Rx")]
    public void RxSubjectStateIsCleared()
    {
      var source = new Subject<int>();
      var connectable = source.Multicast(() => new ReplaySubject<int>(1));
      using (connectable.Connect())
      {
        source.OnNext(42);
      }

      int actual = 0;
      connectable.Subscribe(x => actual = x);

      Assert.AreEqual(0, actual);
    }
  }
}