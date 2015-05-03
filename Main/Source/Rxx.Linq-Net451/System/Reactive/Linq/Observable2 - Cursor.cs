using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Returns a cursor for the specified observable sequence.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable sequence that the cursor moves over.</param>
    /// <remarks>
    /// The cursor's <see cref="IObservableCursor{TSource}.IsForwardOnly"/> property returns <see langword="false"/>, 
    /// indicating that it can move forward and backward.
    /// </remarks>
    /// <returns>A cursor for the specified observable sequence.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    public static IObservableCursor<TSource> ToCursor<TSource>(this IObservable<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>() != null);

      return new ObservableCursor<TSource>(source, isForwardOnly: false, enableBranchOptimizations: false);
    }

    /// <summary>
    /// Returns a cursor for the specified observable sequence with the specified support for backward movement.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable sequence that the cursor moves over.</param>
    /// <param name="forwardOnly">Specifies whether the cursor only moves forward.</param>
    /// <returns>A cursor for the specified observable sequence with the specified support for backward movement.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    public static IObservableCursor<TSource> ToCursor<TSource>(this IObservable<TSource> source, bool forwardOnly)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>() != null);
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>().IsForwardOnly == forwardOnly);

      return new ObservableCursor<TSource>(source, forwardOnly, enableBranchOptimizations: forwardOnly);
    }

    /// <summary>
    /// Returns a cursor for the specified observable sequence with the specified support for backward movement.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable sequence that the cursor moves over.</param>
    /// <param name="forwardOnly">Specifies whether the cursor only moves forward.</param>
    /// <param name="enableBranchOptimizations">Specifies whether a forward-only cursor is allowed to truncate the buffered 
    /// sequence, if necessary, whenever a branch is moved.  In the future, it may control other kinds of branch optimizations
    /// as well.  The default value is <see langword="true"/>.</param>
    /// <returns>A cursor for the specified observable sequence with the specified support for backward movement.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    public static IObservableCursor<TSource> ToCursor<TSource>(this IObservable<TSource> source, bool forwardOnly, bool enableBranchOptimizations)
    {
      Contract.Requires(source != null);
      Contract.Requires(forwardOnly || !enableBranchOptimizations);
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>() != null);
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>().IsForwardOnly == forwardOnly);

      return new ObservableCursor<TSource>(source, forwardOnly, enableBranchOptimizations);
    }

    /// <summary>
    /// Returns a thread-safe wrapper around the specified <paramref name="cursor"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="cursor">The observable cursor to be synchronized.</param>
    /// <remarks>
    /// Branches that are created before calling this method will not be synchronized; however, any new 
    /// branches that are created by the returned cursor will be synchronized with the cursor.
    /// </remarks>
    /// <returns>A thread-safe wrapper around the specified <paramref name="cursor"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    public static IObservableCursor<TSource> Synchronize<TSource>(this IObservableCursor<TSource> cursor)
    {
      Contract.Requires(cursor != null);
      Contract.Ensures(cursor.IsSynchronized == Contract.OldValue(cursor.IsSynchronized));
      Contract.Ensures(cursor.IsForwardOnly == Contract.OldValue(cursor.IsForwardOnly));
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>() != null);
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>().IsSynchronized);
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>().IsForwardOnly == cursor.IsForwardOnly);

      return new SynchronizedObservableCursor<TSource>(cursor);
    }

    /// <summary>
    /// Returns a thread-safe wrapper around the specified <paramref name="cursor"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="cursor">The observable cursor to be synchronized.</param>
    /// <param name="gate">An object used to synchronize the specified <paramref name="cursor"/>.</param>
    /// <remarks>
    /// Branches that are created before calling this method will not be synchronized; however, any new 
    /// branches that are created by the returned cursor will be synchronized with the cursor.
    /// </remarks>
    /// <returns>A thread-safe wrapper around the specified <paramref name="cursor"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    public static IObservableCursor<TSource> Synchronize<TSource>(this IObservableCursor<TSource> cursor, object gate)
    {
      Contract.Requires(cursor != null);
      Contract.Requires(gate != null);
      Contract.Ensures(cursor.IsSynchronized == Contract.OldValue(cursor.IsSynchronized));
      Contract.Ensures(cursor.IsForwardOnly == Contract.OldValue(cursor.IsForwardOnly));
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>() != null);
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>().IsSynchronized);
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>().IsForwardOnly == cursor.IsForwardOnly);

      return new SynchronizedObservableCursor<TSource>(cursor, gate);
    }

    /// <summary>
    /// Branches from the specified <paramref name="cursor"/> and moves the branch forward the specified number of elements.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="cursor">The observable cursor from which to branch.</param>
    /// <param name="skip">The number of elements that the new branch must skip.</param>
    /// <returns>A new branch from the specified <paramref name="cursor"/> with its current index moved ahead the specified 
    /// number of elements.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
      Justification = "Code contracts.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    [ContractVerification(false)]		// Static checker times out (raised time-out setting to 20 minutes and it still timed out)
    public static IObservableCursor<TSource> Remainder<TSource>(this IObservableCursor<TSource> cursor, int skip)
    {
      Contract.Requires(cursor != null);
      Contract.Requires(skip >= 0);
      Contract.Ensures(cursor.IsSynchronized == Contract.OldValue(cursor.IsSynchronized));
      Contract.Ensures(cursor.IsForwardOnly == Contract.OldValue(cursor.IsForwardOnly));
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>() != null);
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>().IsSynchronized == cursor.IsSynchronized);
      Contract.Ensures(Contract.Result<IObservableCursor<TSource>>().IsForwardOnly == cursor.IsForwardOnly);
      Contract.Ensures(cursor.IsSynchronized || Contract.Result<IObservableCursor<TSource>>().IsSequenceTerminated == cursor.IsSequenceTerminated);
      Contract.Ensures(cursor.IsSynchronized || Contract.Result<IObservableCursor<TSource>>().LatestIndex == cursor.LatestIndex);
      Contract.Ensures(cursor.IsSynchronized || Contract.Result<IObservableCursor<TSource>>().CurrentIndex ==
        (cursor.AtEndOfSequence
        ? cursor.CurrentIndex
        : cursor.IsSequenceTerminated
          ? Math.Min(cursor.CurrentIndex + skip, cursor.LatestIndex + 1)
          : cursor.CurrentIndex + skip));

      var branch = cursor.Branch();

      Contract.Assert(branch.IsSynchronized == cursor.IsSynchronized);
      Contract.Assert(branch.IsSynchronized || branch.IsSequenceTerminated == cursor.IsSequenceTerminated);
      Contract.Assert(branch.IsSynchronized || branch.CurrentIndex == cursor.CurrentIndex);
      Contract.Assert(branch.IsSynchronized || branch.LatestIndex == cursor.LatestIndex);
      Contract.Assert(branch.IsSynchronized || branch.AtEndOfSequence == cursor.AtEndOfSequence);

      if (branch.IsSynchronized)
      {
        branch.Move(skip);
      }
      else if (!branch.AtEndOfSequence)
      {
        int count = skip;

        if (branch.IsSequenceTerminated && branch.CurrentIndex + count > branch.LatestIndex + 1)
        {
          count = (branch.LatestIndex + 1) - branch.CurrentIndex;

          Contract.Assert(cursor.CurrentIndex + count == cursor.LatestIndex + 1);
        }

        branch.Move(count);

        Contract.Assert(!branch.IsSynchronized);
        Contract.Assert(branch.CurrentIndex == cursor.CurrentIndex + count);
        Contract.Assert(cursor.IsSequenceTerminated || branch.CurrentIndex == cursor.CurrentIndex + skip);
        Contract.Assume(!cursor.IsSequenceTerminated || cursor.CurrentIndex + skip <= cursor.LatestIndex + 1 || branch.CurrentIndex == cursor.LatestIndex + 1);
        Contract.Assume(!cursor.IsSequenceTerminated || branch.CurrentIndex == Math.Min(cursor.CurrentIndex + skip, cursor.LatestIndex + 1));
      }
      else
      {
        Contract.Assert(branch.CurrentIndex == cursor.CurrentIndex);
      }

      return branch;
    }
  }
}