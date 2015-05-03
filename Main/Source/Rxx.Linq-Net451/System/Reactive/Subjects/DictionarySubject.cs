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
  /// Represents an object that is a dictionary as well as an observable sequence of collection notifications and observer of collection modifications.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
  /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
  /// <remarks>
  /// <para>
  /// <see cref="DictionarySubject{TKey,TValue}"/> implements <see cref="INotifyCollectionChanged"/> and behaves similar to <see cref="ObservableCollection{TValue}"/>, 
  /// so it can be bound directly to an <strong>ItemsPresenter</strong> or a derived type in WPF, Silverlight and Windows Phone.
  /// </para> 
  /// <alert type="tip">
  /// The enumerator that is returned by <see cref="GetEnumerator"/> blocks all methods on the <see cref="DictionarySubject{TKey,TValue}"/>
  /// until the enumeration has completed.  For a snapshot behavior instead, simply call <see cref="Enumerable.ToDictionary{TValue,TKey}(IEnumerable{TValue},Func{TValue,TKey})"/>
  /// or <see cref="Enumerable.ToList"/> to collect the items as fast as possible and then enumerate the new collection.
  /// </alert>
  /// </remarks>
  /// <threadsafety instance="true" />
#else
  /// <summary>
  /// Represents an object that is a dictionary as well as an observable sequence of collection notifications and observer of collection modifications.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
  /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
  /// <remarks>
  /// <alert type="tip">
  /// The enumerator that is returned by <see cref="GetEnumerator"/> blocks all methods on the <see cref="DictionarySubject{TKey,TValue}"/>
  /// until the enumeration has completed.  For a snapshot behavior instead, simply call <see cref="Enumerable.ToDictionary{TValue,TKey}(IEnumerable{TValue},Func{TValue,TKey})"/>
  /// or <see cref="Enumerable.ToList"/> to collect the items as fast as possible and then enumerate the new collection.
  /// </alert>
  /// </remarks>
  /// <threadsafety instance="true" />
#endif
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification = "It's also a subject.")]
  public sealed partial class DictionarySubject<TKey, TValue> : IDictionarySubject<TKey, TValue>, INotifyPropertyChanged
  {
    #region Public Properties
    /// <summary>
    /// Gets the <see cref="IEqualityComparer{TKey}"/> that is used to determine equality of keys for the dictionary.
    /// </summary>
    /// <returns>The <see cref="IEqualityComparer{TKey}"/> generic interface implementation that is used to determine 
    /// equality of keys for the current dictionary and to provide hash values for the keys.</returns>
    public IEqualityComparer<TKey> Comparer
    {
      get
      {
        Contract.Ensures(Contract.Result<IEqualityComparer<TKey>>() != null);

        lock (gate)
        {
          EnsureNotDisposed();

          var comparer = dictionary.Comparer;

          Contract.Assume(comparer != null);

          return comparer;
        }
      }
    }

    /// <summary>
    /// Gets a collection containing a snapshot of the keys in the dictionary.
    /// </summary>
    /// <returns>A collection containing a snapshot of the keys in the dictionary.</returns>
    public ICollection<TKey> Keys
    {
      get
      {
        Contract.Ensures(Contract.Result<ICollection<TKey>>() != null);

        lock (gate)
        {
          EnsureNotDisposed();
          EnsureNotFaulted();

          return dictionary.Keys.ToList().AsReadOnly();
        }
      }
    }

    /// <summary>
    /// Gets a collection containing a snapshot of the values in the dictionary.
    /// </summary>
    /// <returns>A collection containing a snapshot of the values in the dictionary.</returns>
    public ICollection<TValue> Values
    {
      get
      {
        Contract.Ensures(Contract.Result<ICollection<TValue>>() != null);

        lock (gate)
        {
          EnsureNotDisposed();
          EnsureNotFaulted();

          return dictionary.Values.ToList().AsReadOnly();
        }
      }
    }

    /// <summary>
    /// Gets the number of key/value pairs currently contained in the dictionary.
    /// </summary>
    public int Count
    {
      get
      {
        lock (gate)
        {
          EnsureNotDisposed();
          EnsureNotFaulted();

          return dictionary.Count;
        }
      }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets or sets the value associated with the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <returns>The value with the specified <paramref name="key"/>.</returns>
    public TValue this[TKey key]
    {
      get
      {
        lock (gate)
        {
          EnsureNotDisposed();
          EnsureNotFaulted();

          return dictionary[key].Value;
        }
      }
      set
      {
        lock (gate)
        {
          EnsureNotDisposed();

          if (EnsureNotStopped())
          {
            SetInternal(Pair(key, value));
          }
        }
      }
    }
    #endregion

    #region Private / Protected
    private readonly object gate = new object();
    private readonly Subject<CollectionNotification<KeyValuePair<TKey, TValue>>> subject = new Subject<CollectionNotification<KeyValuePair<TKey, TValue>>>();
    private readonly IndexedDictionary dictionary;
    private readonly IDisposable subscription;
    private Exception exception;
    private bool isStopped, isDisposed;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="DictionarySubject{TKey,TValue}" /> class.
    /// </summary>
    public DictionarySubject()
    {
      dictionary = new IndexedDictionary();
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="DictionarySubject{TKey,TValue}" /> class.
    /// </summary>
    /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys, 
    /// or <see langword="null" /> to use the default <see cref="EqualityComparer{TKey}"/> for the type of the key.</param>
    public DictionarySubject(IEqualityComparer<TKey> comparer)
    {
      dictionary = new IndexedDictionary(comparer);
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="DictionarySubject{TKey,TValue}" /> class.
    /// </summary>
    /// <param name="dictionary">The dictionary from which elements are copied to the new dictionary.</param>
    public DictionarySubject(IDictionary<TKey, TValue> dictionary)
    {
      Contract.Requires(dictionary != null);

      this.dictionary = new IndexedDictionary();

      dictionary.ForEach(this.dictionary.Add);
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="DictionarySubject{TKey,TValue}" /> class.
    /// </summary>
    /// <param name="dictionary">The dictionary from which elements are copied to the new dictionary.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys, 
    /// or <see langword="null" /> to use the default <see cref="EqualityComparer{TKey}"/> for the type of the key.</param>
    public DictionarySubject(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
    {
      Contract.Requires(dictionary != null);

      this.dictionary = new IndexedDictionary(comparer);

      dictionary.ForEach(this.dictionary.Add);
    }

    internal DictionarySubject(IDisposable compositedDisposable)
      : this()
    {
      Contract.Requires(compositedDisposable != null);

      subscription = compositedDisposable;
    }

    internal DictionarySubject(IDisposable compositedDisposable, IEqualityComparer<TKey> comparer)
      : this(comparer)
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
      Contract.Invariant(dictionary != null);
    }

    private static KeyValuePair<TKey, TValue> Pair(TKey key, TValue value)
    {
      return new KeyValuePair<TKey, TValue>(key, value);
    }

    /// <summary>
    /// Returns an observable sequence of collection notifications that represent changes to the dictionary.
    /// </summary>
    /// <returns>
    /// An observable sequence of collection notifications that represent changes to the dictionary.
    /// </returns>
    public IObservable<CollectionNotification<KeyValuePair<TKey, TValue>>> Changes()
    {
      return Observable.Create<CollectionNotification<KeyValuePair<TKey, TValue>>>(
        observer => Subscribe(observer, startWithExisting: false));
    }

    /// <summary>
    /// Returns an observable sequence of collection notifications that represent changes to the specified <paramref name="key"/>, 
    /// starting with the existing value if the dictionary already contains the <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key for which changes are to be observed.</param>
    /// <returns>
    /// An observable sequence of collection notifications that represent changes to the specified <paramref name="key"/>, 
    /// starting with the existing value if the dictionary already contains the <paramref name="key"/>.
    /// </returns>
    public IObservable<CollectionNotification<KeyValuePair<TKey, TValue>>> Changes(TKey key)
    {
      return Observable.Create<CollectionNotification<KeyValuePair<TKey, TValue>>>(
        observer =>
        {
          lock (gate)
          {
            if (isDisposed)
            {
              return Observable
                .Throw<CollectionNotification<KeyValuePair<TKey, TValue>>>(new ObjectDisposedException("DictionarySubject<TKey, TValue>"))
                .SubscribeSafe(observer);
            }

            if (exception != null)
            {
              return Observable
                .Throw<CollectionNotification<KeyValuePair<TKey, TValue>>>(exception)
                .SubscribeSafe(observer);
            }

            if (dictionary.Contains(key))
            {
              observer.OnNext(CollectionNotification.CreateExists(dictionary[key]));
            }

            var comparer = dictionary.Comparer;

            return subject
              .Where(change => change.HasValue && comparer.Equals(change.Value.Key, key))
              .SubscribeSafe(observer);
          }
        });
    }

    /// <summary>
    /// Notifies the subject that an observer is to receive collection notifications, starting with a notification
    /// that contains a snapshot of the existing values in the dictionary.
    /// </summary>
    /// <param name="observer">The object that is to receive collection notifications.</param>
    /// <returns>The observer's interface that enables resources to be disposed.</returns>
    public IDisposable Subscribe(IObserver<CollectionNotification<KeyValuePair<TKey, TValue>>> observer)
    {
      return Subscribe(observer, startWithExisting: true);
    }

    private IDisposable Subscribe(IObserver<CollectionNotification<KeyValuePair<TKey, TValue>>> observer, bool startWithExisting)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<IDisposable>() != null);

      lock (gate)
      {
        if (isDisposed)
        {
          return Observable
            .Throw<CollectionNotification<KeyValuePair<TKey, TValue>>>(new ObjectDisposedException("DictionarySubject<TKey, TValue>"))
            .SubscribeSafe(observer);
        }

        if (exception != null)
        {
          return Observable
            .Throw<CollectionNotification<KeyValuePair<TKey, TValue>>>(exception)
            .SubscribeSafe(observer);
        }

        if (startWithExisting)
        {
          var clonedList = dictionary.ToList().AsReadOnly();

          observer.OnNext(CollectionNotification.CreateDictionaryExists(clonedList));
        }

        return subject.SubscribeSafe(observer);
      }
    }

    /// <summary>
    /// Changes the dictionary according to the specified collection modification.
    /// </summary>
    /// <param name="value">A modification that indicates how the dictionary must be changed.</param>
    public void OnNext(CollectionModification<KeyValuePair<TKey, TValue>> value)
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
          IList<KeyValuePair<TKey, TValue>> pairs;

          switch (value.Kind)
          {
            case CollectionModificationKind.Add:
              // value.Accept(this) is not used for additions because it calls the ICollection<KeyValuePair<TKey, TValue>>.Add implementation, which throws if the key already exists.
              pairs = value.Values;

              for (int i = 0; i < pairs.Count; i++)
              {
                SetInternal(pairs[i]);
              }
              break;
            case CollectionModificationKind.Remove:
              // value.Accept(this) is not used for removals because it calls the ICollection<KeyValuePair<TKey, TValue>>.Remove implementation, which compares keys and also compares values.
              pairs = value.Values;

              for (int i = 0; i < pairs.Count; i++)
              {
                var key = pairs[i].Key;

                if (dictionary.Contains(key))
                {
                  RemoveInternal(key);
                }
              }
              break;
            case CollectionModificationKind.Clear:
              ClearInternal();
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
    /// Gets the value associated with the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">When this method returns, the value associated with the specified <paramref name="key"/>, 
    /// if the key is found; otherwise, the default value for the type of the value parameter.</param>
    /// <returns><see langword="True"/> if the dictionary contains an element with the specified <paramref name="key"/>; otherwise, <see langword="false"/>.</returns>
    [ContractVerification(false)]
    public bool TryGetValue(TKey key, out TValue value)
    {
      lock (gate)
      {
        EnsureNotDisposed();
        EnsureNotFaulted();

        if (dictionary.Contains(key))
        {
          value = dictionary[key].Value;
          return true;
        }
        else
        {
          value = default(TValue);
          return false;
        }
      }
    }

    /// <summary>
    /// Determines whether the dictionary contains an element with the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key to locate in the dictionary.</param>
    /// <returns><see langword="True" /> if the dictionary contains an element with the <paramref name="key"/>; otherwise, <see langword="false" />.</returns>
    [ContractVerification(false)]
    public bool ContainsKey(TKey key)
    {
      lock (gate)
      {
        EnsureNotDisposed();
        EnsureNotFaulted();

        return dictionary.Contains(key);
      }
    }

    /// <summary>
    /// Determines whether the dictionary contains a specific <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to locate in the dictionary.  The value can be <see langword="null" /> for reference types.</param>
    /// <returns><see langword="True"/> if the dictionary contains an element with the specified <paramref name="value"/>; otherwise, <see langword="false"/>.</returns>
    public bool ContainsValue(TValue value)
    {
      lock (gate)
      {
        EnsureNotDisposed();
        EnsureNotFaulted();

        return dictionary.Values.Contains(value);
      }
    }

    private void SetInternal(KeyValuePair<TKey, TValue> newPair)
    {
      if (dictionary.Contains(newPair.Key))
      {
        int index = dictionary.IndexOf(newPair.Key);

        Contract.Assume(index >= 0);

        var oldPair = dictionary[index];

        dictionary[index] = newPair;

        OnPropertyChanged(new PropertyChangedEventArgs("Values"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

#if !PORT_40
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newPair, oldPair, index));
#endif

        subject.OnNext(CollectionNotification.CreateOnReplaced(oldPair, newPair));
      }
      else
      {
        AddInternal(newPair);
      }
    }

    private void AddInternal(KeyValuePair<TKey, TValue> pair)
    {
#if !PORT_40
      int index = dictionary.Count;
#endif

      dictionary.Add(pair);

      OnPropertyChanged(new PropertyChangedEventArgs("Count"));
      OnPropertyChanged(new PropertyChangedEventArgs("Keys"));
      OnPropertyChanged(new PropertyChangedEventArgs("Values"));
      OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

#if !PORT_40
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair, index));
#endif

      subject.OnNext(CollectionNotification.CreateOnAdded(pair));
    }

    private void RemoveInternal(TKey key)
    {
      Contract.Assume(dictionary.Contains(key));

      var index = dictionary.IndexOf(key);

      Contract.Assume(index >= 0);

      var pair = dictionary[index];

      dictionary.RemoveAt(index);

      OnPropertyChanged(new PropertyChangedEventArgs("Count"));
      OnPropertyChanged(new PropertyChangedEventArgs("Keys"));
      OnPropertyChanged(new PropertyChangedEventArgs("Values"));
      OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

#if !PORT_40
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, pair, index));
#endif

      subject.OnNext(CollectionNotification.CreateOnRemoved(pair));
    }

    private void ClearInternal()
    {
      dictionary.Clear();

      OnPropertyChanged(new PropertyChangedEventArgs("Count"));
      OnPropertyChanged(new PropertyChangedEventArgs("Keys"));
      OnPropertyChanged(new PropertyChangedEventArgs("Values"));
      OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

#if !PORT_40
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
#endif

      subject.OnNext(CollectionNotification.CreateOnCleared<KeyValuePair<TKey, TValue>>());
    }

    /// <summary>
    /// Adds an element with the provided <paramref name="key"/> and <paramref name="value"/> to the dictionary.
    /// </summary>
    /// <param name="key">The object to use as the key of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    public void Add(TKey key, TValue value)
    {
      lock (gate)
      {
        EnsureNotDisposed();

        if (EnsureNotStopped())
        {
          AddInternal(Pair(key, value));
        }
      }
    }

    /// <summary>
    /// Removes the element with the specified <paramref name="key"/> from the dictionary.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns><see langword="True" /> if the element is successfully removed; otherwise, <see langword="false" />.
    /// This method also returns <see langword="false"/> if <paramref name="key"/> was not found in the dictionary.</returns>
    public bool Remove(TKey key)
    {
      lock (gate)
      {
        EnsureNotDisposed();

        if (EnsureNotStopped())
        {
          if (dictionary.Contains(key))
          {
            RemoveInternal(key);

            return true;
          }
        }
      }

      return false;
    }

    [ContractVerification(false)]
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
      lock (gate)
      {
        EnsureNotDisposed();
        EnsureNotFaulted();

        return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);
      }
    }

    [ContractVerification(false)]
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
      lock (gate)
      {
        EnsureNotDisposed();

        if (EnsureNotStopped())
        {
          AddInternal(item);
        }
      }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
      lock (gate)
      {
        EnsureNotDisposed();

        if (EnsureNotStopped())
        {
          if (dictionary.Contains(item.Key) && EqualityComparer<TValue>.Default.Equals(item.Value, dictionary[item.Key].Value))
          {
            RemoveInternal(item.Key);

            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Removes all key/value pairs from the dictionary.
    /// </summary>
    [ContractVerification(false)]
    public void Clear()
    {
      lock (gate)
      {
        EnsureNotDisposed();

        if (EnsureNotStopped())
        {
          ClearInternal();
        }
      }
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      lock (gate)
      {
        EnsureNotDisposed();
        EnsureNotFaulted();

        Contract.Assume(arrayIndex + dictionary.Count <= array.Length);

        ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the key/value pairs in the dictionary.
    /// </summary>
    /// <remarks>
    /// The dictionary is locked for the entire duration while enumerating.  Any collection modifications that are received 
    /// during the enumeration will be blocked.  When the enumeration has completed, all previous modifications will be 
    /// allowed to acquire the lock and mutate the dictionary.  For this reason it is best to enumerate quickly.  For example, 
    /// you could call the <see cref="System.Linq.Enumerable.ToList"/> or <see cref="System.Linq.Enumerable.ToDictionary{TValue,TKey}(IEnumerable{TValue},Func{TValue,TKey})"/> 
    /// extension method to take a snapshot of the dictionary, then perform work by enumerating the snapshot while the subject 
    /// is free to accept collection modifications.
    /// </remarks>
    /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate through the dictionary.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      lock (gate)
      {
        EnsureNotDisposed();
        EnsureNotFaulted();

        foreach (var item in dictionary)
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
        throw new ObjectDisposedException("DictionarySubject<TKey, TValue>");
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
    /// Occurs when a value is added, removed, changed, moved, or the entire dictionary is refreshed.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;
#endif

    private PropertyChangedEventHandler propertyChanged;
    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
      add
      {
        propertyChanged += value;
      }
      remove
      {
        propertyChanged -= value;
      }
    }

#if !PORT_40
    private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      Contract.Requires(e != null);

      var handler = CollectionChanged;

      if (handler != null)
      {
        handler(this, e);
      }
    }
#endif

    private void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      Contract.Requires(e != null);

      var handler = propertyChanged;

      if (handler != null)
      {
        handler(this, e);
      }
    }
    #endregion
  }
}