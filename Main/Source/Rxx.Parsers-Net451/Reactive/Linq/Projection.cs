using System;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;

namespace Rxx.Parsers.Reactive.Linq
{
  public static partial class ObservableParser
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
    public static IObservableParser<TSource, TResult> Select<TSource, TIntermediate, TResult>(
      this IObservableParser<TSource, TIntermediate> parser,
      Func<TIntermediate, TResult> selector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

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
    public static IObservableParser<TSource, TResult> SelectMany<TSource, TFirstResult, TSecondResult, TResult>(
      this IObservableParser<TSource, TFirstResult> parser,
      Func<TFirstResult, IObservableParser<TSource, TSecondResult>> parserSelector,
      Func<TFirstResult, TSecondResult, TResult> resultSelector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(parserSelector != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return parser.Yield("SelectMany", source => SelectManyInternal(source, parser, parserSelector, resultSelector));
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
    public static IObservableParser<TSource, TResult> SelectMany<TSource, TFirstResult, TSecondResult, TResult>(
      this IObservableParser<TSource, TFirstResult> parser,
      Func<TFirstResult, IObservableParser<TSource, TSecondResult>> parserSelector,
      Func<TFirstResult, TSecondResult, TResult> resultSelector,
      Func<IParseResult<TFirstResult>, IParseResult<TSecondResult>, int> lengthSelector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(parserSelector != null);
      Contract.Requires(resultSelector != null);
      Contract.Requires(lengthSelector != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return parser.Yield("SelectMany", source => SelectManyInternal(source, parser, parserSelector, resultSelector, lengthSelector));
    }

    private static IObservable<IParseResult<TResult>> SelectManyInternal<TSource, TFirstResult, TSecondResult, TResult>(
      IObservableCursor<TSource> source,
      IObservableParser<TSource, TFirstResult> firstParser,
      Func<TFirstResult, IObservableParser<TSource, TSecondResult>> secondSelector,
      Func<TFirstResult, TSecondResult, TResult> resultSelector,
      Func<IParseResult<TFirstResult>, IParseResult<TSecondResult>, int> lengthSelector = null)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Requires(firstParser != null);
      Contract.Requires(secondSelector != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(Contract.Result<IObservable<IParseResult<TResult>>>() != null);

      return from first in firstParser.Parse(source)
             from second in Observable.Create<IParseResult<TSecondResult>>(
              observer =>
              {
                var lookAhead = first as ILookAheadParseResult<TFirstResult>;
                bool hasResult = false;

                var remainder = source.Remainder(first.Length);

                return secondSelector(first.Value)
                  .Parse(remainder)
                  .Finally(remainder.Dispose)
                  .SubscribeSafe(
                     second =>
                     {
                       hasResult = true;

                       if (lookAhead != null)
                       {
                         lookAhead.OnCompleted(success: true);

                         observer.OnCompleted();
                       }
                       else
                       {
                         observer.OnNext(second);
                       }
                     },
                     observer.OnError,
                     () =>
                     {
                       if (!hasResult && lookAhead != null)
                       {
                         lookAhead.OnCompleted(success: false);
                       }

                       observer.OnCompleted();
                     });
              })
             select lengthSelector == null
               ? first.Add(second, resultSelector)
               : first.Yield(second, resultSelector, lengthSelector);
    }

    /// <summary>
    /// Projects each match from the specified <paramref name="parser"/> into an observable sequence, merges all of the results
    /// and transforms them with the result selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TFirstResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TCollection">The type of the elements in the sequences that are projected from the matches.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are projected from the projected sequences.</typeparam>
    /// <param name="parser">The parser from which each match is passed to the specified collection selector function.</param>
    /// <param name="collectionSelector">A transform function to apply to each match from the first <paramref name="parser"/>.</param>
    /// <param name="resultSelector">A transform function to apply to each element from the projected sequences.</param>
    /// <returns>A parser that projects each match from the specified <paramref name="parser"/> into an observable sequence, 
    /// merges all of the results and transforms them with the result selector function.</returns>
    public static IObservableParser<TSource, TResult> SelectMany<TSource, TFirstResult, TCollection, TResult>(
      this IObservableParser<TSource, TFirstResult> parser,
      Func<TFirstResult, IObservable<TCollection>> collectionSelector,
      Func<TFirstResult, TCollection, TResult> resultSelector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(collectionSelector != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return parser.Yield("SelectMany", source => SelectManyInternal(source, parser, collectionSelector, resultSelector));
    }

    /// <summary>
    /// Projects each match from the specified <paramref name="parser"/> into an observable sequence, merges all of the results
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
    /// <returns>A parser that projects each match from the specified <paramref name="parser"/> into an observable sequence, 
    /// merges all of the results and transforms them with the result selector function.</returns>
    public static IObservableParser<TSource, TResult> SelectMany<TSource, TFirstResult, TCollection, TResult>(
      this IObservableParser<TSource, TFirstResult> parser,
      Func<TFirstResult, IObservable<TCollection>> collectionSelector,
      Func<TFirstResult, TCollection, TResult> resultSelector,
      Func<IParseResult<TFirstResult>, TCollection, int> lengthSelector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(collectionSelector != null);
      Contract.Requires(resultSelector != null);
      Contract.Requires(lengthSelector != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return parser.Yield("SelectMany", source => SelectManyInternal(source, parser, collectionSelector, resultSelector, lengthSelector));
    }

    private static IObservable<IParseResult<TResult>> SelectManyInternal<TSource, TFirstResult, TCollection, TResult>(
      IObservableCursor<TSource> source,
      IObservableParser<TSource, TFirstResult> parser,
      Func<TFirstResult, IObservable<TCollection>> collectionSelector,
      Func<TFirstResult, TCollection, TResult> resultSelector,
      Func<IParseResult<TFirstResult>, TCollection, int> lengthSelector = null)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Requires(parser != null);
      Contract.Requires(collectionSelector != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(Contract.Result<IObservable<IParseResult<TResult>>>() != null);

      return from first in parser.Parse(source)
             from second in Observable.Create<Tuple<TCollection, bool>>(
               observer =>
               {
                 var lookAhead = first as ILookAheadParseResult<TFirstResult>;
                 var hasResult = false;
                 TCollection previous = default(TCollection);

                 return collectionSelector(first.Value).SubscribeSafe(
                   second =>
                   {
                     if (lookAhead != null)
                     {
                       hasResult = true;

                       lookAhead.OnCompleted(success: true);

                       observer.OnCompleted();
                     }
                     else
                     {
                       if (hasResult)
                       {
                         observer.OnNext(Tuple.Create(previous, false));
                       }

                       hasResult = true;
                       previous = second;
                     }
                   },
                   observer.OnError,
                   () =>
                   {
                     if (hasResult && lookAhead == null)
                     {
                       observer.OnNext(Tuple.Create(previous, true));
                     }
                     else if (!hasResult && lookAhead != null)
                     {
                       lookAhead.OnCompleted(success: false);
                     }

                     observer.OnCompleted();
                   });
               })
             select lengthSelector == null
               ? first.Yield(second.Item1, resultSelector, (f, s) => second.Item2 ? f.Length : 0)
               : first.Yield(second.Item1, resultSelector, lengthSelector);
    }
  }
}