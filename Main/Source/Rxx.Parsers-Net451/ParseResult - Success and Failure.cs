using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers
{
  public static partial class ParseResult
  {
    /// <summary>
    /// Creates a new <see cref="IParseResult{TValue}"/> with the specified <paramref name="length"/>
    /// and the default value for the specified <typeparamref name="TValue"/> type.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="length">The number of elements that were consumed in the sequence.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the specified 
    /// <paramref name="length"/> and a default value.</returns>
    public static IParseResult<TValue> Success<TValue>(int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParseResult<TValue>>() != null);

      return new ParseResult<TValue>(length);
    }

    /// <summary>
    /// Creates a new <see cref="IParseResult{TValue}"/> with the specified 
    /// <paramref name="length"/> and an empty sequence for the value.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="length">The number of elements that were consumed in the sequence.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the specified 
    /// <paramref name="length"/> and an empty sequence for the value.</returns>
    public static IParseResult<IEnumerable<TValue>> SuccessMany<TValue>(int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParseResult<IEnumerable<TValue>>>() != null);

      return new ParseResult<IEnumerable<TValue>>(Enumerable.Empty<TValue>(), length);
    }

    /// <summary>
    /// Creates a singleton enumerable sequence containing an <see cref="IParseResult{TValue}"/> with 
    /// the specified <paramref name="length"/> and the default value for the specified 
    /// <typeparamref name="TValue"/> type.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="length">The number of elements that were consumed in the sequence.</param>
    /// <returns>A singleton enumerable sequence containing an <see cref="IParseResult{TValue}"/> with 
    /// the specified <paramref name="length"/> and a default value.</returns>
    public static IEnumerable<IParseResult<TValue>> ReturnSuccess<TValue>(int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<TValue>>>() != null);

      return new List<IParseResult<TValue>>()
			{
				Success<TValue>(length)
			}
      .AsReadOnly();
    }

    /// <summary>
    /// Creates a singleton enumerable sequence containing an <see cref="IParseResult{TValue}"/> 
    /// with the specified <paramref name="length"/> and an empty sequence for the value.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="length">The number of elements that were consumed in the sequence.</param>
    /// <returns>A singleton enumerable sequence containing an <see cref="IParseResult{TValue}"/> 
    /// with the specified <paramref name="length"/> and an empty sequence for the value.</returns>
    public static IEnumerable<IParseResult<IEnumerable<TValue>>> ReturnSuccessMany<TValue>(int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<IEnumerable<TValue>>>>() != null);

      return new List<IParseResult<IEnumerable<TValue>>>()
			{
				SuccessMany<TValue>(length)
			}
      .AsReadOnly();
    }

    /// <summary>
    /// Creates an empty enumerable sequence of <see cref="IParseResult{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <returns>An empty enumerable sequence of <see cref="IParseResult{TValue}"/>.</returns>
    public static IEnumerable<IParseResult<TValue>> ReturnFailure<TValue>()
    {
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<TValue>>>() != null);

      return Enumerable.Empty<IParseResult<TValue>>();
    }

    /// <summary>
    /// Creates an empty enumerable sequence of <see cref="IParseResult{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <returns>An empty enumerable sequence of <see cref="IParseResult{TValue}"/>.</returns>
    public static IEnumerable<IParseResult<IEnumerable<TValue>>> ReturnFailureMany<TValue>()
    {
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<IEnumerable<TValue>>>>() != null);

      return Enumerable.Empty<IParseResult<IEnumerable<TValue>>>();
    }
  }
}