using System;
using System.Diagnostics.Contracts;
using System.Reactive;

namespace Rxx.Parsers.Reactive
{
  [ContractVerification(false)]
  internal sealed class DeferredObservableParserCursor<T> : IObservableParserCursor<T>
  {
    #region Public Properties
    public IObservableParser<T, T> Next
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
        return ((IObservableCursor<T>)ParserCursor).CurrentIndex;
      }
    }

    public bool AtEndOfSequence
    {
      get
      {
        return ((IObservableCursor<T>)ParserCursor).AtEndOfSequence;
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

    public bool IsSynchronized
    {
      get
      {
        return ParserCursor.IsSynchronized;
      }
    }

    public bool IsDisposed
    {
      get
      {
        return ParserCursor.IsDisposed;
      }
    }
    #endregion

    #region Private / Protected
    private IObservableParserCursor<T> ParserCursor
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParserCursor<T>>() != null);

        var value = parserFactory.Value;

        Contract.Assume(value != null);

        return value;
      }
    }

    private readonly Lazy<IObservableParserCursor<T>> parserFactory;
    #endregion

    #region Constructors
    public DeferredObservableParserCursor(Func<IObservableParserCursor<T>> parserFactory)
    {
      Contract.Requires(parserFactory != null);

      this.parserFactory = new Lazy<IObservableParserCursor<T>>(parserFactory);
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(parserFactory != null);
    }

    public IObservable<IParseResult<T>> Parse(IObservableCursor<T> source)
    {
      return ParserCursor.Parse(source);
    }

    public void Move(int count)
    {
      ParserCursor.Move(count);
    }

    public IObservableCursor<T> Branch()
    {
      return ParserCursor.Branch();
    }

    public IDisposable Subscribe(IObserver<T> observer, int count)
    {
      return ParserCursor.Subscribe(observer, count);
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
      return ParserCursor.Subscribe(observer);
    }

    public IDisposable Connect()
    {
      return ParserCursor.Connect();
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