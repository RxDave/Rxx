using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers.Linq
{
  public static partial class Parser
  {
    /// <summary>
    /// Projects matches from the specified <paramref name="parser"/> into a new form.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are projected from the matches of the specified <paramref name="parser"/>.</typeparam>
    /// <param name="parser">The parser from which matches will be projected by the specified <paramref name="selector"/> function.</param>
    /// <param name="selector">A transform function to apply to each match.</param>
    /// <returns>A parser that projects matches from the specified <paramref name="parser"/> into a new form.</returns>
    public static IParser<TSource, TResult> Select<TSource, TIntermediate, TResult>(
      this IParser<TSource, TIntermediate> parser,
      Func<TIntermediate, TResult> selector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Yield("Select", source => parser.Parse(source).Select(result => result.Yield(selector)));
    }

    /// <summary>
    /// Projects each match from the specified <paramref name="parser"/> into another parser, merges all of the results
    /// and transforms them with the result selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TFirstResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TSecondResult">The type of the elements that are generated from the projected parsers.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are projected from the matches of the projected parsers.</typeparam>
    /// <param name="parser">The parser from which each match is passed to the specified parser selector function to create 
    /// the next parser.</param>
    /// <param name="parserSelector">A transform function to apply to each match from the first <paramref name="parser"/>.</param>
    /// <param name="resultSelector">A transform function to apply to each match from the projected parsers.</param>
    /// <returns>A parser that projects each match from the specified <paramref name="parser"/> into another parser, 
    /// merges all of the results and transforms them with the result selector function.</returns>
    public static IParser<TSource, TResult> SelectMany<TSource, TFirstResult, TSecondResult, TResult>(
      this IParser<TSource, TFirstResult> parser,
      Func<TFirstResult, IParser<TSource, TSecondResult>> parserSelector,
      Func<TFirstResult, TSecondResult, TResult> resultSelector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(parserSelector != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Yield("SelectMany", source => SelectManyIterator(source, parser, parserSelector, resultSelector));
    }

    /// <summary>
    /// Projects each match from the specified <paramref name="parser"/> into another parser, merges all of the results
    /// and transforms them with the result selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TFirstResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TSecondResult">The type of the elements that are generated from the projected parsers.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are projected from the matches of the projected parsers.</typeparam>
    /// <param name="parser">The parser from which each match is passed to the specified parser selector function to create 
    /// the next parser.</param>
    /// <param name="parserSelector">A transform function to apply to each match from the first <paramref name="parser"/>.</param>
    /// <param name="resultSelector">A transform function to apply to each match from the projected parsers.</param>
    /// <param name="lengthSelector">A function that returns the length for each pair of projected matches.</param>
    /// <returns>A parser that projects each match from the specified <paramref name="parser"/> into another parser, 
    /// merges all of the results and transforms them with the result selector function.</returns>
    public static IParser<TSource, TResult> SelectMany<TSource, TFirstResult, TSecondResult, TResult>(
      this IParser<TSource, TFirstResult> parser,
      Func<TFirstResult, IParser<TSource, TSecondResult>> parserSelector,
      Func<TFirstResult, TSecondResult, TResult> resultSelector,
      Func<IParseResult<TFirstResult>, IParseResult<TSecondResult>, int> lengthSelector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(parserSelector != null);
      Contract.Requires(resultSelector != null);
      Contract.Requires(lengthSelector != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Yield("SelectMany", source => SelectManyIterator(source, parser, parserSelector, resultSelector, lengthSelector));
    }

    private static IEnumerable<IParseResult<TResult>> SelectManyIterator<TSource, TFirstResult, TSecondResult, TResult>(
      ICursor<TSource> source,
      IParser<TSource, TFirstResult> firstParser,
      Func<TFirstResult, IParser<TSource, TSecondResult>> secondSelector,
      Func<TFirstResult, TSecondResult, TResult> resultSelector,
      Func<IParseResult<TFirstResult>, IParseResult<TSecondResult>, int> lengthSelector = null)
    {
      Contract.Requires(source != null);
      Contract.Requires(firstParser != null);
      Contract.Requires(secondSelector != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<TResult>>>() != null);

      foreach (var first in firstParser.Parse(source))
      {
        var lookAhead = first as ILookAheadParseResult<TFirstResult>;
        bool hasResult = false;

        using (var remainder = source.Remainder(first.Length))
        {
          foreach (var second in secondSelector(first.Value).Parse(remainder))
          {
            hasResult = true;

            if (lookAhead != null)
            {
              lookAhead.OnCompleted(success: true);
              break;
            }
            else
            {
              yield return (lengthSelector == null)
                ? first.Add(second, resultSelector)
                : first.Yield(second, resultSelector, lengthSelector);
            }
          }
        }

        if (!hasResult && lookAhead != null)
        {
          lookAhead.OnCompleted(success: false);
        }
      }
    }

    /// <summary>
    /// Projects each match from the specified <paramref name="parser"/> into an enumerable sequence, merges all of the results
    /// and transforms them with the result selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TFirstResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TCollection">The type of the elements in the sequences that are projected from the matches.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are projected from the projected sequences.</typeparam>
    /// <param name="parser">The parser from which each match is passed to the specified collection selector function.</param>
    /// <param name="collectionSelector">A transform function to apply to each match from the first <paramref name="parser"/>.</param>
    /// <param name="resultSelector">A transform function to apply to each element from the projected sequences.</param>
    /// <returns>A parser that projects each match from the specified <paramref name="parser"/> into an enumerable sequence, 
    /// merges all of the results and transforms them with the result selector function.</returns>
    public static IParser<TSource, TResult> SelectMany<TSource, TFirstResult, TCollection, TResult>(
      this IParser<TSource, TFirstResult> parser,
      Func<TFirstResult, IEnumerable<TCollection>> collectionSelector,
      Func<TFirstResult, TCollection, TResult> resultSelector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(collectionSelector != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Yield("SelectMany", source => SelectManyIterator(source, parser, collectionSelector, resultSelector));
    }

    /// <summary>
    /// Projects each match from the specified <paramref name="parser"/> into an enumerable sequence, merges all of the results
    /// and transforms them with the result selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TFirstResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TCollection">The type of the elements in the sequences that are projected from the matches.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are projected from the projected sequences.</typeparam>
    /// <param name="parser">The parser from which each match is passed to the specified collection selector function.</param>
    /// <param name="collectionSelector">A transform function to apply to each match from the first <paramref name="parser"/>.</param>
    /// <param name="resultSelector">A transform function to apply to each element from the projected sequences.</param>
    /// <param name="lengthSelector">A function that returns the length for each pair of projected values.</param>
    /// <returns>A parser that projects each match from the specified <paramref name="parser"/> into an enumerable sequence, 
    /// merges all of the results and transforms them with the result selector function.</returns>
    public static IParser<TSource, TResult> SelectMany<TSource, TFirstResult, TCollection, TResult>(
      this IParser<TSource, TFirstResult> parser,
      Func<TFirstResult, IEnumerable<TCollection>> collectionSelector,
      Func<TFirstResult, TCollection, TResult> resultSelector,
      Func<IParseResult<TFirstResult>, TCollection, int> lengthSelector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(collectionSelector != null);
      Contract.Requires(resultSelector != null);
      Contract.Requires(lengthSelector != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Yield("SelectMany", source => SelectManyIterator(source, parser, collectionSelector, resultSelector, lengthSelector));
    }

    private static IEnumerable<IParseResult<TResult>> SelectManyIterator<TSource, TFirstResult, TCollection, TResult>(
      ICursor<TSource> source,
      IParser<TSource, TFirstResult> parser,
      Func<TFirstResult, IEnumerable<TCollection>> collectionSelector,
      Func<TFirstResult, TCollection, TResult> resultSelector,
      Func<IParseResult<TFirstResult>, TCollection, int> lengthSelector = null)
    {
      Contract.Requires(source != null);
      Contract.Requires(parser != null);
      Contract.Requires(collectionSelector != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<TResult>>>() != null);

      foreach (var first in parser.Parse(source))
      {
        var lookAhead = first as ILookAheadParseResult<TFirstResult>;
        var hasResult = false;
        TCollection previous = default(TCollection);

        foreach (var second in collectionSelector(first.Value))
        {
          if (lookAhead != null)
          {
            hasResult = true;

            lookAhead.OnCompleted(success: true);

            break;
          }
          else
          {
            if (hasResult)
            {
              yield return lengthSelector == null
                ? first.Yield(previous, resultSelector, (f, s) => 0)
                : first.Yield(previous, resultSelector, lengthSelector);
            }

            hasResult = true;
            previous = second;
          }
        }

        if (hasResult && lookAhead == null)
        {
          yield return lengthSelector == null
            ? first.Yield(previous, resultSelector, (f, s) => f.Length)
            : first.Yield(previous, resultSelector, lengthSelector);
        }
        else if (!hasResult && lookAhead != null)
        {
          lookAhead.OnCompleted(success: false);
        }
      }
    }
  }
}