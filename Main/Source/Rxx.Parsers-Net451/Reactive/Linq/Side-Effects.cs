using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace Rxx.Parsers.Reactive.Linq
{
  public static partial class ObservableParser
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
    public static IObservableParser<TSource, TResult> OnSuccess<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser,
      Action<IParseResult<TResult>> action)
    {
      Contract.Requires(parser != null);
      Contract.Requires(action != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "All exceptions are sent to observers.")]
    public static IObservableParser<TSource, TResult> OnFailure<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser,
      Action action)
    {
      Contract.Requires(parser != null);
      Contract.Requires(action != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return parser.Yield(
        "OnFailure",
        (source, observer) =>
        {
          bool hasResult = false;

          return parser.Parse(source).SubscribeSafe(
            result =>
            {
              hasResult = true;

              observer.OnNext(result);
            },
            observer.OnError,
            () =>
            {
              if (!hasResult)
              {
                try
                {
                  action();
                }
                catch (Exception ex)
                {
                  observer.OnError(ex);
                  return;
                }
              }

              observer.OnCompleted();
            });
        });
    }

    /// <summary>
    /// Defers creation of a parser until the <see cref="IObservableParser{TSource,TResult}.Parse"/> method is called.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parserFactory">A function that returns the underlying <see cref="IObservableParser{TSource,TResult}"/> on which 
    /// the <see cref="IObservableParser{TSource,TResult}.Parse"/> method will be called.</param>
    /// <returns>A parser that defers creation of its underlying parser until the <see cref="IObservableParser{TSource,TResult}.Parse"/> 
    /// method is called.</returns>
    public static IObservableParser<TSource, TResult> Defer<TSource, TResult>(
      Func<IObservableParser<TSource, TResult>> parserFactory)
    {
      Contract.Requires(parserFactory != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return new AnonymousObservableParser<TSource, TResult>("Defer", parserFactory);
    }
  }
}