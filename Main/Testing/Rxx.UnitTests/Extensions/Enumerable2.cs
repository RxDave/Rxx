using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Linq
{
  internal static class Enumerable2
  {
    public static void ForEach<TSource>(this IEnumerable<TSource> source)
    {
      Contract.Requires(source != null);

      foreach (var value in source)
      {
        // do nothing
      }
    }
  }
}