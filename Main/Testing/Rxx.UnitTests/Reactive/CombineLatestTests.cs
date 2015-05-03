using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Reactive
{
  [TestClass]
  public sealed class CombineLatestTests : RxxTests
  {
    /// <summary>
    /// Origin of unit test: 
    /// https://social.msdn.microsoft.com/Forums/en-US/bbfc31d0-d337-41d9-946c-0134f9474caa/how-can-i-merge-an-ioiot-to-produce-an-ioienumt-containing-only-the-most-recent-values-of
    /// </summary>
    [TestMethod, TestCategory("Rx")]
    public void CombineLatest()
    {
      var scheduler = new TestScheduler();

      var first = scheduler.CreateHotObservable(
        OnNext(10, 'a'),
        OnNext(20, 'b'),
        OnNext(40, 'c'),
        OnCompleted<char>(70));

      var second = scheduler.CreateHotObservable(
        OnNext(30, 'd'),
        OnNext(60, 'e'),
        OnNext(100, 'f'),
        OnCompleted<char>(110));

      var third = scheduler.CreateHotObservable(
        OnNext(50, 'g'),
        OnNext(80, 'h'),
        OnCompleted<char>(90));

      var input = scheduler.CreateColdObservable(
        OnNext(0, first),
        OnNext(25, second),
        OnNext(45, third),
        OnCompleted<ITestableObservable<char>>(55));

      var result = scheduler.Start(() => input.CombineLatest(), 0, 0, 200).Messages;

      result.AssertEqual(
        OnNext<IList<char>>(10, new[] { 'a' }.SequenceEqual),
        OnNext<IList<char>>(20, new[] { 'b' }.SequenceEqual),
        OnNext<IList<char>>(30, new[] { 'b', 'd' }.SequenceEqual),
        OnNext<IList<char>>(40, new[] { 'c', 'd' }.SequenceEqual),
        OnNext<IList<char>>(50, new[] { 'c', 'd', 'g' }.SequenceEqual),
        OnNext<IList<char>>(60, new[] { 'c', 'e', 'g' }.SequenceEqual),
        OnNext<IList<char>>(80, new[] { 'c', 'e', 'h' }.SequenceEqual),
        OnNext<IList<char>>(100, new[] { 'c', 'f', 'h' }.SequenceEqual),
        OnCompleted<IList<char>>(110));
    }
  }
}