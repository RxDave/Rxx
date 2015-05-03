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
  [ContractClass(typeof(ParserContract<,>))]
  public abstract class Parser<TSource, TResult> : IParser<TSource, TResult>
  {
    #region Public Properties
    /// <summary>
    /// Gets a parser with a grammar that matches the next element in the source sequence.
    /// </summary>
    /// <remarks>
    /// A parser's grammar is defined in terms of grammar rules, each of which is defined in terms of the <see cref="Next"/> parser
    /// or another rule.
    /// </remarks>
    public IParser<TSource, TSource> Next
    {
      get
      {
        return parser.Next;
      }
    }
    #endregion

    #region Private / Protected
    /// <summary>
    /// Gets the parser's grammar as a parser that is defined in terms of the <see cref="Next"/> parser.
    /// </summary>
    protected abstract IParser<TSource, TResult> Start
    {
      get;
    }

    private readonly ParserStart<TSource, TResult> parser;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="Parser{TSource, TResult}" /> class for derived classes.
    /// </summary>
    protected Parser()
    {
      parser = new ParserStart<TSource, TResult>(
        _ => Start);	// Start must be delayed to support in-line parsers
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(parser != null);
    }

    /// <summary>
    /// Applies the parser's grammar, which is defined by <see cref="Start"/>, to generate matches.
    /// </summary>
    /// <param name="source">The enumerable sequence to parse.</param>
    /// <returns>An enumerable sequence of parse results.</returns>
    public IEnumerable<TResult> Parse(ICursor<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      return parser.Parse(source).Select(result => result.Value);
    }

    IEnumerable<IParseResult<TResult>> IParser<TSource, TResult>.Parse(ICursor<TSource> source)
    {
      return parser.Parse(source);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents the current parser.
    /// </summary>
    /// <returns>A <see cref="string"/> that represents the current parser.</returns>
    public override string ToString()
    {
      return "(" + GetType().Name + ") " + parser.ToString();
    }
    #endregion
  }

  [ContractClassFor(typeof(Parser<,>))]
  internal abstract class ParserContract<TSource, TResult> : Parser<TSource, TResult>
  {
    protected override IParser<TSource, TResult> Start
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);
        return null;
      }
    }
  }
}