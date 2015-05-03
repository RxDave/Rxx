using System;
using System.Diagnostics.Contracts;

namespace Rxx.Parsers.Linq
{
  public static partial class Parser
  {
    /// <summary>
    /// Creates a parser from the specified <paramref name="grammar"/> function.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="grammar">A function the accepts a parser that acts as a cursor and returns a parser that matches 
    /// in terms of the cursor.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="grammar"/>.</returns>
    public static IParser<TSource, TResult> Create<TSource, TResult>(
      Func<IParser<TSource, TSource>, IParser<TSource, TResult>> grammar)
    {
      Contract.Requires(grammar != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return new ParserStart<TSource, TResult>(grammar);
    }
  }
}