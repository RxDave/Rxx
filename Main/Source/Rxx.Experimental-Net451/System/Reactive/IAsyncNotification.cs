using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace System.Reactive
{
  /// <summary>
  /// Represents an asynchronous notification to an observer.
  /// </summary>
  [ContractClass(typeof(IAsyncNotificationContract))]
  public interface IAsyncNotification
  {
    /// <summary>
    /// Gets a <see cref="Task"/> that is signaled when <see cref="TrySetCompleted"/>, <see cref="TrySetException"/> or <see cref="TrySetCanceled"/> is called.
    /// </summary>
    Task Task { get; }

    /// <summary>
    /// Gets an object that signals when the observable has cancelled the notification.
    /// </summary>
    CancellationToken Cancel { get; }

    /// <summary>
    /// Attempts to transition the <see cref="Task"/> into the <see cref="TaskStatus.RanToCompletion"/> state.
    /// </summary>
    /// <returns><see langword="True"/> if the operation was successful; otherwise, <see langword="false"/>.</returns>
    bool TrySetCompleted();

    /// <summary>
    /// Attempts to transition the <see cref="Task"/> into the <see cref="TaskStatus.Faulted"/> state.
    /// </summary>
    /// <param name="error">The error to be associated with the <see cref="Task"/>.</param>
    /// <returns><see langword="True"/> if the operation was successful; otherwise, <see langword="false"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error",
      Justification = "Matches the parameter name in Rx's Notification.CreateOnError and IObserver<T>.OnError methods.")]
    bool TrySetException(Exception error);

    /// <summary>
    /// Attempts to transition the <see cref="Task"/> into the <see cref="TaskStatus.Canceled"/> state.
    /// </summary>
    /// <returns><see langword="True"/> if the operation was successful; otherwise, <see langword="false"/>.</returns>
    bool TrySetCanceled();
  }

  [ContractClassFor(typeof(IAsyncNotification))]
  internal abstract class IAsyncNotificationContract : IAsyncNotification
  {
    public Task Task
    {
      get
      {
        Contract.Ensures(Contract.Result<Task>() != null);
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
      Contract.Requires(error != null);
      return false;
    }

    public bool TrySetCanceled()
    {
      return false;
    }
  }
}