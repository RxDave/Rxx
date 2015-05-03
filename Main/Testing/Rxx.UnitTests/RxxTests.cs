using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rxx.Parsers;

namespace Rxx.UnitTests
{
  [TestClass]
  public abstract class RxxTests : ReactiveTest
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    #endregion

    #region Constructors
    protected RxxTests()
    {
    }
    #endregion

    #region Methods
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext context)
    {
      Contract.ContractFailed += (sender, e) =>
      {
        e.SetUnwind();

        Assert.Fail(e.FailureKind.ToString() + ":" + e.Message);
      };
    }

    [TestInitialize]
    public virtual void Initialize()
    {
      // See the Output window when the debugger is attached
      ParserTraceSources.Compilation.Switch.Level = SourceLevels.All;
      ParserTraceSources.Execution.Switch.Level = SourceLevels.All;
      ParserTraceSources.Input.Switch.Level = SourceLevels.All;
    }

    protected IEnumerable<object> Concat<T>(IEnumerable<T> first, params object[] second)
    {
      return first.Cast<object>().Concat(second);
    }

    protected IEnumerable<string> Concat<T>(IEnumerable<string> first, params object[] second)
    {
      return first.Concat(second.Select(value => value.ToString()));
    }

    protected void AssertEqual<T>(IEnumerable<T> actual, IEnumerable<T> expected)
    {
      ReactiveAssert.AreElementsEqual(expected, actual);
    }

    protected void AssertEqual<T>(IEnumerable<T> actual, params T[] expected)
    {
      ReactiveAssert.AreElementsEqual(expected, actual);
    }

    protected void AssertEqual<T>(IEnumerable<T> actual, params IEnumerable<T>[] expected)
    {
      ReactiveAssert.AreElementsEqual(EnumerableEx.Concat(expected), actual);
    }

    protected void AssertEqual<T>(IObservable<T> actual, IEnumerable<T> expected)
    {
      /* Workaround: The actual and expected parameter names are reversed in the 
       * AreElementsEqual overloads that accept observables.
       * (Rx 1.1.10425)
       */
      ReactiveAssert.AreElementsEqual(actual, expected.ToObservable(Scheduler.Immediate));
    }

    protected void AssertEqual<T>(IObservable<T> actual, params T[] expected)
    {
      /* Workaround: The actual and expected parameter names are reversed in the 
       * AreElementsEqual overloads that accept observables.
       * (Rx 1.1.10425)
       */
      ReactiveAssert.AreElementsEqual(actual, expected.ToObservable(Scheduler.Immediate));
    }

    protected void AssertEqual<T>(IObservable<T> actual, params IObservable<T>[] expected)
    {
      /* Workaround: The actual and expected parameter names are reversed in the 
       * AreElementsEqual overloads that accept observables.
       * (Rx 1.1.10425)
       */
      ReactiveAssert.AreElementsEqual(actual, expected.Concat());
    }

    protected void AssertEqual<T>(IObservable<T> actual, params IEnumerable<T>[] expected)
    {
      /* Workaround: The actual and expected parameter names are reversed in the 
       * AreElementsEqual overloads that accept observables.
       * (Rx 1.1.10425)
       */
      ReactiveAssert.AreElementsEqual(actual, EnumerableEx.Concat(expected).ToObservable(Scheduler.Immediate));
    }
    #endregion
  }
}