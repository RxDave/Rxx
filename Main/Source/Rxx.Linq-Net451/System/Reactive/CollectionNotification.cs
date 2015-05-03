using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Reactive
{
  /// <summary>
  /// Provides <see langword="static"/> factory methods for creating <see cref="CollectionNotification{T}"/> objects.
  /// </summary>
  public static class CollectionNotification
  {
    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.Exists"/> 
    /// notification for the specified <paramref name="values"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="values">The list of items that exist.</param>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.Exists"/> 
    /// notification for the specified <paramref name="values"/>.</returns>
    public static CollectionNotification<T> CreateExists<T>(IList<T> values)
    {
      Contract.Requires(values != null);
      Contract.Ensures(Contract.Result<CollectionNotification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<T>>().Kind == CollectionNotificationKind.Exists);

      return new CollectionNotification<T>.Exists(values);
    }

    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.Exists"/> 
    /// notification for the specified <paramref name="values"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="values">The list of items that exist.</param>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.Exists"/> 
    /// notification for the specified <paramref name="values"/>.</returns>
    public static CollectionNotification<T> CreateExists<T>(IEnumerable<T> values)
    {
      Contract.Requires(values != null);
      Contract.Ensures(Contract.Result<CollectionNotification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<T>>().Kind == CollectionNotificationKind.Exists);

      return new CollectionNotification<T>.Exists(values.ToList().AsReadOnly());
    }

    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.Exists"/> 
    /// notification for the specified <paramref name="values"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="values">The list of items that exist.</param>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.Exists"/> 
    /// notification for the specified <paramref name="values"/>.</returns>
    public static CollectionNotification<T> CreateExists<T>(params T[] values)
    {
      Contract.Requires(values != null);
      Contract.Ensures(Contract.Result<CollectionNotification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<T>>().Kind == CollectionNotificationKind.Exists);

      return new CollectionNotification<T>.Exists(values);
    }

    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnAdded"/> 
    /// notification for the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="value">The item being added.</param>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnAdded"/> 
    /// notification for the specified <paramref name="value"/>.</returns>
    public static CollectionNotification<T> CreateOnAdded<T>(T value)
    {
      Contract.Ensures(Contract.Result<CollectionNotification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<T>>().Kind == CollectionNotificationKind.OnAdded);

      return new CollectionNotification<T>.OnAdded(value);
    }

    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnReplaced"/> 
    /// notification for the specified values.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="oldValue">The item being replaced.</param>
    /// <param name="newValue">The item replacing the old item.</param>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnReplaced"/> 
    /// notification for the specified values.</returns>
    public static CollectionNotification<T> CreateOnReplaced<T>(T oldValue, T newValue)
    {
      Contract.Ensures(Contract.Result<CollectionNotification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<T>>().Kind == CollectionNotificationKind.OnReplaced);

      return new CollectionNotification<T>.OnReplaced(oldValue, newValue);
    }

    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnRemoved"/> 
    /// notification for the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="value">The item being removed.</param>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnRemoved"/> 
    /// notification for the specified <paramref name="value"/>.</returns>
    public static CollectionNotification<T> CreateOnRemoved<T>(T value)
    {
      Contract.Ensures(Contract.Result<CollectionNotification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<T>>().Kind == CollectionNotificationKind.OnRemoved);

      return new CollectionNotification<T>.OnRemoved(value);
    }

    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnCleared"/> 
    /// notification.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnCleared"/> 
    /// notification.</returns>
    public static CollectionNotification<T> CreateOnCleared<T>()
    {
      Contract.Ensures(Contract.Result<CollectionNotification<T>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<T>>().Kind == CollectionNotificationKind.OnCleared);

      return new CollectionNotification<T>.OnCleared();
    }

    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.Exists"/> 
    /// notification for the key and value pairs in the specified <paramref name="dictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">The existing key and value pairs.</param>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.Exists"/> 
    /// notification for the key and value pairs in the specified <paramref name="dictionary"/>.</returns>
    public static CollectionNotification<KeyValuePair<TKey, TValue>> CreateDictionaryExists<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {
      Contract.Requires(dictionary != null);
      Contract.Ensures(Contract.Result<CollectionNotification<KeyValuePair<TKey, TValue>>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<KeyValuePair<TKey, TValue>>>().Kind == CollectionNotificationKind.Exists);

      return new CollectionNotification<KeyValuePair<TKey, TValue>>.Exists(dictionary.ToList().AsReadOnly());
    }

    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.Exists"/> 
    /// notification for the specified key and value <paramref name="pairs"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="pairs">The existing key and value pairs.</param>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.Exists"/> 
    /// notification for the specified key and value <paramref name="pairs"/>.</returns>
    public static CollectionNotification<KeyValuePair<TKey, TValue>> CreateDictionaryExists<TKey, TValue>(IList<KeyValuePair<TKey, TValue>> pairs)
    {
      Contract.Requires(pairs != null);
      Contract.Ensures(Contract.Result<CollectionNotification<KeyValuePair<TKey, TValue>>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<KeyValuePair<TKey, TValue>>>().Kind == CollectionNotificationKind.Exists);

      return new CollectionNotification<KeyValuePair<TKey, TValue>>.Exists(pairs);
    }

    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnAdded"/> 
    /// notification for the specified <paramref name="key"/> and <paramref name="value"/> pair.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="key">The key of the item being added.</param>
    /// <param name="value">The item being added.</param>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnAdded"/> 
    /// notification for the specified <paramref name="key"/> and <paramref name="value"/>.</returns>
    public static CollectionNotification<KeyValuePair<TKey, TValue>> CreateDictionaryOnAdded<TKey, TValue>(TKey key, TValue value)
    {
      Contract.Ensures(Contract.Result<CollectionNotification<KeyValuePair<TKey, TValue>>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<KeyValuePair<TKey, TValue>>>().Kind == CollectionNotificationKind.OnAdded);

      return new CollectionNotification<KeyValuePair<TKey, TValue>>.OnAdded(new KeyValuePair<TKey, TValue>(key, value));
    }

    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnReplaced"/> 
    /// notification for the specified <paramref name="key"/> and value pair.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="key">The key of the item being replaced.</param>
    /// <param name="oldValue">The item being replaced.</param>
    /// <param name="newValue">The item replacing the old item.</param>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnReplaced"/> 
    /// notification for the specified <paramref name="key"/> and value.</returns>
    public static CollectionNotification<KeyValuePair<TKey, TValue>> CreateDictionaryOnReplaced<TKey, TValue>(TKey key, TValue oldValue, TValue newValue)
    {
      Contract.Ensures(Contract.Result<CollectionNotification<KeyValuePair<TKey, TValue>>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<KeyValuePair<TKey, TValue>>>().Kind == CollectionNotificationKind.OnReplaced);

      return new CollectionNotification<KeyValuePair<TKey, TValue>>.OnReplaced(new KeyValuePair<TKey, TValue>(key, oldValue), new KeyValuePair<TKey, TValue>(key, newValue));
    }

    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnRemoved"/> 
    /// notification for the specified <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="key">The key of the item being removed.</param>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnRemoved"/> 
    /// notification for the specified <paramref name="key"/>.</returns>
    public static CollectionNotification<KeyValuePair<TKey, TValue>> CreateDictionaryOnRemoved<TKey, TValue>(TKey key)
    {
      Contract.Ensures(Contract.Result<CollectionNotification<KeyValuePair<TKey, TValue>>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<KeyValuePair<TKey, TValue>>>().Kind == CollectionNotificationKind.OnRemoved);

      return new CollectionNotification<KeyValuePair<TKey, TValue>>.OnRemoved(new KeyValuePair<TKey, TValue>(key, default(TValue)));
    }

    /// <summary>
    /// Returns a new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnCleared"/> 
    /// notification.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <returns>A new <see cref="CollectionNotification{T}"/> object that is an <see cref="CollectionNotificationKind.OnCleared"/> 
    /// notification.</returns>
    public static CollectionNotification<KeyValuePair<TKey, TValue>> CreateDictionaryOnCleared<TKey, TValue>()
    {
      Contract.Ensures(Contract.Result<CollectionNotification<KeyValuePair<TKey, TValue>>>() != null);
      Contract.Ensures(Contract.Result<CollectionNotification<KeyValuePair<TKey, TValue>>>().Kind == CollectionNotificationKind.OnCleared);

      return new CollectionNotification<KeyValuePair<TKey, TValue>>.OnCleared();
    }
  }
}