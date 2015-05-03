using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents a parser that begins a parse operation at the beginning of the source sequence.
  /// </summary>
  /// <typeparam name="TSource">The type of the source elements.</typeparam>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
  internal sealed class ParserStart<TSource, TResult> : ParserStartBase<TSource, TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private readonly Func<IParser<TSource, TSource>, IParser<TSource, TResult>> grammar;
    #endregion

    #region Constructors
    public ParserStart(Func<IParser<TSource, TSource>, IParser<TSource, TResult>> grammar)
    {
      Contract.Requires(grammar != null);

      this.grammar = grammar;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(grammar != null);
    }

#if !SILVERLIGHT && !PORT_45 && !PORT_40
    protected override IEnumerable<IParseResult<TResult>> Parse(ParserTraceExecutionContext traceContext)
#else
    protected override IEnumerable<IParseResult<TResult>> Parse()
#endif
    {
      var cursor = Cursor;

      IParser<TSource, TResult> compiledGrammar;

#if !SILVERLIGHT && !PORT_45 && !PORT_40
      using (ParserTraceSources.TraceGrammarCompilation())
      {
        compiledGrammar = grammar(cursor);
      }
#else
      compiledGrammar = grammar(cursor);
#endif

      do
      {
#if !SILVERLIGHT && !PORT_45 && !PORT_40
        traceContext.TraceBeginIteration();
#endif

        bool hasResult = false;

        foreach (var result in compiledGrammar.Parse(cursor))
        {
          var lookAhead = result as ILookAheadParseResult<TResult>;

          if (lookAhead != null)
          {
            lookAhead.OnCompleted(success: true);
            continue;
          }

          hasResult = true;

#if !SILVERLIGHT && !PORT_45 && !PORT_40
          traceContext.TraceResult(result);
#endif

          yield return result;

          if (!cursor.AtEndOfSequence)
          {
            cursor.Move(result.Length);
          }
        }

        if (!hasResult)
        {
          cursor.MoveToEnd();
        }
      }
      while (!cursor.AtEndOfSequence);
    }
    #endregion
  }
}