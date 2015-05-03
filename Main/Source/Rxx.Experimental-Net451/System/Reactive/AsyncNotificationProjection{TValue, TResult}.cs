using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace System.Reactive
{
  internal sealed class AsyncNotificationProjection<TValue, TResult> : IAsyncNotification<TValue, TResult>
  {
    public TValue Value
    {
      get
      {
        return value;
      }
    }

    public Task<TResult> Task
    {
      get
      {
        return notification.Task;
      }
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
        return notification.Cancel;
      }
    }

    private readonly IAsyncNotificationWithResult<TResult> notification;
    private TValue value;

    public AsyncNotificationProjection(TValue value, IAsyncNotificationWithResult<TResult> notification)
    {
      Contract.Requires(notification != null);

      this.value = value;
      this.notification = notification;
    }

    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(notification != null);
    }

    public bool TrySetCompleted()
    {
      return notification.TrySetCompleted();
    }

    public bool TrySetCompleted(TResult result)
    {
      return notification.TrySetCompleted(result);
    }

    public bool TrySetException(Exception error)
    {
      return notification.TrySetException(error);
    }

    public bool TrySetCanceled()
    {
      return notification.TrySetCanceled();
    }

    public IDisposable Subscribe(IObserver<TResult> observer)
    {
      return notification.SubscribeSafe(observer);
    }
  }
}