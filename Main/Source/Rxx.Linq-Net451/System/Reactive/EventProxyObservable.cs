using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
#if WINDOWS_PHONE || PORT_45
using System.Reflection;
#endif

namespace System.Reactive
{
  internal sealed class EventProxyObservable : IObservable<EventPattern<EventArgs>>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private readonly IObservable<EventPattern<EventArgs>> observable;
    #endregion

    #region Constructors
    public EventProxyObservable(object source, EventDescriptor @event)
    {
      Contract.Requires(source != null);
      Contract.Requires(@event != null);

#if WINDOWS_PHONE || PORT_45
      var onEvent = GetType().GetTypeInfo().GetDeclaredMethod("OnEvent").CreateDelegate(
        @event.EventType,
        this);
#else
      var onEvent = Delegate.CreateDelegate(
        @event.EventType,
        this,
        GetType().GetMethod("OnEvent"));
#endif

      observable = Observable.FromEventPattern<EventHandler, EventArgs>(
        handler => handler.Invoke,
        handler =>
        {
          Proxy += handler;
          @event.AddEventHandler(source, onEvent);
        },
        handler =>
        {
          Proxy -= handler;
          @event.RemoveEventHandler(source, onEvent);
        });
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(observable != null);
    }

    public IDisposable Subscribe(IObserver<EventPattern<EventArgs>> observer)
    {
      return observable.SubscribeSafe(observer);
    }

    public void OnEvent(object sender, EventArgs e)
    {
      var proxy = Proxy;

      if (proxy != null)
        proxy(sender, e);
    }
    #endregion

    #region Events
    private event EventHandler Proxy;
    #endregion
  }
}