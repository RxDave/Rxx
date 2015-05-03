using System.Collections.Generic;
#if !PORT_40
using System.Collections.Specialized;
#endif
using System.Diagnostics.Contracts;

namespace System.Reactive.Subjects
{
  /// <summary>
  /// Represents an object that is a dictionary as well as an observable sequence of collection notifications and observer of collection modifications.
  /// </summary>
  /// <remarks>
  /// <alert type="implement">
  /// The <see cref="IObservable{T}.Subscribe"/> implementation must push a single <see cref="CollectionNotificationKind.Exists"/> notification 
  /// containing the entire dictionary before pushing any change notifications.
  /// </alert>
  /// </remarks>
  /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
  /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification = "It's also a subject.")]
  [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1001:CommasMustBeSpacedCorrectly",
    Justification = "Conditional compilation section.")]
  [ContractClass(typeof(IDictionarySubjectContract<,>))]
  public interface IDictionarySubject<TKey, TValue> : IDictionary<TKey, TValue>, ISubject<CollectionModification<KeyValuePair<TKey, TValue>>, CollectionNotification<KeyValuePair<TKey, TValue>>>, IDisposable
#if !PORT_40
, INotifyCollectionChanged
#endif
  {
    /// <summary>
    /// Returns an observable sequence of collection notifications that represent changes to the dictionary.
    /// </summary>
    /// <returns>
    /// An observable sequence of collection notifications that represent changes to the dictionary.
    /// </returns>
    IObservable<CollectionNotification<KeyValuePair<TKey, TValue>>> Changes();

    /// <summary>
    /// Returns an observable sequence of collection notifications that represent changes to the specified <paramref name="key"/>, 
    /// starting with the existing value if the dictionary already contains the <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key for which changes are to be observed.</param>
    /// <returns>
    /// An observable sequence of collection notifications that represent changes to the specified <paramref name="key"/>, 
    /// starting with the existing value if the dictionary already contains the <paramref name="key"/>.
    /// </returns>
    IObservable<CollectionNotification<KeyValuePair<TKey, TValue>>> Changes(TKey key);
  }

  [ContractClassFor(typeof(IDictionarySubject<,>))]
  internal abstract class IDictionarySubjectContract<TKey, TValue> : IDictionarySubject<TKey, TValue>
  {
    public IObservable<CollectionNotification<KeyValuePair<TKey, TValue>>> Changes()
    {
      Contract.Ensures(Contract.Result<IObservable<CollectionNotification<KeyValuePair<TKey, TValue>>>>() != null);
      return null;
    }

    public IObservable<CollectionNotification<KeyValuePair<TKey, TValue>>> Changes(TKey key)
    {
      Contract.Ensures(Contract.Result<IObservable<CollectionNotification<KeyValuePair<TKey, TValue>>>>() != null);
      return null;
    }

    #region Inherited
    public void Add(TKey key, TValue value)
    {
    }

    public bool ContainsKey(TKey key)
    {
      return false;
    }

    public ICollection<TKey> Keys
    {
      get
      {
        return null;
      }
    }

    public bool Remove(TKey key)
    {
      return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      value = default(TValue);
      return false;
    }

    public ICollection<TValue> Values
    {
      get
      {
        return null;
      }
    }

    public TValue this[TKey key]
    {
      get
      {
        return default(TValue);
      }
      set
      {
      }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      return false;
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      return null;
    }

    public void OnNext(CollectionModification<KeyValuePair<TKey, TValue>> value)
    {
    }

    public IDisposable Subscribe(IObserver<CollectionNotification<KeyValuePair<TKey, TValue>>> observer)
    {
      return null;
    }

    public void Clear()
    {
    }

    public int Count
    {
      get
      {
        return 0;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
    {
      return null;
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void Dispose()
    {
    }

#if !PORT_40
    event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
    {
      add
      {
      }
      remove
      {
      }
    }
#endif
    #endregion
  }
}