using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a key.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IOrderedObservable{TSource}"/> whose elements are sorted according to a key.</returns>
    public static IOrderedObservable<TSource> OrderBy<TSource, TKey>(
      this IObservable<TSource> source,
      Func<TSource, TKey> keySelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<TSource>>() != null);

      return new OrderedObservable<TSource, TKey>(source, keySelector, comparer: null, descending: false);
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IOrderedObservable{TSource}"/> whose elements are sorted according to a key.</returns>
    public static IOrderedObservable<TSource> OrderBy<TSource, TKey>(
      this IObservable<TSource> source,
      Func<TSource, TKey> keySelector,
      IComparer<TKey> comparer)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<TSource>>() != null);

      return new OrderedObservable<TSource, TKey>(source, keySelector, comparer, descending: false);
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to a key.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IOrderedObservable{TSource}"/> whose elements are sorted in descending order according to a key.</returns>
    public static IOrderedObservable<TSource> OrderByDescending<TSource, TKey>(
      this IObservable<TSource> source,
      Func<TSource, TKey> keySelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<TSource>>() != null);

      return new OrderedObservable<TSource, TKey>(source, keySelector, comparer: null, descending: true);
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IOrderedObservable{TSource}"/> whose elements are sorted in descending order according to a key.</returns>
    public static IOrderedObservable<TSource> OrderByDescending<TSource, TKey>(
      this IObservable<TSource> source,
      Func<TSource, TKey> keySelector,
      IComparer<TKey> comparer)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<TSource>>() != null);

      return new OrderedObservable<TSource, TKey>(source, keySelector, comparer, descending: true);
    }

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IOrderedObservable{TSource}"/> whose elements are sorted according to a key.</returns>
    public static IOrderedObservable<TSource> ThenBy<TSource, TKey>(
      this IOrderedObservable<TSource> source,
      Func<TSource, TKey> keySelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<TSource>>() != null);

      return source.CreateOrderedObservable(keySelector, comparer: null, descending: false);
    }

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in ascending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IOrderedObservable{TSource}"/> whose elements are sorted according to a key.</returns>
    public static IOrderedObservable<TSource> ThenBy<TSource, TKey>(
      this IOrderedObservable<TSource> source,
      Func<TSource, TKey> keySelector,
      IComparer<TKey> comparer)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<TSource>>() != null);

      return source.CreateOrderedObservable(keySelector, comparer, descending: false);
    }

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in descending order according to a key.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IOrderedObservable{TSource}"/> whose elements are sorted in descending order according to a key.</returns>
    public static IOrderedObservable<TSource> ThenByDescending<TSource, TKey>(
      this IOrderedObservable<TSource> source,
      Func<TSource, TKey> keySelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<TSource>>() != null);

      return source.CreateOrderedObservable(keySelector, comparer: null, descending: true);
    }

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in descending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IOrderedObservable{TSource}"/> whose elements are sorted in descending order according to a key.</returns>
    public static IOrderedObservable<TSource> ThenByDescending<TSource, TKey>(
      this IOrderedObservable<TSource> source,
      Func<TSource, TKey> keySelector,
      IComparer<TKey> comparer)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<TSource>>() != null);

      return source.CreateOrderedObservable(keySelector, comparer, descending: true);
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to the times at which corresponding observable sequences complete.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TOther">The type of the elements in the observable returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function that returns an observable for an element indicating the time at which that element should appear in the ordering, by completing.</param>
    /// <returns>An <see cref="IOrderedObservable{TSource}"/> whose elements are sorted according to the times at which corresponding observable sequences complete.</returns>
    public static IOrderedObservable<TSource> OrderBy<TSource, TOther>(
      this IObservable<TSource> source,
      Func<TSource, IObservable<TOther>> keySelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<TSource>>() != null);

      return new OrderedObservable<TSource, TOther>(source, keySelector, descending: false);
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to the times at which corresponding observable sequences complete.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TOther">The type of the elements in the observable returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function that returns an observable for an element indicating the time at which that element should appear in the ordering, by completing.</param>
    /// <returns>An <see cref="IOrderedObservable{TSource}"/> whose elements are sorted in descending order according to the times at which corresponding observable sequences complete.</returns>
    public static IOrderedObservable<TSource> OrderByDescending<TSource, TOther>(
      this IObservable<TSource> source,
      Func<TSource, IObservable<TOther>> keySelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<TSource>>() != null);

      return new OrderedObservable<TSource, TOther>(source, keySelector, descending: true);
    }

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in ascending order according to the times at which corresponding observable sequences complete.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TOther">The type of the elements in the observable returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function that returns an observable for an element indicating the time at which that element should appear in the ordering, by completing.</param>
    /// <returns>An <see cref="IOrderedObservable{TSource}"/> whose elements are sorted according to the times at which corresponding observable sequences complete..</returns>
    public static IOrderedObservable<TSource> ThenBy<TSource, TOther>(
      this IOrderedObservable<TSource> source,
      Func<TSource, IObservable<TOther>> keySelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<TSource>>() != null);

      return source.CreateOrderedObservable(keySelector, descending: false);
    }

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in descending order according to the times at which corresponding observable sequences complete.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TOther">The type of the elements in the observable returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function that returns an observable for an element indicating the time at which that element should appear in the ordering, by completing.</param>
    /// <returns>An <see cref="IOrderedObservable{TSource}"/> whose elements are sorted in descending order according to the times at which corresponding observable sequences complete..</returns>
    public static IOrderedObservable<TSource> ThenByDescending<TSource, TOther>(
      this IOrderedObservable<TSource> source,
      Func<TSource, IObservable<TOther>> keySelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Ensures(Contract.Result<IOrderedObservable<TSource>>() != null);

      return source.CreateOrderedObservable(keySelector, descending: true);
    }
  }
}