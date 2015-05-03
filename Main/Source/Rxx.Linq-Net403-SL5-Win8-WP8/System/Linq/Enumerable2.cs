using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Linq
{
  internal static partial class Enumerable2
  {
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNext != null);

      foreach (var value in source)
      {
        onNext(value);
      }
    }
  }
}