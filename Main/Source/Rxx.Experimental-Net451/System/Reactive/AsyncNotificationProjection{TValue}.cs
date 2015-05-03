using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace System.Reactive
{
  internal sealed class AsyncNotificationProjection<TValue> : IAsyncNotification<TValue>
  {
    public TValue Value
    {
      get
      {
        return value;
      }
    }

    public Task Task
    {
      get
      {
        return notification.Task;
      }
    }

    public CancellationToken Cancel
    {
      get
      {
        return notification.Cancel;
      }
    }

    private readonly IAsyncNotification notification;
    private TValue value;

    public AsyncNotificationProjection(TValue value, IAsyncNotification notification)
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

    public bool TrySetException(Exception error)
    {
      return notification.TrySetException(error);
    }

    public bool TrySetCanceled()
    {
      return notification.TrySetCanceled();
    }
  }
}