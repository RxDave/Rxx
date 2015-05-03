using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace Rxx.Parsers.Reactive
{
  public static partial class ObservableParseResult
  {
    /// <summary>
    /// Creates a new <see cref="IParseResult{TValue}"/> with the specified 
    /// <paramref name="length"/> and an empty sequence for the value.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="length">The number of elements that were consumed in the sequence.</param>
    /// <returns>A new <see cref="IParseResult{TValue}"/> with the specified 
    /// <paramref name="length"/> and an empty sequence for the value.</returns>
    public static IParseResult<IObservable<TValue>> SuccessMany<TValue>(int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParseResult<IObservable<TValue>>>() != null);

      return new ParseResult<IObservable<TValue>>(Observable.Empty<TValue>(), length);
    }

    /// <summary>
    /// Creates a singleton observable sequence containing an <see cref="IParseResult{TValue}"/> with 
    /// the specified <paramref name="length"/> and the default value for the specified 
    /// <typeparamref name="TValue"/> type.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="length">The number of elements that were consumed in the sequence.</param>
    /// <returns>A singleton observable sequence containing an <see cref="IParseResult{TValue}"/> with 
    /// the specified <paramref name="length"/> and a default value.</returns>
    public static IObservable<IParseResult<TValue>> ReturnSuccess<TValue>(int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IObservable<IParseResult<TValue>>>() != null);

      return Observable.Return(ParseResult.Success<TValue>(length));
    }

    /// <summary>
    /// Creates a singleton observable sequence containing an <see cref="IParseResult{TValue}"/> 
    /// with the specified <paramref name="length"/> and an empty sequence for the value.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <param name="length">The number of elements that were consumed in the sequence.</param>
    /// <returns>A singleton observable sequence containing an <see cref="IParseResult{TValue}"/> 
    /// with the specified <paramref name="length"/> and an empty sequence for the value.</returns>
    public static IObservable<IParseResult<IObservable<TValue>>> ReturnSuccessMany<TValue>(int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IObservable<IParseResult<IObservable<TValue>>>>() != null);

      return Observable.Return(SuccessMany<TValue>(length));
    }

    /// <summary>
    /// Creates an empty observable sequence of <see cref="IParseResult{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <returns>An empty observable sequence of <see cref="IParseResult{TValue}"/>.</returns>
    public static IObservable<IParseResult<TValue>> ReturnFailure<TValue>()
    {
      Contract.Ensures(Contract.Result<IObservable<IParseResult<TValue>>>() != null);

      return Observable.Empty<IParseResult<TValue>>();
    }

    /// <summary>
    /// Creates an empty observable sequence of <see cref="IParseResult{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
    /// <returns>An empty observable sequence of <see cref="IParseResult{TValue}"/>.</returns>
    public static IObservable<IParseResult<IObservable<TValue>>> ReturnFailureMany<TValue>()
    {
      Contract.Ensures(Contract.Result<IObservable<IParseResult<IObservable<TValue>>>>() != null);

      return Observable.Empty<IParseResult<IObservable<TValue>>>();
    }
  }
}
