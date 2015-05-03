using System;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;

namespace Rxx.Parsers.Reactive.Linq
{
  public static partial class ObservableParser
  {
    /// <summary>
    /// Creates a parser with the specified <paramref name="parse"/> function, starting at the index of the specified parser.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from the specified <paramref name="parse"/> 
    /// function.</typeparam>
    /// <param name="parser">The parser at which parsing begins.</param>
    /// <param name="parse">A function that defines the behavior of the <see cref="IObservableParser{TSource,TResult}.Parse"/> method 
    /// for the generated parser.</param>
    /// <returns>A parser with the specified <paramref name="parse"/> function.</returns>
    public static IObservableParser<TSource, TResult> Yield<TSource, TIntermediate, TResult>(
      this IObservableParser<TSource, TIntermediate> parser,
      Func<IObservableCursor<TSource>, IObservable<IParseResult<TResult>>> parse)
    {
      Contract.Requires(parser != null);
      Contract.Requires(parse != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return new AnonymousObservableParser<TSource, TResult>(
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
    /// <param name="parse">A function that defines the behavior of the <see cref="IObservableParser{TSource,TResult}.Parse"/> method 
    /// for the generated parser.</param>
    /// <returns>A parser with the specified <paramref name="parse"/> function.</returns>
    public static IObservableParser<TSource, TResult> Yield<TSource, TIntermediate, TResult>(
      this IObservableParser<TSource, TIntermediate> parser,
      string name,
      Func<IObservableCursor<TSource>, IObservable<IParseResult<TResult>>> parse)
    {
      Contract.Requires(parser != null);
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Requires(parse != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return new AnonymousObservableParser<TSource, TResult>(
        name,
        () => parser.Next,
        parse);
    }

    /// <summary>
    /// Creates a parser with the specified subscriber function, starting at the index of the specified parser.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser at which parsing begins.</param>
    /// <param name="parseSubscribe">A function that defines the behavior of the <see cref="IObservableParser{TSource,TResult}.Parse"/> method 
    /// for the generated parser as an observable subscription.</param>
    /// <returns>A parser with the specified subscriber function.</returns>
    public static IObservableParser<TSource, TResult> Yield<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser,
      Func<IObservableCursor<TSource>, IObserver<IParseResult<TResult>>, IDisposable> parseSubscribe)
    {
      Contract.Requires(parser != null);
      Contract.Requires(parseSubscribe != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return new AnonymousObservableParser<TSource, TResult>(
        null,
        () => parser.Next,
        source => Observable.Create<IParseResult<TResult>>(observer => parseSubscribe(source, observer)));
    }

    /// <summary>
    /// Creates a parser with the specified subscriber function, starting at the index of the specified parser.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser at which parsing begins.</param>
    /// <param name="name">The name of the parser that will appear in diagnostic trace output.</param>
    /// <param name="parseSubscribe">A function that defines the behavior of the <see cref="IObservableParser{TSource,TResult}.Parse"/> method 
    /// for the generated parser as an observable subscription.</param>
    /// <returns>A parser with the specified subscriber function.</returns>
    public static IObservableParser<TSource, TResult> Yield<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser,
      string name,
      Func<IObservableCursor<TSource>, IObserver<IParseResult<TResult>>, IDisposable> parseSubscribe)
    {
      Contract.Requires(parser != null);
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Requires(parseSubscribe != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return new AnonymousObservableParser<TSource, TResult>(
        name,
        () => parser.Next,
        source => Observable.Create<IParseResult<TResult>>(observer => parseSubscribe(source, observer)));
    }

    /// <summary>
    /// Creates a parser with the specified subscriber function, starting at the index of the specified parser.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from the specified subscriber function.</typeparam>
    /// <param name="parser">The parser at which parsing begins.</param>
    /// <param name="parseSubscribe">A function that defines the behavior of the <see cref="IObservableParser{TSource,TResult}.Parse"/> method 
    /// for the generated parser as an observable subscription.</param>
    /// <returns>A parser with the specified subscriber function.</returns>
    public static IObservableParser<TSource, TResult> Yield<TSource, TIntermediate, TResult>(
      this IObservableParser<TSource, TIntermediate> parser,
      Func<IObservableCursor<TSource>, IObserver<IParseResult<TResult>>, IDisposable> parseSubscribe)
    {
      Contract.Requires(parser != null);
      Contract.Requires(parseSubscribe != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return new AnonymousObservableParser<TSource, TResult>(
        null,
        () => parser.Next,
        source => Observable.Create<IParseResult<TResult>>(observer => parseSubscribe(source, observer)));
    }

    /// <summary>
    /// Creates a parser with the specified subscriber function, starting at the index of the specified parser.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from the specified subscriber function.</typeparam>
    /// <param name="parser">The parser at which parsing begins.</param>
    /// <param name="name">The name of the parser that will appear in diagnostic trace output.</param>
    /// <param name="parseSubscribe">A function that defines the behavior of the <see cref="IObservableParser{TSource,TResult}.Parse"/> method 
    /// for the generated parser as an observable subscription.</param>
    /// <returns>A parser with the specified subscriber function.</returns>
    public static IObservableParser<TSource, TResult> Yield<TSource, TIntermediate, TResult>(
      this IObservableParser<TSource, TIntermediate> parser,
      string name,
      Func<IObservableCursor<TSource>, IObserver<IParseResult<TResult>>, IDisposable> parseSubscribe)
    {
      Contract.Requires(parser != null);
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Requires(parseSubscribe != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return new AnonymousObservableParser<TSource, TResult>(
        name,
        () => parser.Next,
        source => Observable.Create<IParseResult<TResult>>(observer => parseSubscribe(source, observer)));
    }
  }
}