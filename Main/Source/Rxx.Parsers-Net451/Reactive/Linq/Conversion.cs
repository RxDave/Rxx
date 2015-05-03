using System.Diagnostics.Contracts;

namespace Rxx.Parsers.Reactive.Linq
{
  public static partial class ObservableParser
  {
    /// <summary>
    /// Converts matches from the specified <paramref name="parser"/> into strings.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser from which matches will be converted into strings.</param>
    /// <returns>A parser that yields strings for the matches from the specified <paramref name="parser"/>.</returns>
    public static IObservableParser<TSource, string> AsString<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, string>>() != null);

      return parser.Select(value => (value == null) ? null : value.ToString());
    }
  }
}