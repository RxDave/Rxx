using System;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents a possible result of a parse operation over a sequence, depending upon whether a subsequent parser matches.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This type of result is used by lazy quantifiers.  It does not represent <em>look-ahead</em> as in the Regular
  /// Expression sense.  That kind of look-ahead can be expressed using a normal <c>SelectMany</c> query by simply applying 
  /// the <c>NonGreedy</c> operator to the first parser.  That will effectively allow the first parser to look-ahead for a 
  /// match from the current position of the cursor, without consuming the results.  The second parser is then executed from 
  /// the same starting position as the first parser, but only when the first parser matches.
  /// </para>
  /// <para>
  /// Instead, this type of result allows quantifiers to end lazily by asking <c>SelectMany</c> if the second parser matches 
  /// after each successful match from the quantified parser, starting at the end of the current match.  Essentially, it's like 
  /// adding <c>NonGreedy</c> to each individual match instead of adding it to the entire quantified parser.  Behaviorally, it's 
  /// a special kind of result that indicates to <c>SelectMany</c> to look ahead at the second parser and notify the result 
  /// whether it's successful or not, without yielding.
  /// </para>
  /// <alert type="implement">
  /// The <see cref="OnCompleted"/> implementation must only accept a single call, specifying <see langword="true"/> to 
  /// indicate that the look-ahead succeeded and that the parse result can be captured by the parser that generated it; 
  /// otherwise, <see langword="false"/> should be specified.  The specified value must be buffered and replayed to all 
  /// observers.
  /// </alert>
  /// <alert type="warning">
  /// When developing custom parser operators, there is no need to use the <see cref="IObservable{T}"/>
  /// implementation of <see cref="ILookAheadParseResult{TValue}"/>.  In other words, do not call 
  /// <strong>Subscribe</strong> or <strong>OnCompleted</strong>.  Look-aheads are handled by the infrastructure, so simply 
  /// treat this type of result like a normal <see cref="IParseResult{TValue}"/>.
  /// </alert>
  /// </remarks>
  /// <typeparam name="TValue">The type of the parse result's value.</typeparam>
  public interface ILookAheadParseResult<out TValue> : IParseResult<TValue>, IObservable<bool>, IDisposable
  {
    /// <summary>
    /// Completes the look-ahead operation and notifies subscribers.
    /// </summary>
    /// <param name="success"><see langword="True"/> to indicate that the look-ahead succeeded and that the parse result can 
    /// be captured by the parser that generated it; otherwise, <see langword="false"/>.</param>
    /// <exception cref="InvalidOperationException"><see cref="OnCompleted"/> has already been called on this parse result.</exception>
    void OnCompleted(bool success);
  }
}