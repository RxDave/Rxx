using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Rxx.Parsers
{
  internal sealed class ParserTraceExecutionContext : IDisposable
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private static long lastTracedExecutionId;

    private readonly long id;
    private readonly string name;
    private readonly IParserCursorState cursor;
    private readonly Stopwatch stopwatch;
    private int iteration;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="ParserTraceExecutionContext" /> class.
    /// </summary>
    /// <param name="name">Name of the parser to be traced.</param>
    /// <param name="cursor">The parser's cursor.</param>
    public ParserTraceExecutionContext(string name, IParserCursorState cursor)
    {
      Contract.Requires(name == null || name.Length > 0);
      Contract.Requires(cursor != null);

      if (ParserTraceSources.Execution.Switch.ShouldTrace(TraceEventType.Verbose)
        || ParserTraceSources.Execution.Switch.ShouldTrace(TraceEventType.Start))
      {
        this.id = Interlocked.Increment(ref lastTracedExecutionId);
        this.name = ParserTraceSources.FormatName(name);
        this.cursor = cursor;

        stopwatch = new Stopwatch();
      }
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(name == null || name.Length > 0);
      Contract.Invariant(iteration >= 0);
      Contract.Invariant(stopwatch == null || cursor != null);
    }

    [Conditional("TRACE")]
    [DebuggerStepThrough]
    public void TraceBeginIteration()
    {
      if (stopwatch != null)
      {
        TraceEndIteration();

        ParserTraceSources.Execution.TraceEvent(
          TraceEventType.Start,
          0,
          "Parser {0}{1} begin iteration {2} at index {3}",
          id,
          name,
          ++iteration,
          cursor.CurrentIndex);

        stopwatch.Restart();
      }
    }

    private void TraceEndIteration()
    {
      if (iteration > 0)
      {
        Contract.Assume(stopwatch != null);

        ParserTraceSources.Execution.TraceEvent(
          TraceEventType.Stop,
          0,
          "Parser {0}{1} end iteration {2} at index {3} in {4}",
          id,
          name,
          iteration,
          cursor.CurrentIndex,
          stopwatch.Elapsed);
      }
    }

    [Conditional("TRACE")]
    [DebuggerStepThrough]
    public void TraceResult<TResult>(IParseResult<TResult> result)
    {
      Contract.Requires(result != null);

      if (stopwatch != null)
      {
        int endIndex = cursor.CurrentIndex + result.Length - 1;

        string end =
          endIndex > cursor.CurrentIndex
          ? "-" + endIndex
          : endIndex < cursor.CurrentIndex
            ? " (Non-Greedy)"
            : string.Empty;

        ParserTraceSources.Execution.TraceEvent(
          TraceEventType.Verbose,
          0,
          "Parser {0}{1} iteration {2} in {3} at index {4}{5} = {6}",
          id,
          name,
          iteration,
          stopwatch.Elapsed,
          cursor.CurrentIndex,
          end,
          result.Value);
      }
    }

    [DebuggerStepThrough]
    public void Dispose()
    {
      if (stopwatch != null)
      {
        TraceEndIteration();
      }
    }
    #endregion
  }
}