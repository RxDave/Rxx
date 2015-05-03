using System;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Represents a parser over an observable sequence.
  /// </summary>
  /// <typeparam name="TSource">The type of the source elements.</typeparam>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
  [ContractClass(typeof(ObservableParserContract<,>))]
  public abstract class ObservableParser<TSource, TResult> : IObservableParser<TSource, TResult>
  {
    #region Public Properties
    /// <summary>
    /// Gets a parser with a grammar that matches the next element in the source sequence.
    /// </summary>
    /// <remarks>
    /// A parser's grammar is defined in terms of grammar rules, each of which is defined in terms of the <see cref="Next"/> parser
    /// or another rule.
    /// </remarks>
    public IObservableParser<TSource, TSource> Next
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
    protected abstract IObservableParser<TSource, TResult> Start
    {
      get;
    }

    private readonly ObservableParserStart<TSource, TResult> parser;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="Parser{TSource, TResult}" /> class for derived classes.
    /// </summary>
    protected ObservableParser()
    {
      parser = new ObservableParserStart<TSource, TResult>(
        _ => Start);		// Start must be delayed to support in-line parsers
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(parser != null);
    }

    /// <summary>
    /// Applies the parser's grammar, which is defined by <see cref="Start"/>, to generate matches asynchronously.
    /// </summary>
    /// <param name="source">The observable sequence to parse.</param>
    /// <returns>An observable sequence of parse results.</returns>
    public IObservable<TResult> Parse(IObservableCursor<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      return parser.Parse(source).Select(result => result.Value);
    }

    IObservable<IParseResult<TResult>> IObservableParser<TSource, TResult>.Parse(IObservableCursor<TSource> source)
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

  [ContractClassFor(typeof(ObservableParser<,>))]
  internal abstract class ObservableParserContract<TSource, TResult> : ObservableParser<TSource, TResult>
  {
    protected override IObservableParser<TSource, TResult> Start
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);
        return null;
      }
    }
  }
}