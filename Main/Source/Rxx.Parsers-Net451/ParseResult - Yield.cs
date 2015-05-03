using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Rxx.Parsers
{
  public static partial class ParseResult
  {
    /// <summary>
    /// Clones the specified <paramref name="result"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="result">The <see cref="IParseResult{TValue}"/> from which to create a new parse result.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the same length and value as the 
    /// specified <paramref name="result"/>.</returns>
    public static IParseResult<TValue> Yield<TValue>(
      this IParseResult<TValue> result)
    {
      Contract.Requires(result != null);
      Contract.Ensures(Contract.Result<IParseResult<TValue>>() != null);

      return result.Yield(result.Value, result.Length);
    }

    /// <summary>
    /// Creates a new parse result with the specified <paramref name="length"/> 
    /// and the same value as the specified <paramref name="result"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="result">The <see cref="IParseResult{TValue}"/> that provides the length.</param>
    /// <param name="length">The length of the new parse result.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the specified <paramref name="length"/> 
    /// and the same value as the specified <paramref name="result"/>.</returns>
    public static IParseResult<TValue> Yield<TValue>(
      this IParseResult<TValue> result,
      int length)
    {
      Contract.Requires(result != null);
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParseResult<TValue>>() != null);

      return result.Yield(result.Value, length);
    }

    /// <summary>
    /// Creates a new parse result with the specified value and the length of the specified 
    /// <paramref name="result"/>.
    /// </summary>
    /// <typeparam name="TOldValue">The type of the old parse result's value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new parse result's value</typeparam>
    /// <param name="result">The <see cref="IParseResult{TOldValue}"/> that provides the length.</param>
    /// <param name="value">The value of the new parse result.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the specified value and the length of the specified 
    /// <paramref name="result"/>.</returns>
    public static IParseResult<TNewValue> Yield<TOldValue, TNewValue>(
      this IParseResult<TOldValue> result,
      TNewValue value)
    {
      Contract.Requires(result != null);
      Contract.Ensures(Contract.Result<IParseResult<TNewValue>>() != null);

      return result.Yield(value, result.Length);
    }

    /// <summary>
    /// Creates a new parse result with the value returned by the specified selector and the length of the 
    /// specified <paramref name="result"/>.
    /// </summary>
    /// <typeparam name="TOldValue">The type of the old parse result's value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new parse result's value</typeparam>
    /// <param name="result">The <see cref="IParseResult{TOldValue}"/> that provides the length.</param>
    /// <param name="valueSelector">A function that selects the value for the new parse result.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the value returned by the specified selector 
    /// and the length of the specified <paramref name="result"/>.</returns>
    public static IParseResult<TNewValue> Yield<TOldValue, TNewValue>(
      this IParseResult<TOldValue> result,
      Func<TOldValue, TNewValue> valueSelector)
    {
      Contract.Requires(result != null);
      Contract.Requires(valueSelector != null);
      Contract.Ensures(Contract.Result<IParseResult<TNewValue>>() != null);

      return result.Yield(valueSelector(result.Value), result.Length);
    }

    /// <summary>
    /// Creates a new parse result from two parse results with the length and value returned 
    /// by the specified selectors.
    /// </summary>
    /// <typeparam name="TFirstValue">The type of the first parse result's value.</typeparam>
    /// <typeparam name="TSecondValue">The type of the second parse result's value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new parse result's value.</typeparam>
    /// <param name="firstResult">The first parse result.</param>
    /// <param name="secondResult">The second parse result.</param>
    /// <param name="valueSelector">A function that selects the value for the new parse result.</param>
    /// <param name="lengthSelector">A function that selects the length for the new parse result.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the length and value returned by the 
    /// specified selectors.</returns>
    public static IParseResult<TNewValue> Yield<TFirstValue, TSecondValue, TNewValue>(
      this IParseResult<TFirstValue> firstResult,
      IParseResult<TSecondValue> secondResult,
      Func<TFirstValue, TSecondValue, TNewValue> valueSelector,
      Func<IParseResult<TFirstValue>, IParseResult<TSecondValue>, int> lengthSelector)
    {
      Contract.Requires(firstResult != null);
      Contract.Requires(!(firstResult is ILookAheadParseResult<TFirstValue>));
      Contract.Requires(secondResult != null);
      Contract.Requires(valueSelector != null);
      Contract.Requires(lengthSelector != null);
      Contract.Ensures(Contract.Result<IParseResult<TNewValue>>() != null);

      int length = lengthSelector(firstResult, secondResult);

      Contract.Assume(length >= 0);

      return secondResult.Yield(
        valueSelector(firstResult.Value, secondResult.Value),
        length);
    }

    /// <summary>
    /// Creates a new parse result from a parse result and another value with the length and value 
    /// returned by the specified selectors.
    /// </summary>
    /// <typeparam name="TOldValue">The type of the old parse result's value.</typeparam>
    /// <typeparam name="TOther">The type of the other value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new parse result's value.</typeparam>
    /// <param name="result">The old parse result from which to create a new parse result.</param>
    /// <param name="other">The other value from which to create a new parse result.</param>
    /// <param name="valueSelector">A function that selects the value for the new parse result.</param>
    /// <param name="lengthSelector">A function that selects the length for the new parse result.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the length and value returned by the 
    /// specified selectors.</returns>
    public static IParseResult<TNewValue> Yield<TOldValue, TOther, TNewValue>(
      this IParseResult<TOldValue> result,
      TOther other,
      Func<TOldValue, TOther, TNewValue> valueSelector,
      Func<IParseResult<TOldValue>, TOther, int> lengthSelector)
    {
      Contract.Requires(result != null);
      Contract.Requires(other != null);
      Contract.Requires(valueSelector != null);
      Contract.Requires(lengthSelector != null);
      Contract.Ensures(Contract.Result<IParseResult<TNewValue>>() != null);

      int length = lengthSelector(result, other);

      Contract.Assume(length >= 0);

      return result.Yield(
        valueSelector(result.Value, other),
        length);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Return value.")]
    internal static IParseResult<TNewValue> Yield<TOldValue, TNewValue>(
      this IParseResult<TOldValue> result,
      TNewValue value,
      int length)
    {
      Contract.Requires(result != null);
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParseResult<TNewValue>>() != null);

      var lookAhead = result as ILookAheadParseResult<TOldValue>;

      if (lookAhead != null)
      {
        var newLookAhead = new LookAheadParseResult<TNewValue>(value, length);

        newLookAhead.Subscribe(lookAhead.OnCompleted);

        return newLookAhead;
      }
      else
      {
        return new ParseResult<TNewValue>(value, length);
      }
    }

    /// <summary>
    /// Creates a new parse result with the length of the specified <paramref name="result"/>
    /// and a singleton enumerable sequence containing the value of the specified <paramref name="result"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="result">The <see cref="IParseResult{TValue}"/> from which to create a new parse result.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the same length and value as the 
    /// specified <paramref name="result"/>, although with the value wrapped in a singleton enumerable sequence.</returns>
    public static IParseResult<IEnumerable<TValue>> YieldMany<TValue>(
      this IParseResult<TValue> result)
    {
      Contract.Requires(result != null);
      Contract.Ensures(Contract.Result<IParseResult<IEnumerable<TValue>>>() != null);

      return result.YieldMany(result.Value, result.Length);
    }

    /// <summary>
    /// Creates a new parse result with the specified <paramref name="length"/> and a singleton enumerable 
    /// sequence containing the value of the specified <paramref name="result"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="result">The <see cref="IParseResult{TValue}"/> from which to create a new parse result.</param>
    /// <param name="length">The length of the new result.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the same length and value as the 
    /// specified <paramref name="result"/>, although with the value wrapped in a singleton enumerable sequence.</returns>
    public static IParseResult<IEnumerable<TValue>> YieldMany<TValue>(
      this IParseResult<TValue> result,
      int length)
    {
      Contract.Requires(result != null);
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParseResult<IEnumerable<TValue>>>() != null);

      return result.YieldMany(result.Value, length);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Return value.")]
    private static IParseResult<IEnumerable<TValue>> YieldMany<TValue>(
      this IParseResult<TValue> result,
      TValue value,
      int length)
    {
      Contract.Requires(result != null);
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParseResult<IEnumerable<TValue>>>() != null);

      var lookAhead = result as ILookAheadParseResult<TValue>;

      if (lookAhead != null)
      {
        var newLookAhead = new LookAheadParseResult<IEnumerable<TValue>>(
          new List<TValue>()
          {
            value
          }
          .AsReadOnly(),
          length);

        newLookAhead.Subscribe(lookAhead.OnCompleted);

        return newLookAhead;
      }
      else
      {
        return new ParseResult<IEnumerable<TValue>>(
          new List<TValue>()
          {
            value
          }
          .AsReadOnly(),
          length);
      }
    }
  }
}