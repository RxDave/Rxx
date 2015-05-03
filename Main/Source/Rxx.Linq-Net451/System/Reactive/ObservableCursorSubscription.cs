using System.Diagnostics.Contracts;

namespace System.Reactive
{
  internal partial class ObservableCursor<T>
  {
    private sealed class ObservableCursorSubscription : IDisposable
    {
      public IObserver<T> Observer
      {
        get
        {
          Contract.Ensures(Contract.Result<IObserver<T>>() != null);

          return observer;
        }
      }

      public int Index
      {
        get
        {
          Contract.Ensures(Contract.Result<int>() >= 0);

          return index;
        }
      }

      public int Take
      {
        get
        {
          Contract.Ensures(Contract.Result<int>() >= subscribeUnlimited);

          return take;
        }
        set
        {
          Contract.Requires(value >= 0);

          take = value;
        }
      }

      private readonly ObservableCursor<T> cursor;
      private readonly IObserver<T> observer;
      private readonly int index;
      private int take;
      private bool disposed;

      public ObservableCursorSubscription(ObservableCursor<T> cursor, int index, int take, IObserver<T> observer)
      {
        Contract.Requires(cursor != null);
        Contract.Requires(index >= 0);
        Contract.Requires(take == subscribeUnlimited || take > 0);
        Contract.Requires(observer != null);

        this.cursor = cursor;
        this.index = index;
        this.take = take;
        this.observer = observer;

        Contract.Assume(cursor.subscriptions != null);

        cursor.subscriptions.Add(this);
      }

      [ContractInvariantMethod]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
      private void ObjectInvariant()
      {
        Contract.Invariant(cursor != null);
        Contract.Invariant(observer != null);
        Contract.Invariant(index >= 0);
        Contract.Invariant(take >= subscribeUnlimited);
      }

      public void Dispose()
      {
        if (!disposed)
        {
          Contract.Assume(cursor.subscriptions != null);

          cursor.subscriptions.Remove(this);

          disposed = true;
        }
      }
    }
  }
}