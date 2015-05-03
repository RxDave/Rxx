using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers
{
  [ContractClass(typeof(IParserCursorContract<>))]
  internal interface IParserCursor<T> : IParser<T, T>, ICursor<T>, IParserCursorState
  {
  }

  [ContractClassFor(typeof(IParserCursor<>))]
  internal abstract class IParserCursorContract<T> : IParserCursor<T>
  {
    public int CurrentIndex
    {
      get
      {
        return 0;
      }
    }

    public bool AtEndOfSequence
    {
      get
      {
        return false;
      }
    }

    public bool IsForwardOnly
    {
      get
      {
        return false;
      }
    }

    public int LatestIndex
    {
      get
      {
        return 0;
      }
    }

    public bool IsSequenceTerminated
    {
      get
      {
        return false;
      }
    }

    int IParserCursorState.CurrentIndex
    {
      get
      {
        return CurrentIndex;
      }
    }

    bool IParserCursorState.AtEndOfSequence
    {
      get
      {
        return AtEndOfSequence;
      }
    }

    public IParser<T, T> Next
    {
      get
      {
        return null;
      }
    }

    public IEnumerable<IParseResult<T>> Parse(ICursor<T> source)
    {
      return null;
    }

    public void Move(int count)
    {
    }

    public ICursor<T> Branch()
    {
      return null;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return null;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return null;
    }

    public void Reset()
    {
    }

    public void Dispose()
    {
    }
  }
}