using System.Threading;
using System.Threading.Tasks;

namespace System.Reactive
{
  internal sealed class AsyncNotification<TValue> : TaskCompletionSource<bool>, IAsyncNotification<TValue>
  {
    public TValue Value
    {
      get;
      private set;
    }

    Task IAsyncNotification.Task
    {
      get
      {
        return Task;
      }
    }

    public CancellationToken Cancel
    {
      get
      {
        return cancel;
      }
    }

    private readonly CancellationToken cancel;

    public AsyncNotification(TValue value)
    {
      Value = value;
    }

    public AsyncNotification(TValue value, CancellationToken cancel)
    {
      Value = value;
      this.cancel = cancel;
    }

    public AsyncNotification(TValue value, object state)
      : base(state)
    {
      Value = value;
    }

    public AsyncNotification(TValue value, object state, CancellationToken cancel)
      : base(state)
    {
      Value = value;
      this.cancel = cancel;
    }

    public AsyncNotification(TValue value, TaskCreationOptions creationOptions)
      : base(creationOptions)
    {
      Value = value;
    }

    public AsyncNotification(TValue value, TaskCreationOptions creationOptions, CancellationToken cancel)
      : base(creationOptions)
    {
      Value = value;
      this.cancel = cancel;
    }

    public AsyncNotification(TValue value, object state, TaskCreationOptions creationOptions)
      : base(state, creationOptions)
    {
      Value = value;
    }

    public AsyncNotification(TValue value, object state, TaskCreationOptions creationOptions, CancellationToken cancel)
      : base(state, creationOptions)
    {
      Value = value;
      this.cancel = cancel;
    }

    public bool TrySetCompleted()
    {
      return TrySetResult(false);
    }
  }
}