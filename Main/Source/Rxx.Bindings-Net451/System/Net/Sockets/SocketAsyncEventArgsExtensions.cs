using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace System.Net.Sockets
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for instances of the <see cref="SocketAsyncEventArgs"/> class.
  /// </summary>
  public static class SocketAsyncEventArgsExtensions
  {
    /// <summary>
    /// Returns a connectable observable that notifies observers when the next asynchronous socket operation has completed.
    /// </summary>
    /// <param name="eventArgs">The <see cref="SocketAsyncEventArgs"/> object containing the results of the asynchronous 
    /// socket operation.</param>
    /// <returns>A singleton observable sequence containing the specified <see cref="SocketAsyncEventArgs"/> object
    /// when the next asynchronous socket operation completes.</returns>
    public static IConnectableObservable<SocketAsyncEventArgs> CompletedObservable(this SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      return Observable.FromEventPattern<SocketAsyncEventArgs>(
        eh => eventArgs.Completed += eh,
        eh => eventArgs.Completed -= eh)
        .SelectMany(e => GetResult(e.EventArgs))
        .Take(1)
        .PublishLast();
    }

    internal static IObservable<SocketAsyncEventArgs> InvokeAsync(this SocketAsyncEventArgs eventArgs, Func<SocketAsyncEventArgs, bool> invoke)
    {
      Contract.Requires(eventArgs != null);
      Contract.Requires(invoke != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      var completed = eventArgs.CompletedObservable();

      var connection = completed.Connect();

      if (invoke(eventArgs))
      {
        return completed.AsObservable();
      }
      else
      {
        connection.Dispose();

        return eventArgs.GetResult();
      }
    }

    private static IObservable<SocketAsyncEventArgs> GetResult(this SocketAsyncEventArgs eventArgs)
    {
      Contract.Requires(eventArgs != null);
      Contract.Ensures(Contract.Result<IObservable<SocketAsyncEventArgs>>() != null);

      if (eventArgs.LastOperation == SocketAsyncOperation.Connect && eventArgs.ConnectByNameError != null)
      {
        return Observable.Throw<SocketAsyncEventArgs>(eventArgs.ConnectByNameError);
      }
      else if (eventArgs.SocketError != SocketError.Success)
      {
        return Observable.Throw<SocketAsyncEventArgs>(new SocketException((int)eventArgs.SocketError));
      }
      else
      {
        return Observable.Return(eventArgs);
      }
    }
  }
}