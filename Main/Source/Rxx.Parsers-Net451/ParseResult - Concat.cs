using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers
{
  /// <summary>
  /// Provides <see langword="static" /> methods for creating <see cref="ParseResult{TValue}"/> objects.
  /// </summary>
  public static partial class ParseResult
  {
    /// <summary>
    /// Applies a selector function to two parse results to produce a new parse result.
    /// The lengths are summed to indicate that the new parse result 
    /// encompasses both of the specified parse results and that they match consecutively
    /// in the input sequence.
    /// </summary>
    /// <typeparam name="TFirstResult">The type of the first parse result's value.</typeparam>
    /// <typeparam name="TSecondResult">The type of the second parse result's value.</typeparam>
    /// <typeparam name="TResult">The type of the new parse result's value.</typeparam>
    /// <param name="firstResult">The parse result to be combined with <paramref name="secondResult"/>.</param>
    /// <param name="secondResult">The parse result to be combined with <paramref name="firstResult"/>.</param>
    /// <param name="resultSelector">A function that selects a new parse result from the two specified parse results.</param>
    /// <returns>A new <see cref="IParseResult{TResult}"/> object containing the sum of the specified parse results' 
    /// lengths and the return value of the <paramref name="resultSelector"/> function.</returns>
    public static IParseResult<TResult> Add<TFirstResult, TSecondResult, TResult>(
      this IParseResult<TFirstResult> firstResult,
      IParseResult<TSecondResult> secondResult,
      Func<TFirstResult, TSecondResult, TResult> resultSelector)
    {
      Contract.Requires(firstResult != null);
      Contract.Requires(!(firstResult is ILookAheadParseResult<TFirstResult>));
      Contract.Requires(secondResult != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(Contract.Result<IParseResult<TResult>>() != null);

      return secondResult.Yield(
        resultSelector(firstResult.Value, secondResult.Value),
        firstResult.Length + secondResult.Length);
    }

    /// <summary>
    /// Concatenates the specified parse result value sequence and scalar parse result value into a new parse result value sequence.
    /// The lengths are summed to indicate that the new parse result 
    /// encompasses both of the specified parse results and that they match consecutively
    /// in the input sequence.
    /// </summary>
    /// <typeparam name="TResult">The type of the new parse result's value.</typeparam>
    /// <param name="firstResult">The parse result value sequence to which <paramref name="secondResult"/> will be concatenated.</param>
    /// <param name="secondResult">The scalar parse result value to be concatenated to the value of <paramref name="firstResult"/>.</param>
    /// <returns>A new <see cref="IParseResult{TResult}"/> object containing the sum of the specified parse results' lengths and 
    /// the concatenation of their values.</returns>
    public static IParseResult<IEnumerable<TResult>> Concat<TResult>(
      this IParseResult<IEnumerable<TResult>> firstResult,
      IParseResult<TResult> secondResult)
    {
      Contract.Requires(firstResult != null);
      Contract.Requires(firstResult.Value != null);
      Contract.Requires(!(firstResult is ILookAheadParseResult<IEnumerable<TResult>>));
      Contract.Requires(secondResult != null);
      Contract.Ensures(Contract.Result<IParseResult<IEnumerable<TResult>>>() != null);

      return secondResult.Yield(
        firstResult.Value.ConcatOptimized(new[] { secondResult.Value }),
        firstResult.Length + secondResult.Length);
    }

    /// <summary>
    /// Concatenates the specified scalar parse result value and parse result value sequence into a new parse result value sequence.
    /// The lengths are summed to indicate that the new parse result 
    /// encompasses both of the specified parse results and that they match consecutively
    /// in the input sequence.
    /// </summary>
    /// <typeparam name="TResult">The type of the new parse result's value.</typeparam>
    /// <param name="firstResult">The scalar parse result value to which <paramref name="secondResult"/> will be concatenated.</param>
    /// <param name="secondResult">The parse result value sequence to be concatenated to the value of <paramref name="firstResult"/>.</param>
    /// <returns>A new <see cref="IParseResult{TResult}"/> object containing the sum of the specified parse results' 
    /// lengths and the concatenation of their values.</returns>
    public static IParseResult<IEnumerable<TResult>> Concat<TResult>(
      this IParseResult<TResult> firstResult,
      IParseResult<IEnumerable<TResult>> secondResult)
    {
      Contract.Requires(firstResult != null);
      Contract.Requires(!(firstResult is ILookAheadParseResult<TResult>));
      Contract.Requires(secondResult != null);
      Contract.Requires(secondResult.Value != null);
      Contract.Ensures(Contract.Result<IParseResult<IEnumerable<TResult>>>() != null);

      return secondResult.Yield(
        secondResult.Value.StartWith(firstResult.Value),
        firstResult.Length + secondResult.Length);
    }

    /// <summary>
    /// Concatenates the specified parse result value sequences into a new parse result value sequence.
    /// The lengths are summed to indicate that the new parse result 
    /// encompasses both of the specified parse results and that they match consecutively
    /// in the input sequence.
    /// </summary>
    /// <typeparam name="TResult">The type of the new parse result's value.</typeparam>
    /// <param name="firstResult">The parse result value sequence to which <paramref name="secondResult"/> will be concatenated.</param>
    /// <param name="secondResult">The parse result value sequence to be concatenated to the value sequence of <paramref name="firstResult"/>.</param>
    /// <returns>A new <see cref="IParseResult{TResult}"/> object containing the sum of the specified parse results' 
    /// lengths and the concatenation of their values.</returns>
    public static IParseResult<IEnumerable<TResult>> Concat<TResult>(
      this IParseResult<IEnumerable<TResult>> firstResult,
      IParseResult<IEnumerable<TResult>> secondResult)
    {
      Contract.Requires(firstResult != null);
      Contract.Requires(firstResult.Value != null);
      Contract.Requires(!(firstResult is ILookAheadParseResult<IEnumerable<TResult>>));
      Contract.Requires(secondResult != null);
      Contract.Requires(secondResult.Value != null);
      Contract.Ensures(Contract.Result<IParseResult<IEnumerable<TResult>>>() != null);

      return secondResult.Yield(
        firstResult.Value.ConcatOptimized(secondResult.Value),
        firstResult.Length + secondResult.Length);
    }

    /// <summary>
    /// Concatenates the specified parse result value sequences into a new parse result value sequence.
    /// The lengths are summed to indicate that the new parse result encompasses all of the specified 
    /// parse results and that they match consecutively in the input sequence.
    /// </summary>
    /// <typeparam name="TResult">The type of the new parse result's value.</typeparam>
    /// <param name="firstResult">The parse result value sequence to which <paramref name="otherResults"/> will be concatenated.</param>
    /// <param name="otherResults">Zero or more parse result value sequences to be concatenated to the value sequence of <paramref name="firstResult"/>.</param>
    /// <returns>A new <see cref="IParseResult{TResult}"/> object containing the sum of the specified parse results' lengths and the 
    /// concatenation of their values.</returns>
    public static IParseResult<IEnumerable<TResult>> Concat<TResult>(
      this IParseResult<IEnumerable<TResult>> firstResult,
      IEnumerable<IParseResult<IEnumerable<TResult>>> otherResults)
    {
      Contract.Requires(firstResult != null);
      Contract.Requires(firstResult.Value != null);
      Contract.Requires(!(firstResult is ILookAheadParseResult<IEnumerable<TResult>>));
      Contract.Requires(otherResults != null);
      Contract.Ensures(Contract.Result<IParseResult<IEnumerable<TResult>>>() != null);

      IParseResult<IEnumerable<TResult>> lastResult = null;

      int length = firstResult.Length;

      var value = firstResult.Value.ConcatOptimized(
        otherResults.Do(
          result =>
          {
            lastResult = result;
            length += result.Length;
          })
        .Select(result => result.Value));

      if (lastResult == null)
      {
        return firstResult;
      }
      else
      {
        // yield from lastResult to support ILookAheadParseResult
        return lastResult.Yield(value, length);
      }
    }
  }
}