using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Adds the elements from the specified observable sequence into a <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="source">The sequence from which elements are collected.</param>
    /// <param name="keySelector">A function that maps values to keys.</param>
    /// <returns>A <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> that receives the elements from the specified sequence.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "All disposables are composited by the subject that is returned to the caller.")]
    public static ReadOnlyDictionarySubject<TKey, TValue> ToDictionaryObservable<TKey, TValue>(this IObservable<TValue> source, Func<TValue, TKey> keySelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Ensures(Contract.Result<ReadOnlyDictionarySubject<TKey, TValue>>() != null);

      var subscription = new SingleAssignmentDisposable();

      var dictionary = new DictionarySubject<TKey, TValue>(subscription);

      subscription.Disposable = source.SubscribeSafe(value => dictionary[keySelector(value)] = value, dictionary.OnError, dictionary.OnCompleted);

      return new ReadOnlyDictionarySubject<TKey, TValue>(dictionary);
    }

    /// <summary>
    /// Adds the elements from the specified observable sequence into a <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="source">The sequence from which elements are collected.</param>
    /// <param name="keySelector">A function that maps values to keys.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys.</param>
    /// <returns>A <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> that receives the elements from the specified sequence.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "All disposables are composited by the subject that is returned to the caller.")]
    public static ReadOnlyDictionarySubject<TKey, TValue> ToDictionaryObservable<TKey, TValue>(this IObservable<TValue> source, Func<TValue, TKey> keySelector, IEqualityComparer<TKey> comparer)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<ReadOnlyDictionarySubject<TKey, TValue>>() != null);

      var subscription = new SingleAssignmentDisposable();

      var dictionary = new DictionarySubject<TKey, TValue>(subscription, comparer);

      subscription.Disposable = source.SubscribeSafe(value => dictionary[keySelector(value)] = value, dictionary.OnError, dictionary.OnCompleted);

      return new ReadOnlyDictionarySubject<TKey, TValue>(dictionary);
    }

    /// <summary>
    /// Adds the elements from the specified observable sequence into a <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/>, or removes
    /// individual elements or clears all elements, depending upon the <see cref="CollectionModificationKind"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="source">The sequence from which collection modifications are received.</param>
    /// <returns>A <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> that receives the elements from the specified sequence.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "All disposables are composited by the subject that is returned to the caller.")]
    public static ReadOnlyDictionarySubject<TKey, TValue> ToDictionaryObservable<TKey, TValue>(this IObservable<CollectionModification<KeyValuePair<TKey, TValue>>> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<ReadOnlyDictionarySubject<TKey, TValue>>() != null);

      var subscription = new SingleAssignmentDisposable();

      var dictionary = new DictionarySubject<TKey, TValue>(subscription);

      subscription.Disposable = source.SubscribeSafe(dictionary);

      return new ReadOnlyDictionarySubject<TKey, TValue>(dictionary);
    }

    /// <summary>
    /// Adds the elements from the specified observable sequence into a <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/>, or removes
    /// individual elements or clears all elements, depending upon the <see cref="CollectionModificationKind"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="source">The sequence from which collection modifications are received.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys.</param>
    /// <returns>A <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> that receives the elements from the specified sequence.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "All disposables are composited by the subject that is returned to the caller.")]
    public static ReadOnlyDictionarySubject<TKey, TValue> ToDictionaryObservable<TKey, TValue>(this IObservable<CollectionModification<KeyValuePair<TKey, TValue>>> source, IEqualityComparer<TKey> comparer)
    {
      Contract.Requires(source != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<ReadOnlyDictionarySubject<TKey, TValue>>() != null);

      var subscription = new SingleAssignmentDisposable();

      var dictionary = new DictionarySubject<TKey, TValue>(subscription, comparer);

      subscription.Disposable = source.SubscribeSafe(dictionary);

      return new ReadOnlyDictionarySubject<TKey, TValue>(dictionary);
    }

    /// <summary>
    /// Concurrently populates a <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the default equality comparer for 
    /// <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> is initially populated.</param>
    /// <param name="keySelector">A function that maps values from the <paramref name="existing"/> sequence to keys.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> are received.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyDictionarySubject<TKey, TValue> ToDictionaryObservable<TKey, TValue>(
      this IEnumerable<TValue> existing,
      Func<TValue, TKey> keySelector,
      IObservable<CollectionModification<KeyValuePair<TKey, TValue>>> changes)
    {
      Contract.Requires(existing != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(changes != null);
      Contract.Ensures(Contract.Result<ReadOnlyDictionarySubject<TKey, TValue>>() != null);

      return ToDictionaryObservable(existing, keySelector, changes, EqualityComparer<TKey>.Default);
    }

    /// <summary>
    /// Concurrently populates a <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the specified equality comparer for 
    /// <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> is initially populated.</param>
    /// <param name="keySelector">A function that maps values from the <paramref name="existing"/> sequence to keys.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> are received.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyDictionarySubject<TKey, TValue> ToDictionaryObservable<TKey, TValue>(
      this IEnumerable<TValue> existing,
      Func<TValue, TKey> keySelector,
      IObservable<CollectionModification<KeyValuePair<TKey, TValue>>> changes,
      IEqualityComparer<TKey> comparer)
    {
      Contract.Requires(existing != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(changes != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<ReadOnlyDictionarySubject<TKey, TValue>>() != null);

      return ToDictionaryObservable(existing.ToObservable(PlatformSchedulers.Concurrent), keySelector, changes, comparer);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the default equality comparer for 
    /// <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> is initially populated.</param>
    /// <param name="keySelector">A function that maps values from the <paramref name="existing"/> sequence to keys.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> are received.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyDictionarySubject<TKey, TValue> ToDictionaryObservable<TKey, TValue>(
      this IObservable<TValue> existing,
      Func<TValue, TKey> keySelector,
      IObservable<CollectionModification<KeyValuePair<TKey, TValue>>> changes)
    {
      Contract.Requires(existing != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(changes != null);
      Contract.Ensures(Contract.Result<ReadOnlyDictionarySubject<TKey, TValue>>() != null);

      return ToDictionaryObservable(existing, keySelector, changes, EqualityComparer<TKey>.Default);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the specified equality comparer for 
    /// <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> is initially populated.</param>
    /// <param name="keySelector">A function that maps values from the <paramref name="existing"/> sequence to keys.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> are received.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyDictionarySubject<TKey, TValue> ToDictionaryObservable<TKey, TValue>(
      this IObservable<TValue> existing,
      Func<TValue, TKey> keySelector,
      IObservable<CollectionModification<KeyValuePair<TKey, TValue>>> changes,
      IEqualityComparer<TKey> comparer)
    {
      Contract.Requires(existing != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(changes != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<ReadOnlyDictionarySubject<TKey, TValue>>() != null);

      return ToDictionaryObservable(existing, keySelector, changes, all => all.SelectMany(n => n.ToModifications()), comparer);
    }

    /// <summary>
    /// Concurrently populates a <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the default equality comparer for 
    /// <typeparamref name="TKey"/> and projecting notifications using the specified <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="TSource">The type of objects in the source sequences.</typeparam>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TResult">The type of objects to which reconciled elements are projected.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> is initially populated.</param>
    /// <param name="keySelector">A function that maps values from the <paramref name="existing"/> sequence to keys.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> are received.</param>
    /// <param name="selector">Projects a sequence of reconciled collection notifications, combining the <paramref name="existing"/> and 
    /// <paramref name="changes"/> sequences, into a sequence from which the dictionary is populated.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyDictionarySubject<TKey, TResult> ToDictionaryObservable<TSource, TKey, TResult>(
      this IEnumerable<TSource> existing,
      Func<TSource, TKey> keySelector,
      IObservable<CollectionModification<KeyValuePair<TKey, TSource>>> changes,
      Func<IObservable<CollectionNotification<KeyValuePair<TKey, TSource>>>, IObservable<CollectionModification<KeyValuePair<TKey, TResult>>>> selector)
    {
      Contract.Requires(existing != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(changes != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyDictionarySubject<TKey, TResult>>() != null);

      return ToDictionaryObservable(existing, keySelector, changes, selector, EqualityComparer<TKey>.Default);
    }

    /// <summary>
    /// Concurrently populates a <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the specified equality comparer for 
    /// <typeparamref name="TKey"/> and projecting notifications using the specified <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="TSource">The type of objects in the source sequences.</typeparam>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TResult">The type of objects to which reconciled elements are projected.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> is initially populated.</param>
    /// <param name="keySelector">A function that maps values from the <paramref name="existing"/> sequence to keys.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> are received.</param>
    /// <param name="selector">Projects a sequence of reconciled collection notifications, combining the <paramref name="existing"/> and 
    /// <paramref name="changes"/> sequences, into a sequence from which the dictionary is populated.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyDictionarySubject<TKey, TResult> ToDictionaryObservable<TSource, TKey, TResult>(
      this IEnumerable<TSource> existing,
      Func<TSource, TKey> keySelector,
      IObservable<CollectionModification<KeyValuePair<TKey, TSource>>> changes,
      Func<IObservable<CollectionNotification<KeyValuePair<TKey, TSource>>>, IObservable<CollectionModification<KeyValuePair<TKey, TResult>>>> selector,
      IEqualityComparer<TKey> comparer)
    {
      Contract.Requires(existing != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(changes != null);
      Contract.Requires(selector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<ReadOnlyDictionarySubject<TKey, TResult>>() != null);

      return ToDictionaryObservable(existing.ToObservable(PlatformSchedulers.Concurrent), keySelector, changes, selector, comparer);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the default equality comparer for 
    /// <typeparamref name="TKey"/> and projecting notifications using the specified <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="TSource">The type of objects in the source sequences.</typeparam>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TResult">The type of objects to which reconciled elements are projected.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> is initially populated.</param>
    /// <param name="keySelector">A function that maps values from the <paramref name="existing"/> sequence to keys.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> are received.</param>
    /// <param name="selector">Projects a sequence of reconciled collection notifications, combining the <paramref name="existing"/> and 
    /// <paramref name="changes"/> sequences, into a sequence from which the dictionary is populated.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> that receives the elements from the specified sequences.</returns>
    public static ReadOnlyDictionarySubject<TKey, TResult> ToDictionaryObservable<TSource, TKey, TResult>(
      this IObservable<TSource> existing,
      Func<TSource, TKey> keySelector,
      IObservable<CollectionModification<KeyValuePair<TKey, TSource>>> changes,
      Func<IObservable<CollectionNotification<KeyValuePair<TKey, TSource>>>, IObservable<CollectionModification<KeyValuePair<TKey, TResult>>>> selector)
    {
      Contract.Requires(existing != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(changes != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyDictionarySubject<TKey, TResult>>() != null);

      return ToDictionaryObservable(existing, keySelector, changes, selector, EqualityComparer<TKey>.Default);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> with elements from the first sequence while also handling changes 
    /// from the second sequence of collection modifications, reconciling conflicts using the specified equality comparer for 
    /// <typeparamref name="TKey"/> and projecting notifications using the specified <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="TSource">The type of objects in the source sequences.</typeparam>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TResult">The type of objects to which reconciled elements are projected.</typeparam>
    /// <param name="existing">The sequence from which the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> is initially populated.</param>
    /// <param name="keySelector">A function that maps values from the <paramref name="existing"/> sequence to keys.</param>
    /// <param name="changes">The sequence from which collection notifications that modify the <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> are received.</param>
    /// <param name="selector">Projects a sequence of reconciled collection notifications, combining the <paramref name="existing"/> and 
    /// <paramref name="changes"/> sequences, into a sequence from which the dictionary is populated.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys.</param>
    /// <include file='Observable2 - Collection and Dictionary.xml' path='//extension[@name="Collect"]/remarks'/>
    /// <returns>A <see cref="ReadOnlyDictionarySubject{TKey,TValue}"/> that receives the elements from the specified sequences.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Disposable is composited by the subject that is returned to the caller.")]
    public static ReadOnlyDictionarySubject<TKey, TResult> ToDictionaryObservable<TSource, TKey, TResult>(
      this IObservable<TSource> existing,
      Func<TSource, TKey> keySelector,
      IObservable<CollectionModification<KeyValuePair<TKey, TSource>>> changes,
      Func<IObservable<CollectionNotification<KeyValuePair<TKey, TSource>>>, IObservable<CollectionModification<KeyValuePair<TKey, TResult>>>> selector,
      IEqualityComparer<TKey> comparer)
    {
      Contract.Requires(existing != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(changes != null);
      Contract.Requires(selector != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<ReadOnlyDictionarySubject<TKey, TResult>>() != null);

      return new ReadOnlyDictionarySubject<TKey, TResult>(
        Collect(
          d => new DictionarySubject<TKey, TResult>(d, comparer),
          existing,
          changes,
          keySelector,
          pair => pair.Key,
          (key, value) => CollectionNotification.CreateExists(new KeyValuePair<TKey, TSource>(key, value)),
          selector,
          comparer));
    }
  }
}