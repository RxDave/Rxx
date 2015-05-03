using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Reactive.Linq
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for <see cref="INotifyCollectionChanged"/> objects.
  /// </summary>
  public static class NotifyCollectionChangedExtensions
  {
#if !SILVERLIGHT
    /// <summary>
    /// Converts <see cref="INotifyCollectionChanged.CollectionChanged"/> events into an observable sequence of <see cref="CollectionNotification{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">An implementation of <see cref="INotifyCollectionChanged"/> that raises events when a collection changes.</param>
    /// <remarks>
    /// An <see cref="NotifyCollectionChangedAction.Add"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnAdded"/> notifications.
    /// A <see cref="NotifyCollectionChangedAction.Remove"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnRemoved"/> notifications.
    /// A <see cref="NotifyCollectionChangedAction.Move"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnReplaced"/> notifications 
    /// where <see cref="CollectionNotification{T}.Value"/> and <see cref="CollectionNotification{T}.ReplacedValue"/> refer to the same value. 
    /// A <see cref="NotifyCollectionChangedAction.Replace"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnReplaced"/> notifications.
    /// A <see cref="NotifyCollectionChangedAction.Reset"/> event is projected into a single <see cref="CollectionNotificationKind.OnCleared"/> notification.
    /// </remarks>
    /// <returns>An observable sequence of <see cref="CollectionNotification{T}"/> objects corresponding to raised events.</returns> 
#else
    /// <summary>
    /// Converts <see cref="INotifyCollectionChanged.CollectionChanged"/> events into an observable sequence of <see cref="CollectionNotification{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">An implementation of <see cref="INotifyCollectionChanged"/> that raises events when a collection changes.</param>
    /// <remarks>
    /// An <see cref="NotifyCollectionChangedAction.Add"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnAdded"/> notifications.
    /// A <see cref="NotifyCollectionChangedAction.Remove"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnRemoved"/> notifications.
    /// A <see cref="NotifyCollectionChangedAction.Replace"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnReplaced"/> notifications.
    /// A <see cref="NotifyCollectionChangedAction.Reset"/> event is projected into a single <see cref="CollectionNotificationKind.OnCleared"/> notification.
    /// </remarks>
    /// <returns>An observable sequence of <see cref="CollectionNotification{T}"/> objects corresponding to raised events.</returns>
#endif
    public static IObservable<CollectionNotification<T>> AsCollectionNotifications<T>(this INotifyCollectionChanged source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<CollectionNotification<T>>>() != null);

      return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
        eh => source.CollectionChanged += eh,
        eh => source.CollectionChanged -= eh)
        .SelectMany(e =>
        {
          var args = e.EventArgs;

          switch (args.Action)
          {
            case NotifyCollectionChangedAction.Add:
              return EnsureSequence<T>(args.NewItems).Select(CollectionNotification.CreateOnAdded).ToObservable();
            case NotifyCollectionChangedAction.Remove:
              return EnsureSequence<T>(args.OldItems).Select(CollectionNotification.CreateOnRemoved).ToObservable();
#if !SILVERLIGHT
            case NotifyCollectionChangedAction.Move:
              return EnsureSequence<T>(args.OldItems).Select(value => CollectionNotification.CreateOnReplaced(value, value)).ToObservable();
#endif
            case NotifyCollectionChangedAction.Replace:
              return EnsureSequence<T>(args.NewItems)
                .Zip(
                  EnsureSequence<T>(args.OldItems),
                  (newValue, oldValue) => CollectionNotification.CreateOnReplaced(oldValue, newValue))
                .ToObservable();
            case NotifyCollectionChangedAction.Reset:
              return Observable.Return(CollectionNotification.CreateOnCleared<T>());
            default:
              throw new InvalidOperationException();
          }
        });
    }

#if !SILVERLIGHT
    /// <summary>
    /// Converts <see cref="INotifyCollectionChanged.CollectionChanged"/> events into an observable sequence of <see cref="CollectionNotification{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">An implementation of <see cref="INotifyCollectionChanged"/> that raises events when a collection changes.</param>
    /// <remarks>
    /// An <see cref="NotifyCollectionChangedAction.Add"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnAdded"/> notifications.
    /// A <see cref="NotifyCollectionChangedAction.Remove"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnRemoved"/> notifications.
    /// A <see cref="NotifyCollectionChangedAction.Move"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnReplaced"/> notifications 
    /// where <see cref="CollectionNotification{T}.Value"/> and <see cref="CollectionNotification{T}.ReplacedValue"/> refer to the same value. 
    /// A <see cref="NotifyCollectionChangedAction.Replace"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnReplaced"/> notifications.
    /// A <see cref="NotifyCollectionChangedAction.Reset"/> event is projected into a single <see cref="CollectionNotificationKind.OnCleared"/> notification.
    /// </remarks>
    /// <returns>An observable sequence of <see cref="CollectionNotification{T}"/> objects corresponding to raised events.</returns> 
#else
    /// <summary>
    /// Converts <see cref="INotifyCollectionChanged.CollectionChanged"/> events into an observable sequence of <see cref="CollectionNotification{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">An implementation of <see cref="INotifyCollectionChanged"/> that raises events when a collection changes.</param>
    /// <remarks>
    /// An <see cref="NotifyCollectionChangedAction.Add"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnAdded"/> notifications.
    /// A <see cref="NotifyCollectionChangedAction.Remove"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnRemoved"/> notifications.
    /// A <see cref="NotifyCollectionChangedAction.Replace"/> event is projected into zero or more <see cref="CollectionNotificationKind.OnReplaced"/> notifications.
    /// A <see cref="NotifyCollectionChangedAction.Reset"/> event is projected into a single <see cref="CollectionNotificationKind.OnCleared"/> notification.
    /// </remarks>
    /// <returns>An observable sequence of <see cref="CollectionNotification{T}"/> objects corresponding to raised events.</returns>
#endif
    public static IObservable<CollectionNotification<T>> AsCollectionNotifications<T>(this ObservableCollection<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<CollectionNotification<T>>>() != null);

      return AsCollectionNotifications<T>((INotifyCollectionChanged)source);
    }

#if !SILVERLIGHT
    /// <summary>
    /// Converts <see cref="INotifyCollectionChanged.CollectionChanged"/> events into an observable sequence of <see cref="CollectionModification{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">An implementation of <see cref="INotifyCollectionChanged"/> that raises events when a collection changes.</param>
    /// <remarks>
    /// An <see cref="NotifyCollectionChangedAction.Add"/> event is projected into zero or more <see cref="CollectionModificationKind.Add"/> modifications.
    /// A <see cref="NotifyCollectionChangedAction.Remove"/> event is projected into zero or more <see cref="CollectionModificationKind.Remove"/> modifications.
    /// A <see cref="NotifyCollectionChangedAction.Move"/> event is ignored. 
    /// A <see cref="NotifyCollectionChangedAction.Replace"/> event is projected into zero or more sequential 
    /// <see cref="CollectionModificationKind.Remove"/> and <see cref="CollectionModificationKind.Add"/> modifications.
    /// A <see cref="NotifyCollectionChangedAction.Reset"/> event is projected into a single <see cref="CollectionModificationKind.Clear"/> modification.
    /// </remarks>
    /// <returns>An observable sequence of <see cref="CollectionModification{T}"/> objects corresponding to raised events.</returns>
#else
    /// <summary>
    /// Converts <see cref="INotifyCollectionChanged.CollectionChanged"/> events into an observable sequence of <see cref="CollectionModification{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">An implementation of <see cref="INotifyCollectionChanged"/> that raises events when a collection changes.</param>
    /// <remarks>
    /// An <see cref="NotifyCollectionChangedAction.Add"/> event is projected into zero or more <see cref="CollectionModificationKind.Add"/> modifications.
    /// A <see cref="NotifyCollectionChangedAction.Remove"/> event is projected into zero or more <see cref="CollectionModificationKind.Remove"/> modifications.
    /// A <see cref="NotifyCollectionChangedAction.Replace"/> event is projected into zero or more sequential 
    /// <see cref="CollectionModificationKind.Remove"/> and <see cref="CollectionModificationKind.Add"/> modifications.
    /// A <see cref="NotifyCollectionChangedAction.Reset"/> event is projected into a single <see cref="CollectionModificationKind.Clear"/> modification.
    /// </remarks>
    /// <returns>An observable sequence of <see cref="CollectionModification{T}"/> objects corresponding to raised events.</returns>
#endif
    public static IObservable<CollectionModification<T>> AsCollectionModifications<T>(this INotifyCollectionChanged source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<CollectionModification<T>>>() != null);

      return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
        eh => source.CollectionChanged += eh,
        eh => source.CollectionChanged -= eh)
        .SelectMany(e =>
        {
          var args = e.EventArgs;

          switch (args.Action)
          {
            case NotifyCollectionChangedAction.Add:
              return Observable.Return(CollectionModification.CreateAdd(EnsureList<T>(args.NewItems)));
            case NotifyCollectionChangedAction.Remove:
              return Observable.Return(CollectionModification.CreateRemove(EnsureList<T>(args.OldItems)));
#if !SILVERLIGHT
            case NotifyCollectionChangedAction.Move:
              return Observable.Empty<CollectionModification<T>>();
#endif
            case NotifyCollectionChangedAction.Replace:
              return Observable.Return(CollectionModification.CreateRemove(EnsureList<T>(args.OldItems)))
                .Concat(Observable.Return(CollectionModification.CreateAdd(EnsureList<T>(args.NewItems))));
            case NotifyCollectionChangedAction.Reset:
              return Observable.Return(CollectionModification.CreateClear<T>());
            default:
              throw new InvalidOperationException();
          }
        });
    }

#if !SILVERLIGHT
    /// <summary>
    /// Converts <see cref="INotifyCollectionChanged.CollectionChanged"/> events into an observable sequence of <see cref="CollectionModification{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">An implementation of <see cref="INotifyCollectionChanged"/> that raises events when a collection changes.</param>
    /// <remarks>
    /// An <see cref="NotifyCollectionChangedAction.Add"/> event is projected into zero or more <see cref="CollectionModificationKind.Add"/> modifications.
    /// A <see cref="NotifyCollectionChangedAction.Remove"/> event is projected into zero or more <see cref="CollectionModificationKind.Remove"/> modifications.
    /// A <see cref="NotifyCollectionChangedAction.Move"/> event is ignored. 
    /// A <see cref="NotifyCollectionChangedAction.Replace"/> event is projected into zero or more sequential 
    /// <see cref="CollectionModificationKind.Remove"/> and <see cref="CollectionModificationKind.Add"/> modifications.
    /// A <see cref="NotifyCollectionChangedAction.Reset"/> event is projected into a single <see cref="CollectionModificationKind.Clear"/> modification.
    /// </remarks>
    /// <returns>An observable sequence of <see cref="CollectionModification{T}"/> objects corresponding to raised events.</returns>
#else
    /// <summary>
    /// Converts <see cref="INotifyCollectionChanged.CollectionChanged"/> events into an observable sequence of <see cref="CollectionModification{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">An implementation of <see cref="INotifyCollectionChanged"/> that raises events when a collection changes.</param>
    /// <remarks>
    /// An <see cref="NotifyCollectionChangedAction.Add"/> event is projected into zero or more <see cref="CollectionModificationKind.Add"/> modifications.
    /// A <see cref="NotifyCollectionChangedAction.Remove"/> event is projected into zero or more <see cref="CollectionModificationKind.Remove"/> modifications.
    /// A <see cref="NotifyCollectionChangedAction.Replace"/> event is projected into zero or more sequential 
    /// <see cref="CollectionModificationKind.Remove"/> and <see cref="CollectionModificationKind.Add"/> modifications.
    /// A <see cref="NotifyCollectionChangedAction.Reset"/> event is projected into a single <see cref="CollectionModificationKind.Clear"/> modification.
    /// </remarks>
    /// <returns>An observable sequence of <see cref="CollectionModification{T}"/> objects corresponding to raised events.</returns>
#endif
    public static IObservable<CollectionModification<T>> AsCollectionModifications<T>(this ObservableCollection<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<CollectionModification<T>>>() != null);

      return AsCollectionModifications<T>((INotifyCollectionChanged)source);
    }

    private static IEnumerable<T> EnsureSequence<T>(IList source)
    {
      return source == null ? new T[0] : source.Cast<T>();
    }

    private static IList<T> EnsureList<T>(IList source)
    {
      return source == null ? new T[0] : (IList<T>)source.Cast<T>().ToList().AsReadOnly();
    }
  }
}