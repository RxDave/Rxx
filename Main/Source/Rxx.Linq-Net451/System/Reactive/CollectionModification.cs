using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Reactive
{
  /// <summary>
  /// Provides <see langword="static"/> factory methods for creating <see cref="CollectionModification{T}"/> objects.
  /// </summary>
  public static class CollectionModification
  {
    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the specified <paramref name="values"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides modification information.</typeparam>
    /// <param name="values">The items to be added.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the specified <paramref name="values"/>.</returns>
    public static CollectionModification<T> CreateAdd<T>(IList<T> values)
    {
      Contract.Requires(values != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>().Kind == CollectionModificationKind.Add);

      return new CollectionModification<T>.Add(values);
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the specified <paramref name="values"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides modification information.</typeparam>
    /// <param name="values">The items to be added.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the specified <paramref name="values"/>.</returns>
    public static CollectionModification<T> CreateAdd<T>(IEnumerable<T> values)
    {
      Contract.Requires(values != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>().Kind == CollectionModificationKind.Add);

      return new CollectionModification<T>.Add(values.ToList().AsReadOnly());
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the specified <paramref name="values"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides modification information.</typeparam>
    /// <param name="values">The items to be added.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the specified <paramref name="values"/>.</returns>
    public static CollectionModification<T> CreateAdd<T>(params T[] values)
    {
      Contract.Requires(values != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>().Kind == CollectionModificationKind.Add);

      return new CollectionModification<T>.Add(values);
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides modification information.</typeparam>
    /// <param name="value">The items to be added.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the specified <paramref name="value"/>.</returns>
    public static CollectionModification<T> CreateAdd<T>(T value)
    {
      Contract.Ensures(Contract.Result<CollectionModification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>().Kind == CollectionModificationKind.Add);

      return new CollectionModification<T>.Add(new[] { value }.ToList().AsReadOnly());
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Remove"/> 
    /// modification for the specified <paramref name="values"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides modification information.</typeparam>
    /// <param name="values">The items to be removed.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Remove"/> 
    /// modification for the specified <paramref name="values"/>.</returns>
    public static CollectionModification<T> CreateRemove<T>(IList<T> values)
    {
      Contract.Requires(values != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>().Kind == CollectionModificationKind.Remove);

      return new CollectionModification<T>.Remove(values);
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Remove"/> 
    /// modification for the specified <paramref name="values"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides modification information.</typeparam>
    /// <param name="values">The items to be removed.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Remove"/> 
    /// modification for the specified <paramref name="values"/>.</returns>
    public static CollectionModification<T> CreateRemove<T>(IEnumerable<T> values)
    {
      Contract.Requires(values != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>().Kind == CollectionModificationKind.Remove);

      return new CollectionModification<T>.Remove(values.ToList().AsReadOnly());
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Remove"/> 
    /// modification for the specified <paramref name="values"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides modification information.</typeparam>
    /// <param name="values">The items to be removed.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Remove"/> 
    /// modification for the specified <paramref name="values"/>.</returns>
    public static CollectionModification<T> CreateRemove<T>(params T[] values)
    {
      Contract.Requires(values != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>().Kind == CollectionModificationKind.Remove);

      return new CollectionModification<T>.Remove(values);
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Remove"/> 
    /// modification for the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides modification information.</typeparam>
    /// <param name="value">The items to be removed.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Remove"/> 
    /// modification for the specified <paramref name="value"/>.</returns>
    public static CollectionModification<T> CreateRemove<T>(T value)
    {
      Contract.Ensures(Contract.Result<CollectionModification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>().Kind == CollectionModificationKind.Remove);

      return new CollectionModification<T>.Remove(new[] { value }.ToList().AsReadOnly());
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Clear"/> 
    /// modification.
    /// </summary>
    /// <typeparam name="T">The object that provides modification information.</typeparam>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Clear"/> 
    /// modification.</returns>
    public static CollectionModification<T> CreateClear<T>()
    {
      Contract.Ensures(Contract.Result<CollectionModification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<T>>().Kind == CollectionModificationKind.Clear);

      return new CollectionModification<T>.Clear();
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the key and value pairs in the specified <paramref name="dictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">The key and value pairs to be added.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the key and value pairs in the specified <paramref name="dictionary"/>.</returns>
    public static CollectionModification<KeyValuePair<TKey, TValue>> CreateDictionaryAdd<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {
      Contract.Requires(dictionary != null);
      Contract.Ensures(Contract.Result<CollectionModification<KeyValuePair<TKey, TValue>>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<KeyValuePair<TKey, TValue>>>().Kind == CollectionModificationKind.Add);

      return new CollectionModification<KeyValuePair<TKey, TValue>>.Add(dictionary.ToList().AsReadOnly());
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the specified key and value <paramref name="pairs"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="pairs">The key and value pairs to be added.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the specified key and value <paramref name="pairs"/>.</returns>
    public static CollectionModification<KeyValuePair<TKey, TValue>> CreateDictionaryAdd<TKey, TValue>(params KeyValuePair<TKey, TValue>[] pairs)
    {
      Contract.Requires(pairs != null);
      Contract.Ensures(Contract.Result<CollectionModification<KeyValuePair<TKey, TValue>>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<KeyValuePair<TKey, TValue>>>().Kind == CollectionModificationKind.Add);

      return new CollectionModification<KeyValuePair<TKey, TValue>>.Add(pairs);
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the specified <paramref name="key"/> and <paramref name="value"/> pair.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="key">The key of the item to be added.</param>
    /// <param name="value">The item to be added.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is an <see cref="CollectionModificationKind.Add"/> 
    /// modification for the specified <paramref name="key"/> and <paramref name="value"/>.</returns>
    public static CollectionModification<KeyValuePair<TKey, TValue>> CreateDictionaryAdd<TKey, TValue>(TKey key, TValue value)
    {
      Contract.Ensures(Contract.Result<CollectionModification<KeyValuePair<TKey, TValue>>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<KeyValuePair<TKey, TValue>>>().Kind == CollectionModificationKind.Add);

      return new CollectionModification<KeyValuePair<TKey, TValue>>.Add(
        new List<KeyValuePair<TKey, TValue>>()
        {
          new KeyValuePair<TKey, TValue>(key, value)
        }
        .AsReadOnly());
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Remove"/> 
    /// modification for the key and value pairs in the specified <paramref name="dictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">The key and value pairs to be removed.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Remove"/> 
    /// modification for the key and value pairs in the specified <paramref name="dictionary"/>.</returns>
    public static CollectionModification<KeyValuePair<TKey, TValue>> CreateDictionaryRemove<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {
      Contract.Requires(dictionary != null);
      Contract.Ensures(Contract.Result<CollectionModification<KeyValuePair<TKey, TValue>>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<KeyValuePair<TKey, TValue>>>().Kind == CollectionModificationKind.Remove);

      return new CollectionModification<KeyValuePair<TKey, TValue>>.Remove(dictionary.ToList().AsReadOnly());
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Remove"/> 
    /// modification for the specified <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="keys">The keys to be removed.</param>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Remove"/> 
    /// modification for the specified <paramref name="keys"/>.</returns>
    public static CollectionModification<KeyValuePair<TKey, TValue>> CreateDictionaryRemove<TKey, TValue>(params TKey[] keys)
    {
      Contract.Requires(keys != null);
      Contract.Ensures(Contract.Result<CollectionModification<KeyValuePair<TKey, TValue>>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<KeyValuePair<TKey, TValue>>>().Kind == CollectionModificationKind.Remove);

      return new CollectionModification<KeyValuePair<TKey, TValue>>.Remove(
        keys
          .Select(key => new KeyValuePair<TKey, TValue>(key, default(TValue)))
          .ToList()
          .AsReadOnly());
    }

    /// <summary>
    /// Returns a new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Clear"/> 
    /// modification.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <returns>A new <see cref="CollectionModification{T}"/> object that is a <see cref="CollectionModificationKind.Clear"/> 
    /// modification.</returns>
    public static CollectionModification<KeyValuePair<TKey, TValue>> CreateDictionaryClear<TKey, TValue>()
    {
      Contract.Ensures(Contract.Result<CollectionModification<KeyValuePair<TKey, TValue>>>() != null);
      Contract.Ensures(Contract.Result<CollectionModification<KeyValuePair<TKey, TValue>>>().Kind == CollectionModificationKind.Clear);

      return new CollectionModification<KeyValuePair<TKey, TValue>>.Clear();
    }
  }
}