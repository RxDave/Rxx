using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Rxx.Parsers.Properties;

namespace Rxx.Parsers
{
  internal sealed partial class ParserCursor<T> : IParserCursor<T>
  {
    #region Public Properties
    public IParser<T, T> Next
    {
      get
      {
        return this;
      }
    }

    public bool IsForwardOnly
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>() == cursor.IsForwardOnly);

        return cursor.IsForwardOnly;
      }
    }

    public int CurrentIndex
    {
      get
      {
        Contract.Ensures(Contract.Result<int>() == cursor.CurrentIndex);

        return cursor.CurrentIndex;
      }
    }

    public int LatestIndex
    {
      get
      {
        Contract.Ensures(Contract.Result<int>() == cursor.LatestIndex);

        return cursor.LatestIndex;
      }
    }

    public bool AtEndOfSequence
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>() == cursor.AtEndOfSequence);

        return cursor.AtEndOfSequence;
      }
    }

    public bool IsSequenceTerminated
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>() == cursor.IsSequenceTerminated);

        return cursor.IsSequenceTerminated;
      }
    }
    #endregion

    #region Private / Protected
    private readonly ICursor<T> cursor;
    private readonly List<ICursor<T>> branches = new List<ICursor<T>>();
    #endregion

    #region Constructors
    public ParserCursor(ICursor<T> cursor)
    {
      Contract.Requires(cursor != null);
      Contract.Requires(cursor.IsForwardOnly);

      this.cursor = cursor;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(cursor != null);
      Contract.Invariant(cursor.IsForwardOnly);
      Contract.Invariant(branches != null);
      Contract.Invariant(IsForwardOnly == cursor.IsForwardOnly);
      Contract.Invariant(CurrentIndex == cursor.CurrentIndex);
      Contract.Invariant(LatestIndex == cursor.LatestIndex);
      Contract.Invariant(AtEndOfSequence == cursor.AtEndOfSequence);
      Contract.Invariant(IsSequenceTerminated == cursor.IsSequenceTerminated);
    }

    public IEnumerable<IParseResult<T>> Parse(ICursor<T> source)
    {
      foreach (var value in source)
      {
#if !SILVERLIGHT && !PORT_45 && !PORT_40
        ParserTraceSources.TraceInput(value);
#endif

        yield return ParseResult.Create(value, length: 1);
        yield break;
      }
    }

    public IEnumerator<T> GetEnumerator()
    {
      return cursor.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return cursor.GetEnumerator();
    }

    public void Move(int count)
    {
      cursor.Move(count);
    }

    public void MoveToEnd()
    {
      var count = (cursor.LatestIndex + 1) - cursor.CurrentIndex;

      if (count < 0)
      {
        throw new InvalidOperationException(Errors.ParserCannotMoveToEndBackward);
      }

      Contract.Assert(count >= 0);
      Contract.Assume(count == 0 || cursor.CurrentIndex < cursor.LatestIndex + 1);
      Contract.Assert(cursor.AtEndOfSequence == (cursor.IsSequenceTerminated && cursor.CurrentIndex == cursor.LatestIndex + 1));
      Contract.Assert(!cursor.AtEndOfSequence || count == 0);

      cursor.Move(count);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    [ContractVerification(false)]
    public ICursor<T> Branch()
    {
      var branch = cursor.Branch();

      branches.Add(branch);

      return new ParserCursorBranch(branch, () => branches.Remove(branch));
    }

    public override string ToString()
    {
      var branch = branches.Count > 0 ? branches[branches.Count - 1] : null;

      return (branch ?? cursor).ToString();
    }

    public void Reset()
    {
      cursor.Reset();
    }

    public void Dispose()
    {
      branches.Clear();

      cursor.Dispose();
    }
    #endregion
  }
}