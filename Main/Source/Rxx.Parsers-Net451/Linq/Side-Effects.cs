using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers.Linq
{
  public static partial class Parser
  {
    /// <summary>
    /// Invokes the specified <paramref name="action"/> on each result for its side-effects.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser from which results will be supplied to the specified <paramref name="action"/>.</param>
    /// <param name="action">The method that will be called for each parser result.</param>
    /// <returns>A new parser that is the same as the specified parser and also invokes the specified 
    /// <paramref name="action"/> with each result for its side-effects.</returns>
    public static IParser<TSource, TResult> OnSuccess<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      Action<IParseResult<TResult>> action)
    {
      Contract.Requires(parser != null);
      Contract.Requires(action != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Yield("OnSuccess", source => parser.Parse(source).Do(action));
    }

    /// <summary>
    /// Invokes the specified <paramref name="action"/> for its side-effects if the specified <paramref name="parser"/>
    /// does not yield any results.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser for which no results will cause the specified <paramref name="action"/> to be invoked.</param>
    /// <param name="action">Invoked if the <paramref name="parser"/> does not yield any results.</param>
    /// <returns>A new parser that is the same as the specified parser and also invokes the specified 
    /// <paramref name="action"/> for its side-effects if the specified <paramref name="parser"/> does not yield
    /// any results.</returns>
    public static IParser<TSource, TResult> OnFailure<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      Action action)
    {
      Contract.Requires(parser != null);
      Contract.Requires(action != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Yield("OnFailure", OnFailureIterator, action);
    }

    private static IEnumerable<IParseResult<TResult>> OnFailureIterator<TSource, TResult>(
      this ICursor<TSource> source,
      IParser<TSource, TResult> parser,
      Action action)
    {
      Contract.Requires(source != null);
      Contract.Requires(parser != null);
      Contract.Requires(action != null);
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<TResult>>>() != null);

      bool hasResult = false;

      foreach (var result in parser.Parse(source))
      {
        hasResult = true;

        yield return result;
      }

      if (!hasResult)
      {
        action();
      }
    }

    /// <summary>
    /// Defers creation of a parser until the <see cref="IParser{TSource,TResult}.Parse"/> method is called.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parserFactory">A function that returns the underlying <see cref="IParser{TSource,TResult}"/> on which 
    /// the <see cref="IParser{TSource,TResult}.Parse"/> method will be called.</param>
    /// <returns>A parser that defers creation of its underlying parser until the <see cref="IParser{TSource,TResult}.Parse"/> 
    /// method is called.</returns>
    public static IParser<TSource, TResult> Defer<TSource, TResult>(
      Func<IParser<TSource, TResult>> parserFactory)
    {
      Contract.Requires(parserFactory != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return new AnonymousParser<TSource, TResult>("Defer", parserFactory);
    }
  }
}