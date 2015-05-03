using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers
{
  [ContractVerification(false)]
  internal sealed class DeferredParserCursor<T> : IParserCursor<T>
  {
    #region Public Properties
    public IParser<T, T> Next
    {
      get
      {
        return ParserCursor.Next;
      }
    }

    public int CurrentIndex
    {
      get
      {
        return ((ICursor<T>)ParserCursor).CurrentIndex;
      }
    }

    public bool AtEndOfSequence
    {
      get
      {
        return ((ICursor<T>)ParserCursor).AtEndOfSequence;
      }
    }

    public bool IsForwardOnly
    {
      get
      {
        return ParserCursor.IsForwardOnly;
      }
    }

    public int LatestIndex
    {
      get
      {
        return ParserCursor.LatestIndex;
      }
    }

    public bool IsSequenceTerminated
    {
      get
      {
        return ParserCursor.IsSequenceTerminated;
      }
    }
    #endregion

    #region Private / Protected
    private IParserCursor<T> ParserCursor
    {
      get
      {
        Contract.Ensures(Contract.Result<IParserCursor<T>>() != null);

        var value = parserFactory.Value;

        Contract.Assume(value != null);

        return value;
      }
    }

    private readonly Lazy<IParserCursor<T>> parserFactory;
    #endregion

    #region Constructors
    public DeferredParserCursor(Func<IParserCursor<T>> parserFactory)
    {
      Contract.Requires(parserFactory != null);

      this.parserFactory = new Lazy<IParserCursor<T>>(parserFactory);
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(parserFactory != null);
    }

    public IEnumerable<IParseResult<T>> Parse(ICursor<T> source)
    {
      return ParserCursor.Parse(source);
    }

    public void Move(int count)
    {
      ParserCursor.Move(count);
    }

    public ICursor<T> Branch()
    {
      return ParserCursor.Branch();
    }

    public void Reset()
    {
      ParserCursor.Reset();
    }

    public IEnumerator<T> GetEnumerator()
    {
      return ParserCursor.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return ParserCursor.GetEnumerator();
    }

    public void Dispose()
    {
      ParserCursor.Dispose();
    }

    public override string ToString()
    {
      return ParserCursor.ToString();
    }
    #endregion
  }
}