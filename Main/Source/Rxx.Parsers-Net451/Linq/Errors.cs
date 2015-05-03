using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers.Linq
{
  public static partial class Parser
  {
    /// <summary>
    /// Throws a <see cref="ParseException"/> if the specified <paramref name="parser"/> does not match.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that must succeed otherwise a <see cref="ParseException"/> is thrown.</param>
    /// <returns>A parser that yields the matches from the specified <paramref name="parser"/> or throws
    /// a <see cref="ParseException"/> if there are no matches.</returns>
    public static IParser<TSource, TResult> Required<TSource, TResult>(
      this IParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Required(index => new ParseException(index));
    }

    /// <summary>
    /// Throws a <see cref="ParseException"/> with the specified message if the specified <paramref name="parser"/> 
    /// does not match.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that must succeed otherwise a <see cref="ParseException"/> is thrown.</param>
    /// <param name="errorMessage">A <see cref="string"/> that describes the failed expectation to be used as the message 
    /// in the <see cref="ParseException"/>.</param>
    /// <returns>A parser that yields the matches from the specified <paramref name="parser"/> or throws
    /// a <see cref="ParseException"/> with the specified message if there are no matches.</returns>
    public static IParser<TSource, TResult> Required<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      string errorMessage)
    {
      Contract.Requires(parser != null);
      Contract.Requires(!string.IsNullOrWhiteSpace(errorMessage));
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Required(index => new ParseException(index, errorMessage));
    }

    /// <summary>
    /// Throws a <see cref="ParseException"/> with a message returned by the specified function if the specified 
    /// <paramref name="parser"/> does not match.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that must succeed otherwise a <see cref="ParseException"/> is thrown.</param>
    /// <param name="errorMessageFactory">A function that returns a <see cref="string"/> describing the failed expectation 
    /// to be used as the message in the <see cref="ParseException"/>.</param>
    /// <returns>A parser that yields the matches from the specified <paramref name="parser"/> or throws
    /// a <see cref="ParseException"/> with a message returned by the specified functions if there are no matches.</returns>
    public static IParser<TSource, TResult> Required<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      Func<string> errorMessageFactory)
    {
      Contract.Requires(parser != null);
      Contract.Requires(errorMessageFactory != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Required(index => new ParseException(index, errorMessageFactory()));
    }

    /// <summary>
    /// Throws a <see cref="ParseException"/> returned by the specified function if the specified 
    /// <paramref name="parser"/> does not match.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that must succeed otherwise a <see cref="ParseException"/> is thrown.</param>
    /// <param name="exceptionFactory">A function that returns the <see cref="ParseException"/> to be thrown describing the 
    /// failed expectation.</param>
    /// <returns>A parser that yields the matches from the specified <paramref name="parser"/> or throws
    /// a <see cref="ParseException"/> returned by the specified functions if there are no matches.</returns>
    public static IParser<TSource, TResult> Required<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      Func<int, Exception> exceptionFactory)
    {
      Contract.Requires(parser != null);
      Contract.Requires(exceptionFactory != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Yield("Required", RequiredIterator, exceptionFactory);
    }

    private static IEnumerable<IParseResult<TResult>> RequiredIterator<TSource, TResult>(
      ICursor<TSource> source,
      IParser<TSource, TResult> parser,
      Func<int, Exception> exceptionFactory)
    {
      var index = source.CurrentIndex;

      var hasResult = false;

      foreach (var result in parser.Parse(source))
      {
        hasResult = true;

        yield return result;
      }

      if (!hasResult)
      {
        Exception exception = exceptionFactory(index);

        if (exception == null)
        {
          exception = new ParseException(index);
        }
        else if (!(exception is ParseException))
        {
          exception = new ParseException(index, exception);
        }

#if !SILVERLIGHT && !PORT_45 && !PORT_40
        ParserTraceSources.Input.TraceEvent(System.Diagnostics.TraceEventType.Error, 0, "{0}", exception);
#endif

        throw exception;
      }
    }
  }
}