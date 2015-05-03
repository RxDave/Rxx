using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Disposables;

namespace System.Reactive
{
  internal sealed partial class ObservableCursor<T> : IObservableCursor<T>
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
        Contract.Ensures(Contract.Result<bool>() == isForwardOnly);

        return isForwardOnly;
      }
    }

    public int CurrentIndex
    {
      get
      {
        Contract.Ensures(Contract.Result<int>() == currentIndex);

        return currentIndex;
      }
    }

    public int LatestIndex
    {
      get
      {
        Contract.Ensures(Contract.Result<int>() == latestIndex);

        return latestIndex;
      }
    }

    public bool AtEndOfSequence
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>() == (stopped && currentIndex == latestIndex + oneForTerminationNotification));

        return stopped && currentIndex == latestIndex + oneForTerminationNotification;
      }
    }

    public bool IsSequenceTerminated
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>() == stopped);

        return stopped;
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

    private const int oneForTerminationNotification = 1;
    private const int subscribeUnlimited = -1;

    private readonly List<ObservableCursorSubscription> subscriptions = new List<ObservableCursorSubscription>();
    private readonly List<Notification<T>> elements = new List<Notification<T>>();
    private readonly List<ObservableCursorBranch> branches = new List<ObservableCursorBranch>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly IObservable<T> source;
    private readonly bool isForwardOnly, truncateWhileBranched;
    private IDisposable sourceSubscription;
    private bool connected, disposed;
    private bool stopped;
    private int currentIndex;
    private int latestIndex = -1;

    /// <summary>
    /// Stores the index offset, relative to the source sequence, of the first value in the <see cref="elements"/> list.
    /// </summary>
    /// <remarks>
    /// This field is used as part of a memory optimization in a forward-only cursor that allows elements to be removed
    /// when the cursor and all dependent branches have moved passed them, consequently offsetting the indexes in the 
    /// <see cref="elements"/> list with respect to the actual source sequence.
    /// </remarks>
    private int firstElementIndex;

    public ObservableCursor(IObservable<T> source, bool isForwardOnly, bool enableBranchOptimizations)
    {
      Contract.Requires(source != null);
      Contract.Requires(isForwardOnly || !enableBranchOptimizations);
      Contract.Ensures(IsForwardOnly == isForwardOnly);

      this.source = source;
      this.isForwardOnly = isForwardOnly;
      this.truncateWhileBranched = enableBranchOptimizations;
    }

    public ObservableCursor(bool isForwardOnly, bool enableBranchOptimizations)
    {
      Contract.Requires(isForwardOnly || !enableBranchOptimizations);
      Contract.Ensures(IsForwardOnly == isForwardOnly);

      this.isForwardOnly = isForwardOnly;
      this.truncateWhileBranched = enableBranchOptimizations;
    }

    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(disposables != null);
      Contract.Invariant(!IsSynchronized);
      Contract.Invariant(subscriptions != null);
      Contract.Invariant(elements != null);
      Contract.Invariant(branches != null);
      Contract.Invariant(!connected || sourceSubscription != null);
      Contract.Invariant(isForwardOnly || !truncateWhileBranched);

      // currentIndex must start at the "end" (index = 0) of the empty sequence (latestIndex = -1) so that AtEndOfSequence returns true.
      Contract.Invariant(currentIndex >= 0);

      Contract.Invariant(currentIndex >= firstElementIndex);
      Contract.Invariant(latestIndex >= -1);
      Contract.Invariant(firstElementIndex >= 0);
      Contract.Invariant(isForwardOnly || firstElementIndex == 0);
      Contract.Invariant(latestIndex == -1 || firstElementIndex > 0 || elements.Count > 0);
      Contract.Invariant(currentIndex > latestIndex || elements.Count >= latestIndex - currentIndex);
      Contract.Invariant(stopped || latestIndex < firstElementIndex + elements.Count);
      Contract.Invariant(!stopped || latestIndex < firstElementIndex + (elements.Count - oneForTerminationNotification));
      Contract.Invariant(!stopped || firstElementIndex <= latestIndex + oneForTerminationNotification);
      Contract.Invariant(!stopped || currentIndex <= latestIndex + oneForTerminationNotification);
      Contract.Invariant(!stopped || elements.Count > 0, "Completion must enqueue an OnError or OnCompleted notification in addition to any memoized OnNext notifications.");
    }

    [ContractVerification(false)]		// Static checker times out, although contracts were proven by increasing the timeout setting temporarily.
    private void EnsureNotDisposed()
    {
      Contract.Ensures(!disposed);
      Contract.Ensures(CurrentIndex == Contract.OldValue(CurrentIndex));
      Contract.Ensures(LatestIndex == Contract.OldValue(LatestIndex));
      Contract.Ensures(IsSequenceTerminated == Contract.OldValue(IsSequenceTerminated));
      Contract.Ensures(AtEndOfSequence == Contract.OldValue(AtEndOfSequence));
      Contract.Ensures(firstElementIndex == Contract.OldValue(firstElementIndex));
      Contract.Ensures(elements.Count == Contract.OldValue(elements.Count));

      if (disposed)
      {
        throw new ObjectDisposedException(GetType().FullName);
      }
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
      EnsureNotDisposed();

      return Subscribe(observer, currentIndex, subscribeUnlimited);
    }

    public IDisposable Subscribe(IObserver<T> observer, int count)
    {
      EnsureNotDisposed();

      return Subscribe(observer, currentIndex, count);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Invariant",
      Justification = "Invariants cannot be proven through contracts because user code is invoked.")]
    [ContractVerification(false)]		// Static checker times out, although contracts were proven by increasing the timeout setting temporarily.
    private IDisposable Subscribe(IObserver<T> observer, int index, int count)
    {
      Contract.Requires(observer != null);
      Contract.Requires(index >= -1);
      Contract.Requires(index == -1 || index >= firstElementIndex);
      Contract.Requires(index > latestIndex || elements.Count >= latestIndex - index);
      Contract.Requires(!stopped || index <= latestIndex + oneForTerminationNotification);
      Contract.Requires(count >= subscribeUnlimited);
      Contract.Ensures(Contract.Result<IDisposable>() != null);		// Ensurances about field mutability cannot be made because user code is invoked.
      Contract.Ensures(latestIndex >= -1);	// Helps the static checker when analyzing the nested branch class, since invariants are ignored.

      IDisposable disposable;

      int existingCount = Math.Max(0, (latestIndex - index) + 1);

      bool covered = count != subscribeUnlimited && count <= existingCount;

      if (count != subscribeUnlimited)
      {
        if (count < existingCount)
        {
          existingCount = count;
        }

        Contract.Assert(covered ? existingCount == count : existingCount < count);
      }

      int remainder = count == subscribeUnlimited ? subscribeUnlimited : count - existingCount;

      Contract.Assert(existingCount >= 0);
      Contract.Assert(remainder >= subscribeUnlimited);
      Contract.Assert(covered || count == subscribeUnlimited || existingCount < count);
      Contract.Assume(covered == (existingCount == count));
      Contract.Assume(covered == (remainder == 0));

      int startIndex = index == -1 ? 0 : index;

      if (stopped && !covered)
      {
        existingCount++;

        disposable = Disposable.Empty;
      }
      else if (remainder != 0)
      {
        disposable = new ObservableCursorSubscription(this, startIndex, remainder, observer);
      }
      else
      {
        disposable = Disposable.Empty;
      }

      if (existingCount > 0)
      {
        var currentElements = new Notification<T>[existingCount];

        // Accepting a notification can modify the collection so we must iterate a copy.
        CopyExistingElementsTo(currentElements, existingCount, startIndex);

        /* All elements must be pushed before Subscribe returns to the caller to ensure their order.
         * CurrentThreadScheduler cannot be used.  In testing, newer notifications were sometimes 
         * already enqueued before this method was called, so enqueuing the following replay 
         * notifications causes them to be observed after the newer notifications.
         */
        foreach (var element in currentElements)
        {
          Contract.Assume(element != null);

          element.Accept(observer);
        }
      }

      if (covered)
      {
        observer.OnCompleted();
      }

      return disposable;
    }

    // Contract bug: http://social.msdn.microsoft.com/Forums/en-AU/codecontracts/thread/c933a14f-2853-450d-a8e6-5f8f062477f4
    [ContractVerification(false)]
    private void CopyExistingElementsTo(Notification<T>[] currentElements, int existingCount, int startIndex)
    {
      Contract.Ensures(currentIndex == Contract.OldValue(currentIndex));
      Contract.Ensures(latestIndex == Contract.OldValue(latestIndex));
      Contract.Ensures(firstElementIndex == Contract.OldValue(firstElementIndex));
      Contract.Ensures(elements.Count == Contract.OldValue(elements.Count));
      Contract.Ensures(stopped == Contract.OldValue(stopped));

      elements.CopyTo(startIndex - firstElementIndex, currentElements, 0, existingCount);
    }

    [ContractVerification(false)]		// Static checker is timing out
    public IDisposable Connect()
    {
      // Ensurances about field mutability cannot be made because user code is invoked.
      Contract.Ensures(latestIndex >= -1);	// Helps the static checker when analyzing the nested branch class, since invariants are ignored.

      EnsureNotDisposed();

      if (source == null)
      {
        throw new InvalidOperationException();
      }

      if (!connected)
      {
        // Set the variable first in case of reentry.
        connected = true;

        var subscription = source.SubscribeSafe(OnNext, OnError, OnCompleted);

        sourceSubscription = Disposable.Create(() =>
          {
            subscription.Dispose();

            disposables.Remove(sourceSubscription);

            connected = false;

            Reset();
          });

        disposables.Add(sourceSubscription);
      }

      return sourceSubscription;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Invariant",
      Justification = "Invariants cannot be proven through contracts because user code is invoked.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "TestAlwaysEvaluatingToAConstant",
      Justification = "False positive on 'completed' local.")]
    internal void OnNext(T value)
    {
      if (!stopped)
      {
        latestIndex++;

        Contract.Assert(isForwardOnly || firstElementIndex == 0);

        if (firstElementIndex <= latestIndex)
        {
          elements.Add(Notification.CreateOnNext(value));
        }

        // Calling OnNext can modify the collection so we must iterate a copy.
        var currentSubscriptions = subscriptions.ToArray();

        foreach (var subscription in currentSubscriptions.Where(subscription => subscription.Index <= latestIndex))
        {
          Contract.Assume(subscription != null);

          var observer = subscription.Observer;

          bool completed = false;

          if (subscription.Take != subscribeUnlimited)
          {
            Contract.Assume(subscription.Take > 0);

            subscription.Take--;

            completed = subscription.Take == 0;

            if (completed)
            {
              // Dispose of the subscription before pushing a value in case OnNext causes reentry.
              subscription.Dispose();
            }
          }

          observer.OnNext(value);

          if (completed)
          {
            observer.OnCompleted();
          }
        }
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Invariant",
      Justification = "Invariants cannot be proven through contracts because user code is invoked.")]
    internal void OnError(Exception error)
    {
      Contract.Requires(error != null);

      if (!stopped)
      {
        elements.Add(Notification.CreateOnError<T>(error));

        Terminated();

        // Calling OnError can modify the collection so we must iterate a copy.
        var currentSubscriptions = subscriptions.ToArray();

        foreach (var subscription in currentSubscriptions)
        {
          Contract.Assume(subscription != null);

          var observer = subscription.Observer;

          subscription.Dispose();

          observer.OnError(error);
        }
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Invariant",
      Justification = "Invariants cannot be proven through contracts because user code is invoked.")]
    internal void OnCompleted()
    {
      if (!stopped)
      {
        elements.Add(Notification.CreateOnCompleted<T>());

        Terminated();

        // Calling OnCompleted can modify the collection so we must iterate a copy.
        var currentSubscriptions = subscriptions.ToArray();

        foreach (var subscription in currentSubscriptions)
        {
          Contract.Assume(subscription != null);

          var observer = subscription.Observer;

          subscription.Dispose();

          observer.OnCompleted();
        }
      }
    }

    [ContractVerification(false)]		// The static checker claims that "Requires (including invariants) are unsatisfiable"
    private void Terminated()
    {
      Contract.Requires(!stopped);
      Contract.Ensures(stopped);
      Contract.Ensures(latestIndex == Contract.OldValue(latestIndex));
      Contract.Ensures(currentIndex <= latestIndex + oneForTerminationNotification);
      Contract.Ensures(firstElementIndex <= latestIndex + oneForTerminationNotification);
      Contract.Ensures(elements.Count == Contract.OldValue(elements.Count));

      var stopIndex = latestIndex + oneForTerminationNotification;

      if (currentIndex > stopIndex)
      {
        currentIndex = stopIndex;
      }

      if (firstElementIndex > stopIndex)
      {
        firstElementIndex = stopIndex;
      }

      foreach (var branch in branches)
      {
        if (branch.CurrentIndex > stopIndex)
        {
          branch.CurrentIndex = stopIndex;
        }
      }

      stopped = true;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
      Justification = "Code contracts.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Invariant",
      Justification = "The static checker cannot prove invariants that are provable by the specified contracts.")]
    [ContractVerification(false)]		// Static checker times out, although contracts were proven by increasing the timeout setting temporarily.
    public void Move(int count)
    {
      Contract.Ensures(elements.Count <= Contract.OldValue(elements.Count));

      EnsureNotDisposed();

      currentIndex += count;

      if (isForwardOnly
        && count > 0
        && (truncateWhileBranched || branches.Count == 0))
      {
        RemovePassedElements();
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Invariant",
      Justification = "The static checker cannot prove invariants that are provable by the specified contracts.")]
    [ContractVerification(false)]		// Static checker times out, although contracts were proven by increasing the timeout setting temporarily.
    private void RemovePassedElements()
    {
      Contract.Requires(isForwardOnly);
      Contract.Ensures(CurrentIndex == Contract.OldValue(CurrentIndex));
      Contract.Ensures(LatestIndex == Contract.OldValue(LatestIndex));
      Contract.Ensures(IsSequenceTerminated == Contract.OldValue(IsSequenceTerminated));
      Contract.Ensures(firstElementIndex >= Contract.OldValue(firstElementIndex));
      Contract.Ensures(elements.Count <= Contract.OldValue(elements.Count));
      Contract.Ensures((Contract.OldValue(elements.Count) - elements.Count) <= (firstElementIndex - Contract.OldValue(firstElementIndex)));

#if DEBUG
      int oldCount = elements.Count;
      int oldIndex = firstElementIndex;
#endif

      int lowestIndex = GetLowestIndex();

#if DEBUG
      Contract.Assert(elements.Count == oldCount);
      Contract.Assert(firstElementIndex == oldIndex);
#endif

      Contract.Assert(lowestIndex <= currentIndex);

      if (firstElementIndex < lowestIndex)
      {
        if (elements.Count > 0)
        {
          int delta = lowestIndex - firstElementIndex;
          int remove = Math.Min(delta, elements.Count);

          elements.RemoveRange(0, remove);

#if DEBUG
          Contract.Assume(remove == oldCount - elements.Count);
#endif

          Contract.Assert(delta == lowestIndex - firstElementIndex);
          Contract.Assert(remove <= delta);
        }

        firstElementIndex = lowestIndex;
      }

      Contract.Assert(firstElementIndex >= lowestIndex);
      Contract.Assume(firstElementIndex <= latestIndex || !stopped || (elements.Count > 0 && elements[0] != null && elements[0].Kind != NotificationKind.OnNext));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Invariant",
      Justification = "The static checker cannot prove invariants that are provable by the specified contracts.")]
    private int GetLowestIndex()
    {
      Contract.Ensures(Contract.Result<int>() >= 0);
      Contract.Ensures(Contract.Result<int>() <= currentIndex);
      Contract.Ensures(IsForwardOnly == Contract.OldValue(IsForwardOnly));
      Contract.Ensures(CurrentIndex == Contract.OldValue(CurrentIndex));
      Contract.Ensures(LatestIndex == Contract.OldValue(LatestIndex));
      Contract.Ensures(IsSequenceTerminated == Contract.OldValue(IsSequenceTerminated));
      Contract.Ensures(firstElementIndex == Contract.OldValue(firstElementIndex));
      Contract.Ensures(elements.Count == Contract.OldValue(elements.Count));

      int lowestIndex = currentIndex;

      /* Profiling shows Cursor.GetLowestIndex to be the hotest path in some (all?) parser queries.  55% of the total time may be spent there.
       * Changing to a for loop has proven to be up to 25% faster than foreach in a query with many SelectMany operations (each creates a branch).
       * The same change is being made here under the assumption that it will be beneficial without any negative impact to reactive queries.
       */
      for (int i = 0; i < branches.Count; i++)
      {
        var branch = branches[i];

        Contract.Assume(branch != null);

        lowestIndex = Math.Min(lowestIndex, branch.CurrentIndex);
      }

      if (!connected)
      {
        foreach (var subscription in subscriptions)
        {
          Contract.Assume(subscription != null);

          lowestIndex = Math.Min(lowestIndex, subscription.Index);
        }
      }

      return lowestIndex;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "This is a factory method.")]
    [ContractVerification(false)]		// Static checker times out, although contracts were proven by increasing the timeout setting temporarily.
    public IObservableCursor<T> Branch()
    {
      EnsureNotDisposed();

      var branch = new ObservableCursorBranch(this, currentIndex, disposables);

      Contract.Assume(branch.AtEndOfSequence == AtEndOfSequence);

      return branch;
    }

    public override string ToString()
    {
      return ToString(currentIndex);
    }

    [ContractVerification(false)]		// Static checker times out, although contracts were proven by increasing the timeout setting temporarily.
    private string ToString(int index, string prefix = null)
    {
      Contract.Requires(index >= 0);
      Contract.Ensures(Contract.Result<string>() != null);

      if (prefix != null)
      {
        prefix += " ";
      }

      if (latestIndex == -1)
      {
        return prefix + "Index: " + index;
      }
      else
      {
        var toIndex = firstElementIndex + ((stopped ? elements.Count - oneForTerminationNotification : elements.Count) - 1);

        var value = prefix + "Index: " + index + " of [" + firstElementIndex + "-"
          + (toIndex < firstElementIndex ? string.Empty : toIndex.ToString(System.Globalization.CultureInfo.CurrentCulture))
          + (stopped ? "|" : "...")
          + "]";

#if DEBUG
        value += " = " + MemoizedValuesToString(index);
#endif

        return value;
      }
    }

#if DEBUG
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Invariant",
      Justification = "The static checker cannot prove invariants that are provable by the fact that this method doesn't mutate any state.")]
    private string MemoizedValuesToString(int index)
    {
      Contract.Requires(index >= 0);

      var builder = new System.Text.StringBuilder("{ ");

      var virtualIndex = index - firstElementIndex;

      if (virtualIndex > 0)
      {
        builder.Append("[skip:");
      }

      int s = virtualIndex;
      bool first = true;

      foreach (var element in elements.Take(stopped ? elements.Count - oneForTerminationNotification : elements.Count))
      {
        Contract.Assume(element != null);

        if (!first)
        {
          builder.Append(',');
        }

        Contract.Assume(element.HasValue);

        builder.AppendFormat(
          System.Globalization.CultureInfo.CurrentCulture,
          "\"{0}\"",
          element.Value == null ? "null" : element.Value.ToString());

        if (virtualIndex > 0 && s == 1)
        {
          builder.Append("]");
        }

        s--;
        first = false;
      }

      if (virtualIndex > 0 && s > 0)
      {
        builder.Append(',', s);
        builder.Append(":skip],");

        first = true;
      }

      if (stopped)
      {
        builder.Append(" |");
      }
      else
      {
        if (!first)
        {
          builder.Append(',');
        }

        builder.Append('?');
      }

      builder.Append(" }");

      return builder.ToString();
    }
#endif

    private void Reset()
    {
      Contract.Ensures(!stopped);
      Contract.Ensures(currentIndex == 0);
      Contract.Ensures(latestIndex == -1);

      disposables.Clear();

      Contract.Assume(branches.Count == 0);

      elements.Clear();
      subscriptions.Clear();

      currentIndex = 0;
      firstElementIndex = 0;
      latestIndex = -1;
      stopped = false;
    }

    public void Dispose()
    {
      Contract.Ensures(disposed);

      if (!disposed)
      {
        Reset();

        disposables.Dispose();

        disposed = true;
      }
    }
  }
}