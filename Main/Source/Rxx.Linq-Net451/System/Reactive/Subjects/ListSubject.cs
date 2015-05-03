using System.Collections;
using System.Collections.Generic;
#if !PORT_40
using System.Collections.ObjectModel;
using System.Collections.Specialized;
#endif
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;

namespace System.Reactive.Subjects
{
#if !PORT_40
  /// <summary>
  /// Represents an object that is a list as well as an observable sequence of collection notifications and observer of collection modifications.
  /// </summary>
  /// <typeparam name="T">The type of the elements in the collection.</typeparam>
  /// <remarks>
  /// <para>
  /// <see cref="ListSubject{T}"/> implements <see cref="INotifyCollectionChanged"/> and behaves similar to <see cref="ObservableCollection{T}"/>, 
  /// so it can be bound directly to an <strong>ItemsPresenter</strong> or a derived type in WPF, Silverlight and Windows Phone.
  /// </para> 
  /// <para>
  /// The <see cref="Subscribe(IObserver{CollectionNotification{T}})"/> method pushes a single <see cref="CollectionNotificationKind.Exists"/> notification 
  /// containing a snapshot of the entire list, followed by zero or more notifications that represent changes to the list.
  /// </para>
  /// <para>
  /// The enumerator that is returned by <see cref="GetEnumerator"/> blocks all methods on the <see cref="ListSubject{T}"/>
  /// until the enumeration has completed.
  /// </para>
  /// <alert type="tip">
  /// To take a thread-safe snapshot of the list, simply call <see cref="System.Linq.Enumerable.ToList"/> to collect the items 
  /// as fast as possible and then enumerate the new list.
  /// </alert>
  /// <alert type="tip">
  /// To create a synchronized clone or projection of this list, use one of the <strong>Collect</strong> extension methods.
  /// </alert>
  /// </remarks>
  /// <threadsafety instance="true" /> 
#else
  /// <summary>
  /// Represents an object that is a list as well as an observable sequence of collection notifications and observer of collection modifications.
  /// </summary>
  /// <typeparam name="T">The type of the elements in the collection.</typeparam>
  /// <remarks>
  /// <para>
  /// The <see cref="Subscribe(IObserver{CollectionNotification{T}})"/> method pushes a single <see cref="CollectionNotificationKind.Exists"/> notification 
  /// containing a snapshot of the entire list, followed by zero or more notifications that represent changes to the list.
  /// </para>
  /// <para>
  /// The enumerator that is returned by <see cref="GetEnumerator"/> blocks all methods on the <see cref="ListSubject{T}"/>
  /// until the enumeration has completed.
  /// </para>
  /// <alert type="tip">
  /// To take a thread-safe snapshot of the list, simply call <see cref="System.Linq.Enumerable.ToList"/> to collect the items 
  /// as fast as possible and then enumerate the new list.
  /// </alert>
  /// <alert type="tip">
  /// To create a synchronized clone or projection of this list, use one of the <strong>Collect</strong> extension methods.
  /// </alert>
  /// </remarks>
  /// <threadsafety instance="true" />
#endif
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification = "It's a list, not just a collection.  It's also a subject.")]
  public sealed class ListSubject<T> : IListSubject<T>, IList, INotifyPropertyChanged
  {
    #region Public Properties
    /// <summary>
    /// Gets the number of elements currently contained in the list.
    /// </summary>
    public int Count
    {
      get
      {
        lock (gate)
        {
          EnsureNotDisposed();
          EnsureNotFaulted();

          return list.Count;
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the list is read-only.
    /// </summary>
    /// <value>Always returns <see langword="false" />.</value>
    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the list.</exception>
    public T this[int index]
    {
      get
      {
        lock (gate)
        {
          EnsureNotDisposed();
          EnsureNotFaulted();

          Contract.Assume(index < list.Count);

          return list[index];
        }
      }
      set
      {
        lock (gate)
        {
          EnsureNotDisposed();

          if (EnsureNotStopped())
          {
            Contract.Assume(index < list.Count);

            T previous = list[index];

            list[index] = value;

            subject.OnNext(CollectionNotification.CreateOnReplaced(previous, value));
          }
        }
      }
    }
    #endregion

    #region Private / Protected
    private readonly object gate = new object();
    private readonly Subject<CollectionNotification<T>> subject = new Subject<CollectionNotification<T>>();
#if PORT_40
    private readonly List<T> list;
#else
    private readonly ObservableCollection<T> list;
#endif
    private readonly IDisposable subscription;
    private Exception exception;
    private bool isStopped, isDisposed;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="ListSubject{T}" /> class.
    /// </summary>
    public ListSubject()
    {
#if PORT_40
      list = new List<T>();
#else
      list = new ObservableCollection<T>();
#endif
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="ListSubject{T}" /> class.
    /// </summary>
    /// <param name="collection">The sequence from which the elements are copied.</param>
    public ListSubject(IEnumerable<T> collection)
    {
      Contract.Requires(collection != null);

#if PORT_40
      list = new List<T>(collection);
#else
      list = new ObservableCollection<T>(collection);
#endif
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="ListSubject{T}" /> class.
    /// </summary>
    /// <param name="list">The list from which the elements are copied.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists",
      Justification = "The parameter is passed directly to the ObservableCollection's constructor, which requires the concrete type: List<T>.")]
    public ListSubject(List<T> list)
    {
      Contract.Requires(list != null);

#if PORT_40
      this.list = new List<T>(list);
#else
      this.list = new ObservableCollection<T>(list);
#endif
    }

    internal ListSubject(IDisposable compositedDisposable)
      : this()
    {
      Contract.Requires(compositedDisposable != null);

      subscription = compositedDisposable;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(gate != null);
      Contract.Invariant(subject != null);
      Contract.Invariant(list != null);
    }

    /// <summary>
    /// Returns an observable sequence of collection notifications that represent changes to the list.
    /// </summary>
    /// <returns>An observable sequence of collection notifications that represent changes to the list.</returns>
    public IObservable<CollectionNotification<T>> Changes()
    {
      return Observable.Create<CollectionNotification<T>>(
        observer => Subscribe(observer, startWithExisting: false));
    }

    /// <summary>
    /// Notifies the subject that an observer is to receive collection notifications, starting with a notification
    /// that contains a snapshot of the existing values in the list.
    /// </summary>
    /// <param name="observer">The object that is to receive collection notifications.</param>
    /// <returns>The observer's interface that enables resources to be disposed.</returns>
    public IDisposable Subscribe(IObserver<CollectionNotification<T>> observer)
    {
      return Subscribe(observer, startWithExisting: true);
    }

    private IDisposable Subscribe(IObserver<CollectionNotification<T>> observer, bool startWithExisting)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      lock (gate)
      {
        if (isDisposed)
        {
          return Observable
            .Throw<CollectionNotification<T>>(new ObjectDisposedException("ListSubject<T>"))
            .SubscribeSafe(observer);
        }

        if (exception != null)
        {
          return Observable
            .Throw<CollectionNotification<T>>(exception)
            .SubscribeSafe(observer);
        }

        if (startWithExisting)
        {
          IList<T> clonedList = list.ToList().AsReadOnly();

          observer.OnNext(CollectionNotification.CreateExists(clonedList));
        }

        return subject.SubscribeSafe(observer);
      }
    }

    /// <summary>
    /// Changes the list according to the specified collection notification.
    /// </summary>
    /// <param name="value">A modification that indicates how the list must be changed.</param>
    public void OnNext(CollectionModification<T> value)
    {
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }

      lock (gate)
      {
        EnsureNotDisposed();

        if (EnsureNotStopped())
        {
          IList<T> values;

          switch (value.Kind)
          {
            case CollectionModificationKind.Add:
              values = value.Values;

              for (int i = 0; i < values.Count; i++)
              {
                var item = values[i];

                list.Add(item);

                subject.OnNext(CollectionNotification.CreateOnAdded(item));
              }
              break;
            case CollectionModificationKind.Remove:
              values = value.Values;

              for (int i = 0; i < values.Count; i++)
              {
                var item = values[i];

                if (list.Remove(item))
                {
                  subject.OnNext(CollectionNotification.CreateOnRemoved(item));
                }
              }
              break;
            case CollectionModificationKind.Clear:
              list.Clear();

              subject.OnNext(CollectionNotification.CreateOnCleared<T>());
              break;
          }
        }
      }
    }

    /// <summary>
    /// Terminates the subject with an error condition.
    /// </summary>
    /// <param name="error">An object that provides additional information about the error.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#",
      Justification = "Windows Phone defines IObserver<T>.OnError with the name \"exception\" for the \"error\" parameter, but it's better to" +
                      "leave it as \"error\" here so that callers can use the same named parameter across all platforms.")]
    public void OnError(Exception error)
    {
      lock (gate)
      {
        EnsureNotDisposed();

        if (!isStopped)
        {
          isStopped = true;
          exception = error;

          subject.OnError(error);
        }
      }
    }

    /// <summary>
    /// Notifies the subject to stop accepting collection modifications.
    /// </summary>
    public void OnCompleted()
    {
      lock (gate)
      {
        EnsureNotDisposed();

        if (!isStopped)
        {
          isStopped = true;

          subject.OnCompleted();
        }
      }
    }

    /// <summary>
    /// Determines whether the list contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the list.</param>
    /// <returns><see langword="True"/> if <paramref name="item"/> is found in the list; otherwise, <see langword="false"/></returns>
    [ContractVerification(false)]
    public bool Contains(T item)
    {
      lock (gate)
      {
        EnsureNotDisposed();
        EnsureNotFaulted();

        return list.Contains(item);
      }
    }

    /// <summary>
    /// Determines the index of a specific item in the list.
    /// </summary>
    /// <param name="item">The object to locate in the list.</param>
    /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
    [ContractVerification(false)]
    public int IndexOf(T item)
    {
      lock (gate)
      {
        EnsureNotDisposed();
        EnsureNotFaulted();

        return list.IndexOf(item);
      }
    }

    /// <summary>
    /// Adds an item to the list.
    /// </summary>
    /// <param name="item">The object to add to the list.</param>
    [ContractVerification(false)]
    public void Add(T item)
    {
      lock (gate)
      {
        EnsureNotDisposed();

        if (EnsureNotStopped())
        {
          list.Add(item);

          subject.OnNext(CollectionNotification.CreateOnAdded(item));
        }
      }
    }

    /// <summary>
    /// Inserts an item to the list at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert into the list.</param>
    public void Insert(int index, T item)
    {
      lock (gate)
      {
        EnsureNotDisposed();

        if (EnsureNotStopped())
        {
          Contract.Assume(index <= list.Count);

          list.Insert(index, item);

          subject.OnNext(CollectionNotification.CreateOnAdded(item));
        }
      }
    }

#if !PORT_40 && (!SILVERLIGHT || WINDOWS_PHONE)
    /// <summary>
    /// Moves the item at the specified index to a new location in the list.
    /// </summary>
    /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
    /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
    public void Move(int oldIndex, int newIndex)
    {
      Contract.Requires(oldIndex >= 0);
      Contract.Requires(newIndex >= 0);

      lock (gate)
      {
        EnsureNotDisposed();

        if (EnsureNotStopped())
        {
          list.Move(oldIndex, newIndex);
        }
      }
    }
#endif

    /// <summary>
    /// Removes the first occurrence of a specific object from the list.
    /// </summary>
    /// <param name="item">The object to remove from the list.</param>
    /// <returns><see langword="True" /> if <paramref name="item"/> was successfully removed from the list; otherwise, <see langword="false" />.
    /// This method also returns <see langword="false" /> if <paramref name="item"/> is not found in the list.</returns>
    public bool Remove(T item)
    {
      lock (gate)
      {
        EnsureNotDisposed();

        if (EnsureNotStopped())
        {
          if (list.Remove(item))
          {
            subject.OnNext(CollectionNotification.CreateOnRemoved(item));

            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Removes the list item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    [ContractVerification(false)]
    public void RemoveAt(int index)
    {
      lock (gate)
      {
        EnsureNotDisposed();

        if (EnsureNotStopped())
        {
          Contract.Assume(index < list.Count);

          var item = list[index];

          list.RemoveAt(index);

          subject.OnNext(CollectionNotification.CreateOnRemoved(item));
        }
      }
    }

    /// <summary>
    /// Removes all items from the list.
    /// </summary>
    [ContractVerification(false)]
    public void Clear()
    {
      lock (gate)
      {
        EnsureNotDisposed();

        if (EnsureNotStopped())
        {
          list.Clear();

          subject.OnNext(CollectionNotification.CreateOnCleared<T>());
        }
      }
    }

    /// <summary>
    /// Copies the elements of the list to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from 
    /// the list. The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
      lock (gate)
      {
        EnsureNotDisposed();
        EnsureNotFaulted();

        Contract.Assume(arrayIndex + list.Count <= array.Length);

        list.CopyTo(array, arrayIndex);
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the list.
    /// </summary>
    /// <remarks>
    /// The list is locked for the entire duration while enumerating.  Any collection modifications that are received 
    /// during the enumeration will be blocked.  When the enumeration has completed, all previous modifications will be 
    /// allowed to acquire the lock and mutate the list.  For this reason it is best to enumerate quickly.  For example, 
    /// you could call the <see cref="System.Linq.Enumerable.ToList"/> extension method to take a snapshot of the list, 
    /// then perform work by enumerating the snapshot while the subject is free to accept collection modifications.
    /// </remarks>
    /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
    public IEnumerator<T> GetEnumerator()
    {
      lock (gate)
      {
        EnsureNotDisposed();
        EnsureNotFaulted();

        foreach (var item in list)
        {
          yield return item;
        }
      }
    }

    Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private bool EnsureNotStopped()
    {
      if (isStopped)
      {
        EnsureNotFaulted();

        return false;
      }

      return true;
    }

    private void EnsureNotFaulted()
    {
      if (exception != null)
      {
        throw exception;
      }
    }

    private void EnsureNotDisposed()
    {
      if (isDisposed)
      {
        throw new ObjectDisposedException("ListSubject<T>");
      }
    }

    /// <summary>
    /// Unsubscribes all observers and releases resources. 
    /// </summary>
    public void Dispose()
    {
      lock (gate)
      {
        if (!isDisposed)
        {
          isDisposed = true;

          if (subscription != null)
          {
            subscription.Dispose();
          }

          subject.Dispose();
        }
      }
    }
    #endregion

    #region Events
#if !PORT_40
    /// <summary>
    /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged
    {
      add
      {
        list.CollectionChanged += value;
      }
      remove
      {
        list.CollectionChanged -= value;
      }
    }
#endif

    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
      add
      {
        ((INotifyPropertyChanged)list).PropertyChanged += value;
      }
      remove
      {
        ((INotifyPropertyChanged)list).PropertyChanged -= value;
      }
    }
    #endregion

    // The non-generic IList interface is required to support editable data-binding in WPF.
    #region IList Members
    int IList.Add(object value)
    {
      lock (gate)
      {
        var index = Count;

        Add((T)value);

        return index;
      }
    }

    bool IList.Contains(object value)
    {
      return Contains((T)value);
    }

    int IList.IndexOf(object value)
    {
      return IndexOf((T)value);
    }

    void IList.Insert(int index, object value)
    {
      Insert(index, (T)value);
    }

    bool IList.IsFixedSize
    {
      get
      {
        return false;
      }
    }

    void IList.Remove(object value)
    {
      Remove((T)value);
    }

    void IList.RemoveAt(int index)
    {
      RemoveAt(index);
    }

    object IList.this[int index]
    {
      get
      {
        return this[index];
      }
      set
      {
        this[index] = (T)value;
      }
    }
    #endregion

    #region ICollection Members
    void ICollection.CopyTo(Array array, int index)
    {
      CopyTo((T[])array, index);
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        return true;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return gate;
      }
    }
    #endregion
  }
}