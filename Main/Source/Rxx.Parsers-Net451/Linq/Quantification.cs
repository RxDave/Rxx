using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers.Linq
{
  public static partial class Parser
  {
    /// <summary>
    /// Matches the specified <paramref name="parser"/> or yields the specified default result if there are 
    /// no matches.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that might produce matches.</param>
    /// <param name="defaultResult">The value that is yielded if the specified <paramref name="parser"/> does not match.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> or the specified default result
    /// if the <paramref name="parser"/> does not match.</returns>
    public static IParser<TSource, TResult> WithDefault<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      TResult defaultResult)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Yield("WithDefault", WithDefaultIterator, defaultResult);
    }

    private static IEnumerable<IParseResult<TResult>> WithDefaultIterator<TSource, TResult>(
      ICursor<TSource> source,
      IParser<TSource, TResult> parser,
      TResult defaultResult)
    {
      bool hasResult = false;

      foreach (var result in parser.Parse(source))
      {
        hasResult = true;

        yield return result;
      }

      if (!hasResult)
      {
        yield return ParseResult.Create(defaultResult, length: 0);
      }
    }

    /// <summary>
    /// Yields success when the specified <paramref name="parser"/> does not match.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser for which any match results in failure.</param>
    /// <returns>A parser that yields failure when the specified <paramref name="parser"/> matches or 
    /// an empty sequence to indicate success when it does not match.</returns>
    public static IParser<TSource, IEnumerable<TResult>> None<TSource, TResult>(
      this IParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      if (parser is IParserCursor<TSource>)
      {
        return parser.AtEndOfSequence();
      }
      else
      {
        return parser.Yield(
          "None",
          source =>
            parser.Parse(source).Any()
            ? ParseResult.ReturnFailureMany<TResult>()
            : ParseResult.ReturnSuccessMany<TResult>(length: 0));
      }
    }

    /// <summary>
    /// Yields success when the specified <paramref name="parser"/> does not match.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="parser">The parser for which any match results in failure.</param>
    /// <param name="successResult">The value that is yielded if the specified <paramref name="parser"/> does not match.</param>
    /// <returns>A parser that yields failure when the specified <paramref name="parser"/> matches or success when 
    /// it does not match.</returns>
    public static IParser<TSource, TSuccess> None<TSource, TResult, TSuccess>(
      this IParser<TSource, TResult> parser,
      TSuccess successResult)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TSuccess>>() != null);

      if (parser is IParserCursor<TSource>)
      {
        return parser.AtEndOfSequence(successResult);
      }
      else
      {
        return parser.Yield(
          "None",
          source =>
            parser.Parse(source).Any()
            ? ParseResult.ReturnFailure<TSuccess>()
            : ParseResult.Return(successResult, length: 0));
      }
    }

    /// <summary>
    /// Yields success if the specified parser starts at the end of the input sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that provides the context in which to check whether the cursor is at the end of the input sequence.</param>
    /// <returns>A new parser that yields success without parsing if the cursor is positioned at the end of the input sequence; otherwise, yields no results.</returns>
    public static IParser<TSource, IEnumerable<TResult>> AtEndOfSequence<TSource, TResult>(
      this IParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      return parser.Yield("AtEndOfSequence", AtEndOfSequenceIterator);
    }

    private static IEnumerable<IParseResult<IEnumerable<TResult>>> AtEndOfSequenceIterator<TSource, TResult>(
      this ICursor<TSource> source,
      IParser<TSource, TResult> parser)
    {
      Contract.Requires(source != null);
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<IEnumerable<TResult>>>>() != null);

      if (source.IsSequenceTerminated)
      {
        if (!source.AtEndOfSequence)
        {
          yield break;
        }
      }
      else
      {
        foreach (var result in source)
        {
          yield break;
        }

        Contract.Assume(source.AtEndOfSequence);
      }

      yield return ParseResult.SuccessMany<TResult>(length: 0);
    }

    /// <summary>
    /// Yields success if the specified parser starts at the end of the input sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="parser">The parser that provides the context in which to check whether the cursor is at the end of the input sequence.</param>
    /// <param name="successResult">The value that is yielded if the specified parser starts at the end of the input sequence.</param>
    /// <returns>A new parser that yields success without parsing if the cursor is positioned at the end of the input sequence; otherwise, yields no results.</returns>
    public static IParser<TSource, TSuccess> AtEndOfSequence<TSource, TResult, TSuccess>(
      this IParser<TSource, TResult> parser,
      TSuccess successResult)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TSuccess>>() != null);

      return parser.Yield("AtEndOfSequence", AtEndOfSequenceIterator, successResult);
    }

    private static IEnumerable<IParseResult<TSuccess>> AtEndOfSequenceIterator<TSource, TResult, TSuccess>(
      this ICursor<TSource> source,
      IParser<TSource, TResult> parser,
      TSuccess successResult)
    {
      Contract.Requires(source != null);
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<TSuccess>>>() != null);

      if (source.IsSequenceTerminated)
      {
        if (!source.AtEndOfSequence)
        {
          yield break;
        }
      }
      else
      {
        foreach (var result in source)
        {
          yield break;
        }

        Contract.Assume(source.AtEndOfSequence);
      }

      yield return ParseResult.Create(successResult, length: 0);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> or yields success without a value when it does not match.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that might produce matches.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> or 
    /// an empty enumerable sequence to indicate success when it does not match.</returns>
    public static IParser<TSource, IEnumerable<TResult>> Maybe<TSource, TResult>(
      this IParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      return parser.Yield("Maybe", MaybeIterator);
    }

    private static IEnumerable<IParseResult<IEnumerable<TResult>>> MaybeIterator<TSource, TResult>(
      ICursor<TSource> source,
      IParser<TSource, TResult> parser)
    {
      bool hasResult = false;

      foreach (var result in parser.Parse(source))
      {
        hasResult = true;

        yield return result.YieldMany();
      }

      if (!hasResult)
      {
        yield return ParseResult.SuccessMany<TResult>(length: 0);
      }
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> or yields success without a value when it does not match.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements of the result sequences that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that might produce matches.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> or 
    /// an empty enumerable sequence to indicate success when it does not match.</returns>
    public static IParser<TSource, IEnumerable<TResult>> Maybe<TSource, TResult>(
      this IParser<TSource, IEnumerable<TResult>> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      return parser.Yield("Maybe", MaybeIterator);
    }

    private static IEnumerable<IParseResult<IEnumerable<TResult>>> MaybeIterator<TSource, TResult>(
      ICursor<TSource> source,
      IParser<TSource, IEnumerable<TResult>> parser)
    {
      bool hasResult = false;

      foreach (var result in parser.Parse(source))
      {
        hasResult = true;

        yield return result;
      }

      if (!hasResult)
      {
        yield return ParseResult.SuccessMany<TResult>(length: 0);
      }
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> the specified number of times.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to be matched.</param>
    /// <param name="count">The specified number of times to match the specified <paramref name="parser"/>.</param>
    /// <returns>A parser that matches the specified <paramref name="parser"/> the specified number of times.</returns>
    public static IParser<TSource, IEnumerable<TResult>> Exactly<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      int count)
    {
      Contract.Requires(parser != null);
      Contract.Requires(count >= 0);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      if (count == 0)
      {
        return parser.Yield(_ => ParseResult.ReturnSuccessMany<TResult>(length: 0));
      }
      else if (count == 1)
      {
        // Profiling has shown this to be about 50% faster than Repeat(parser, 1).All()
        return parser.Amplify();
      }
      else if (parser is IParserCursor<TSource>)
      {
        // Profiling has shown this to be exponentially faster in next.Exactly(largeN) queries.
        return parser.Yield<TSource, TResult, IEnumerable<TResult>>(
          "Exactly",
          source =>
          {
            var list = source.Take(count).Cast<TResult>().ToList().AsReadOnly();

            if (list.Count == count)
            {
              return ParseResult.Return((IEnumerable<TResult>)list, count);
            }
            else
            {
              return ParseResult.ReturnFailureMany<TResult>();
            }
          });
      }
      else
      {
        return Enumerable.Repeat(parser, count).All();
      }
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> zero or more times consecutively.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match zero or more times consecutively.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> zero or more times consecutively.</returns>
    public static IParser<TSource, IEnumerable<TResult>> NoneOrMore<TSource, TResult>(
      this IParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      return parser.AtLeast<TSource, TResult, TResult>("NoneOrMore", 0);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> zero or more times consecutively, making the least number of matches possible.
    /// This is the non-greedy variant of <see cref="NoneOrMore{TSource,TResult}(IParser{TSource,TResult})"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match zero or more times consecutively.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> zero or more times consecutively, 
    /// making the least number of matches possible.</returns>
    public static IParser<TSource, IEnumerable<TResult>> NoneOrMoreNonGreedy<TSource, TResult>(
      this IParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      return parser.AtLeast<TSource, TResult, TResult>("NoneOrMore-NonGreedy", 0, nonGreedy: true);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> zero or more times consecutively, 
    /// matching the specified <paramref name="separator"/> in between.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match zero or more times consecutively.</param>
    /// <param name="separator">The parser that matches between consecutive matches of the specified <paramref name="parser"/>.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> zero or more times consecutively, 
    /// matching the specified <paramref name="separator"/> in between.</returns>
    public static IParser<TSource, IEnumerable<TResult>> NoneOrMore<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      IParser<TSource, TResult> separator)
    {
      Contract.Requires(parser != null);
      Contract.Requires(separator != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      return parser.AtLeast("NoneOrMore-Separated", 0, separator: separator);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> zero or more times consecutively, matching the specified 
    /// <paramref name="separator"/> in between and making the least number of matches possible.
    /// This is the non-greedy variant of <see cref="NoneOrMore{TSource,TResult}(IParser{TSource,TResult},IParser{TSource,TResult})"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match zero or more times consecutively.</param>
    /// <param name="separator">The parser that matches between consecutive matches of the specified <paramref name="parser"/>.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> zero or more times consecutively, 
    /// matching the specified <paramref name="separator"/> in between and making the least number of matches possible.</returns>
    public static IParser<TSource, IEnumerable<TResult>> NoneOrMoreNonGreedy<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      IParser<TSource, TResult> separator)
    {
      Contract.Requires(parser != null);
      Contract.Requires(separator != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      return parser.AtLeast("NoneOrMore-Separated-NonGreedy", 0, separator: separator, nonGreedy: true);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> one or more times consecutively.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match one or more times consecutively.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> one or more times consecutively.</returns>
    public static IParser<TSource, IEnumerable<TResult>> OneOrMore<TSource, TResult>(
      this IParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      return parser.AtLeast<TSource, TResult, TResult>("OneOrMore", 1);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> one or more times consecutively, making the least number of matches possible.
    /// This is the non-greedy variant of <see cref="OneOrMore{TSource,TResult}(IParser{TSource,TResult})"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match one or more times consecutively.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> one or more times consecutively, 
    /// making the least number of matches possible..</returns>
    public static IParser<TSource, IEnumerable<TResult>> OneOrMoreNonGreedy<TSource, TResult>(
      this IParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      return parser.AtLeast<TSource, TResult, TResult>("OneOrMore-NonGreedy", 1, nonGreedy: true);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> one or more times consecutively, 
    /// matching the specified <paramref name="separator"/> in between.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match one or more times consecutively.</param>
    /// <param name="separator">The parser that matches between consecutive matches of the specified <paramref name="parser"/>.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> one or more times consecutively, 
    /// matching the specified <paramref name="separator"/> in between.</returns>
    public static IParser<TSource, IEnumerable<TResult>> OneOrMore<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      IParser<TSource, TResult> separator)
    {
      Contract.Requires(parser != null);
      Contract.Requires(separator != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      return parser.AtLeast("OneOrMore-Separated", 1, separator: separator);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> one or more times consecutively, matching the specified 
    /// <paramref name="separator"/> in between and making the least number of matches possible.
    /// This is the non-greedy variant of <see cref="OneOrMore{TSource,TResult}(IParser{TSource,TResult},IParser{TSource,TResult})"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match one or more times consecutively.</param>
    /// <param name="separator">The parser that matches between consecutive matches of the specified <paramref name="parser"/>.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> one or more times consecutively, 
    /// matching the specified <paramref name="separator"/> in between and making the least number of matches possible.</returns>
    public static IParser<TSource, IEnumerable<TResult>> OneOrMoreNonGreedy<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      IParser<TSource, TResult> separator)
    {
      Contract.Requires(parser != null);
      Contract.Requires(separator != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      return parser.AtLeast("OneOrMore-Separated-NonGreedy", 1, separator: separator, nonGreedy: true);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> consecutively a minimum number of times.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to be matched consecutively a minimum number of times.</param>
    /// <param name="count">The minimum number of times to match the specified <paramref name="parser"/> consecutively.</param>
    /// <returns>A parser that consecutively matches the specified <paramref name="parser"/> the minimum 
    /// number of times specified by <paramref name="count"/>.</returns>
    public static IParser<TSource, IEnumerable<TResult>> AtLeast<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      int count)
    {
      Contract.Requires(parser != null);
      Contract.Requires(count > 0);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      var name = "AtLeast-" + count;

      Contract.Assume(!string.IsNullOrEmpty(name));

      return parser.AtLeast<TSource, TResult, TResult>(name, count);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> consecutively between the specified number of times, inclusive.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to be matched consecutively between the specified number of times.</param>
    /// <param name="count">The minimum number of times to match the specified <paramref name="parser"/> consecutively.</param>
    /// <param name="maximum">The maximum number of times to match the specified <paramref name="parser"/> consecutively.</param>
    /// <returns>A parser that consecutively matches the specified <paramref name="parser"/> between the specified number of 
    /// times, inclusive.</returns>
    public static IParser<TSource, IEnumerable<TResult>> AtLeast<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      int count,
      int maximum)
    {
      Contract.Requires(parser != null);
      Contract.Requires(count > 0);
      Contract.Requires(maximum >= count);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      if (maximum == count)
      {
        return parser.Exactly(count);
      }
      else
      {
        var name = "AtLeast-" + count + "-to-" + maximum;

        Contract.Assume(!string.IsNullOrEmpty(name));

        return parser.AtLeast<TSource, TResult, TResult>(name, count, maximum);
      }
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> consecutively a minimum number of times, making the least number of matches possible.
    /// This is the non-greedy variant of <see cref="AtLeast{TSource,TResult}(IParser{TSource,TResult},int)"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to be matched consecutively a minimum number of times.</param>
    /// <param name="count">The minimum number of times to match the specified <paramref name="parser"/> consecutively.</param>
    /// <returns>A parser that consecutively matches the specified <paramref name="parser"/> the minimum 
    /// number of times specified by <paramref name="count"/>, making the least number of matches possible.</returns>
    public static IParser<TSource, IEnumerable<TResult>> AtLeastNonGreedy<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      int count)
    {
      Contract.Requires(parser != null);
      Contract.Requires(count > 0);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      var name = "AtLeast-NonGreedy-" + count;

      Contract.Assume(!string.IsNullOrEmpty(name));

      return parser.AtLeast<TSource, TResult, TResult>(name, count, nonGreedy: true);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> consecutively between the specified number of times, inclusive, 
    /// making the least number of matches possible.
    /// This is the non-greedy variant of <see cref="AtLeast{TSource,TResult}(IParser{TSource,TResult},int,int)"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to be matched consecutively a minimum number of times.</param>
    /// <param name="count">The minimum number of times to match the specified <paramref name="parser"/> consecutively.</param>
    /// <param name="maximum">The maximum number of times to match the specified <paramref name="parser"/> consecutively.</param>
    /// <returns>A parser that consecutively matches the specified <paramref name="parser"/> between the specified number of 
    /// times, inclusive, making the least number of matches possible.</returns>
    public static IParser<TSource, IEnumerable<TResult>> AtLeastNonGreedy<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      int count,
      int maximum)
    {
      Contract.Requires(parser != null);
      Contract.Requires(count > 0);
      Contract.Requires(maximum >= count);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      if (maximum == count)
      {
        return parser.Exactly(count);
      }
      else
      {
        var name = "AtLeast-NonGreedy-" + count + "-to-" + maximum;

        Contract.Assume(!string.IsNullOrEmpty(name));

        return parser.AtLeast<TSource, TResult, TResult>(name, count, maximum, nonGreedy: true);
      }
    }

    private static IParser<TSource, IEnumerable<TResult>> AtLeast<TSource, TSeparator, TResult>(
      this IParser<TSource, TResult> parser,
      string name,
      int count,
      int maximum = -1,
      IParser<TSource, TSeparator> separator = null,
      bool nonGreedy = false)
    {
      Contract.Requires(parser != null);
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Requires(count >= 0);
      Contract.Requires(maximum == -1 || maximum >= count);
      Contract.Requires(maximum != 0);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TResult>>>() != null);

      return parser.Yield(name, source => AtLeastIterator(source, parser, count, maximum, separator, nonGreedy));
    }

    private static IEnumerable<IParseResult<IEnumerable<TResult>>> AtLeastIterator<TSource, TSeparator, TResult>(
      ICursor<TSource> source,
      IParser<TSource, TResult> parser,
      int count,
      int maximum,
      IParser<TSource, TSeparator> separator,
      bool nonGreedy)
    {
      Contract.Assume(source != null);
      Contract.Assume(source.IsForwardOnly);
      Contract.Assume(parser != null);
      Contract.Assume(count >= 0);
      Contract.Assume(maximum == -1 || maximum >= count);
      Contract.Assume(maximum != 0);

      // TODO: Update this method to properly support multi-result parsers.
      /* The current implementation just uses Math.Max for the lengths and aggregates all of the results into a single list.
       * The correct behavior is more like a SelectMany query, so consider using the new SelectMany overload that AllParser uses.
       */

      /* This method is optimized to prevent stack overflows due to two factors: recursion and using Skip to move the source cursor.
       * 
       * The previous implementation used recursive calls to NoneOrMore, in which there was a linear relationship between the number 
       * of stack frames and the number of elements in the input sequence.  As an input sequence grew and the parser continued matching
       * elements, the number of calls to the Skip operator (via the Remainder extension) grew linearly, and so did the number of branches 
       * due to NoneOrMore using the Or operator, which not only added the Or operator to the stack but added all of the calls to the 
       * quantified parser between the stack frames that Or added, for every subsequent element in the sequence that the parser matched.
       */
      using (var branch = source.Branch())
      {
        var list = new List<TResult>();

        int total = 0;
        int totalLength = 0;

        bool iterate = true;

        if (nonGreedy && count == 0)
        {
          using (var lookAhead = new LookAheadParseResult<IEnumerable<TResult>>(Enumerable.Empty<TResult>(), length: 0))
          {
            yield return lookAhead;

            Contract.Assume(lookAhead.Succeeded.HasValue);

            if (lookAhead.Succeeded.Value)
            {
              iterate = false;
            }
          }
        }

        while (iterate)
        {
          bool hasResult = false;
          bool hasSeparatorResult = false;

          int length = 0;
          int separatorLength = 0;

          foreach (var result in parser.Parse(branch))
          {
            hasResult = true;
            length = Math.Max(length, result.Length);

            list.Add(result.Value);
          }

          branch.Move(length);

          if (separator != null)
          {
            foreach (var separatorResult in separator.Parse(branch))
            {
              hasSeparatorResult = true;
              separatorLength = Math.Max(separatorLength, separatorResult.Length);
            }

            branch.Move(separatorLength);
          }

          if (hasResult)
          {
            totalLength += length + separatorLength;

            if (total < (maximum == -1 ? count : maximum))
            {
              total++;

              if (total == maximum)
              {
                break;
              }
            }

            if (separator == null || hasSeparatorResult)
            {
              if (nonGreedy && total >= count)
              {
                using (var lookAhead = new LookAheadParseResult<IEnumerable<TResult>>(list.AsReadOnly(), totalLength))
                {
                  yield return lookAhead;

                  Contract.Assume(lookAhead.Succeeded.HasValue);

                  if (lookAhead.Succeeded.Value)
                  {
                    break;
                  }
                }
              }

              continue;
            }
          }

          break;
        }

        if (total >= count)
        {
          yield return new ParseResult<IEnumerable<TResult>>(list.AsReadOnly(), totalLength);
        }
      }
    }
  }
}