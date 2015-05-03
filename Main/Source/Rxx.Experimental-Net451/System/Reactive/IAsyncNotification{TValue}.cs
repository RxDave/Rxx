using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace System.Reactive
{
  /// <summary>
  /// Represents an asynchronous notification to an observer.
  /// </summary>
  /// <typeparam name="TValue">The type of the notification information.</typeparam>
  [ContractClass(typeof(IAsyncNotificationContract<>))]
  public interface IAsyncNotification<out TValue> : IAsyncNotification
  {
    /// <summary>
    /// Gets the object that provides information to observers.
    /// </summary>
    TValue Value { get; }
  }

  [ContractClassFor(typeof(IAsyncNotification<>))]
  internal abstract class IAsyncNotificationContract<TValue> : IAsyncNotification<TValue>
  {
    public TValue Value
    {
      get;
      private set;
    }

    public Task Task
    {
      get
      {
        return null;
      }
    }

    public CancellationToken Cancel
    {
      get;
      private set;
    }

    public bool TrySetCompleted()
    {
      return false;
    }

    public bool TrySetException(Exception error)
    {
      return false;
    }

    public bool TrySetCanceled()
    {
      return false;
    }
  }
}