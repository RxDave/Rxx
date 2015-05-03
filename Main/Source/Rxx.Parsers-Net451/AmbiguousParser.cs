using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents an ambiguous parser that begins a parse operation at the current position of the source sequence.
  /// </summary>
  /// <typeparam name="TSource">The type of the source elements.</typeparam>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
  internal sealed class AmbiguousParser<TSource, TResult> : IParser<TSource, IEnumerable<TResult>>
  {
    #region Public Properties
    public IParser<TSource, TSource> Next
    {
      get
      {
        return parser.Next;
      }
    }
    #endregion

    #region Private / Protected
    private const int unlimitedCount = -1;

    private readonly IParser<TSource, TResult> parser;
    private readonly Func<ICursor<TSource>, bool> untilPredicate;
    private readonly int untilCount;
    #endregion

    #region Constructors
    public AmbiguousParser(IParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);

      this.parser = parser;
      this.untilCount = unlimitedCount;
    }

    public AmbiguousParser(IParser<TSource, TResult> parser, int untilCount)
    {
      Contract.Requires(parser != null);
      Contract.Requires(untilCount >= 0);

      this.parser = parser;
      this.untilCount = untilCount;
    }

    public AmbiguousParser(IParser<TSource, TResult> parser, Func<ICursor<TSource>, bool> untilPredicate)
    {
      Contract.Requires(parser != null);
      Contract.Requires(untilPredicate != null);

      this.parser = parser;
      this.untilCount = unlimitedCount;
      this.untilPredicate = untilPredicate;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(parser != null);
      Contract.Invariant(untilCount >= unlimitedCount);
    }

    public IEnumerable<IParseResult<IEnumerable<TResult>>> Parse(ICursor<TSource> source)
    {
      int matchCount = 0;
      int remainingLength = 0;

      while (!source.AtEndOfSequence
        && (untilCount == unlimitedCount || matchCount < untilCount)
        && (untilPredicate == null || !untilPredicate(source)))
      {
        bool hasResult = false;
        int length = 0;

        var branch = source.Branch();

        var values = parser.Parse(branch)
          .Finally(branch.Dispose)
          .Select(
            result =>
            {
              if (!hasResult)
              {
                matchCount++;
                hasResult = true;
              }

              length = Math.Max(length, result.Length);

              return result.Value;
            });

        yield return ParseResult.Create(values, length: 1);

        /* Assume that the results have been iterated by the caller at this point;
         * otherwise, the cursor's state cannot be determined when the results 
         * are eventually iterated, causing unpredictable results.
         *
         * We must respect the greediness of the results unless the length is zero since the
         * cursor would have already moved to the following element.  It is acceptable to ignore
         * zero-length results because marking an entirely non-greedy parser as ambiguous would
         * otherwise cause the parser to continously parse the first element indefinitely.
         */
        if (length > 0)
        {
          remainingLength = length - 1;
        }
        else if (remainingLength > 0)
        {
          remainingLength--;
        }
      }

      if (remainingLength > 0)
      {
        yield return ParseResult.SuccessMany<TResult>(remainingLength);
      }
    }
    #endregion
  }
}