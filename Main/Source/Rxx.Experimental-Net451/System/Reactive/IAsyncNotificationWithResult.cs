using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace System.Reactive
{
  /// <summary>
  /// Represents an asynchronous notification to an observer with a result.
  /// </summary>
  /// <typeparam name="TResult">The type of the asynchronous result.</typeparam>
  [ContractClass(typeof(IAsyncNotificationWithResultContract<>))]
  public interface IAsyncNotificationWithResult<TResult> : IAsyncNotification, IObservable<TResult>
  {
    /// <summary>
    /// Gets a <see cref="System.Threading.Tasks.Task{TResult}"/> that is signaled when <see cref="TrySetCompleted"/>, <see cref="IAsyncNotification.TrySetException"/>
    /// or <see cref="IAsyncNotification.TrySetCanceled"/> is called.
    /// </summary>
    new Task<TResult> Task { get; }

    /// <summary>
    /// Attempts to transition the <see cref="Task"/> into the <see cref="TaskStatus.RanToCompletion"/> state.
    /// </summary>
    /// <param name="result">The result value to bind to the <see cref="Task"/>.</param>
    /// <returns><see langword="True"/> if the operation was successful; otherwise, <see langword="false"/>.</returns>
    bool TrySetCompleted(TResult result);
  }

  [ContractClassFor(typeof(IAsyncNotificationWithResult<>))]
  internal abstract class IAsyncNotificationWithResultContract<TResult> : IAsyncNotificationWithResult<TResult>
  {
    public Task<TResult> Task
    {
      get
      {
        Contract.Ensures(Contract.Result<Task<TResult>>() != null);
        return null;
      }
    }

    Task IAsyncNotification.Task
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

    public bool TrySetCompleted(TResult result)
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

    public IDisposable Subscribe(IObserver<TResult> observer)
    {
      return null;
    }
  }
}