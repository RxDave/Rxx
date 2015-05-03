using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Disposables;

namespace System.Linq
{
  internal sealed partial class Cursor<T> : ICursor<T>
  {
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

    private const int oneForTerminationNotification = 1;

    private readonly List<Notification<T>> elements = new List<Notification<T>>();
    private readonly List<CursorBranch> branches = new List<CursorBranch>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly bool isForwardOnly, truncateWhileBranched;
    private readonly IEnumerable<T> source;
    private IEnumerator<T> sourceEnumerator;
    private bool disposed;
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

    public Cursor(IEnumerable<T> source, bool isForwardOnly, bool enableBranchOptimizations)
    {
      Contract.Requires(source != null);
      Contract.Requires(isForwardOnly || !enableBranchOptimizations);
      Contract.Ensures(IsForwardOnly == isForwardOnly);

      // TODO: Consider this optimization: if source implements IList<T> use it as the internal buffer to avoid duplication.  Does it have to be a read-only list though?

      this.source = source;
      this.isForwardOnly = isForwardOnly;
      this.truncateWhileBranched = enableBranchOptimizations;

      sourceEnumerator = IterateSource();

      disposables.Add(sourceEnumerator);
    }

    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(disposables != null);
      Contract.Invariant(source != null);
      Contract.Invariant(sourceEnumerator != null);
      Contract.Invariant(elements != null);
      Contract.Invariant(branches != null);
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Invariant",
      Justification = "The static checker cannot prove invariants that are provable by the specified contracts.")]
    [ContractVerification(false)]		// Static checker times out.
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

    public IEnumerator<T> GetEnumerator()
    {
      EnsureNotDisposed();

      Contract.Assert(currentIndex >= firstElementIndex);
      Contract.Assert(!stopped || currentIndex <= latestIndex + oneForTerminationNotification);

      return GetEnumerator(currentIndex);
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      EnsureNotDisposed();

      Contract.Assert(currentIndex >= firstElementIndex);
      Contract.Assert(!stopped || currentIndex <= latestIndex + oneForTerminationNotification);

      return GetEnumerator(currentIndex);
    }

    private IEnumerator<T> GetEnumerator(int index)
    {
      Contract.Requires(index >= firstElementIndex);

      // Contract.Requires(index > latestIndex || elements.Count >= latestIndex - index);		// A bug in the Code Contracts rewriter causes a NullReferenceException at runtime.
      Contract.Requires(!stopped || index <= latestIndex + oneForTerminationNotification);
      Contract.Ensures(Contract.Result<IEnumerator<T>>() != null);

      return GetEnumeratorIterator(index);
    }

    private IEnumerator<T> GetEnumeratorIterator(int index)
    {
      int startingLatestIndex = latestIndex;

      int existingCount = Math.Max(0, (latestIndex - index) + 1);

      if (stopped)
      {
        existingCount++;
      }

      if (existingCount > 0)
      {
        foreach (var element in GetExistingElements(index, existingCount))
        {
          Contract.Assume(element != null);

          switch (element.Kind)
          {
            case NotificationKind.OnNext:
              yield return element.Value;
              continue;
            case NotificationKind.OnError:
              throw element.Exception;
            case NotificationKind.OnCompleted:
              yield break;
          }
        }

        Contract.Assume(!stopped);

#if DEBUG
        index = latestIndex + 1;
#endif
      }
      else
      {
        while (index > latestIndex + 1)
        {
          if (!sourceEnumerator.MoveNext())
          {
            yield break;
          }

          Contract.Assume(latestIndex == ++startingLatestIndex);
        }
      }

      Contract.Assume(startingLatestIndex == latestIndex);

#if DEBUG
      Contract.Assert(index == latestIndex + 1);
#endif

      while (sourceEnumerator.MoveNext())
      {
        var nextIndex = startingLatestIndex + 1;

        if (latestIndex > nextIndex)
        {
          foreach (var element in GetExistingElements(nextIndex, latestIndex - nextIndex))
          {
            Contract.Assume(element != null);

            switch (element.Kind)
            {
              case NotificationKind.OnNext:
                yield return element.Value;
                continue;
              case NotificationKind.OnError:
                throw element.Exception;
              case NotificationKind.OnCompleted:
                yield break;
            }
          }
        }

        startingLatestIndex = latestIndex;

        yield return sourceEnumerator.Current;
      }

      Contract.Assume(stopped);

      if (latestIndex > startingLatestIndex)
      {
        var nextIndex = startingLatestIndex + 1;

        foreach (var element in GetExistingElements(nextIndex, (latestIndex - startingLatestIndex) + oneForTerminationNotification))
        {
          Contract.Assume(element != null);

          switch (element.Kind)
          {
            case NotificationKind.OnNext:
              yield return element.Value;
              continue;
            case NotificationKind.OnError:
              throw element.Exception;
            case NotificationKind.OnCompleted:
              yield break;
          }
        }
      }
    }

    private IEnumerable<Notification<T>> GetExistingElements(int startIndex, int count)
    {
      Contract.Requires(startIndex >= -1);
      Contract.Requires(!stopped || startIndex <= latestIndex + oneForTerminationNotification);
      Contract.Requires(stopped || startIndex <= latestIndex);
      Contract.Requires(count > 0);
      Contract.Ensures(Contract.Result<IEnumerable<Notification<T>>>() != null);

      var currentElements = new Notification<T>[count];

      // Yielding can modify the collection so we must iterate a copy.
      CopyExistingElementsTo(currentElements, count, startIndex == -1 ? 0 : startIndex);

      return currentElements;
    }

    // Contract bug: http://social.msdn.microsoft.com/Forums/en-AU/codecontracts/thread/c933a14f-2853-450d-a8e6-5f8f062477f4
    [ContractVerification(false)]
    private void CopyExistingElementsTo(Notification<T>[] currentElements, int existingCount, int startIndex)
    {
      elements.CopyTo(startIndex - firstElementIndex, currentElements, 0, existingCount);
    }

    private IEnumerator<T> IterateSource()
    {
      Contract.Ensures(Contract.Result<IEnumerator<T>>() != null);

      using (var enumerator = source.GetEnumerator())
      {
        bool moved;

        do
        {
          T value = default(T);

          try
          {
            moved = enumerator.MoveNext();

            if (moved)
            {
              value = enumerator.Current;
            }
          }
          catch (Exception ex)
          {
            OnError(ex);
            throw;
          }

          if (moved)
          {
            OnNext(value);

            yield return value;
          }
        }
        while (moved);

        OnCompleted();
      }
    }

    private void OnNext(T value)
    {
      if (!stopped)
      {
        Contract.Assert(latestIndex < firstElementIndex + elements.Count);

        latestIndex++;

        Contract.Assert(latestIndex - 1 < firstElementIndex + elements.Count);
        Contract.Assert(isForwardOnly || firstElementIndex == 0);

        if (firstElementIndex <= latestIndex)
        {
          Contract.Assert(latestIndex - 1 < firstElementIndex + elements.Count);

          elements.Add(Notification.CreateOnNext(value));

          Contract.Assert(latestIndex < firstElementIndex + elements.Count);
        }
        else
        {
          Contract.Assert(latestIndex < firstElementIndex + elements.Count);
        }

        Contract.Assert(currentIndex > latestIndex || elements.Count >= latestIndex - currentIndex);
      }
    }

    private void OnError(Exception error)
    {
      Contract.Requires(error != null);

      if (!stopped)
      {
        elements.Add(Notification.CreateOnError<T>(error));

        Terminated();
      }
    }

    private void OnCompleted()
    {
      if (!stopped)
      {
        elements.Add(Notification.CreateOnCompleted<T>());

        Terminated();
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

#if SILVERLIGHT
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Invariant",
      Justification = "The static checker cannot prove invariants that are provable by the specified contracts.")]
#endif
    [ContractVerification(false)]		// Static checker is unable to prove any invariants.
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

      /* Profiling shows this method to be the hotest path in some (all?) parser queries.  55% of the total time may be spent here.
       * Changing to a for loop has proven to be up to 25% faster than foreach in a query with many SelectMany operations (each creates a branch).
       */
      for (int i = 0; i < branches.Count; i++)
      {
        var branch = branches[i];

        Contract.Assume(branch != null);

        lowestIndex = Math.Min(lowestIndex, branch.CurrentIndex);
      }

      return lowestIndex;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "This is a factory method.")]
    [ContractVerification(false)]		// Static checker is timing out
    public ICursor<T> Branch()
    {
      EnsureNotDisposed();

      var branch = new CursorBranch(this, currentIndex, disposables);

      Contract.Assume(branch.AtEndOfSequence == AtEndOfSequence);

      return branch;
    }

    public override string ToString()
    {
      return ToString(currentIndex);
    }

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
    [ContractVerification(false)]		// Static checker times out, although contracts were proven by increasing the timeout setting temporarily.
    private string MemoizedValuesToString(int index)
    {
      Contract.Requires(index >= 0);

      var builder = new System.Text.StringBuilder("{ ");

      var skip = index - firstElementIndex;

      if (skip > 0)
      {
        builder.Append("[skip:");
      }

      int s = skip;
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

        if (skip > 0 && s == 1)
        {
          builder.Append("]");
        }

        s--;
        first = false;
      }

      if (skip > 0 && s > 0)
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

    /// <summary>
    /// Clears any buffered elements, branches and state, and ensures that a subsequent iteration will re-enumerate the source sequence.
    /// </summary>
    public void Reset()
    {
      Reset(permanent: false);
    }

    private void Reset(bool permanent)
    {
      Contract.Ensures(!stopped);
      Contract.Ensures(currentIndex == 0);
      Contract.Ensures(latestIndex == -1);

      disposables.Clear();

      Contract.Assume(branches.Count == 0);

      elements.Clear();

      if (!permanent)
      {
        sourceEnumerator = IterateSource();

        disposables.Add(sourceEnumerator);
      }

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
        Reset(permanent: true);

        disposables.Dispose();

        disposed = true;
      }
    }
  }
}