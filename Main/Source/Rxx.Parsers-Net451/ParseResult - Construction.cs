using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Rxx.Parsers
{
  public static partial class ParseResult
  {
    /// <summary>
    /// Creates a new <see cref="IParseResult{TValue}"/> instance from the specified 
    /// <paramref name="value"/> and <paramref name="length"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="value">The projection of elements of a parse operation.</param>
    /// <param name="length">The number of elements that were consumed in the sequence to generate 
    /// the specified <paramref name="value"/>.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> instance containing the specified 
    /// <paramref name="value"/> and <paramref name="length"/>.</returns>
    public static IParseResult<TValue> Create<TValue>(
      TValue value,
      int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParseResult<TValue>>() != null);

      return new ParseResult<TValue>(value, length);
    }

    /// <summary>
    /// Creates a new <see cref="ILookAheadParseResult{TValue}"/> instance from the specified 
    /// <paramref name="value"/> and <paramref name="length"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="value">The projection of elements of a parse operation.</param>
    /// <param name="length">The number of elements that were consumed in the sequence to generate 
    /// the specified <paramref name="value"/>.</param>
    /// <returns>A new <see cref="ILookAheadParseResult{TValue}"/> instance containing the specified 
    /// <paramref name="value"/> and <paramref name="length"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Static factory method.")]
    public static ILookAheadParseResult<TValue> CreateLookAhead<TValue>(
      TValue value,
      int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<ILookAheadParseResult<TValue>>() != null);

      return new LookAheadParseResult<TValue>(value, length);
    }

    /// <summary>
    /// Creates a singleton enumerable sequence containing an <see cref="IParseResult{TValue}"/>  
    /// with the specified <paramref name="value"/> and <paramref name="length"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="value">The projection of elements of a parse operation.</param>
    /// <param name="length">The number of elements that were consumed in the sequence to generate 
    /// the specified <paramref name="value"/>.</param>
    /// <returns>A singleton enumerable sequence containing an <see cref="IParseResult{TValue}"/>  
    /// with the specified <paramref name="value"/> and <paramref name="length"/>.</returns>
    public static IEnumerable<IParseResult<TValue>> Return<TValue>(
      TValue value,
      int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<TValue>>>() != null);

      return new List<IParseResult<TValue>>()
      {
        Create(value, length)
      }
      .AsReadOnly();
    }
  }
}