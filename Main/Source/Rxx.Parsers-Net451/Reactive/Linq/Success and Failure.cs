using System;
using System.Diagnostics.Contracts;

namespace Rxx.Parsers.Reactive.Linq
{
  public static partial class ObservableParser
  {
    /// <summary>
    /// Indicates a successful parse operation without actually parsing by yielding the specified scalar <paramref name="result"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the <paramref name="result"/>.</typeparam>
    /// <param name="parser">The parser for which the specified <paramref name="result"/> indicates success.</param>
    /// <param name="result">The value of the created parser's result.</param>
    /// <returns>A parser that always returns the specified scalar <paramref name="result"/> with a length 
    /// of zero, starting from the index at which the specified <paramref name="parser"/> starts.</returns>
    public static IObservableParser<TSource, TResult> Success<TSource, TIntermediate, TResult>(
      this IObservableParser<TSource, TIntermediate> parser,
      TResult result)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return parser.Success(result, length: 0);
    }

    /// <summary>
    /// Indicates a successful parse operation without actually parsing by yielding the specified scalar <paramref name="result"/> 
    /// with the specified length.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIntermediate">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the <paramref name="result"/>.</typeparam>
    /// <param name="parser">The parser for which the specified <paramref name="result"/> indicates success.</param>
    /// <param name="result">The value of the created parser's result.</param>
    /// <param name="length">The length of the created parser's result.</param>
    /// <returns>A parser that always returns the specified scalar <paramref name="result"/> with the specified 
    /// length, starting from the index at which the specified <paramref name="parser"/> starts.</returns>
    public static IObservableParser<TSource, TResult> Success<TSource, TIntermediate, TResult>(
      this IObservableParser<TSource, TIntermediate> parser,
      TResult result,
      int length)
    {
      Contract.Requires(parser != null);
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return parser.Yield("Success", source => ObservableParseResult.Return(result, length));
    }

    /// <summary>
    /// Indicates a successful parse operation without actually parsing by yielding a single result containing an empty sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser for which a single empty result sequence is returned to indicate success.</param>
    /// <returns>A parser that returns a single result containing an empty sequence with a length 
    /// of zero, starting from the index at which the specified <paramref name="parser"/> starts.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> Success<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.Yield("Success", source => ObservableParseResult.ReturnSuccessMany<TResult>(length: 0));
    }

    /// <summary>
    /// Indicates a successful parse operation without actually parsing by yielding a single result containing an empty sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements in the sequences that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser for which a single empty result sequence is returned to indicate success.</param>
    /// <remarks>
    /// <see cref="Success{TSource,TResult}(IObservableParser{TSource,IObservable{TResult}})"/> is required as an explicit overload
    /// because the meaning of the parser's result sequence is special and must not be compounded into a sequence of sequences, 
    /// which would happen if the <see cref="Success{TSource,TResult}(IObservableParser{TSource,TResult})"/> overload were to be called
    /// instead.
    /// </remarks>
    /// <returns>A parser that returns a single result containing an empty sequence with a length 
    /// of zero, starting from the index at which the specified <paramref name="parser"/> starts.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> Success<TSource, TResult>(
      this IObservableParser<TSource, IObservable<TResult>> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.Yield("Success", source => ObservableParseResult.ReturnSuccessMany<TResult>(length: 0));
    }

    /// <summary>
    /// Indicates a failure to parse without actually parsing by returning an empty sequence of parse results.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that is to fail.</param>
    /// <returns>A parser that always returns an empty sequence of parse results, starting from the index at which 
    /// the specified <paramref name="parser"/> starts.</returns>
    public static IObservableParser<TSource, TResult> Failure<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return parser.Yield("Failure", source => ObservableParseResult.ReturnFailure<TResult>());
    }
  }
}
