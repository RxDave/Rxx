using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;

namespace System
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for <see cref="IObservable{T}"/> regarding <see cref="CollectionNotification{T}"/>.
  /// </summary>
  public static partial class CollectionObservableExtensions
  {
    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="onAdded">The handler for <see cref="CollectionNotificationKind.OnAdded"/> or <see cref="CollectionNotificationKind.OnReplaced"/> notifications.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, Action<TSource> onAdded)
    {
      Contract.Requires(source != null);
      Contract.Requires(onAdded != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(n => n.Accept(_ => { }, onAdded, (added, __) => onAdded(added), _ => { }, () => { }));
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="onAdded">The handler for <see cref="CollectionNotificationKind.OnAdded"/> or <see cref="CollectionNotificationKind.OnReplaced"/> notifications.</param>
    /// <param name="onRemoved">The handler for <see cref="CollectionNotificationKind.OnRemoved"/> or <see cref="CollectionNotificationKind.OnReplaced"/> notifications.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, Action<TSource> onAdded, Action<TSource> onRemoved)
    {
      Contract.Requires(source != null);
      Contract.Requires(onAdded != null);
      Contract.Requires(onRemoved != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(n => n.Accept(_ => { }, onAdded, (added, removed) => { onAdded(added); onRemoved(removed); }, onRemoved, () => { }));
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="exists">The handler for <see cref="CollectionNotificationKind.Exists"/> notifications.</param>
    /// <param name="onAdded">The handler for <see cref="CollectionNotificationKind.OnAdded"/> notifications.</param>
    /// <param name="onReplaced">The handler for <see cref="CollectionNotificationKind.OnReplaced"/> notifications.</param>
    /// <param name="onRemoved">The handler for <see cref="CollectionNotificationKind.OnRemoved"/> notifications.</param>
    /// <param name="onCleared">The handler for <see cref="CollectionNotificationKind.OnCleared"/> notifications.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, Action<IList<TSource>> exists, Action<TSource> onAdded, Action<TSource, TSource> onReplaced, Action<TSource> onRemoved, Action onCleared)
    {
      Contract.Requires(source != null);
      Contract.Requires(exists != null);
      Contract.Requires(onAdded != null);
      Contract.Requires(onReplaced != null);
      Contract.Requires(onRemoved != null);
      Contract.Requires(onCleared != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(n => n.Accept(exists, onAdded, onReplaced, onRemoved, onCleared));
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="exists">The handler for <see cref="CollectionNotificationKind.Exists"/> notifications.</param>
    /// <param name="onAdded">The handler for <see cref="CollectionNotificationKind.OnAdded"/> notifications.</param>
    /// <param name="onReplaced">The handler for <see cref="CollectionNotificationKind.OnReplaced"/> notifications.</param>
    /// <param name="onRemoved">The handler for <see cref="CollectionNotificationKind.OnRemoved"/> notifications.</param>
    /// <param name="onCleared">The handler for <see cref="CollectionNotificationKind.OnCleared"/> notifications.</param>
    /// <param name="onCompleted">The handler of a completion notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, Action<IList<TSource>> exists, Action<TSource> onAdded, Action<TSource, TSource> onReplaced, Action<TSource> onRemoved, Action onCleared, Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(exists != null);
      Contract.Requires(onAdded != null);
      Contract.Requires(onReplaced != null);
      Contract.Requires(onRemoved != null);
      Contract.Requires(onCleared != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(n => n.Accept(exists, onAdded, onReplaced, onRemoved, onCleared), onCompleted);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="exists">The handler for <see cref="CollectionNotificationKind.Exists"/> notifications.</param>
    /// <param name="onAdded">The handler for <see cref="CollectionNotificationKind.OnAdded"/> notifications.</param>
    /// <param name="onReplaced">The handler for <see cref="CollectionNotificationKind.OnReplaced"/> notifications.</param>
    /// <param name="onRemoved">The handler for <see cref="CollectionNotificationKind.OnRemoved"/> notifications.</param>
    /// <param name="onCleared">The handler for <see cref="CollectionNotificationKind.OnCleared"/> notifications.</param>
    /// <param name="onError">The handler of an error notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, Action<IList<TSource>> exists, Action<TSource> onAdded, Action<TSource, TSource> onReplaced, Action<TSource> onRemoved, Action onCleared, Action<Exception> onError)
    {
      Contract.Requires(source != null);
      Contract.Requires(exists != null);
      Contract.Requires(onAdded != null);
      Contract.Requires(onReplaced != null);
      Contract.Requires(onRemoved != null);
      Contract.Requires(onCleared != null);
      Contract.Requires(onError != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(n => n.Accept(exists, onAdded, onReplaced, onRemoved, onCleared), onError);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="exists">The handler for <see cref="CollectionNotificationKind.Exists"/> notifications.</param>
    /// <param name="onAdded">The handler for <see cref="CollectionNotificationKind.OnAdded"/> notifications.</param>
    /// <param name="onReplaced">The handler for <see cref="CollectionNotificationKind.OnReplaced"/> notifications.</param>
    /// <param name="onRemoved">The handler for <see cref="CollectionNotificationKind.OnRemoved"/> notifications.</param>
    /// <param name="onCleared">The handler for <see cref="CollectionNotificationKind.OnCleared"/> notifications.</param>
    /// <param name="onError">The handler of an error notification.</param>
    /// <param name="onCompleted">The handler of a completion notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, Action<IList<TSource>> exists, Action<TSource> onAdded, Action<TSource, TSource> onReplaced, Action<TSource> onRemoved, Action onCleared, Action<Exception> onError, Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(exists != null);
      Contract.Requires(onAdded != null);
      Contract.Requires(onReplaced != null);
      Contract.Requires(onRemoved != null);
      Contract.Requires(onCleared != null);
      Contract.Requires(onError != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(n => n.Accept(exists, onAdded, onReplaced, onRemoved, onCleared), onError, onCompleted);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, ICollection<TSource> collection)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(notification => notification.ToModifications().ForEach(mod => mod.Accept(collection)));
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="onError">The handler of an error notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, ICollection<TSource> collection, Action<Exception> onError)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(onError != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(notification => notification.ToModifications().ForEach(mod => mod.Accept(collection)), onError);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="onCompleted">The handler of a completion notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, ICollection<TSource> collection, Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(notification => notification.ToModifications().ForEach(mod => mod.Accept(collection)), onCompleted);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="onError">The handler of an error notification.</param>
    /// <param name="onCompleted">The handler of a completion notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, ICollection<TSource> collection, Action<Exception> onError, Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(onError != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(notification => notification.ToModifications().ForEach(mod => mod.Accept(collection)), onError, onCompleted);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of elements contained by the specified <paramref name="collection"/>.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="selector">A function that projects a notification of the source type to a value of the type contained by the specified <paramref name="collection"/>.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource, TResult>(this IObservable<CollectionNotification<TSource>> source, ICollection<TResult> collection, Func<TSource, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(notification => notification.ToModifications().ForEach(mod => mod.Accept(
        add => add.ForEach(item => collection.Add(selector(item))),
        remove => remove.ForEach(item => collection.Remove(selector(item))),
        collection.Clear)));
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of elements contained by the specified <paramref name="collection"/>.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="selector">A function that projects a notification of the source type to a value of the type contained by the specified <paramref name="collection"/>.</param>
    /// <param name="onError">The handler of an error notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource, TResult>(this IObservable<CollectionNotification<TSource>> source, ICollection<TResult> collection, Func<TSource, TResult> selector, Action<Exception> onError)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(selector != null);
      Contract.Requires(onError != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(
        notification => notification.ToModifications()
                                    .ForEach(mod => mod.Accept(
                                      add => add.ForEach(item => collection.Add(selector(item))),
                                      remove => remove.ForEach(item => collection.Remove(selector(item))),
                                      collection.Clear)),
        onError);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of elements contained by the specified <paramref name="collection"/>.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="selector">A function that projects a notification of the source type to a value of the type contained by the specified <paramref name="collection"/>.</param>
    /// <param name="onCompleted">The handler of a completion notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource, TResult>(this IObservable<CollectionNotification<TSource>> source, ICollection<TResult> collection, Func<TSource, TResult> selector, Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(selector != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(
        notification => notification.ToModifications()
                                    .ForEach(mod => mod.Accept(
                                      add => add.ForEach(item => collection.Add(selector(item))),
                                      remove => remove.ForEach(item => collection.Remove(selector(item))),
                                      collection.Clear)),
        onCompleted);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of elements contained by the specified <paramref name="collection"/>.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="selector">A function that projects a notification of the source type to a value of the type contained by the specified <paramref name="collection"/>.</param>
    /// <param name="onError">The handler of an error notification.</param>
    /// <param name="onCompleted">The handler of a completion notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource, TResult>(this IObservable<CollectionNotification<TSource>> source, ICollection<TResult> collection, Func<TSource, TResult> selector, Action<Exception> onError, Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(selector != null);
      Contract.Requires(onError != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(
        notification => notification.ToModifications()
                                    .ForEach(mod => mod.Accept(
                                      add => add.ForEach(item => collection.Add(selector(item))),
                                      remove => remove.ForEach(item => collection.Remove(selector(item))),
                                      collection.Clear)),
        onError,
        onCompleted);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="filter">A predicate function that indicates whether a given notification will be applied to the <paramref name="collection"/>.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, ICollection<TSource> collection, Func<TSource, bool> filter)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(filter != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(
        notification => notification.ToModifications()
                                    .ForEach(mod => mod.Accept(
                                      add => add.Where(filter).ForEach(item => collection.Add(item)),
                                      remove => remove.Where(filter).ForEach(item => collection.Remove(item)),
                                      collection.Clear)));
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="filter">A predicate function that indicates whether a given notification will be applied to the <paramref name="collection"/>.</param>
    /// <param name="onError">The handler of an error notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, ICollection<TSource> collection, Func<TSource, bool> filter, Action<Exception> onError)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(filter != null);
      Contract.Requires(onError != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(
        notification => notification.ToModifications()
                                    .ForEach(mod => mod.Accept(
                                      add => add.Where(filter).ForEach(item => collection.Add(item)),
                                      remove => remove.Where(filter).ForEach(item => collection.Remove(item)),
                                      collection.Clear)),
        onError);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="filter">A predicate function that indicates whether a given notification will be applied to the <paramref name="collection"/>.</param>
    /// <param name="onCompleted">The handler of a completion notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, ICollection<TSource> collection, Func<TSource, bool> filter, Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(filter != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(
        notification => notification.ToModifications()
                                    .ForEach(mod => mod.Accept(
                                      add => add.Where(filter).ForEach(item => collection.Add(item)),
                                      remove => remove.Where(filter).ForEach(item => collection.Remove(item)),
                                      collection.Clear)),
        onCompleted);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="filter">A predicate function that indicates whether a given notification will be applied to the <paramref name="collection"/>.</param>
    /// <param name="onError">The handler of an error notification.</param>
    /// <param name="onCompleted">The handler of a completion notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource>(this IObservable<CollectionNotification<TSource>> source, ICollection<TSource> collection, Func<TSource, bool> filter, Action<Exception> onError, Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(filter != null);
      Contract.Requires(onError != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(
        notification => notification.ToModifications()
                                    .ForEach(mod => mod.Accept(
                                      add => add.Where(filter).ForEach(item => collection.Add(item)),
                                      remove => remove.Where(filter).ForEach(item => collection.Remove(item)),
                                      collection.Clear)),
        onError,
        onCompleted);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of elements contained by the specified <paramref name="collection"/>.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="filter">A predicate function that indicates whether a given notification will be included in the projection.</param>
    /// <param name="selector">A function that projects a notification of the source type to a value of the type contained by the specified <paramref name="collection"/>.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource, TResult>(this IObservable<CollectionNotification<TSource>> source, ICollection<TResult> collection, Func<TSource, bool> filter, Func<TSource, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(
        notification => notification.ToModifications()
                                    .ForEach(mod => mod.Accept(
                                      add => add.Where(filter).ForEach(item => collection.Add(selector(item))),
                                      remove => remove.Where(filter).ForEach(item => collection.Remove(selector(item))),
                                      collection.Clear)));
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of elements contained by the specified <paramref name="collection"/>.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="filter">A predicate function that indicates whether a given notification will be included in the projection.</param>
    /// <param name="selector">A function that projects a notification of the source type to a value of the type contained by the specified <paramref name="collection"/>.</param>
    /// <param name="onError">The handler of an error notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource, TResult>(this IObservable<CollectionNotification<TSource>> source, ICollection<TResult> collection, Func<TSource, bool> filter, Func<TSource, TResult> selector, Action<Exception> onError)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Requires(onError != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(
        notification => notification.ToModifications()
                                    .ForEach(mod => mod.Accept(
                                      add => add.Where(filter).ForEach(item => collection.Add(selector(item))),
                                      remove => remove.Where(filter).ForEach(item => collection.Remove(selector(item))),
                                      collection.Clear)),
        onError);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of elements contained by the specified <paramref name="collection"/>.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="filter">A predicate function that indicates whether a given notification will be included in the projection.</param>
    /// <param name="selector">A function that projects a notification of the source type to a value of the type contained by the specified <paramref name="collection"/>.</param>
    /// <param name="onCompleted">The handler of a completion notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource, TResult>(this IObservable<CollectionNotification<TSource>> source, ICollection<TResult> collection, Func<TSource, bool> filter, Func<TSource, TResult> selector, Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(
        notification => notification.ToModifications()
                                    .ForEach(mod => mod.Accept(
                                      add => add.Where(filter).ForEach(item => collection.Add(selector(item))),
                                      remove => remove.Where(filter).ForEach(item => collection.Remove(selector(item))),
                                      collection.Clear)),
        onCompleted);
    }

    /// <summary>
    /// Notifies the observable that an observer is to receive notifications.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TResult">The type of elements contained by the specified <paramref name="collection"/>.</typeparam>
    /// <param name="source">The observable for which a subscription is created.</param>
    /// <param name="collection">The target object to be modified for each notification.</param>
    /// <param name="filter">A predicate function that indicates whether a given notification will be included in the projection.</param>
    /// <param name="selector">A function that projects a notification of the source type to a value of the type contained by the specified <paramref name="collection"/>.</param>
    /// <param name="onError">The handler of an error notification.</param>
    /// <param name="onCompleted">The handler of a completion notification.</param>
    /// <returns>The observer's interface that enables cancelation of the subscription so that it stops receiving notifications.</returns>
    public static IDisposable Subscribe<TSource, TResult>(this IObservable<CollectionNotification<TSource>> source, ICollection<TResult> collection, Func<TSource, bool> filter, Func<TSource, TResult> selector, Action<Exception> onError, Action onCompleted)
    {
      Contract.Requires(source != null);
      Contract.Requires(collection != null);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Requires(onError != null);
      Contract.Requires(onCompleted != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      return source.Subscribe(
        notification => notification.ToModifications()
                                    .ForEach(mod => mod.Accept(
                                      add => add.Where(filter).ForEach(item => collection.Add(selector(item))),
                                      remove => remove.Where(filter).ForEach(item => collection.Remove(selector(item))),
                                      collection.Clear)),
        onError,
        onCompleted);
    }
  }
}