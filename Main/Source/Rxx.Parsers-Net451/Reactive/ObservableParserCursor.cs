using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;
using Rxx.Parsers.Properties;

namespace Rxx.Parsers.Reactive
{
  internal sealed partial class ObservableParserCursor<T> : IObservableParserCursor<T>
  {
    #region Public Properties
    public IObservableParser<T, T> Next
    {
      get
      {
        return this;
      }
    }

    public bool IsSynchronized
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>() == cursor.IsSynchronized);

        return cursor.IsSynchronized;
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
        Contract.Ensures(IsSynchronized || Contract.Result<int>() == cursor.CurrentIndex);

        return cursor.CurrentIndex;
      }
    }

    public int LatestIndex
    {
      get
      {
        Contract.Ensures(IsSynchronized || Contract.Result<int>() == cursor.LatestIndex);

        return cursor.LatestIndex;
      }
    }

    public bool AtEndOfSequence
    {
      get
      {
        Contract.Ensures(IsSynchronized || Contract.Result<bool>() == cursor.AtEndOfSequence);

        var value = cursor.AtEndOfSequence;

        Contract.Assert(IsSynchronized || value == (IsSequenceTerminated && CurrentIndex == LatestIndex + 1));

        return value;
      }
    }

    public bool IsSequenceTerminated
    {
      get
      {
        Contract.Ensures(IsSynchronized || Contract.Result<bool>() == cursor.IsSequenceTerminated);
        Contract.Ensures(!cursor.IsSequenceTerminated || Contract.Result<bool>() == true);

        return cursor.IsSequenceTerminated;
      }
    }

    public bool IsDisposed
    {
      get
      {
        Contract.Ensures(IsSynchronized || Contract.Result<bool>() == cursor.IsDisposed);

        return cursor.IsDisposed;
      }
    }
    #endregion

    #region Private / Protected
    private readonly IObservableCursor<T> cursor;
    private readonly List<IObservableCursor<T>> branches = new List<IObservableCursor<T>>();
    #endregion

    #region Constructors
    public ObservableParserCursor(IObservableCursor<T> cursor)
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
      Contract.Invariant(IsSynchronized == cursor.IsSynchronized);
      Contract.Invariant(IsForwardOnly == cursor.IsForwardOnly);
      Contract.Invariant(IsSynchronized || CurrentIndex == cursor.CurrentIndex);
      Contract.Invariant(IsSynchronized || LatestIndex == cursor.LatestIndex);
      Contract.Invariant(IsSynchronized || AtEndOfSequence == cursor.AtEndOfSequence);
      Contract.Invariant(IsSynchronized || IsSequenceTerminated == cursor.IsSequenceTerminated);
    }

    public IObservable<IParseResult<T>> Parse(IObservableCursor<T> source)
    {
      return Observable.Create<IParseResult<T>>(
        observer =>
        {
          return source.Subscribe(
            Observer.Create<T>(
              value =>
              {
#if !SILVERLIGHT && !PORT_45 && !PORT_40
                ParserTraceSources.TraceInput(value);
#endif

                observer.OnNext(ParseResult.Create(value, length: 1));
              },
              observer.OnError,
              observer.OnCompleted),
            count: 1);
        });
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
      var subscription = cursor.Subscribe(observer);

      Contract.Assume(cursor.IsForwardOnly);

      return subscription;
    }

    public IDisposable Subscribe(IObserver<T> observer, int count)
    {
      return cursor.Subscribe(observer, count);
    }

    public IDisposable Connect()
    {
      var connection = cursor.Connect();

      Contract.Assume(cursor.IsForwardOnly);

      return connection;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
      Justification = "Inherited code contracts and invariants.")]
    [ContractVerification(false)]
    public void Move(int count)
    {
      Contract.Assert(IsSynchronized || !AtEndOfSequence || count <= 0);

      cursor.Move(count);
    }

    [ContractVerification(false)]
    public void MoveToEnd()
    {
      var count = (cursor.LatestIndex + 1) - cursor.CurrentIndex;

      if (count < 0)
      {
        throw new InvalidOperationException(Errors.ParserCannotMoveToEndBackward);
      }

      Contract.Assert(IsSynchronized || !AtEndOfSequence || count == 0);

      cursor.Move(count);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    [ContractVerification(false)]
    public IObservableCursor<T> Branch()
    {
      var branch = cursor.Branch();

      branches.Add(branch);

      return new ObservableParserCursorBranch(branch, () => branches.Remove(branch));
    }

    public override string ToString()
    {
      var branch = branches.Count > 0 ? branches[branches.Count - 1] : null;

      return (branch ?? cursor).ToString();
    }

    public void Dispose()
    {
      branches.Clear();

      cursor.Dispose();
    }
    #endregion
  }
}