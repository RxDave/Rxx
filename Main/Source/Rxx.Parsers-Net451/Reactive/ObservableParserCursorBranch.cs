using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;

namespace Rxx.Parsers.Reactive
{
  internal sealed partial class ObservableParserCursor<T>
  {
    private sealed class ObservableParserCursorBranch : IObservableCursor<T>
    {
      #region Public Properties
      public bool IsSynchronized
      {
        get
        {
          Contract.Ensures(Contract.Result<bool>() == branch.IsSynchronized);

          return branch.IsSynchronized;
        }
      }

      public bool IsForwardOnly
      {
        get
        {
          Contract.Ensures(Contract.Result<bool>() == branch.IsForwardOnly);

          return branch.IsForwardOnly;
        }
      }

      public int CurrentIndex
      {
        get
        {
          Contract.Ensures(IsSynchronized || Contract.Result<int>() == branch.CurrentIndex);

          return branch.CurrentIndex;
        }
      }

      public int LatestIndex
      {
        get
        {
          Contract.Ensures(IsSynchronized || Contract.Result<int>() == branch.LatestIndex);

          return branch.LatestIndex;
        }
      }

      public bool AtEndOfSequence
      {
        get
        {
          Contract.Ensures(IsSynchronized || Contract.Result<bool>() == branch.AtEndOfSequence);

          var value = branch.AtEndOfSequence;

          Contract.Assume(IsSynchronized || !branch.IsDisposed);
          Contract.Assert(IsSynchronized || value == (IsSequenceTerminated && CurrentIndex == LatestIndex + 1));

          return value;
        }
      }

      public bool IsSequenceTerminated
      {
        get
        {
          Contract.Ensures(IsSynchronized || Contract.Result<bool>() == branch.IsSequenceTerminated);
          Contract.Ensures(!branch.IsSequenceTerminated || Contract.Result<bool>() == true);

          return branch.IsSequenceTerminated;
        }
      }

      public bool IsDisposed
      {
        get
        {
          Contract.Ensures(IsSynchronized || Contract.Result<bool>() == branch.IsDisposed);

          return branch.IsDisposed;
        }
      }
      #endregion

      #region Private / Protected
      private readonly List<IObservableCursor<T>> branches = new List<IObservableCursor<T>>();
      private readonly IObservableCursor<T> branch;
      private readonly Action dispose;
      #endregion

      #region Constructors
      public ObservableParserCursorBranch(IObservableCursor<T> branch, Action dispose)
      {
        Contract.Requires(branch != null);
        Contract.Requires(branch.IsForwardOnly);
        Contract.Requires(dispose != null);
        Contract.Ensures(IsSynchronized == branch.IsSynchronized);
        Contract.Ensures(IsForwardOnly == branch.IsForwardOnly);
        Contract.Ensures(IsSynchronized || CurrentIndex == branch.CurrentIndex);
        Contract.Ensures(IsSynchronized || LatestIndex == branch.LatestIndex);
        Contract.Ensures(IsSynchronized || AtEndOfSequence == branch.AtEndOfSequence);
        Contract.Ensures(IsSynchronized || IsSequenceTerminated == branch.IsSequenceTerminated);

        this.branch = branch;
        this.dispose = dispose;
      }
      #endregion

      #region Methods
      [ContractInvariantMethod]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
      private void ObjectInvariant()
      {
        Contract.Invariant(branch != null);
        Contract.Invariant(branch.IsForwardOnly);
        Contract.Invariant(branches != null);
        Contract.Invariant(dispose != null);
        Contract.Invariant(IsSynchronized == branch.IsSynchronized);
        Contract.Invariant(IsForwardOnly == branch.IsForwardOnly);

        /* Subscribing can cause values to be replayed, which can cause side-effects that dispose of the branch before Subscribe returns
         * to the caller.  The following invariants might not hold at the end of the Subscribe method unless they are defined as implications 
         * based on the disposed state of the branch.  See the comments in the ObservableCursorBranch class for more information.
         */
        Contract.Invariant(IsSynchronized || branch.IsDisposed || CurrentIndex == branch.CurrentIndex);
        Contract.Invariant(IsSynchronized || branch.IsDisposed || LatestIndex == branch.LatestIndex);
        Contract.Invariant(IsSynchronized || branch.IsDisposed || AtEndOfSequence == branch.AtEndOfSequence);
        Contract.Invariant(IsSynchronized || branch.IsDisposed || IsSequenceTerminated == branch.IsSequenceTerminated);
      }

      public IDisposable Subscribe(IObserver<T> observer)
      {
        var subscription = branch.Subscribe(observer);

        Contract.Assume(branch.IsForwardOnly);

        return subscription;
      }

      public IDisposable Subscribe(IObserver<T> observer, int count)
      {
        return branch.Subscribe(observer, count);
      }

      public IDisposable Connect()
      {
        var connection = branch.Connect();

        Contract.Assume(branch.IsForwardOnly);

        return connection;
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
        Justification = "Inherited code contracts and invariants.")]
      [ContractVerification(false)]
      public void Move(int count)
      {
        Contract.Assert(IsSynchronized || !AtEndOfSequence || count <= 0);

        branch.Move(count);
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
        Justification = "The disposable is returned to the caller.")]
      [ContractVerification(false)]
      public IObservableCursor<T> Branch()
      {
        var next = branch.Branch();

        branches.Add(next);

        return new ObservableParserCursorBranch(next, () => branches.Remove(next));
      }

      public override string ToString()
      {
        var child = branches.Count > 0 ? branches[branches.Count - 1] : null;

        return (child ?? branch).ToString();
      }

      public void Dispose()
      {
        dispose();

        branches.Clear();

        branch.Dispose();
      }
      #endregion
    }
  }
}