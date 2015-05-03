using System.Collections.Generic;
#if !PORT_40
using System.Collections.Specialized;
#endif
using System.Diagnostics.Contracts;

namespace System.Reactive.Subjects
{
  /// <summary>
  /// Represents an object that is a list as well as an observable sequence of collection notifications and observer of collection modifications.
  /// </summary>
  /// <remarks>
  /// <alert type="implement">
  /// The <see cref="IObservable{T}.Subscribe"/> implementation must push a single <see cref="CollectionNotificationKind.Exists"/> notification 
  /// containing the entire list before pushing any change notifications.
  /// </alert>
  /// </remarks>
  /// <typeparam name="T">The type of the elements in the collection.</typeparam>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification = "It's a list, not just a collection.  It's also a subject.")]
  [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1001:CommasMustBeSpacedCorrectly",
    Justification = "Conditional compilation section.")]
  [ContractClass(typeof(IListSubjectContract<>))]
  public interface IListSubject<T> : IList<T>, ISubject<CollectionModification<T>, CollectionNotification<T>>, IDisposable
#if !PORT_40
, INotifyCollectionChanged
#endif
  {
    /// <summary>
    /// Returns an observable sequence of collection notifications that represent changes to the list.
    /// </summary>
    /// <returns>An observable sequence of collection notifications that represent changes to the list.</returns>
    IObservable<CollectionNotification<T>> Changes();
  }

  [ContractClassFor(typeof(IListSubject<>))]
  internal abstract class IListSubjectContract<T> : IListSubject<T>
  {
    public IObservable<CollectionNotification<T>> Changes()
    {
      Contract.Ensures(Contract.Result<IObservable<CollectionNotification<T>>>() != null);
      return null;
    }

    #region Inherited
    public int IndexOf(T item)
    {
      return 0;
    }

    public void Insert(int index, T item)
    {
    }

    public void RemoveAt(int index)
    {
    }

    public T this[int index]
    {
      get
      {
        return default(T);
      }
      set
      {
      }
    }

    public void Add(T item)
    {
    }

    public void Clear()
    {
    }

    public bool Contains(T item)
    {
      return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
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

    public bool Remove(T item)
    {
      return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return null;
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

    public void OnNext(CollectionModification<T> value)
    {
    }

    public IDisposable Subscribe(IObserver<CollectionNotification<T>> observer)
    {
      return null;
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