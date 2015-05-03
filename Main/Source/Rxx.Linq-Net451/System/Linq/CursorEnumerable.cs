using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Linq
{
  /// <summary>
  /// Provides a set of <see langword="static"/> methods for query operations over enumerable sequences.
  /// </summary>
  public static partial class CursorEnumerable
  {
    /// <summary>
    /// Returns a cursor for the specified enumerable sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of objects to enumerate.</typeparam>
    /// <param name="source">The enumerable sequence that the cursor moves over.</param>
    /// <remarks>
    /// The cursor's <see cref="ICursor{TSource}.IsForwardOnly"/> property returns <see langword="false"/>, 
    /// indicating that it can move forward and backward.
    /// </remarks>
    /// <returns>A cursor for the specified enumerable sequence.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    public static ICursor<TSource> ToCursor<TSource>(this IEnumerable<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<ICursor<TSource>>() != null);

      return new Cursor<TSource>(source, isForwardOnly: false, enableBranchOptimizations: false);
    }

    /// <summary>
    /// Returns a cursor for the specified enumerable sequence with the specified support for backward movement.
    /// </summary>
    /// <typeparam name="TSource">The type of objects to enumerate.</typeparam>
    /// <param name="source">The enumerable sequence that the cursor moves over.</param>
    /// <param name="forwardOnly">Specifies whether the cursor only moves forward.</param>
    /// <returns>A cursor for the specified enumerable sequence with the specified support for backward movement.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    public static ICursor<TSource> ToCursor<TSource>(this IEnumerable<TSource> source, bool forwardOnly)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<ICursor<TSource>>() != null);
      Contract.Ensures(Contract.Result<ICursor<TSource>>().IsForwardOnly == forwardOnly);

      return new Cursor<TSource>(source, forwardOnly, enableBranchOptimizations: forwardOnly);
    }

    /// <summary>
    /// Returns a cursor for the specified enumerable sequence with the specified support for backward movement.
    /// </summary>
    /// <typeparam name="TSource">The type of objects to enumerate.</typeparam>
    /// <param name="source">The enumerable sequence that the cursor moves over.</param>
    /// <param name="forwardOnly">Specifies whether the cursor only moves forward.</param>
    /// <param name="enableBranchOptimizations">Specifies whether a forward-only cursor is allowed to truncate the buffered 
    /// sequence, if necessary, whenever a branch is moved.  In the future, it may control other kinds of branch optimizations
    /// as well.  The default value is <see langword="true"/>.</param>
    /// <returns>A cursor for the specified enumerable sequence with the specified support for backward movement.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    public static ICursor<TSource> ToCursor<TSource>(this IEnumerable<TSource> source, bool forwardOnly, bool enableBranchOptimizations)
    {
      Contract.Requires(source != null);
      Contract.Requires(forwardOnly || !enableBranchOptimizations);
      Contract.Ensures(Contract.Result<ICursor<TSource>>() != null);
      Contract.Ensures(Contract.Result<ICursor<TSource>>().IsForwardOnly == forwardOnly);

      return new Cursor<TSource>(source, forwardOnly, enableBranchOptimizations);
    }

    /// <summary>
    /// Branches from the specified <paramref name="cursor"/> and moves the branch forward the specified number of elements.
    /// </summary>
    /// <typeparam name="TSource">The type of objects to enumerate.</typeparam>
    /// <param name="cursor">The enumerable cursor from which to branch.</param>
    /// <param name="skip">The number of elements that the new branch must skip.</param>
    /// <returns>A new branch from the specified <paramref name="cursor"/> with its current index moved ahead the specified 
    /// number of elements.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
      Justification = "Code contracts.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures",
      Justification = "Assertions prove the long Ensures but static checker still warns that Ensures is unproven.")]
    public static ICursor<TSource> Remainder<TSource>(this ICursor<TSource> cursor, int skip)
    {
      Contract.Requires(cursor != null);
      Contract.Requires(skip >= 0);
      Contract.Ensures(cursor.IsForwardOnly == Contract.OldValue(cursor.IsForwardOnly));
      Contract.Ensures(Contract.Result<ICursor<TSource>>() != null);
      Contract.Ensures(Contract.Result<ICursor<TSource>>().IsForwardOnly == cursor.IsForwardOnly);
      Contract.Ensures(Contract.Result<ICursor<TSource>>().IsSequenceTerminated == cursor.IsSequenceTerminated);
      Contract.Ensures(Contract.Result<ICursor<TSource>>().LatestIndex == cursor.LatestIndex);
      Contract.Ensures(Contract.Result<ICursor<TSource>>().CurrentIndex ==
        (cursor.AtEndOfSequence
        ? cursor.CurrentIndex
        : cursor.IsSequenceTerminated
          ? Math.Min(cursor.CurrentIndex + skip, cursor.LatestIndex + 1)
          : cursor.CurrentIndex + skip));

      var branch = cursor.Branch();

      Contract.Assert(branch.IsSequenceTerminated == cursor.IsSequenceTerminated);
      Contract.Assert(branch.CurrentIndex == cursor.CurrentIndex);
      Contract.Assert(branch.LatestIndex == cursor.LatestIndex);
      Contract.Assert(branch.AtEndOfSequence == cursor.AtEndOfSequence);

      if (!branch.AtEndOfSequence)
      {
        int count = skip;

        if (branch.IsSequenceTerminated && branch.CurrentIndex + count > branch.LatestIndex + 1)
        {
          count = (branch.LatestIndex + 1) - branch.CurrentIndex;

          Contract.Assert(cursor.CurrentIndex + count == cursor.LatestIndex + 1);
        }

        branch.Move(count);

        Contract.Assert(branch.CurrentIndex == cursor.CurrentIndex + count);
        Contract.Assert(cursor.IsSequenceTerminated || branch.CurrentIndex == cursor.CurrentIndex + skip);
        Contract.Assert(!cursor.IsSequenceTerminated || cursor.CurrentIndex + skip <= cursor.LatestIndex + 1 || branch.CurrentIndex == cursor.LatestIndex + 1);
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