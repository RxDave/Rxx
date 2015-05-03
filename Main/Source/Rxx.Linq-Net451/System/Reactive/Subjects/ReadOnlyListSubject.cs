using System.Collections.Generic;
#if !PORT_40
using System.Collections.Specialized;
#endif
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace System.Reactive.Subjects
{
  /// <summary>
  /// Provides a read-only wrapper around an <see cref="IListSubject{T}"/>.
  /// </summary>
  /// <typeparam name="T">The type of the elements in the collection.</typeparam>
  /// <threadsafety instance="true" />
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification = "It's a list, not just a collection.  It's also a subject.")]
  public sealed class ReadOnlyListSubject<T> : IListSubject<T>, INotifyPropertyChanged
  {
    #region Public Properties
    /// <summary>
    /// Gets the number of elements currently contained in the list.
    /// </summary>
    public int Count
    {
      get
      {
        return subject.Count;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the list is read-only.
    /// </summary>
    /// <value>Always returns <see langword="true" />.</value>
    public bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets the element at the specified index.  Setting this property is not supported.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="NotSupportedException">Attempted to set an item in a read-only list.</exception>
    [ContractVerification(false)]
    public T this[int index]
    {
      get
      {
        return subject[index];
      }
      set
      {
        throw new NotSupportedException();
      }
    }
    #endregion

    #region Private / Protected
    private readonly IListSubject<T> subject;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="ReadOnlyListSubject{T}" /> class.
    /// </summary>
    /// <param name="subject">The subject to be decorated with a read-only wrapper.</param>
    public ReadOnlyListSubject(IListSubject<T> subject)
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
    /// Returns an observable sequence of collection notifications that represent changes to the list.
    /// </summary>
    /// <returns>An observable sequence of collection notifications that represent changes to the list.</returns>
    public IObservable<CollectionNotification<T>> Changes()
    {
      return subject.Changes();
    }

    /// <summary>
    /// Notifies the subject that an observer is to receive collection notifications, starting with a notification
    /// that contains a snapshot of the existing values in the list.
    /// </summary>
    /// <param name="observer">The object that is to receive collection notifications.</param>
    /// <returns>The observer's interface that enables resources to be disposed.</returns>
    public IDisposable Subscribe(IObserver<CollectionNotification<T>> observer)
    {
      return subject.Subscribe(observer);
    }

    /// <summary>
    /// Changes the list according to the specified collection modification.  This method is not supported.
    /// </summary>
    /// <param name="value">A modification that indicates how the list must be changed.</param>
    /// <exception cref="NotSupportedException">Attempted to modify a read-only list.</exception>
    public void OnNext(CollectionModification<T> value)
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Terminates the subject with an error condition.  This method is not supported.
    /// </summary>
    /// <param name="error">An object that provides additional information about the error.</param>
    /// <exception cref="NotSupportedException">Attempted to modify a read-only list.</exception>
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
    /// <exception cref="NotSupportedException">Attempted to modify a read-only list.</exception>
    public void OnCompleted()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Determines whether the list contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the list.</param>
    /// <returns><see langword="True"/> if <paramref name="item"/> is found in the list; otherwise, <see langword="false"/></returns>
    [ContractVerification(false)]
    public bool Contains(T item)
    {
      return subject.Contains(item);
    }

    /// <summary>
    /// Determines the index of a specific item in the list.
    /// </summary>
    /// <param name="item">The object to locate in the list.</param>
    /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
    [ContractVerification(false)]
    public int IndexOf(T item)
    {
      return subject.IndexOf(item);
    }

    /// <summary>
    /// Adds an item to the list.  This method is not supported.
    /// </summary>
    /// <param name="item">The object to add to the list.</param>
    /// <exception cref="NotSupportedException">Attempted to modify a read-only list.</exception>
    public void Add(T item)
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Inserts an item to the list at the specified index.  This method is not supported.
    /// </summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert into the list.</param>
    /// <exception cref="NotSupportedException">Attempted to modify a read-only list.</exception>
    public void Insert(int index, T item)
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the list.  This method is not supported.
    /// </summary>
    /// <param name="item">The object to remove from the list.</param>
    /// <returns><see langword="True" /> if <paramref name="item"/> was successfully removed from the list; otherwise, <see langword="false" />.
    /// This method also returns <see langword="false" /> if <paramref name="item"/> is not found in the list.</returns>
    /// <exception cref="NotSupportedException">Attempted to modify a read-only list.</exception>
    public bool Remove(T item)
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Removes the list item at the specified index.  This method is not supported.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    /// <exception cref="NotSupportedException">Attempted to modify a read-only list.</exception>
    public void RemoveAt(int index)
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Removes all items from the list.  This method is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">Attempted to modify a read-only list.</exception>
    public void Clear()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Copies the elements of the list to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from 
    /// the list. The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    [ContractVerification(false)]
    public void CopyTo(T[] array, int arrayIndex)
    {
      subject.CopyTo(array, arrayIndex);
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
    /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
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