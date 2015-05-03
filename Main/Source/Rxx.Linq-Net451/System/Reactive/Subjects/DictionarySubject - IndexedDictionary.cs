using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Reactive.Subjects
{
  public sealed partial class DictionarySubject<TKey, TValue>
  {
    private sealed class IndexedDictionary : KeyedCollection<TKey, KeyValuePair<TKey, TValue>>
    {
      #region Public Properties
      public IEnumerable<TKey> Keys
      {
        get
        {
          Contract.Ensures(Contract.Result<IEnumerable<TKey>>() != null);

          var dictionary = Dictionary;

          if (dictionary == null)
          {
            return Enumerable.Empty<TKey>();
          }
          else
          {
            return dictionary.Keys;
          }
        }
      }

      public IEnumerable<TValue> Values
      {
        get
        {
          Contract.Ensures(Contract.Result<IEnumerable<TValue>>() != null);

          var dictionary = Dictionary;

          if (dictionary == null)
          {
            return Enumerable.Empty<TValue>();
          }
          else
          {
            return dictionary.Values.Select(pair => pair.Value);
          }
        }
      }
      #endregion

      #region Private / Protected
      #endregion

      #region Constructors
      public IndexedDictionary()
      {
      }

      public IndexedDictionary(IEqualityComparer<TKey> comparer)
        : base(comparer)
      {
      }
      #endregion

      #region Methods
      protected override TKey GetKeyForItem(KeyValuePair<TKey, TValue> item)
      {
        return item.Key;
      }

      public int IndexOf(TKey key)
      {
        Contract.Ensures(Contract.Result<int>() >= -1);
        Contract.Ensures(Contract.Result<int>() < Count);

        var comparer = Comparer;

        Contract.Assume(comparer != null);

        for (int i = 0; i < this.Count; i++)
        {
          if (comparer.Equals(key, this[i].Key))
          {
            return i;
          }
        }

        return -1;
      }
      #endregion
    }
  }
}