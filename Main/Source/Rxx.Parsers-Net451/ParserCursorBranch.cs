using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers
{
  internal sealed partial class ParserCursor<T>
  {
    private sealed class ParserCursorBranch : ICursor<T>
    {
      #region Public Properties
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
          Contract.Ensures(Contract.Result<int>() == branch.CurrentIndex);

          return branch.CurrentIndex;
        }
      }

      public int LatestIndex
      {
        get
        {
          Contract.Ensures(Contract.Result<int>() == branch.LatestIndex);

          return branch.LatestIndex;
        }
      }

      public bool AtEndOfSequence
      {
        get
        {
          Contract.Ensures(Contract.Result<bool>() == branch.AtEndOfSequence);

          return branch.AtEndOfSequence;
        }
      }

      public bool IsSequenceTerminated
      {
        get
        {
          Contract.Ensures(Contract.Result<bool>() == branch.IsSequenceTerminated);

          return branch.IsSequenceTerminated;
        }
      }
      #endregion

      #region Private / Protected
      private readonly List<ICursor<T>> branches = new List<ICursor<T>>();
      private readonly ICursor<T> branch;
      private readonly Action dispose;
      #endregion

      #region Constructors
      public ParserCursorBranch(ICursor<T> branch, Action dispose)
      {
        Contract.Requires(branch != null);
        Contract.Requires(branch.IsForwardOnly);
        Contract.Requires(dispose != null);
        Contract.Ensures(IsForwardOnly == branch.IsForwardOnly);
        Contract.Ensures(CurrentIndex == branch.CurrentIndex);
        Contract.Ensures(LatestIndex == branch.LatestIndex);
        Contract.Ensures(AtEndOfSequence == branch.AtEndOfSequence);
        Contract.Ensures(IsSequenceTerminated == branch.IsSequenceTerminated);

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
        Contract.Invariant(IsForwardOnly == branch.IsForwardOnly);
        Contract.Invariant(CurrentIndex == branch.CurrentIndex);
        Contract.Invariant(LatestIndex == branch.LatestIndex);
        Contract.Invariant(AtEndOfSequence == branch.AtEndOfSequence);
        Contract.Invariant(IsSequenceTerminated == branch.IsSequenceTerminated);
      }

      public IEnumerator<T> GetEnumerator()
      {
        return branch.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return branch.GetEnumerator();
      }

      public void Move(int count)
      {
        branch.Move(count);
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
        Justification = "The disposable is returned to the caller.")]
      public ICursor<T> Branch()
      {
        var next = branch.Branch();

        branches.Add(next);

        return new ParserCursorBranch(next, () => branches.Remove(next));
      }

      public override string ToString()
      {
        var child = branches.Count > 0 ? branches[branches.Count - 1] : null;

        return (child ?? branch).ToString();
      }

      public void Reset()
      {
        branch.Reset();
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