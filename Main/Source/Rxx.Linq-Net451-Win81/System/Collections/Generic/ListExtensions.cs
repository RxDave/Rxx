using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace System.Collections.Generic
{
  internal static class ListExtensions
  {
    public static ReadOnlyCollection<T> AsReadOnly<T>(this List<T> list)
    {
      Contract.Requires(list != null);
      Contract.Ensures(Contract.Result<ReadOnlyCollection<T>>() != null);

      return new ReadOnlyCollection<T>(list);
    }
  }
}