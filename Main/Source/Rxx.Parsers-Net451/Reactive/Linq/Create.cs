using System;
using System.Diagnostics.Contracts;

namespace Rxx.Parsers.Reactive.Linq
{
  public static partial class ObservableParser
  {
    /// <summary>
    /// Creates a parser from the specified <paramref name="grammar"/> function.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="grammar">A function the accepts a parser that acts as a cursor and returns a parser that matches 
    /// in terms of the cursor.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="grammar"/>.</returns>
    public static IObservableParser<TSource, TResult> Create<TSource, TResult>(
      Func<IObservableParser<TSource, TSource>, IObservableParser<TSource, TResult>> grammar)
    {
      Contract.Requires(grammar != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return new ObservableParserStart<TSource, TResult>(grammar);
    }
  }
}