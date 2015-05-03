using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every element in the specified 
    /// <paramref name="source"/> sequence, including properties of properties to any arbitrary depth.
    /// </summary>
    /// <typeparam name="TSource">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence containing elements that raise property changed events.</param>
    /// <remarks>
    /// The following property changed notification patterns are supported: 
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <returns>A flattened observable sequence of property changed notifications for every element in the specified 
    /// <paramref name="source"/> sequence.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource>(
      this IObservable<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(maximumDepth: int.MaxValue);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every element in the specified 
    /// <paramref name="source"/> sequence, including properties of properties to the specified maximum depth.
    /// </summary>
    /// <typeparam name="TSource">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence containing elements that raise property changed events.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <remarks>
    /// The following property changed notification patterns are supported: 
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <returns>A flattened observable sequence of property changed notifications for every element in the specified 
    /// <paramref name="source"/> sequence.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource>(
      this IObservable<TSource> source,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return from value in source
             where value != null
             from change in FromPropertyChangedPattern(value, maximumDepth)
             select change;
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every element in the specified 
    /// <paramref name="source"/> sequence, including properties of properties to any arbitrary depth.
    /// </summary>
    /// <typeparam name="TSource">The object that raises property changed events.</typeparam>
    /// <typeparam name="TUntil">Unused data that signals the completion of property changed notifications.</typeparam>
    /// <param name="source">An observable sequence containing elements that raise property changed events.</param>
    /// <param name="takeUntilSelector">A function returning an observable that signals the completion of property changed notifications for the given element.</param>
    /// <remarks>
    /// The following property changed notification patterns are supported: 
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <returns>A flattened observable sequence of property changed notifications for every element in the specified 
    /// <paramref name="source"/> sequence.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TUntil>(
      this IObservable<TSource> source,
      Func<TSource, IObservable<TUntil>> takeUntilSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(takeUntilSelector != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(takeUntilSelector, maximumDepth: int.MaxValue);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every element in the specified 
    /// <paramref name="source"/> sequence, including properties of properties to the specified maximum depth.
    /// </summary>
    /// <typeparam name="TSource">The object that raises property changed events.</typeparam>
    /// <typeparam name="TUntil">Unused data that signals the completion of property changed notifications.</typeparam>
    /// <param name="source">An observable sequence containing elements that raise property changed events.</param>
    /// <param name="takeUntilSelector">A function returning an observable that signals the completion of property changed notifications for the given element.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <remarks>
    /// The following property changed notification patterns are supported: 
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <returns>A flattened observable sequence of property changed notifications for every element in the specified 
    /// <paramref name="source"/> sequence.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TUntil>(
      this IObservable<TSource> source,
      Func<TSource, IObservable<TUntil>> takeUntilSelector,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Requires(takeUntilSelector != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return from value in source
             where value != null
             from change in FromPropertyChangedPattern(value, maximumDepth).TakeUntil(takeUntilSelector(value))
             select change;
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications, including properties of 
    /// properties to any arbitrary depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given element are ignored after receiving a collection notification indicating 
    /// that the element was removed, replaced or cleared, based on the default comparer for <typeparamref name="TSource"/>.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that raise property changed events.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource>(
      this IObservable<CollectionNotification<TSource>> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(new Func<TSource, IObservable<TSource>>(value => Observable.Empty<TSource>()));
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications, including properties of 
    /// properties to the specified maximum depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given element are ignored after receiving a collection notification indicating 
    /// that the element was removed, replaced or cleared, based on the default comparer for <typeparamref name="TSource"/>.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that raise property changed events.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource>(
      this IObservable<CollectionNotification<TSource>> source,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(new Func<TSource, IObservable<TSource>>(value => Observable.Empty<TSource>()), maximumDepth);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications, including properties of 
    /// properties to any arbitrary depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given element are ignored after receiving a collection notification indicating 
    /// that the element was removed, replaced or cleared, based on the specified <paramref name="comparer"/>.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that raise property changed events.</param>
    /// <param name="comparer">An object that compares elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource>(
      this IObservable<CollectionNotification<TSource>> source,
      IEqualityComparer<TSource> comparer)
    {
      Contract.Requires(source != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(value => Observable.Empty<TSource>(), comparer);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications, including properties of 
    /// properties to the specified maximum depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given element are ignored after receiving a collection notification indicating 
    /// that the element was removed, replaced or cleared, based on the specified <paramref name="comparer"/>.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that raise property changed events.</param>
    /// <param name="comparer">An object that compares elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource>(
      this IObservable<CollectionNotification<TSource>> source,
      IEqualityComparer<TSource> comparer,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(comparer != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(value => Observable.Empty<TSource>(), comparer, maximumDepth);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and dependent objects returned
    /// by the specified <paramref name="selector"/> function, including properties of properties to any arbitrary depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the default comparer for <typeparamref name="TSource"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return an object 
    /// that raises property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// object's property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get an object
    /// that raises property changed events.  A dependency is created between the returned object and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from either 
    /// object will be ignored.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and dependent objects returned
    /// by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, TNotifier> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(value => Observable.Return(selector(value)));
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and dependent objects returned
    /// by the specified <paramref name="selector"/> function, including properties of properties to the specified maximum depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the default comparer for <typeparamref name="TSource"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return an object 
    /// that raises property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// object's property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get an object
    /// that raises property changed events.  A dependency is created between the returned object and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from either 
    /// object will be ignored.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and dependent objects returned
    /// by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, TNotifier> selector,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(value => Observable.Return(selector(value)), maximumDepth);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and dependent objects returned
    /// by the specified <paramref name="selector"/> function, including properties of properties to any arbitrary depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the specified <paramref name="comparer"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return an object 
    /// that raises property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// object's property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get an object
    /// that raises property changed events.  A dependency is created between the returned object and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from either 
    /// object will be ignored.</param>
    /// <param name="comparer">An object that compares elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and dependent objects returned
    /// by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, TNotifier> selector,
      IEqualityComparer<TSource> comparer)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(value => Observable.Return(selector(value)), comparer);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and dependent objects returned
    /// by the specified <paramref name="selector"/> function, including properties of properties to the specified maximum depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the specified <paramref name="comparer"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return an object 
    /// that raises property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// object's property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get an object
    /// that raises property changed events.  A dependency is created between the returned object and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from either 
    /// object will be ignored.</param>
    /// <param name="comparer">An object that compares elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and dependent objects returned
    /// by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, TNotifier> selector,
      IEqualityComparer<TSource> comparer,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(comparer != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(value => Observable.Return(selector(value)), comparer, maximumDepth);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function, including properties of properties to any arbitrary depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the default comparer for <typeparamref name="TSource"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return a sequence 
    /// of objects that raise property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// objects' property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of objects
    /// that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any
    /// of the corresponding objects will be ignored.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, IEnumerable<TNotifier>> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(value => selector(value).ToObservable());
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function, including properties of properties to the specified 
    /// maximum depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the default comparer for <typeparamref name="TSource"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return a sequence 
    /// of objects that raise property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// objects' property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of objects
    /// that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any
    /// of the corresponding objects will be ignored.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, IEnumerable<TNotifier>> selector,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(value => selector(value).ToObservable(), maximumDepth);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function, including properties of properties to any arbitrary depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the specified <paramref name="comparer"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return a sequence 
    /// of objects that raise property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// objects' property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of objects
    /// that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any
    /// of the corresponding objects will be ignored.</param>
    /// <param name="comparer">An object that compares elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, IEnumerable<TNotifier>> selector,
      IEqualityComparer<TSource> comparer)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(value => selector(value).ToObservable(), comparer);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function, including properties of properties to the specified 
    /// maximum depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the specified <paramref name="comparer"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return a sequence 
    /// of objects that raise property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// objects' property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of objects
    /// that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any
    /// of the corresponding objects will be ignored.</param>
    /// <param name="comparer">An object that compares elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, IEnumerable<TNotifier>> selector,
      IEqualityComparer<TSource> comparer,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(comparer != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(value => selector(value).ToObservable(), comparer, maximumDepth);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function, including properties of properties to any arbitrary depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the default comparer for <typeparamref name="TSource"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return a sequence 
    /// of objects that raise property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// objects' property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of objects
    /// that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any
    /// of the corresponding objects will be ignored.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, IObservable<TNotifier>> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(selector, EqualityComparer<TSource>.Default);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function, including properties of properties to the specified 
    /// maximum depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the default comparer for <typeparamref name="TSource"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return a sequence 
    /// of objects that raise property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// objects' property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of objects
    /// that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any
    /// of the corresponding objects will be ignored.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, IObservable<TNotifier>> selector,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(selector, EqualityComparer<TSource>.Default, maximumDepth);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function, including properties of properties to any arbitrary depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the specified <paramref name="comparer"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return a sequence 
    /// of objects that raise property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// objects' property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of objects
    /// that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any
    /// of the corresponding objects will be ignored.</param>
    /// <param name="comparer">An object that compares elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, IObservable<TNotifier>> selector,
      IEqualityComparer<TSource> comparer)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(
        value => value,
        selector,
        dependencySelector: x => x,
        dependencyChangesUntilSelector: (_, __) => Observable.Empty<Unit>(),
        comparer: comparer,
        maximumDepth: int.MaxValue);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function, including properties of properties to the specified 
    /// maximum depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the specified <paramref name="comparer"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return a sequence 
    /// of objects that raise property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// objects' property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of objects
    /// that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any
    /// of the corresponding objects will be ignored.</param>
    /// <param name="comparer">An object that compares elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent objects 
    /// returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, IObservable<TNotifier>> selector,
      IEqualityComparer<TSource> comparer,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(comparer != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(
        value => value,
        selector,
        dependencySelector: x => x,
        dependencyChangesUntilSelector: (_, __) => Observable.Empty<Unit>(),
        comparer: comparer,
        maximumDepth: maximumDepth);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function, including properties of properties to 
    /// any arbitrary depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the default comparers for <typeparamref name="TSource"/>
    /// and <typeparamref name="TDependency"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence 
    /// contain child sequences of collection notifications, in a hierarchical relationship.  The function should return a
    /// sequence of collections notifications containing objects that raise property changed events on behalf of or in addition to 
    /// a given element, implicitly forming a dependency between them so that removal of the element from the <paramref name="source"/> 
    /// or removal of the dependent object from the dependent sequence automatically disconnects the dependent object's property 
    /// changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TDependency">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of collection
    /// notifications containing objects that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any of its dependents will be ignored.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TDependency>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, IObservable<CollectionNotification<TDependency>>> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(selector, EqualityComparer<TSource>.Default, EqualityComparer<TDependency>.Default);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function, including properties of properties to 
    /// the specified maximum depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the default comparers for <typeparamref name="TSource"/>
    /// and <typeparamref name="TDependency"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence 
    /// contain child sequences of collection notifications, in a hierarchical relationship.  The function should return a
    /// sequence of collections notifications containing objects that raise property changed events on behalf of or in addition to 
    /// a given element, implicitly forming a dependency between them so that removal of the element from the <paramref name="source"/> 
    /// or removal of the dependent object from the dependent sequence automatically disconnects the dependent object's property 
    /// changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TDependency">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of collection
    /// notifications containing objects that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any of its dependents will be ignored.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TDependency>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, IObservable<CollectionNotification<TDependency>>> selector,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(selector, EqualityComparer<TSource>.Default, EqualityComparer<TDependency>.Default, maximumDepth);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function, including properties of properties to 
    /// any arbitrary depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the specified <paramref name="sourceComparer"/>
    /// and <paramref name="dependencyComparer"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence 
    /// contain child sequences of collection notifications, in a hierarchical relationship.  The function should return a
    /// sequence of collection notifications containing objects that raise property changed events on behalf of or in addition to 
    /// a given element, implicitly forming a dependency between them so that removal of the element from the <paramref name="source"/> 
    /// or removal of the dependent object from the dependent sequence automatically disconnects the dependent object's property 
    /// changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TDependency">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of collection
    /// notifications containing objects that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any of its dependents will be ignored.</param>
    /// <param name="sourceComparer">An object that compares elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <param name="dependencyComparer">An object that compares <typeparamref name="TDependency"/> elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TDependency>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, IObservable<CollectionNotification<TDependency>>> selector,
      IEqualityComparer<TSource> sourceComparer,
      IEqualityComparer<TDependency> dependencyComparer)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(sourceComparer != null);
      Contract.Requires(dependencyComparer != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(
        value => value,
        selector,
        dependencies => dependencies.ExistingOrAddedOrReplacements(),
        (dependencies, dependency) => dependencies.RemovedOrReplaced().Where(value => dependencyComparer.Equals(value, dependency))
          .Merge(dependencies.Cleared().Select(_ => default(TDependency))),
        sourceComparer,
        maximumDepth: int.MaxValue);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function, including properties of properties to 
    /// the specified maximum depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the specified <paramref name="sourceComparer"/>
    /// and <paramref name="dependencyComparer"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence 
    /// contain child sequences of collection notifications, in a hierarchical relationship.  The function should return a
    /// sequence of collection notifications containing objects that raise property changed events on behalf of or in addition to 
    /// a given element, implicitly forming a dependency between them so that removal of the element from the <paramref name="source"/> 
    /// or removal of the dependent object from the dependent sequence automatically disconnects the dependent object's property 
    /// changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TDependency">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of collection
    /// notifications containing objects that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any of its dependents will be ignored.</param>
    /// <param name="sourceComparer">An object that compares elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <param name="dependencyComparer">An object that compares <typeparamref name="TDependency"/> elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TDependency>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, IObservable<CollectionNotification<TDependency>>> selector,
      IEqualityComparer<TSource> sourceComparer,
      IEqualityComparer<TDependency> dependencyComparer,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(sourceComparer != null);
      Contract.Requires(dependencyComparer != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(
        value => value,
        selector,
        dependencies => dependencies.ExistingOrAddedOrReplacements(),
        (dependencies, dependency) => dependencies.RemovedOrReplaced().Where(value => dependencyComparer.Equals(value, dependency))
          .Merge(dependencies.Cleared().Select(_ => default(TDependency))),
        sourceComparer,
        maximumDepth);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function, including properties of properties to 
    /// any arbitrary depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the default comparers for <typeparamref name="TSource"/>
    /// and <typeparamref name="TDependency"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return an object 
    /// that raises property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// object's property changed events.
    /// </para>
    /// <para>
    /// The <paramref name="dependencySelector"/> function is useful when the objects in the <paramref name="source"/> sequence 
    /// contain child sequences of collection notifications, in a hierarchical relationship.  The function should return a
    /// sequence of collection notifications containing objects that raise property changed events, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> or removal of the dependent object from the 
    /// dependent sequence automatically disconnects the dependent object's property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events on behalf of or instead of an element.</typeparam>
    /// <typeparam name="TDependency">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get an object
    /// that raises property changed events.  A dependency is created between the returned object and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from either 
    /// object will be ignored.</param>
    /// <param name="dependencySelector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of collection
    /// notifications containing objects that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any of its dependents will be ignored.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier, TDependency>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, TNotifier> selector,
      Func<TSource, IObservable<CollectionNotification<TDependency>>> dependencySelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(dependencySelector != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(selector, dependencySelector, EqualityComparer<TSource>.Default, EqualityComparer<TDependency>.Default);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function, including properties of properties to 
    /// the specified maximum depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the default comparers for <typeparamref name="TSource"/>
    /// and <typeparamref name="TDependency"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return an object 
    /// that raises property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// object's property changed events.
    /// </para>
    /// <para>
    /// The <paramref name="dependencySelector"/> function is useful when the objects in the <paramref name="source"/> sequence 
    /// contain child sequences of collection notifications, in a hierarchical relationship.  The function should return a
    /// sequence of collection notifications containing objects that raise property changed events, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> or removal of the dependent object from the 
    /// dependent sequence automatically disconnects the dependent object's property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events on behalf of or instead of an element.</typeparam>
    /// <typeparam name="TDependency">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get an object
    /// that raises property changed events.  A dependency is created between the returned object and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from either 
    /// object will be ignored.</param>
    /// <param name="dependencySelector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of collection
    /// notifications containing objects that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any of its dependents will be ignored.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier, TDependency>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, TNotifier> selector,
      Func<TSource, IObservable<CollectionNotification<TDependency>>> dependencySelector,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(dependencySelector != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(selector, dependencySelector, EqualityComparer<TSource>.Default, EqualityComparer<TDependency>.Default, maximumDepth);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function, including properties of properties to 
    /// any arbitrary depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the specified <paramref name="sourceComparer"/>
    /// and <paramref name="dependencyComparer"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return an object 
    /// that raises property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// object's property changed events.
    /// </para>
    /// <para>
    /// The <paramref name="dependencySelector"/> function is useful when the objects in the <paramref name="source"/> sequence 
    /// contain child sequences of collection notifications, in a hierarchical relationship.  The function should return a
    /// sequence of collection notifications containing objects that raise property changed events, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> or removal of the dependent object from the 
    /// dependent sequence automatically disconnects the dependent object's property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events on behalf of or instead of an element.</typeparam>
    /// <typeparam name="TDependency">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get an object
    /// that raises property changed events.  A dependency is created between the returned object and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from either 
    /// object will be ignored.</param>
    /// <param name="dependencySelector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of collection
    /// notifications containing objects that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any of its dependents will be ignored.</param>
    /// <param name="sourceComparer">An object that compares elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <param name="dependencyComparer">An object that compares <typeparamref name="TDependency"/> elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier, TDependency>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, TNotifier> selector,
      Func<TSource, IObservable<CollectionNotification<TDependency>>> dependencySelector,
      IEqualityComparer<TSource> sourceComparer,
      IEqualityComparer<TDependency> dependencyComparer)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(dependencySelector != null);
      Contract.Requires(sourceComparer != null);
      Contract.Requires(dependencyComparer != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(
        selector,
        dependencySelector,
        dependencies => dependencies.ExistingOrAddedOrReplacements(),
        (dependencies, dependency) => dependencies.RemovedOrReplaced().Where(value => dependencyComparer.Equals(value, dependency))
          .Merge(dependencies.Cleared().Select(_ => default(TDependency))),
        sourceComparer,
        maximumDepth: int.MaxValue);
    }

    /// <summary>
    /// Returns a flattened observable sequence of property changed notifications for every existing, added and replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function, including properties of properties to 
    /// the specified maximum depth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Property changed notifications for a given object are ignored after receiving a collection notification indicating 
    /// that a corresponding element was removed, replaced or cleared, based on the specified <paramref name="sourceComparer"/>
    /// and <paramref name="dependencyComparer"/>.
    /// </para>
    /// <para>
    /// The <paramref name="selector"/> function is useful when the objects in the <paramref name="source"/> sequence do not 
    /// raise property changed events themselves, although they are permitted to as well.  The function should return an object 
    /// that raises property changed events on behalf of or in addition to a given element, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> automatically disconnects the dependent 
    /// object's property changed events.
    /// </para>
    /// <para>
    /// The <paramref name="dependencySelector"/> function is useful when the objects in the <paramref name="source"/> sequence 
    /// contain child sequences of collection notifications, in a hierarchical relationship.  The function should return a
    /// sequence of collection notifications containing objects that raise property changed events, implicitly forming a dependency 
    /// between them so that removal of the element from the <paramref name="source"/> or removal of the dependent object from the 
    /// dependent sequence automatically disconnects the dependent object's property changed events.
    /// </para>
    /// <para>
    /// The following property changed notification patterns are supported: 
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TSource">The object that possibly raises property changed events.</typeparam>
    /// <typeparam name="TNotifier">The object that raises property changed events on behalf of or instead of an element.</typeparam>
    /// <typeparam name="TDependency">The object that raises property changed events.</typeparam>
    /// <param name="source">An observable sequence of collection notifications containing new and existing elements that possibly raise property changed events.</param>
    /// <param name="selector">A function that is called for every element in the <paramref name="source"/> sequence to get an object
    /// that raises property changed events.  A dependency is created between the returned object and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from either 
    /// object will be ignored.</param>
    /// <param name="dependencySelector">A function that is called for every element in the <paramref name="source"/> sequence to get a sequence of collection
    /// notifications containing objects that raise property changed events.  A dependency is created between the returned objects and the given element so that 
    /// when the given element receives a removed, replaced or cleared notification, any future property changed events from any of its dependents will be ignored.</param>
    /// <param name="sourceComparer">An object that compares elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <param name="dependencyComparer">An object that compares <typeparamref name="TDependency"/> elements for equality to determine whether a removed or replaced
    /// notification corresponds to a given element so that future property changed events may be ignored.</param>
    /// <param name="maximumDepth">The maximum depth in each element's property hierarchy at which to subscribe to property changed events.
    /// Specify <em>0</em> to only listen for property changes on each element.</param>
    /// <returns>A flattened observable sequence of property changed notifications for every existing, added or replacement
    /// element in the specified <paramref name="source"/> sequence of collection notifications and any dependent collection 
    /// notifications returned by the specified <paramref name="selector"/> function.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier, TDependency>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, TNotifier> selector,
      Func<TSource, IObservable<CollectionNotification<TDependency>>> dependencySelector,
      IEqualityComparer<TSource> sourceComparer,
      IEqualityComparer<TDependency> dependencyComparer,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(dependencySelector != null);
      Contract.Requires(sourceComparer != null);
      Contract.Requires(dependencyComparer != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.PropertyChanges(
        selector,
        dependencySelector,
        dependencies => dependencies.ExistingOrAddedOrReplacements(),
        (dependencies, dependency) => dependencies.RemovedOrReplaced().Where(value => dependencyComparer.Equals(value, dependency))
          .Merge(dependencies.Cleared().Select(_ => default(TDependency))),
        sourceComparer,
        maximumDepth);
    }

    private static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChanges<TSource, TNotifier, TIntermediate, TDependency, TUntil>(
      this IObservable<CollectionNotification<TSource>> source,
      Func<TSource, TNotifier> selector,
      Func<TSource, IObservable<TIntermediate>> intermediateSelector,
      Func<IObservable<TIntermediate>, IObservable<TDependency>> dependencySelector,
      Func<IObservable<TIntermediate>, TDependency, IObservable<TUntil>> dependencyChangesUntilSelector,
      IEqualityComparer<TSource> comparer,
      int maximumDepth)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Requires(intermediateSelector != null);
      Contract.Requires(dependencySelector != null);
      Contract.Requires(dependencyChangesUntilSelector != null);
      Contract.Requires(comparer != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return source.Publish(published =>
        from value in published.ExistingOrAddedOrReplacements()
        let valueReleased = published.RemovedOrReplaced().Where(released => comparer.Equals(released, value))
          .Merge(published.Cleared().Select(_ => default(TSource)))
          .Select(_ => default(TUntil))
        let dependencyChanges = intermediateSelector(value).Publish(dependenciesPublished =>
          dependencySelector(dependenciesPublished).PropertyChanges(
            dependency => valueReleased.Merge(dependencyChangesUntilSelector(dependenciesPublished, dependency)),
            maximumDepth))
        let notifier = selector(value)
        from change in
          notifier == null
          ? dependencyChanges
          : Scalar.Return(notifier).PropertyChanges(_ => valueReleased, maximumDepth).Merge(dependencyChanges)
        select change);
    }
  }
}