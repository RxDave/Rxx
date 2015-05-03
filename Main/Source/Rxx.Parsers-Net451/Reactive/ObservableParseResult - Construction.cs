using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace Rxx.Parsers.Reactive
{
  public static partial class ObservableParseResult
  {
    /// <summary>
    /// Creates a singleton observable sequence containing an <see cref="IParseResult{TValue}"/>  
    /// with the specified <paramref name="value"/> and <paramref name="length"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="value">The projection of elements of a parse operation.</param>
    /// <param name="length">The number of elements that were consumed in the sequence to generate 
    /// the specified <paramref name="value"/>.</param>
    /// <returns>A singleton observable sequence containing an <see cref="IParseResult{TValue}"/>  
    /// with the specified <paramref name="value"/> and <paramref name="length"/>.</returns>
    public static IObservable<IParseResult<TValue>> Return<TValue>(
      TValue value,
      int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IObservable<IParseResult<TValue>>>() != null);

      return Observable.Return(ParseResult.Create(value, length));
    }
  }
}
