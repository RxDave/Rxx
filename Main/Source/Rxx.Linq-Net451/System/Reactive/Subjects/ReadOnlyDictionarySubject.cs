using System.Collections.Generic;
#if !PORT_40
using System.Collections.Specialized;
#endif
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace System.Reactive.Subjects
{
  /// <summary>
  /// Provides a read-only wrapper around an <see cref="IDictionarySubject{TKey,TValue}"/>.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
  /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
  /// <threadsafety instance="true" />
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification = "It's also a subject.")]
  public sealed class ReadOnlyDictionarySubject<TKey, TValue> : IDictionarySubject<TKey, TValue>, INotifyPropertyChanged
  {
    #region Public Properties
    /// <summary>
    /// Gets a collection containing a snapshot of the keys in the dictionary.
    /// </summary>
    /// <returns>A collection containing a snapshot of the keys in the dictionary.</returns>
    public ICollection<TKey> Keys
    {
      get
      {
        return subject.Keys;
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
        return subject.Values;
      }
    }

    /// <summary>
    /// Gets the number of key/value pairs currently contained in the dictionary.
    /// </summary>
    public int Count
    {
      get
      {
        return subject.Count;
      }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets the value associated with the specified <paramref name="key"/>.  Setting this property is not supported.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <returns>The value with the specified <paramref name="key"/>.</returns>
    /// <exception cref="NotSupportedException">Attempted to set an item in a read-only dictionary.</exception>
    public TValue this[TKey key]
    {
      get
      {
        return subject[key];
      }
      set
      {
        throw new NotSupportedException();
      }
    }
    #endregion

    #region Private / Protected
    private readonly IDictionarySubject<TKey, TValue> subject;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="ReadOnlyDictionarySubject{TKey,TValue}" /> class.
    /// </summary>
    /// <param name="subject">The subject to be decorated with a read-only wrapper.</param>
    public ReadOnlyDictionarySubject(IDictionarySubject<TKey, TValue> subject)
    {
      Contract.Requires(subject != null);

      this.subject = subject;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(subject != null);
    }

    /// <summary>
    /// Returns an observable sequence of collection notifications that represent changes to the dictionary.
    /// </summary>
    /// <returns>
    /// An observable sequence of collection notifications that represent changes to the dictionary.
    /// </returns>
    public IObservable<CollectionNotification<KeyValuePair<TKey, TValue>>> Changes()
    {
      return subject.Changes();
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
      return subject.Changes(key);
    }

    /// <summary>
    /// Notifies the subject that an observer is to receive collection notifications.
    /// </summary>
    /// <param name="observer">The object that is to receive collection notifications.</param>
    /// <returns>The observer's interface that enables resources to be disposed.</returns>
    public IDisposable Subscribe(IObserver<CollectionNotification<KeyValuePair<TKey, TValue>>> observer)
    {
      return subject.Subscribe(observer);
    }

    /// <summary>
    /// Changes the dictionary according to the specified collection modification.  This method is not supported.
    /// </summary>
    /// <param name="value">A modification that indicates how the dictionary must be changed.</param>
    /// <exception cref="NotSupportedException">Attempted to set an item in a read-only dictionary.</exception>
    public void OnNext(CollectionModification<KeyValuePair<TKey, TValue>> value)
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Terminates the subject with an error condition.  This method is not supported.
    /// </summary>
    /// <param name="error">An object that provides additional information about the error.</param>
    /// <exception cref="NotSupportedException">Attempted to set an item in a read-only dictionary.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#",
      Justification = "Windows Phone defines IObserver<T>.OnError with the name \"exception\" for the \"error\" parameter, but it's better to" +
                      "leave it as \"error\" here so that callers can use the same named parameter across all platforms.")]
    public void OnError(Exception error)
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Notifies the subject to stop accepting collection modifications.  This method is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">Attempted to set an item in a read-only dictionary.</exception>
    public void OnCompleted()
    {
      throw new NotSupportedException();
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
      return subject.TryGetValue(key, out value);
    }

    /// <summary>
    /// Determines whether the dictionary contains an element with the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key to locate in the dictionary.</param>
    /// <returns><see langword="True" /> if the dictionary contains an element with the <paramref name="key"/>; otherwise, <see langword="false" />.</returns>
    [ContractVerification(false)]
    public bool ContainsKey(TKey key)
    {
      return subject.ContainsKey(key);
    }

    /// <summary>
    /// Adds an element with the provided <paramref name="key"/> and <paramref name="value"/> to the dictionary.  This method is not supported.
    /// </summary>
    /// <param name="key">The object to use as the key of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    /// <exception cref="NotSupportedException">Attempted to set an item in a read-only dictionary.</exception>
    public void Add(TKey key, TValue value)
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Removes the element with the specified <paramref name="key"/> from the dictionary.  This method is not supported.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns><see langword="True" /> if the element is successfully removed; otherwise, <see langword="false" />.
    /// This method also returns <see langword="false"/> if <paramref name="key"/> was not found in the dictionary.</returns>
    /// <exception cref="NotSupportedException">Attempted to set an item in a read-only dictionary.</exception>
    public bool Remove(TKey key)
    {
      throw new NotSupportedException();
    }

    [ContractVerification(false)]
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
      return ((ICollection<KeyValuePair<TKey, TValue>>)subject).Contains(item);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
      throw new NotSupportedException();
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Removes all key/value pairs from the dictionary.  This method is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">Attempted to set an item in a read-only dictionary.</exception>
    [ContractVerification(false)]
    public void Clear()
    {
      throw new NotSupportedException();
    }

    [ContractVerification(false)]
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      ((ICollection<KeyValuePair<TKey, TValue>>)subject).CopyTo(array, arrayIndex);
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
      return subject.GetEnumerator();
    }

    Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
    {
      return subject.GetEnumerator();
    }

    /// <summary>
    /// Unsubscribes all observers and releases resources. 
    /// </summary>
    public void Dispose()
    {
      subject.Dispose();
    }
    #endregion

    #region Events
#if !PORT_40
    /// <summary>
    /// Occurs when a value is added, removed, changed, moved, or the entire dictionary is refreshed.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged
    {
      add
      {
        subject.CollectionChanged += value;
      }
      remove
      {
        subject.CollectionChanged -= value;
      }
    }
#endif

    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
      add
      {
        ((INotifyPropertyChanged)subject).PropertyChanged += value;
      }
      remove
      {
        ((INotifyPropertyChanged)subject).PropertyChanged -= value;
      }
    }
    #endregion
  }
}