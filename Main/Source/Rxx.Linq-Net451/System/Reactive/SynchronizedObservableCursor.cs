using System.Diagnostics.Contracts;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace System.Reactive
{
  internal sealed class SynchronizedObservableCursor<T> : IObservableCursor<T>
  {
    public bool IsSynchronized
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>());

        return true;
      }
    }

    public bool IsForwardOnly
    {
      get
      {
        lock (gate)
        {
          return cursor.IsForwardOnly;
        }
      }
    }

    public int CurrentIndex
    {
      get
      {
        lock (gate)
        {
          return cursor.CurrentIndex;
        }
      }
    }

    public int LatestIndex
    {
      get
      {
        lock (gate)
        {
          return cursor.LatestIndex;
        }
      }
    }

    public bool AtEndOfSequence
    {
      get
      {
        bool atEnd;

        lock (gate)
        {
          atEnd = cursor.AtEndOfSequence;
        }

        Contract.Assume(!atEnd || IsSequenceTerminated);

        return atEnd;
      }
    }

    public bool IsSequenceTerminated
    {
      get
      {
        lock (gate)
        {
          return cursor.IsSequenceTerminated;
        }
      }
    }

    public bool IsDisposed
    {
      get
      {
        lock (gate)
        {
          return cursor.IsDisposed;
        }
      }
    }

    private readonly object gate;
    private readonly IObservableCursor<T> cursor;

    public SynchronizedObservableCursor(IObservableCursor<T> cursor, object gate = null)
    {
      Contract.Requires(cursor != null);
      Contract.Ensures(IsSynchronized);
      Contract.Ensures(IsForwardOnly == cursor.IsForwardOnly);

      this.cursor = cursor;
      this.gate = gate ?? new object();

      Contract.Assume(IsForwardOnly == cursor.IsForwardOnly);
    }

    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(gate != null);
      Contract.Invariant(cursor != null);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    public IDisposable Subscribe(IObserver<T> observer)
    {
      lock (gate)
      {
        var subscription = cursor.Subscribe(observer);

        return Disposable.Create(() =>
          {
            lock (gate)
            {
              subscription.Dispose();
            }
          });
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures",
      Justification = "IsForwardOnly is immutable.")]
    public IDisposable Subscribe(IObserver<T> observer, int count)
    {
      lock (gate)
      {
        var subscription = cursor.Subscribe(observer, count);

        return Disposable.Create(() =>
        {
          lock (gate)
          {
            subscription.Dispose();
          }
        });
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    public IDisposable Connect()
    {
      lock (gate)
      {
        var subscription = cursor.Connect();

        return Disposable.Create(() =>
        {
          lock (gate)
          {
            subscription.Dispose();
          }
        });
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
      Justification = "Inherited code contracts.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Requires",
      Justification = "Allow precondition failures at runtime.  They cannot be checked up front due to multi-threading race conditions.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures",
      Justification = "IsForwardOnly is immutable.")]
    public void Move(int count)
    {
      lock (gate)
      {
        cursor.Move(count);
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures",
      Justification = "IsForwardOnly is immutable.")]
    public IObservableCursor<T> Branch()
    {
      lock (gate)
      {
        return cursor.Branch().Synchronize(gate);
      }
    }

    public override string ToString()
    {
      lock (gate)
      {
        return cursor.ToString();
      }
    }

    public void Dispose()
    {
      lock (gate)
      {
        cursor.Dispose();
      }
    }
  }
}