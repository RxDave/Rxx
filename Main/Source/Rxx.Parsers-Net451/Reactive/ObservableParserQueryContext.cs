using System;
using System.Diagnostics.Contracts;
using System.Reactive;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Represents a parser context over an observable sequence to support in-line grammars.
  /// </summary>
  /// <typeparam name="TSource">The type of the source elements.</typeparam>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
  /// <typeparam name="TQueryValue">The type of the current value in the query context.</typeparam>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
    Justification = "Designed for functional programming.")]
  public sealed class ObservableParserQueryContext<TSource, TResult, TQueryValue> : IObservableParser<TSource, TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    internal IObservableParser<TSource, TResult> Parser
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

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

    private readonly IObservableParser<TSource, TResult> parser;
    private readonly TQueryValue queryValue;
    #endregion

    #region Constructors
    internal ObservableParserQueryContext(IObservableParser<TSource, TResult> parser, TQueryValue value)
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
    IObservableParser<TSource, TSource> IObservableParser<TSource, TResult>.Next
    {
      get
      {
        return parser.Next;
      }
    }

    IObservable<IParseResult<TResult>> IObservableParser<TSource, TResult>.Parse(IObservableCursor<TSource> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion
  }
}