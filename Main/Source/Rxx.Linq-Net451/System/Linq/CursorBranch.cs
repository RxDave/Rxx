using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;
using Rxx.Linq.Properties;

namespace System.Linq
{
  internal partial class Cursor<T>
  {
    private sealed class CursorBranch : ICursor<T>
    {
      public bool IsForwardOnly
      {
        get
        {
          Contract.Ensures(Contract.Result<bool>() == cursor.isForwardOnly);

          return cursor.isForwardOnly;
        }
      }

      public int CurrentIndex
      {
        get
        {
          Contract.Ensures(Contract.Result<int>() == currentIndex);

          return currentIndex;
        }
        internal set
        {
          Contract.Requires(value >= 0);
          Contract.Requires(value <= LatestIndex + 1);

          Contract.Assume(value >= cursor.firstElementIndex);
          Contract.Assume(value == cursor.latestIndex + 1 || cursor.elements.Count >= cursor.latestIndex - value);

          currentIndex = value;
        }
      }

      public int LatestIndex
      {
        get
        {
          Contract.Ensures(Contract.Result<int>() == cursor.latestIndex);

          return cursor.latestIndex;
        }
      }

      public bool AtEndOfSequence
      {
        get
        {
          Contract.Ensures(Contract.Result<bool>() == (cursor.stopped && currentIndex == cursor.latestIndex + 1));

          return cursor.stopped && currentIndex == cursor.latestIndex + 1;
        }
      }

      public bool IsSequenceTerminated
      {
        get
        {
          Contract.Ensures(Contract.Result<bool>() == cursor.stopped);

          return cursor.stopped;
        }
      }

      private readonly CompositeDisposable disposables = new CompositeDisposable();
      private readonly Cursor<T> cursor;
      private bool disposed;
      private int currentIndex;

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures",
        Justification = "Assertions were able to prove that cursor.AtEndOfSequence isn't mutated but static checker still warns that Ensures is unproven.")]
      public CursorBranch(Cursor<T> cursor, int currentIndex, CompositeDisposable parentDisposables)
      {
        Contract.Requires(cursor != null);
        Contract.Requires(cursor.branches != null);
        Contract.Requires(cursor.elements != null);
        Contract.Requires(cursor.latestIndex >= -1);
        Contract.Requires(currentIndex >= 0);
        Contract.Requires(currentIndex >= cursor.firstElementIndex);
        Contract.Requires(currentIndex > cursor.latestIndex || cursor.elements.Count >= cursor.latestIndex - currentIndex);
        Contract.Requires(!cursor.stopped || currentIndex <= cursor.latestIndex + 1);
        Contract.Requires(parentDisposables != null);
        Contract.Ensures(this.cursor == cursor);
        Contract.Ensures(this.currentIndex == currentIndex);
        Contract.Ensures(IsForwardOnly == cursor.IsForwardOnly);
        Contract.Ensures(IsSequenceTerminated == cursor.IsSequenceTerminated);
        Contract.Ensures(LatestIndex == cursor.latestIndex);
        Contract.Ensures(CurrentIndex == currentIndex);
        Contract.Ensures(AtEndOfSequence == (cursor.IsSequenceTerminated && currentIndex == cursor.latestIndex + 1));
        Contract.Ensures(cursor.AtEndOfSequence == Contract.OldValue(cursor.AtEndOfSequence));

        this.cursor = cursor;
        this.currentIndex = currentIndex;

        parentDisposables.Add(this);
        cursor.branches.Add(this);

        bool removed = false;

        var subscription = Disposable.Create(() =>
        {
          if (!removed)
          {
            // Set this variable first in case of reentry.
            removed = true;

            parentDisposables.Remove(this);
            cursor.branches.Remove(this);
          }
        });

        disposables.Add(subscription);

        Contract.Assert(currentIndex > cursor.latestIndex || cursor.elements.Count >= cursor.latestIndex - currentIndex);
      }

      [ContractInvariantMethod]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
      private void ObjectInvariant()
      {
        Contract.Invariant(disposables != null);
        Contract.Invariant(cursor != null);
        Contract.Invariant(cursor.branches != null);
        Contract.Invariant(cursor.elements != null);
        Contract.Invariant(cursor.latestIndex >= -1);
        Contract.Invariant(IsForwardOnly == cursor.IsForwardOnly);
        Contract.Invariant(LatestIndex == cursor.LatestIndex);
        Contract.Invariant(IsSequenceTerminated == cursor.IsSequenceTerminated);

        // currentIndex must start at the "end" (index = 0) of the empty sequence (latestIndex = -1) so that AtEndOfSequence returns true.
        Contract.Invariant(currentIndex >= 0);

        Contract.Invariant(currentIndex >= cursor.firstElementIndex);
        Contract.Invariant(currentIndex > cursor.latestIndex || cursor.elements.Count >= cursor.latestIndex - currentIndex);
        Contract.Invariant(!cursor.stopped || currentIndex <= cursor.latestIndex + oneForTerminationNotification);
      }

      private void EnsureNotDisposed()
      {
        Contract.Ensures(!disposed);
        Contract.Ensures(currentIndex == Contract.OldValue(currentIndex));
        Contract.Ensures(cursor.latestIndex == Contract.OldValue(cursor.latestIndex));
        Contract.Ensures(cursor.elements.Count == Contract.OldValue(cursor.elements.Count));
        Contract.Ensures(cursor.stopped == Contract.OldValue(cursor.stopped));
        Contract.Ensures(cursor.firstElementIndex == Contract.OldValue(cursor.firstElementIndex));
        Contract.Ensures(AtEndOfSequence == Contract.OldValue(AtEndOfSequence));

#if DEBUG
        bool oldEnd = AtEndOfSequence;
#endif

        cursor.EnsureNotDisposed();

        if (disposed)
        {
          throw new ObjectDisposedException(GetType().FullName);
        }

        Contract.Assert(currentIndex > cursor.latestIndex || cursor.elements.Count >= cursor.latestIndex - currentIndex);

#if DEBUG
        Contract.Assume(AtEndOfSequence == oldEnd);
#endif
      }

      public IEnumerator<T> GetEnumerator()
      {
        EnsureNotDisposed();

        Contract.Assert(currentIndex >= cursor.firstElementIndex);
        Contract.Assert(!cursor.stopped || currentIndex <= cursor.latestIndex + oneForTerminationNotification);

        return cursor.GetEnumerator(currentIndex);
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        EnsureNotDisposed();

        Contract.Assert(currentIndex >= cursor.firstElementIndex);
        Contract.Assert(!cursor.stopped || currentIndex <= cursor.latestIndex + oneForTerminationNotification);

        return cursor.GetEnumerator(currentIndex);
      }

      public void Move(int count)
      {
        EnsureNotDisposed();

        currentIndex += count;

        if (cursor.isForwardOnly
          && count > 0
          && cursor.truncateWhileBranched)
        {
          cursor.RemovePassedElements();
        }

        Contract.Assume(currentIndex >= cursor.firstElementIndex);
        Contract.Assume(currentIndex > cursor.latestIndex || cursor.elements.Count >= cursor.latestIndex - currentIndex);
        Contract.Assume(!cursor.stopped || currentIndex <= cursor.latestIndex + 1);
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
        Justification = "This is a factory method.")]
      public ICursor<T> Branch()
      {
        EnsureNotDisposed();

        var branch = new CursorBranch(cursor, currentIndex, disposables);

        Contract.Assume(branch.AtEndOfSequence == AtEndOfSequence);

        return branch;
      }

      public override string ToString()
      {
        return cursor.ToString(currentIndex, "Branch");
      }

      public void Reset()
      {
        throw new NotSupportedException(Errors.CannotResetCursorBranch);
      }

      public void Dispose()
      {
        Contract.Ensures(disposed);

        if (!disposed)
        {
          // Disposing disposables causes reentry, so we must set disposed to true first.
          disposed = true;

          // Must dispose of the disposables before calling RemovePassedElements to ensure that this branch is removed from the cursor.
          disposables.Dispose();

          if (cursor.isForwardOnly
            && (cursor.truncateWhileBranched || cursor.branches.Count == 0))
          {
            cursor.RemovePassedElements();
          }
        }
      }
    }
  }
}