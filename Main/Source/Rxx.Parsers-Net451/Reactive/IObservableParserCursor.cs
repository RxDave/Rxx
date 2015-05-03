using System;
using System.Diagnostics.Contracts;
using System.Reactive;

namespace Rxx.Parsers.Reactive
{
  [ContractClass(typeof(IObservableParserCursorContract<>))]
  internal interface IObservableParserCursor<T> : IObservableParser<T, T>, IObservableCursor<T>, IParserCursorState
  {
  }

  [ContractClassFor(typeof(IObservableParserCursor<>))]
  internal abstract class IObservableParserCursorContract<T> : IObservableParserCursor<T>
  {
    public IObservableParser<T, T> Next
    {
      get
      {
        return null;
      }
    }

    public bool IsSynchronized
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

    public int CurrentIndex
    {
      get
      {
        return 0;
      }
    }

    public int LatestIndex
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

    public bool IsSequenceTerminated
    {
      get
      {
        return false;
      }
    }

    public bool IsDisposed
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

    public IObservable<IParseResult<T>> Parse(IObservableCursor<T> source)
    {
      return null;
    }

    public void Move(int count)
    {
    }

    public IObservableCursor<T> Branch()
    {
      return null;
    }

    public IDisposable Subscribe(IObserver<T> observer, int count)
    {
      return null;
    }

    public IDisposable Connect()
    {
      return null;
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
      return null;
    }

    public void Dispose()
    {
    }
  }
}