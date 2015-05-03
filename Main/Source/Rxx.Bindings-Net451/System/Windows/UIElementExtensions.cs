using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
#if UNIVERSAL
using System.Reflection;
using Windows.UI.Xaml;
#endif

namespace System.Windows
{
  /// <summary>
  /// Provides <see langword="static" /> methods for creating observables from <see cref="UIElement"/> objects.
  /// </summary>
  public static class UIElementExtensions
  {
#if !SILVERLIGHT
    /// <summary>
    /// Gets an observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.
    /// </summary>
    /// <param name="element">The <see cref="UIElement"/> on which to listen for the specified <paramref name="event"/>.</param>
    /// <param name="event">The <see cref="RoutedEvent"/> that is raised.</param>
    /// <returns>An observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.</returns>
    public static IObservable<EventPattern<RoutedEventArgs>> RoutedEventRaised(
      this UIElement element,
      RoutedEvent @event)
    {
      Contract.Requires(element != null);
      Contract.Requires(@event != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<RoutedEventArgs>>>() != null);

      return RoutedEventRaised<RoutedEventArgs>(element, @event, handledEventsToo: false);
    }

    /// <summary>
    /// Gets an observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.
    /// </summary>
    /// <param name="element">The <see cref="UIElement"/> on which to listen for the specified <paramref name="event"/>.</param>
    /// <param name="event">The <see cref="RoutedEvent"/> that is raised.</param>
    /// <param name="handledEventsToo"><see langword="true"/> to include events marked handled in their event data; 
    /// <see langword="false"/> to exclude routed events that are already marked handled.</param>
    /// <returns>An observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.</returns>
    public static IObservable<EventPattern<RoutedEventArgs>> RoutedEventRaised(
      this UIElement element,
      RoutedEvent @event,
      bool handledEventsToo)
    {
      Contract.Requires(element != null);
      Contract.Requires(@event != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<RoutedEventArgs>>>() != null);

      return RoutedEventRaised<RoutedEventArgs>(element, @event, handledEventsToo);
    }

    /// <summary>
    /// Gets an observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of the object containing the routed event's arguments.</typeparam>
    /// <param name="element">The <see cref="UIElement"/> on which to listen for the specified <paramref name="event"/>.</param>
    /// <param name="event">The <see cref="RoutedEvent"/> that is raised.</param>
    /// <returns>An observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.</returns>
    public static IObservable<EventPattern<TEventArgs>> RoutedEventRaised<TEventArgs>(
      this UIElement element,
      RoutedEvent @event)
      where TEventArgs : RoutedEventArgs
    {
      Contract.Requires(element != null);
      Contract.Requires(@event != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<TEventArgs>>>() != null);

      return RoutedEventRaised<TEventArgs>(element, @event, handledEventsToo: false);
    }

    /// <summary>
    /// Gets an observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of the object containing the routed event's arguments.</typeparam>
    /// <param name="element">The <see cref="UIElement"/> on which to listen for the specified <paramref name="event"/>.</param>
    /// <param name="event">The <see cref="RoutedEvent"/> that is raised.</param>
    /// <param name="handledEventsToo"><see langword="true"/> to include events marked handled in their event data; 
    /// <see langword="false"/> to exclude routed events that are already marked handled.</param>
    /// <returns>An observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.</returns>
    public static IObservable<EventPattern<TEventArgs>> RoutedEventRaised<TEventArgs>(
      this UIElement element,
      RoutedEvent @event,
      bool handledEventsToo)
      where TEventArgs : RoutedEventArgs
    {
      Contract.Requires(element != null);
      Contract.Requires(@event != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<TEventArgs>>>() != null);

      return Observable.Create<EventPattern<TEventArgs>>(
        observer =>
        {
          RoutedEventHandler handler = (sender, e) => observer.OnNext(new EventPattern<TEventArgs>(sender, (TEventArgs)e));

          element.AddHandler(@event, handler, handledEventsToo);

          return Disposable.Create(() => element.RemoveHandler(@event, handler));
        });
    }
#else
    /// <summary>
    /// Gets an observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.
    /// </summary>
    /// <param name="element">The <see cref="UIElement"/> on which to listen for the specified <paramref name="event"/>.</param>
    /// <param name="event">The <see cref="RoutedEvent"/> that is raised.</param>
    /// <returns>An observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
      Justification = "Similar to FromEventPattern.")]
#if UNIVERSAL
    [CLSCompliant(false)]
#endif
    public static IObservable<EventPattern<RoutedEventArgs>> RoutedEventRaised(
      this UIElement element,
      RoutedEvent @event)
    {
      Contract.Requires(element != null);
      Contract.Requires(@event != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<RoutedEventArgs>>>() != null);

      return RoutedEventRaised<RoutedEventHandler, RoutedEventArgs>(element, @event, handledEventsToo: false);
    }

    /// <summary>
    /// Gets an observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.
    /// </summary>
    /// <param name="element">The <see cref="UIElement"/> on which to listen for the specified <paramref name="event"/>.</param>
    /// <param name="event">The <see cref="RoutedEvent"/> that is raised.</param>
    /// <param name="handledEventsToo"><see langword="true"/> to include events marked handled in their event data; 
    /// <see langword="false"/> to exclude routed events that are already marked handled.</param>
    /// <returns>An observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
      Justification = "Similar to FromEventPattern.")]
#if UNIVERSAL
    [CLSCompliant(false)]
#endif
    public static IObservable<EventPattern<RoutedEventArgs>> RoutedEventRaised(
      this UIElement element,
      RoutedEvent @event,
      bool handledEventsToo)
    {
      Contract.Requires(element != null);
      Contract.Requires(@event != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<RoutedEventArgs>>>() != null);

      return RoutedEventRaised<RoutedEventHandler, RoutedEventArgs>(element, @event, handledEventsToo);
    }

    /// <summary>
    /// Gets an observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.
    /// </summary>
    /// <typeparam name="TDelegate">Type of the delegate used by the routed event.</typeparam>
    /// <typeparam name="TEventArgs">Type of the object containing the routed event's arguments.</typeparam>
    /// <param name="element">The <see cref="UIElement"/> on which to listen for the specified <paramref name="event"/>.</param>
    /// <param name="event">The <see cref="RoutedEvent"/> that is raised.</param>
    /// <returns>An observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
      Justification = "Similar to FromEventPattern.")]
#if UNIVERSAL
    [CLSCompliant(false)]
#endif
    public static IObservable<EventPattern<TEventArgs>> RoutedEventRaised<TDelegate, TEventArgs>(
      this UIElement element,
      RoutedEvent @event)
      where TEventArgs : RoutedEventArgs
    {
      Contract.Requires(element != null);
      Contract.Requires(@event != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<TEventArgs>>>() != null);

      return RoutedEventRaised<TDelegate, TEventArgs>(element, @event, handledEventsToo: false);
    }

    /// <summary>
    /// Gets an observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.
    /// </summary>
    /// <typeparam name="TDelegate">Type of the delegate used by the routed event.</typeparam>
    /// <typeparam name="TEventArgs">Type of the object containing the routed event's arguments.</typeparam>
    /// <param name="element">The <see cref="UIElement"/> on which to listen for the specified <paramref name="event"/>.</param>
    /// <param name="event">The <see cref="RoutedEvent"/> that is raised.</param>
    /// <param name="handledEventsToo"><see langword="true"/> to include events marked handled in their event data; 
    /// <see langword="false"/> to exclude routed events that are already marked handled.</param>
    /// <returns>An observable sequence of event notifications for the specified <see cref="RoutedEvent"/>, 
    /// raised by the specified <see cref="UIElement"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
      Justification = "Similar to FromEventPattern.")]
#if UNIVERSAL
    [CLSCompliant(false)]
#endif
    public static IObservable<EventPattern<TEventArgs>> RoutedEventRaised<TDelegate, TEventArgs>(
      this UIElement element,
      RoutedEvent @event,
      bool handledEventsToo)
      where TEventArgs : RoutedEventArgs
    {
      Contract.Requires(element != null);
      Contract.Requires(@event != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<TEventArgs>>>() != null);

      return Observable.Create<EventPattern<TEventArgs>>(
        observer =>
        {
          RoutedEventHandler handler = (sender, e) => observer.OnNext(new EventPattern<TEventArgs>(sender, (TEventArgs)e));

#if UNIVERSAL
          var d = handler.GetMethodInfo().CreateDelegate(typeof(TDelegate), handler.Target);
#else
          var d = Delegate.CreateDelegate(typeof(TDelegate), handler.Target, handler.Method);
#endif

          /* Silverlight throws an ArgumentException with the word "handler" as the message if the handler is 
           * not the exact type used by the specified routed event.  Therefore, it is necessary for this extension
           * method to have a TDelegate type parameter so that it can create the proper handler type via reflection.
           * 
           * TODO: Already tested for WP 7.1 but not tested yet for WP 8.0, so TDelegate might not be required anymore.
           */
          element.AddHandler(@event, d, handledEventsToo);

          return Disposable.Create(() => element.RemoveHandler(@event, d));
        });
    }
#endif
  }
}