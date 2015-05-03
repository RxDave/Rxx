using System.Diagnostics.Contracts;
using System.Reactive.Disposables;

namespace System.Reactive
{
  internal partial class ObservableCursor<T>
  {
    private sealed class ObservableCursorBranch : IObservableCursor<T>
    {
      public bool IsSynchronized
      {
        get
        {
          Contract.Ensures(!Contract.Result<bool>());

          return false;
        }
      }

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
          Contract.Ensures(!disposed);

          EnsureNotDisposed();

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
          Contract.Ensures(!disposed);

          EnsureNotDisposed();

          return cursor.latestIndex;
        }
      }

      public bool AtEndOfSequence
      {
        get
        {
          Contract.Ensures(!disposed);

          EnsureNotDisposed();

          return IsSequenceTerminated && CurrentIndex == LatestIndex + 1;
        }
      }

      public bool IsSequenceTerminated
      {
        get
        {
          Contract.Ensures(Contract.Result<bool>() == cursor.stopped);
          Contract.Ensures(!disposed);

          EnsureNotDisposed();

          return cursor.stopped;
        }
      }

      public bool IsDisposed
      {
        get
        {
          Contract.Ensures(Contract.Result<bool>() == disposed);

          return disposed;
        }
      }

      private readonly CompositeDisposable disposables = new CompositeDisposable();
      private readonly ObservableCursor<T> cursor;
      private bool disposed;
      private int currentIndex;

      public ObservableCursorBranch(ObservableCursor<T> cursor, int currentIndex, CompositeDisposable parentDisposables)
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
        Contract.Ensures(!IsSynchronized);
        Contract.Ensures(IsForwardOnly == cursor.IsForwardOnly);
        Contract.Ensures(IsSequenceTerminated == cursor.IsSequenceTerminated);
        Contract.Ensures(LatestIndex == cursor.latestIndex);
        Contract.Ensures(CurrentIndex == currentIndex);
        Contract.Ensures(AtEndOfSequence == (cursor.IsSequenceTerminated && currentIndex == cursor.latestIndex + 1));

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
        Contract.Invariant(!IsSynchronized);
        Contract.Invariant(disposables != null);
        Contract.Invariant(cursor != null);
        Contract.Invariant(cursor.branches != null);
        Contract.Invariant(cursor.elements != null);
        Contract.Invariant(cursor.latestIndex >= -1);
        Contract.Invariant(IsForwardOnly == cursor.IsForwardOnly);
        Contract.Invariant(disposed || LatestIndex == cursor.LatestIndex);
        Contract.Invariant(disposed || IsSequenceTerminated == cursor.IsSequenceTerminated);

        // currentIndex must start at the "end" (index = 0) of the empty sequence (latestIndex = -1) so that AtEndOfSequence returns true.
        Contract.Invariant(currentIndex >= 0);

        /* Subscribing can cause values to be replayed, which can cause side-effects that dispose of the branch before Subscribe returns
         * to the caller.  The following invariants might not hold at the end of the Subscribe method unless they are defined as implications 
         * based on the disposed state of the branch.
         * 
         * (The above occurred while testing the XML schema lab for WP7.)
         */
        Contract.Invariant(disposed || currentIndex >= cursor.firstElementIndex);
        Contract.Invariant(disposed || currentIndex > cursor.latestIndex || cursor.elements.Count >= cursor.latestIndex - currentIndex);
        Contract.Invariant(disposed || !cursor.stopped || currentIndex <= cursor.latestIndex + oneForTerminationNotification);
      }

      [ContractVerification(false)]		// Static checker times out, although contracts were proven by increasing the timeout setting temporarily.
      private void EnsureNotDisposed()
      {
        Contract.Ensures(!disposed);
        Contract.Ensures(currentIndex == Contract.OldValue(currentIndex));
        Contract.Ensures(cursor.latestIndex == Contract.OldValue(cursor.latestIndex));
        Contract.Ensures(cursor.elements.Count == Contract.OldValue(cursor.elements.Count));
        Contract.Ensures(cursor.stopped == Contract.OldValue(cursor.stopped));
        Contract.Ensures(cursor.firstElementIndex == Contract.OldValue(cursor.firstElementIndex));

        cursor.EnsureNotDisposed();

        if (disposed)
        {
          throw new ObjectDisposedException(GetType().FullName);
        }

        Contract.Assume(currentIndex > cursor.latestIndex || cursor.elements.Count >= cursor.latestIndex - currentIndex);
      }

      public IDisposable Subscribe(IObserver<T> observer)
      {
        return SubscribeInternal(observer, subscribeUnlimited);
      }

      public IDisposable Subscribe(IObserver<T> observer, int count)
      {
        return SubscribeInternal(observer, count);
      }

      private IDisposable SubscribeInternal(IObserver<T> observer, int count)
      {
        Contract.Requires(observer != null);
        Contract.Requires(count >= subscribeUnlimited);
        Contract.Ensures(Contract.Result<IDisposable>() != null);

        EnsureNotDisposed();

        var subscription = new SingleAssignmentDisposable();

        IDisposable wrapper = null;

        wrapper = Disposable.Create(() =>
        {
          subscription.Dispose();

          disposables.Remove(wrapper);
        });

        disposables.Add(wrapper);

        subscription.Disposable = cursor.Subscribe(observer, currentIndex, count);

        // User code is being called, so it's possible that the branch will be disposed synchronously.
        if (!disposed)
        {
          Contract.Assume(currentIndex > cursor.latestIndex || cursor.elements.Count >= cursor.latestIndex - currentIndex);
          Contract.Assume(!cursor.stopped || currentIndex <= cursor.latestIndex + 1);
          Contract.Assume(currentIndex >= cursor.firstElementIndex);
        }

        return wrapper;
      }

      public IDisposable Connect()
      {
        EnsureNotDisposed();

        var connection = cursor.Connect();

        // User code is being called, so it's possible that the branch will be disposed synchronously.
        if (!disposed)
        {
          Contract.Assume(currentIndex > cursor.latestIndex || cursor.elements.Count >= cursor.latestIndex - currentIndex);
          Contract.Assume(!cursor.stopped || currentIndex <= cursor.latestIndex + 1);
          Contract.Assume(currentIndex >= cursor.firstElementIndex);
        }

        return connection;
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
        Justification = "Inherited code contracts.")]
      [ContractVerification(false)]		// Static checker times out, although contracts were proven by increasing the timeout setting temporarily.
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
      [ContractVerification(false)]		// Static checker times out; increasing the timeout setting temporarily to 1200 seconds didn't help.
      public IObservableCursor<T> Branch()
      {
        EnsureNotDisposed();

        var branch = new ObservableCursorBranch(cursor, currentIndex, disposables);

        Contract.Assume(branch.AtEndOfSequence == AtEndOfSequence);

        return branch;
      }

      public override string ToString()
      {
        return cursor.ToString(currentIndex, "Branch");
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