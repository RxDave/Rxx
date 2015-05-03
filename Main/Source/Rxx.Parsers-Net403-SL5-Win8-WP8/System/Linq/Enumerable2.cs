using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Linq
{
  internal static class Enumerable2
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

    public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext)
    {
      Contract.Requires(source != null);
      Contract.Requires(onNext != null);
      Contract.Ensures(Contract.Result<IEnumerable<TSource>>() != null);

      foreach (var value in source)
      {
        onNext(value);

        yield return value;
      }
    }

    public static IEnumerable<TSource> StartWith<TSource>(this IEnumerable<TSource> source, TSource value)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<TSource>>() != null);

      return StartWithIterator(source, value);
    }

    private static IEnumerable<TSource> StartWithIterator<TSource>(this IEnumerable<TSource> source, TSource value)
    {
      yield return value;

      foreach (var v in source)
      {
        yield return v;
      }
    }

    public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<IEnumerable<TSource>> sources)
    {
      Contract.Requires(sources != null);
      Contract.Ensures(Contract.Result<IEnumerable<TSource>>() != null);

      foreach (var source in sources)
      {
        foreach (var value in source)
        {
          yield return value;
        }
      }
    }

    public static IEnumerable<TSource> Finally<TSource>(this IEnumerable<TSource> source, Action action)
    {
      Contract.Requires(source != null);
      Contract.Requires(action != null);
      Contract.Ensures(Contract.Result<IEnumerable<TSource>>() != null);

      try
      {
        foreach (var value in source)
        {
          yield return value;
        }
      }
      finally
      {
        action();
      }
    }
  }
}