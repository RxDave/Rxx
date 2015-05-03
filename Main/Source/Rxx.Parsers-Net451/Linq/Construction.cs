using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers.Linq
{
  public static partial class Parser
  {
    /// <summary>
    /// Creates a parser with the specified <paramref name="parse"/> function, starting at the index of the specified parser.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from the specified <paramref name="parse"/> 
    /// function.</typeparam>
    /// <param name="parser">The parser at which parsing begins.</param>
    /// <param name="parse">A function that defines the behavior of the <see cref="IParser{TSource,TResult}.Parse"/> method 
    /// for the generated parser.</param>
    /// <returns>A parser with the specified <paramref name="parse"/> function.</returns>
    public static IParser<TSource, TResult> Yield<TSource, TIntermediate, TResult>(
      this IParser<TSource, TIntermediate> parser,
      Func<ICursor<TSource>, IEnumerable<IParseResult<TResult>>> parse)
    {
      Contract.Requires(parser != null);
      Contract.Requires(parse != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return new AnonymousParser<TSource, TResult>(
        null,
        () => parser.Next,
        parse);
    }

    /// <summary>
    /// Creates a parser with the specified <paramref name="parse"/> function, starting at the index of the specified parser.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from the specified <paramref name="parse"/> 
    /// function.</typeparam>
    /// <param name="parser">The parser at which parsing begins.</param>
    /// <param name="name">The name of the parser that will appear in diagnostic trace output.</param>
    /// <param name="parse">A function that defines the behavior of the <see cref="IParser{TSource,TResult}.Parse"/> method 
    /// for the generated parser.</param>
    /// <returns>A parser with the specified <paramref name="parse"/> function.</returns>
    public static IParser<TSource, TResult> Yield<TSource, TIntermediate, TResult>(
      this IParser<TSource, TIntermediate> parser,
      string name,
      Func<ICursor<TSource>, IEnumerable<IParseResult<TResult>>> parse)
    {
      Contract.Requires(parser != null);
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Requires(parse != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return new AnonymousParser<TSource, TResult>(
        name,
        () => parser.Next,
        parse);
    }

    /// <summary>
    /// Creates a parser with the specified <paramref name="parse"/> function, starting at the index of the specified parser.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from the specified <paramref name="parse"/> 
    /// function.</typeparam>
    /// <param name="parser">The parser at which parsing begins.</param>
    /// <param name="parse">A function that defines the behavior of the <see cref="IParser{TSource,TResult}.Parse"/> method 
    /// for the generated parser.</param>
    /// <remarks>
    /// <see cref="Yield{TSource,TIntermediate,TResult}(IParser{TSource,TIntermediate},Func{ICursor{TSource},IParser{TSource,TIntermediate},IEnumerable{IParseResult{TResult}}})"/>
    /// is merely a convenience extension that works similar to 
    /// <see cref="Yield{TSource,TIntermediate,TResult}(IParser{TSource,TIntermediate},Func{ICursor{TSource},IEnumerable{IParseResult{TResult}}})"/>
    /// except that it passes the specified <paramref name="parser"/> to the <paramref name="parse"/> function to facilitate writing 
    /// iterator blocks that must reference the original parser.
    /// </remarks>
    /// <returns>A parser with the specified <paramref name="parse"/> function.</returns>
    public static IParser<TSource, TResult> Yield<TSource, TIntermediate, TResult>(
      this IParser<TSource, TIntermediate> parser,
      Func<ICursor<TSource>, IParser<TSource, TIntermediate>, IEnumerable<IParseResult<TResult>>> parse)
    {
      Contract.Requires(parser != null);
      Contract.Requires(parse != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return new AnonymousParser<TSource, TResult>(
        null,
        () => parser.Next,
        source => parse(source, parser));
    }

    /// <summary>
    /// Creates a parser with the specified <paramref name="parse"/> function, starting at the index of the specified parser.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from the specified <paramref name="parse"/> 
    /// function.</typeparam>
    /// <param name="parser">The parser at which parsing begins.</param>
    /// <param name="name">The name of the parser that will appear in diagnostic trace output.</param>
    /// <param name="parse">A function that defines the behavior of the <see cref="IParser{TSource,TResult}.Parse"/> method 
    /// for the generated parser.</param>
    /// <remarks>
    /// <see cref="Yield{TSource,TIntermediate,TResult}(IParser{TSource,TIntermediate},Func{ICursor{TSource},IParser{TSource,TIntermediate},IEnumerable{IParseResult{TResult}}})"/>
    /// is merely a convenience extension that works similar to 
    /// <see cref="Yield{TSource,TIntermediate,TResult}(IParser{TSource,TIntermediate},Func{ICursor{TSource},IEnumerable{IParseResult{TResult}}})"/>
    /// except that it passes the specified <paramref name="parser"/> to the <paramref name="parse"/> function to facilitate writing 
    /// iterator blocks that must reference the original parser.
    /// </remarks>
    /// <returns>A parser with the specified <paramref name="parse"/> function.</returns>
    public static IParser<TSource, TResult> Yield<TSource, TIntermediate, TResult>(
      this IParser<TSource, TIntermediate> parser,
      string name,
      Func<ICursor<TSource>, IParser<TSource, TIntermediate>, IEnumerable<IParseResult<TResult>>> parse)
    {
      Contract.Requires(parser != null);
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Requires(parse != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return new AnonymousParser<TSource, TResult>(
        name,
        () => parser.Next,
        source => parse(source, parser));
    }

    /// <summary>
    /// Creates a parser with the specified <paramref name="parse"/> function and <paramref name="argument"/>, starting at the index 
    /// of the specified parser.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TArgument">The type of the extra argument that is passed to the <paramref name="parse"/> function.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from the specified <paramref name="parse"/> 
    /// function.</typeparam>
    /// <param name="parser">The parser at which parsing begins.</param>
    /// <param name="parse">A function that defines the behavior of the <see cref="IParser{TSource,TResult}.Parse"/> method 
    /// for the generated parser.</param>
    /// <param name="argument">Extra data to be passed to the <paramref name="parse"/> function.</param>
    /// <remarks>
    /// <see cref="Yield{TSource,TIntermediate,TResult}(IParser{TSource,TIntermediate},Func{ICursor{TSource},IParser{TSource,TIntermediate},IEnumerable{IParseResult{TResult}}})"/>
    /// is merely a convenience extension that works similar to 
    /// <see cref="Yield{TSource,TIntermediate,TResult}(IParser{TSource,TIntermediate},Func{ICursor{TSource},IEnumerable{IParseResult{TResult}}})"/>
    /// except that it passes the specified <paramref name="parser"/> to the <paramref name="parse"/> function to facilitate writing 
    /// iterator blocks that must reference the original parser.
    /// </remarks>
    /// <returns>A parser with the specified <paramref name="parse"/> function.</returns>
    public static IParser<TSource, TResult> Yield<TSource, TIntermediate, TArgument, TResult>(
      this IParser<TSource, TIntermediate> parser,
      Func<ICursor<TSource>, IParser<TSource, TIntermediate>, TArgument, IEnumerable<IParseResult<TResult>>> parse,
      TArgument argument)
    {
      Contract.Requires(parser != null);
      Contract.Requires(parse != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return new AnonymousParser<TSource, TResult>(
        null,
        () => parser.Next,
        source => parse(source, parser, argument));
    }

    /// <summary>
    /// Creates a parser with the specified <paramref name="parse"/> function and <paramref name="argument"/>, starting at the index 
    /// of the specified parser.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TArgument">The type of the extra argument that is passed to the <paramref name="parse"/> function.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from the specified <paramref name="parse"/> 
    /// function.</typeparam>
    /// <param name="parser">The parser at which parsing begins.</param>
    /// <param name="name">The name of the parser that will appear in diagnostic trace output.</param>
    /// <param name="parse">A function that defines the behavior of the <see cref="IParser{TSource,TResult}.Parse"/> method 
    /// for the generated parser.</param>
    /// <param name="argument">Extra data to be passed to the <paramref name="parse"/> function.</param>
    /// <remarks>
    /// <see cref="Yield{TSource,TIntermediate,TResult}(IParser{TSource,TIntermediate},Func{ICursor{TSource},IParser{TSource,TIntermediate},IEnumerable{IParseResult{TResult}}})"/>
    /// is merely a convenience extension that works similar to 
    /// <see cref="Yield{TSource,TIntermediate,TResult}(IParser{TSource,TIntermediate},Func{ICursor{TSource},IEnumerable{IParseResult{TResult}}})"/>
    /// except that it passes the specified <paramref name="parser"/> to the <paramref name="parse"/> function to facilitate writing 
    /// iterator blocks that must reference the original parser.
    /// </remarks>
    /// <returns>A parser with the specified <paramref name="parse"/> function.</returns>
    public static IParser<TSource, TResult> Yield<TSource, TIntermediate, TArgument, TResult>(
      this IParser<TSource, TIntermediate> parser,
      string name,
      Func<ICursor<TSource>, IParser<TSource, TIntermediate>, TArgument, IEnumerable<IParseResult<TResult>>> parse,
      TArgument argument)
    {
      Contract.Requires(parser != null);
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Requires(parse != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return new AnonymousParser<TSource, TResult>(
        name,
        () => parser.Next,
        source => parse(source, parser, argument));
    }
  }
}