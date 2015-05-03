using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents a parser context over an enumerable sequence to support in-line grammars.
  /// </summary>
  /// <typeparam name="TSource">The type of the source elements.</typeparam>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
  /// <typeparam name="TQueryValue">The type of the current value in the query context.</typeparam>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
    Justification = "Designed for functional programming.")]
  public sealed class ParserQueryContext<TSource, TResult, TQueryValue> : IParser<TSource, TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    internal IParser<TSource, TResult> Parser
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

        return parser;
      }
    }

    internal TQueryValue Value
    {
      get
      {
        return queryValue;
      }
    }

    private readonly IParser<TSource, TResult> parser;
    private readonly TQueryValue queryValue;
    #endregion

    #region Constructors
    internal ParserQueryContext(IParser<TSource, TResult> parser, TQueryValue value)
    {
      Contract.Requires(parser != null);

      this.parser = parser;
      this.queryValue = value;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(parser != null);
    }
    #endregion

    #region IParser<TSource,TParseResult> Members
    IParser<TSource, TSource> IParser<TSource, TResult>.Next
    {
      get
      {
        return parser.Next;
      }
    }

    IEnumerable<IParseResult<TResult>> IParser<TSource, TResult>.Parse(ICursor<TSource> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion
  }
}
