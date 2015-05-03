#pragma warning disable 0436
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;
using Rxx.Linq.Properties;

namespace System.ComponentModel
{
  /// <summary>
  /// Provides extension methods for <see cref="PropertyDescriptor"/> and <see cref="EventDescriptor"/>.
  /// </summary>
#if !SILVERLIGHT && !PORT_45 && !PORT_40
  public static class MemberDescriptorExtensions
#else
  internal static class MemberDescriptorExtensions
#endif
  {
    /// <summary>
    /// Returns an observable sequence of property changed notifications from the 
    /// specified <paramref name="property"/> descriptor.
    /// </summary>
    /// <param name="property">The descriptor from which to create an observable sequence of changed notifications.</param>
    /// <param name="source">The object to which the <paramref name="property"/> belongs.</param>
    /// <returns>An observable sequence of property changed notifications.</returns>
    /// <exception cref="ArgumentException">The specified property does not support change events.</exception>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanged(
      this PropertyDescriptor property,
      object source)
    {
      Contract.Requires(property != null);
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      if (!property.SupportsChangeEvents)
        throw new ArgumentException(Errors.PropertyDoesNotSupportChangeEvents, "property");

      return
        from e in Observable.FromEventPattern<EventHandler, EventArgs>(
          handler => handler.Invoke,
          handler => property.AddValueChanged(source, handler),
          handler => property.RemoveValueChanged(source, handler))
        select new EventPattern<PropertyChangedEventArgs>(
          e.Sender,
          e.EventArgs as PropertyChangedEventArgs ?? new PropertyChangedEventArgs(property.Name));
    }

    /// <summary>
    /// Returns an observable sequence of events from the specified <paramref name="event"/> descriptor.
    /// </summary>
    /// <param name="event">The descriptor from which to create an observable sequence of changed notifications.</param>
    /// <param name="source">The object to which the <paramref name="event"/> belongs.</param>
    /// <returns>An observable sequence of events.</returns>
    public static IObservable<EventPattern<EventArgs>> EventRaised(
      this EventDescriptor @event,
      object source)
    {
      Contract.Requires(@event != null);
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<EventArgs>>>() != null);

      return @event.EventType == typeof(EventHandler)
        ? Observable.FromEventPattern<EventHandler, EventArgs>(
            handler => handler.Invoke,
            handler => @event.AddEventHandler(source, handler),
            handler => @event.RemoveEventHandler(source, handler))
        : new EventProxyObservable(source, @event);
    }
  }
}