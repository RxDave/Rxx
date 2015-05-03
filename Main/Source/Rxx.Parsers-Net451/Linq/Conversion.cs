using System.Diagnostics.Contracts;

namespace Rxx.Parsers.Linq
{
  public static partial class Parser
  {
    /// <summary>
    /// Converts matches from the specified <paramref name="parser"/> into strings.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser from which matches will be converted into strings.</param>
    /// <returns>A parser that yields strings for the matches from the specified <paramref name="parser"/>.</returns>
    public static IParser<TSource, string> AsString<TSource, TResult>(
      this IParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, string>>() != null);

      return parser.Select(value => (value == null) ? null : value.ToString());
    }

#if SILVERLIGHT && !WINDOWS_PHONE
		/// <summary>
		/// Casts results from the specified <paramref name="parser"/> into the specified type.
		/// </summary>
		/// <remarks>
		/// <see cref="Cast{TSource,TResult,TCastResult>"/> is required for Silverlight because it doesn't support covariance on 
		/// <see cref="System.Collections.Generic.IEnumerable{TSource}"/>, which is required for <see cref="IParser{TSource,TResult}"/> to support covariance on 
		/// <strong>TResult</strong>.  A consequence is that parsers must sometimes be cast down to their base types before
		/// they can be combined using various parser operators.
		/// </remarks>
		/// <typeparam name="TSource">The type of the source elements.</typeparam>
		/// <typeparam name="TResult">The original type of the elements that are generated from parsing the source elements.</typeparam>
		/// <typeparam name="TCastResult">The type to which the elements that are generated from parsing the source elements are cast.</typeparam>
		/// <param name="parser">The parser from which results will be cast into the specified type.</param>
		/// <returns>A parser that casts results from the specified <paramref name="parser"/>.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly",
			Justification = "Double cast.")]
		public static IParser<TSource, TCastResult> Cast<TSource, TResult, TCastResult>(
			this IParser<TSource, TResult> parser)
		{
			Contract.Requires(parser != null);
			Contract.Ensures(Contract.Result<IParser<TSource, TCastResult>>() != null);

			return parser.Select(value => (TCastResult)(object)value);
		}
#endif
  }
}