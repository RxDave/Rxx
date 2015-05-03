using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.ServiceModel
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for instances of <see cref="ICommunicationObject"/>.
  /// </summary>
  public static class CommunicationObjectExtensions
  {
    /// <summary>
    /// Returns an observable sequence of state changes for the <see cref="ICommunicationObject"/>.
    /// </summary>
    /// <param name="obj">An object that transitions between states.</param>
    /// <returns>An observable sequence that contains <see cref="CommunicationState"/> values representing the 
    /// states of the <see cref="ICommunicationObject"/> as it transitions between them.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/ms789041.aspx"/>
    public static IObservable<CommunicationState> AsObservable(this ICommunicationObject obj)
    {
      Contract.Requires(obj != null);
      Contract.Ensures(Contract.Result<IObservable<CommunicationState>>() != null);

      return Observable.Defer(() =>
        {
          var closed = obj.ClosedObservable().PublishLast().RefCount();
          var closing = obj.ClosingObservable().TakeUntil(closed).PublishLast().RefCount();
          var faulted = obj.FaultedObservable().TakeUntil(closing.Amb(closed)).PublishLast().RefCount();
          var terminatation = closing.Amb(closed).Amb(faulted);
          var opened = obj.OpenedObservable().TakeUntil(terminatation).PublishLast().RefCount();
          var opening = obj.OpeningObservable().TakeUntil(opened.Amb(terminatation));
          var created = obj.CreatedObservable();

          return faulted.Merge(created.Concat(opening).Concat(opened).Concat(closing).Concat(closed));
        });
    }

    /// <summary>
    /// Returns an observable sequence of terminating state changes for the <see cref="ICommunicationObject"/>.
    /// </summary>
    /// <param name="obj">An object that transitions between states.</param>
    /// <returns>An observable sequence that contains <see cref="CommunicationState"/> values representing the 
    /// terminating states (<see cref="CommunicationState.Closing"/>, <see cref="CommunicationState.Closed"/> 
    /// and <see cref="CommunicationState.Faulted"/>) of the <see cref="ICommunicationObject"/> as it transitions
    /// between them.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/ms789041.aspx"/>
    public static IObservable<CommunicationState> TerminationObservable(this ICommunicationObject obj)
    {
      Contract.Requires(obj != null);
      Contract.Ensures(Contract.Result<IObservable<CommunicationState>>() != null);

      return Observable.Defer(() =>
        {
          var closed = obj.ClosedObservable().PublishLast().RefCount();
          var closing = obj.ClosingObservable().TakeUntil(closed).PublishLast().RefCount();
          var faulted = obj.FaultedObservable().TakeUntil(closing.Amb(closed)).PublishLast().RefCount();

          return faulted.Merge(closing.Concat(closed));
        });
    }

    private static IObservable<CommunicationState> CreatedObservable(this ICommunicationObject obj)
    {
      Contract.Requires(obj != null);
      Contract.Ensures(Contract.Result<IObservable<CommunicationState>>() != null);

      if (obj.State == CommunicationState.Created)
      {
        return Observable.Return(CommunicationState.Created);
      }
      else
      {
        return Observable.Empty<CommunicationState>();
      }
    }

    private static IObservable<CommunicationState> OpeningObservable(this ICommunicationObject obj)
    {
      Contract.Requires(obj != null);
      Contract.Ensures(Contract.Result<IObservable<CommunicationState>>() != null);

      return obj.CreateStateObservable(
        CommunicationState.Opening,
        eh => obj.Opening += eh,
        eh => obj.Opening -= eh);
    }

    private static IObservable<CommunicationState> OpenedObservable(this ICommunicationObject obj)
    {
      Contract.Requires(obj != null);
      Contract.Ensures(Contract.Result<IObservable<CommunicationState>>() != null);

      return obj.CreateStateObservable(
        CommunicationState.Opened,
        eh => obj.Opened += eh,
        eh => obj.Opened -= eh);
    }

    private static IObservable<CommunicationState> ClosingObservable(this ICommunicationObject obj)
    {
      Contract.Requires(obj != null);
      Contract.Ensures(Contract.Result<IObservable<CommunicationState>>() != null);

      if (obj.State == CommunicationState.Closed)
      {
        return Observable.Empty<CommunicationState>();
      }
      else
      {
        return obj.CreateStateObservable(
          CommunicationState.Closing,
          eh => obj.Closing += eh,
          eh => obj.Closing -= eh,
          useEventWhenPassedState: true);
      }
    }

    private static IObservable<CommunicationState> ClosedObservable(this ICommunicationObject obj)
    {
      Contract.Requires(obj != null);
      Contract.Ensures(Contract.Result<IObservable<CommunicationState>>() != null);

      return obj.CreateStateObservable(
        CommunicationState.Closed,
        eh => obj.Closed += eh,
        eh => obj.Closed -= eh,
        useEventWhenPassedState: true);
    }

    private static IObservable<CommunicationState> FaultedObservable(this ICommunicationObject obj)
    {
      Contract.Requires(obj != null);
      Contract.Ensures(Contract.Result<IObservable<CommunicationState>>() != null);

      if (obj.State == CommunicationState.Closing || obj.State == CommunicationState.Closed)
      {
        return Observable.Empty<CommunicationState>();
      }
      else
      {
        return obj.CreateStateObservable(
          CommunicationState.Faulted,
          eh => obj.Faulted += eh,
          eh => obj.Faulted -= eh);
      }
    }

    private static IObservable<CommunicationState> CreateStateObservable(
      this ICommunicationObject obj,
      CommunicationState state,
      Action<EventHandler> addHandler,
      Action<EventHandler> removeHandler,
      bool useEventWhenPassedState = false)
    {
      Contract.Requires(obj != null);
      Contract.Requires(addHandler != null);
      Contract.Requires(removeHandler != null);
      Contract.Ensures(Contract.Result<IObservable<CommunicationState>>() != null);

      if (obj.State == state)
      {
        return Observable.Return(state);
      }
      else if (useEventWhenPassedState || obj.State < state)
      {
        return Observable
          .FromEventPattern(addHandler, removeHandler)
          .Select(_ => state)
          .Take(1);
      }
      else
      {
        return Observable.Empty<CommunicationState>();
      }
    }
  }
}