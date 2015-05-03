using System;
using System.Diagnostics.Contracts;
using System.Reactive;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Represents a parser over an observable sequence.
  /// </summary>
  /// <typeparam name="TSource">The type of the source elements.</typeparam>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
  [ContractClass(typeof(IObservableParserContract<,>))]
  public interface IObservableParser<TSource, out TResult>
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
    IObservableParser<TSource, TSource> Next { get; }

    /// <summary>
    /// Applies the parser's grammar, which is defined in terms of the <see cref="Next"/> parser, to generate matches asynchronously.
    /// </summary>
    /// <param name="source">The observable sequence to parse.</param>
    /// <returns>An observable sequence of parse results that contain information about the matches.</returns>
    IObservable<IParseResult<TResult>> Parse(IObservableCursor<TSource> source);
  }

  [ContractClassFor(typeof(IObservableParser<,>))]
  internal abstract class IObservableParserContract<TSource, TResult> : IObservableParser<TSource, TResult>
  {
    public IObservableParser<TSource, TSource> Next
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<TSource, TSource>>() != null);
        return null;
      }
    }

    public IObservable<IParseResult<TResult>> Parse(IObservableCursor<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Ensures(Contract.Result<IObservable<IParseResult<TResult>>>() != null);
      return null;
    }
  }
}