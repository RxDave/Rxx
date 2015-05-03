using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System.Linq
{
  internal static partial class OptimizedEnumerable
  {
    /// <summary>
    /// Avoids StackOverflowException when combining a large number of sequential <strong>SelectMany</strong> operations.
    /// The <strong>Tuple</strong> provides an indicator to selectors whether each element is the last element (<see langword="true"/>)
    /// in the sequence or not (<see langword="false"/>).  This extra info is used by the <em>All</em> parser combinator to eliminate 
    /// extra branching on the last element of each sequence.  See the AllParser.Parse method for details.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1618:GenericTypeParametersMustBeDocumented", Justification = "Internal method.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "Internal method.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1615:ElementReturnValueMustBeDocumented", Justification = "Internal method.")]
    public static IEnumerable<TResult> SelectMany<TSource, TResult>(
      this IEnumerable<TSource> source,
      IEnumerable<Func<Tuple<TSource, bool>, IEnumerable<TSource>>> collectionSelectors,
      Func<TSource, IList<TSource>, TResult> resultSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(collectionSelectors != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      using (var enumerator = collectionSelectors.GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          foreach (var result in source.SelectMany(enumerator.Current, enumerator.AsEnumerable(), resultSelector))
          {
            yield return result;
          }
        }
      }
    }

    /// <summary>
    /// Avoids StackOverflowException when combining a large number of sequential <strong>SelectMany</strong> operations.
    /// The <strong>Tuple</strong> provides an indicator to selectors whether each element is the last element (<see langword="true"/>)
    /// in the sequence or not (<see langword="false"/>).  This extra info is used by the <em>All</em> parser combinator to eliminate 
    /// extra branching on the last element of each sequence.  See the AllParser.Parse method for details.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1618:GenericTypeParametersMustBeDocumented", Justification = "Internal method.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "Internal method.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1615:ElementReturnValueMustBeDocumented", Justification = "Internal method.")]
    public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
      this IEnumerable<TSource> source,
      Func<Tuple<TSource, bool>, IEnumerable<TCollection>> firstCollectionSelector,
      IEnumerable<Func<Tuple<TCollection, bool>, IEnumerable<TCollection>>> otherCollectionSelectors,
      Func<TSource, IList<TCollection>, TResult> resultSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(firstCollectionSelector != null);
      Contract.Requires(otherCollectionSelectors != null);
      Contract.Requires(resultSelector != null);

      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      var values = new List<TCollection>();
      var sequences = new Stack<IEnumerator<Tuple<TCollection, bool>>>();
      var selectorCache = new List<Func<Tuple<TCollection, bool>, IEnumerable<TCollection>>>();

      using (var enumerator = otherCollectionSelectors.GetEnumerator())
      {
        foreach (var first in source.WithLastElementIndicator())
        {
          sequences.Push(firstCollectionSelector(first).WithLastElementIndicator().GetEnumerator());

          do
          {
            var sequence = sequences.Peek();

            if (sequence.MoveNext())
            {
              var value = sequence.Current;

              values.Add(value.Item1);

              Func<Tuple<TCollection, bool>, IEnumerable<TCollection>> nextSelector = null;

              if (selectorCache.Count > sequences.Count - 1)
              {
                nextSelector = selectorCache[sequences.Count - 1];
              }
              else if (enumerator.MoveNext())
              {
                nextSelector = enumerator.Current;

                selectorCache.Add(nextSelector);
              }
              else
              {
                selectorCache.Add(null);
              }

              if (nextSelector != null)
              {
                sequences.Push(nextSelector(value).WithLastElementIndicator().GetEnumerator());
              }
              else
              {
                yield return resultSelector(first.Item1, values.AsReadOnly());

                values.RemoveAt(values.Count - 1);
              }
            }
            else
            {
              sequences.Pop().Dispose();

              if (values.Count > 0)
              {
                values.RemoveAt(values.Count - 1);
              }
            }
          }
          while (sequences.Count > 0);
        }
      }
    }

    public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator)
    {
      Contract.Requires(enumerator != null);
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      while (enumerator.MoveNext())
      {
        yield return enumerator.Current;
      }
    }

    private static IEnumerable<Tuple<T, bool>> WithLastElementIndicator<T>(this IEnumerable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<Tuple<T, bool>>>() != null);

      using (var enumerator = source.GetEnumerator())
      {
        bool completed = false;
        bool hasNextValue = false;
        T nextValue = default(T);

        while (!completed && (hasNextValue || enumerator.MoveNext()))
        {
          var value = hasNextValue ? nextValue : enumerator.Current;

          hasNextValue = enumerator.MoveNext();

          if (hasNextValue)
          {
            nextValue = enumerator.Current;
          }
          else
          {
            completed = true;
          }

          yield return Tuple.Create(value, !hasNextValue);
        }
      }
    }

    /// <summary>
    /// Avoids StackOverflowException when iterating a sequence composed of a large number of concatenated collections.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1618:GenericTypeParametersMustBeDocumented", Justification = "Internal method.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "Internal method.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1615:ElementReturnValueMustBeDocumented", Justification = "Internal method.")]
    [ContractVerification(false)]		// Static checker error: "Failed with uncaught exception: The method or operation is not implemented."
    public static IEnumerable<TSource> ConcatOptimized<TSource>(
      this IEnumerable<TSource> first,
      params IEnumerable<TSource>[] others)
    {
      Contract.Requires(first != null);
      Contract.Requires(others != null);
      Contract.Ensures(Contract.Result<IEnumerable<TSource>>() != null);

      return first.ConcatOptimized((IEnumerable<IEnumerable<TSource>>)others);
    }

    /// <summary>
    /// Avoids StackOverflowException when iterating a sequence composed of a large number of concatenated collections.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1618:GenericTypeParametersMustBeDocumented", Justification = "Internal method.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "Internal method.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1615:ElementReturnValueMustBeDocumented", Justification = "Internal method.")]
    public static IEnumerable<TSource> ConcatOptimized<TSource>(
      this IEnumerable<TSource> first,
      IEnumerable<IEnumerable<TSource>> others)
    {
      Contract.Requires(first != null);
      Contract.Requires(others != null);
      Contract.Ensures(Contract.Result<IEnumerable<TSource>>() != null);

      IEnumerable<TSource> results = null;
      List<TSource> tail = null;

      foreach (var sequence in others.StartWith(first))
      {
        Contract.Assume(sequence != null);

        var collection = sequence as ICollection<TSource>;

        if (collection != null)
        {
          if (tail == null)
          {
            tail = new List<TSource>(collection.Count);
          }

          tail.AddRange(collection);
        }
        else if (tail != null)
        {
          if (results != null)
          {
            results = results.Concat(tail);
          }
          else
          {
            results = tail;
          }

          results = results.Concat(sequence);

          tail = null;
        }
        else if (results != null)
        {
          results = results.Concat(sequence);
        }
        else
        {
          results = sequence;
        }
      }

      if (tail != null)
      {
        if (results != null)
        {
          return results.Concat(tail);
        }
        else
        {
          return tail.AsReadOnly();
        }
      }
      else
      {
        return results ?? Enumerable.Empty<TSource>();
      }
    }

    public static IEnumerable<TSource> OnErrorOrDisposed<TSource>(this IEnumerable<TSource> source, Action action)
    {
      Contract.Requires(source != null);
      Contract.Requires(action != null);
      Contract.Ensures(Contract.Result<IEnumerable<TSource>>() != null);

      return new AnonymousEnumerable<TSource>(() =>
      {
        var enumerator = source.GetEnumerator();

        return new AnonymousEnumerator<TSource>(
          () =>
          {
            try
            {
              return enumerator.MoveNext();
            }
            catch
            {
              action();
              throw;
            }
          },
          () =>
          {
            try
            {
              return enumerator.Current;
            }
            catch
            {
              action();
              throw;
            }
          },
          enumerator.Reset,
          () =>
          {
            enumerator.Dispose();

            action();
          });
      });
    }
  }
}