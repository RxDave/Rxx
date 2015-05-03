using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Provides <see langword="static" /> methods for creating <see cref="ParseResult{TValue}"/> objects.
  /// </summary>
  public static partial class ObservableParseResult
  {
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
    public static IParseResult<IObservable<TResult>> Concat<TResult>(
      this IParseResult<IObservable<TResult>> firstResult,
      IParseResult<TResult> secondResult)
    {
      Contract.Requires(firstResult != null);
      Contract.Requires(firstResult.Value != null);
      Contract.Requires(!(firstResult is ILookAheadParseResult<IObservable<TResult>>));
      Contract.Requires(secondResult != null);
      Contract.Ensures(Contract.Result<IParseResult<IObservable<TResult>>>() != null);

      return secondResult.Yield(
        firstResult.Value.Concat(Observable.Return(secondResult.Value)),
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
    public static IParseResult<IObservable<TResult>> Concat<TResult>(
      this IParseResult<TResult> firstResult,
      IParseResult<IObservable<TResult>> secondResult)
    {
      Contract.Requires(firstResult != null);
      Contract.Requires(!(firstResult is ILookAheadParseResult<IObservable<TResult>>));
      Contract.Requires(secondResult != null);
      Contract.Requires(secondResult.Value != null);
      Contract.Ensures(Contract.Result<IParseResult<IObservable<TResult>>>() != null);

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
    public static IParseResult<IObservable<TResult>> Concat<TResult>(
      this IParseResult<IObservable<TResult>> firstResult,
      IParseResult<IObservable<TResult>> secondResult)
    {
      Contract.Requires(firstResult != null);
      Contract.Requires(firstResult.Value != null);
      Contract.Requires(!(firstResult is ILookAheadParseResult<IObservable<TResult>>));
      Contract.Requires(secondResult != null);
      Contract.Requires(secondResult.Value != null);
      Contract.Ensures(Contract.Result<IParseResult<IObservable<TResult>>>() != null);

      return secondResult.Yield(
        firstResult.Value.Concat(secondResult.Value),
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
    public static IParseResult<IObservable<TResult>> Concat<TResult>(
      this IParseResult<IObservable<TResult>> firstResult,
      IEnumerable<IParseResult<IObservable<TResult>>> otherResults)
    {
      Contract.Requires(firstResult != null);
      Contract.Requires(firstResult.Value != null);
      Contract.Requires(!(firstResult is ILookAheadParseResult<IObservable<TResult>>));
      Contract.Requires(otherResults != null);
      Contract.Ensures(Contract.Result<IParseResult<IObservable<TResult>>>() != null);

      IParseResult<IObservable<TResult>> lastResult = null;

      int length = firstResult.Length;

      foreach (var result in otherResults)
      {
        Contract.Assume(result != null);

        lastResult = result;
        length += result.Length;
      }

      if (lastResult == null)
      {
        return firstResult;
      }
      else
      {
        // yield from lastResult to support ILookAheadParseResult
        return lastResult.Yield(
          otherResults
            .StartWith(firstResult)
            .Select(result => result.Value)
            .Concat(),
          length);
      }
    }
  }
}