using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Rxx.Parsers.Linq
{
  /// <summary>
  /// Provides <see langword="static" /> methods for defining <see cref="IParser{TSource,TResult}"/> grammars.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
    Justification = "Since it primarily provides cohesive monadic extension methods, maintainability and discoverability are not degraded.")]
  public static partial class Parser
  {
    /// <summary>
    /// Applies an <paramref name="accumulator"/> function over each result sequence from the 
    /// specified <paramref name="parser"/> and yields a sequence of accumulated results.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TAccumulate">The type of the accumulation.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from projecting the accumulation.</typeparam>
    /// <param name="parser">The parser that produces a sequence of result sequences to be aggregated.</param>
    /// <param name="seed">A function that returns the initial value of the accumulation for each parse result.</param>
    /// <param name="accumulator">A function to be invoked on each element of each parse result.</param>
    /// <param name="selector">A function that projects the final aggregation of each parse result.</param>
    /// <returns>A parser that returns the aggregated results.</returns>
    public static IParser<TSource, TResult> Aggregate<TSource, TIntermediate, TAccumulate, TResult>(
      this IParser<TSource, IEnumerable<TIntermediate>> parser,
      Func<TAccumulate> seed,
      Func<TAccumulate, TIntermediate, TAccumulate> accumulator,
      Func<TAccumulate, TResult> selector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(accumulator != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Yield(
        "Aggregate",
        source =>
          from result in parser.Parse(source)
          let acc = result.Value.Aggregate(seed(), accumulator)
          select result.Yield(selector(acc)));
    }

    /// <summary>
    /// Appends each element in each result sequence from the specified <paramref name="parser"/> 
    /// to an accumulated <see cref="string"/>, yielding a single <see cref="string"/> per result
    /// sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that produces a sequence of result sequences to be joined into strings.</param>
    /// <returns>A parser that returns the aggregated <see cref="string"/> results.</returns>
    public static IParser<TSource, string> Join<TSource, TIntermediate>(
      this IParser<TSource, IEnumerable<TIntermediate>> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, string>>() != null);

      return Join(parser, v => v, v => v);
    }

    /// <summary>
    /// Appends each element in each result from the specified <paramref name="parser"/> 
    /// to an accumulated <see cref="string"/> and projects the strings for each result.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from projecting the accumulated <see cref="string"/>.</typeparam>
    /// <param name="parser">The parser that produces a sequence of result sequences to be joined.</param>
    /// <param name="selector">A function that projects the aggregated <see cref="string"/> of each parse result.</param>
    /// <returns>A parser that returns the joined results.</returns>
    public static IParser<TSource, TResult> Join<TSource, TIntermediate, TResult>(
      this IParser<TSource, IEnumerable<TIntermediate>> parser,
      Func<string, TResult> selector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return Join(parser, v => v, selector);
    }

    /// <summary>
    /// Applies a <paramref name="joiner"/> function over each result from the specified 
    /// <paramref name="parser"/> to create an accumulated <see cref="string"/> and projects 
    /// the strings for each result.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TJoin">The type of the accumulation on which <see cref="object.ToString"/> is called.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from projecting the accumulated <see cref="string"/>.</typeparam>
    /// <param name="parser">The parser that produces a sequence of result sequences to be joined.</param>
    /// <param name="joiner">A function to be invoked on each element of each parse result to produce a value 
    /// on which <see cref="object.ToString"/> is called and appended to the accumulation.</param>
    /// <param name="selector">A function that projects the aggregated <see cref="string"/> of each parse result.</param>
    /// <returns>A parser that returns the joined results.</returns>
    public static IParser<TSource, TResult> Join<TSource, TIntermediate, TJoin, TResult>(
      this IParser<TSource, IEnumerable<TIntermediate>> parser,
      Func<TIntermediate, TJoin> joiner,
      Func<string, TResult> selector)
    {
      Contract.Requires(parser != null);
      Contract.Requires(joiner != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return Aggregate(
        parser,
        () => new StringBuilder(),
        (builder, value) => builder.Append(joiner(value)),
        builder => selector(builder.ToString()));
    }

    /// <summary>
    /// Appends the results of each result sequence from the specified <paramref name="parser"/> into an <see cref="IList{TResult}"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated by the parser.</typeparam>
    /// <param name="parser">The parser that produces a sequence of result sequences to be aggregated.</param>
    /// <returns>A parser that returns the results aggregated into an <see cref="IList{TResult}"/>.</returns>
    public static IParser<TSource, IList<TResult>> ToList<TSource, TResult>(
      this IParser<TSource, IEnumerable<TResult>> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IList<TResult>>>() != null);

      return parser.Aggregate(
        () => new List<TResult>(),
        (list, result) =>
        {
          list.Add(result);
          return list;
        },
        list => (IList<TResult>)list.AsReadOnly());
    }
  }
}