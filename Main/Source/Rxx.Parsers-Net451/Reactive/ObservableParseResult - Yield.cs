using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace Rxx.Parsers.Reactive
{
  public static partial class ObservableParseResult
  {
    /// <summary>
    /// Creates a new parse result with the length of the specified <paramref name="result"/>
    /// and a singleton observable sequence containing the value of the specified <paramref name="result"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="result">The <see cref="IParseResult{TValue}"/> from which to create a new parse result.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the same length and value as the 
    /// specified <paramref name="result"/>, although with the value wrapped in a singleton observable sequence.</returns>
    public static IParseResult<IObservable<TValue>> YieldMany<TValue>(
      this IParseResult<TValue> result)
    {
      Contract.Requires(result != null);
      Contract.Ensures(Contract.Result<IParseResult<IObservable<TValue>>>() != null);

      return result.YieldMany(result.Length);
    }

    /// <summary>
    /// Creates a new parse result with the specified <paramref name="length"/> and a singleton observable 
    /// sequence containing the value of the specified <paramref name="result"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="result">The <see cref="IParseResult{TValue}"/> from which to create a new parse result.</param>
    /// <param name="length">The length of the new result.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the same length and value as the 
    /// specified <paramref name="result"/>, although with the value wrapped in a singleton observable sequence.</returns>
    public static IParseResult<IObservable<TValue>> YieldMany<TValue>(
      this IParseResult<TValue> result,
      int length)
    {
      Contract.Requires(result != null);
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParseResult<IObservable<TValue>>>() != null);

      return new ParseResult<IObservable<TValue>>(Observable.Return(result.Value), length);
    }
  }
}
