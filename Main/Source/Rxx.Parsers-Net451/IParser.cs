using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents a parser over an enumerable sequence.
  /// </summary>
  /// <typeparam name="TSource">The type of the source elements.</typeparam>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
  [ContractClass(typeof(IParserContract<,>))]
#if !SILVERLIGHT || WINDOWS_PHONE
  public interface IParser<TSource, out TResult>
#else
	public interface IParser<TSource, TResult>
#endif
  {
    /// <summary>
    /// Gets a parser with a grammar that matches the next element in the source sequence.
    /// </summary>
    /// <remarks>
    /// A parser's grammar is defined in terms of grammar rules, each of which is defined in terms of the <see cref="Next"/> parser
    /// or another rule.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Next",
      Justification = "Most appropriate term.")]
    IParser<TSource, TSource> Next { get; }

    /// <summary>
    /// Applies the parser's grammar, which is defined in terms of the <see cref="Next"/> parser, to generate matches.
    /// </summary>
    /// <param name="source">The enumerable sequence to parse.</param>
    /// <returns>An enumerable sequence of parse results that contain information about the matches.</returns>
    IEnumerable<IParseResult<TResult>> Parse(ICursor<TSource> source);
  }

  [ContractClassFor(typeof(IParser<,>))]
  internal abstract class IParserContract<TSource, TResult> : IParser<TSource, TResult>
  {
    public IParser<TSource, TSource> Next
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<TSource, TSource>>() != null);
        return null;
      }
    }

    public IEnumerable<IParseResult<TResult>> Parse(ICursor<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<TResult>>>() != null);
      return null;
    }
  }
}