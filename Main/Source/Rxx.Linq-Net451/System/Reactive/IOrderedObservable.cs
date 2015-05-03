using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  /// <summary>
  /// Represents a sorted sequence.
  /// </summary>
  /// <typeparam name="T">The object that provides notification information.</typeparam>
  [ContractClass(typeof(IOrderedObservableContract<>))]
  public interface IOrderedObservable<T> : IObservable<T>
  {
    /// <summary>
    /// Performs a subsequent ordering on the elements of an <see cref="IOrderedObservable{T}"/> according to a key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key produced by <paramref name="keySelector"/>.</typeparam>
    /// <param name="keySelector">The function used to extract the key for each element.</param>
    /// <param name="comparer">The <see cref="IComparer{TKey}"/> used to compare keys for placement in the returned sequence.</param>
    /// <param name="descending"><see langword="True"/> to sort the elements in descending order; <see langword="false"/> to sort the 
    /// elements in ascending order.</param>
    /// <returns>An <see cref="IOrderedObservable{T}"/> whose elements are sorted according to a key.</returns>
    IOrderedObservable<T> CreateOrderedObservable<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer, bool descending);

    /// <summary>
    /// Performs a subsequent ordering on the elements of an <see cref="IOrderedObservable{T}"/> according to other observable sequences.
    /// </summary>
    /// <typeparam name="TOther">The type of the elements in the observable returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="keySelector">A function that returns an observable for an element indicating the time at which that element should appear in the ordering, by completing.</param>
    /// <param name="descending"><see langword="True"/> to sort the elements in descending order; <see langword="false"/> to sort the 
    /// elements in ascending order.</param>
    /// <returns>An <see cref="IOrderedObservable{T}"/> whose elements are sorted according to other observable sequences.</returns>
    IOrderedObservable<T> CreateOrderedObservable<TOther>(Func<T, IObservable<TOther>> keySelector, bool descending);
  }

  [ContractClassFor(typeof(IOrderedObservable<>))]
  internal abstract class IOrderedObservableContract<T> : IOrderedObservable<T>
  {
    public IOrderedObservable<T> CreateOrderedObservable<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer, bool descending)
    {
      Contract.Requires(keySelector != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<T>>() != null);
      return null;
    }

    public IOrderedObservable<T> CreateOrderedObservable<TOther>(Func<T, IObservable<TOther>> keySelector, bool descending)
    {
      Contract.Requires(keySelector != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<T>>() != null);
      return null;
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
      return null;
    }
  }
}