using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;

namespace Rxx.Parsers
{
  /// <summary>
  /// Provides <see cref="TraceSource"/> objects that are used by <see cref="Parser{TSource,TResult}"/>.
  /// </summary>
  public static class ParserTraceSources
  {
    #region Public Properties
    /// <summary>
    /// Gets a collection of all <see cref="TraceSource"/> objects that are used by <see cref="Parser{TSource,TResult}"/>.
    /// </summary>
    public static ICollection<TraceSource> All
    {
      [ContractVerification(false)]		// static checker cannot prove ForAll => source != null
      get
      {
        Contract.Ensures(Contract.Result<ICollection<TraceSource>>() != null);
        Contract.Ensures(Contract.Result<ICollection<TraceSource>>().Count > 0);
        Contract.Ensures(Contract.Result<ICollection<TraceSource>>().IsReadOnly);
        Contract.Ensures(Contract.ForAll(Contract.Result<ICollection<TraceSource>>(), source => source != null));

        return sources;
      }
    }

    /// <summary>
    /// A <see cref="TraceSource"/> used by <see cref="Parser{TSource,TResult}"/> to trace diagnostic information 
    /// when compiling grammars at runtime.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
      Justification = "Mutability is acceptable.")]
    public static readonly TraceSource Compilation = new TraceSource("Rxx.Parsers.Compilation", SourceLevels.Off);

    /// <summary>
    /// A <see cref="TraceSource"/> used by <see cref="Parser{TSource,TResult}"/> to trace diagnostic information 
    /// when executing parse operations.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
      Justification = "Mutability is acceptable.")]
    public static readonly TraceSource Execution = new TraceSource("Rxx.Parsers.Execution", SourceLevels.Error);

    /// <summary>
    /// A <see cref="TraceSource"/> used by <see cref="Parser{TSource,TResult}"/> to trace the input data as it's
    /// being evaluated by parsers.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
      Justification = "Mutability is acceptable.")]
    public static readonly TraceSource Input = new TraceSource("Rxx.Parsers.Input", SourceLevels.Off);
    #endregion

    #region Private / Protected
    private static readonly ICollection<TraceSource> sources = new[]
		{
			Compilation, 
			Execution, 
			Input
		}
    .ToList().AsReadOnly();

    private static long lastTracedParserGrammarId;
    private static long lastTracedParserQueryId;
    #endregion

    #region Methods
    /// <summary>
    /// Sets the trace levels for any of the <see cref="TraceSource"/> objects that are used by 
    /// <see cref="Parser{TSource,TResult}"/> and returns an <see cref="IDisposable"/> that, when disposed, 
    /// resets the trace levels of the modified sources to their values before <see cref="SetLevels"/> was 
    /// called.
    /// </summary>
    /// <param name="compilation">Sets the trace level for the <see cref="Compilation"/> source.</param>
    /// <param name="execution">Sets the trace level for the <see cref="Execution"/> source.</param>
    /// <param name="input">Sets the trace level for the <see cref="Input"/> source.</param>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, resets the trace levels of the modified 
    /// sources to their values before <see cref="SetLevels"/> was called.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "Convenience for all possible combinations.")]
    public static IDisposable SetLevels(
      SourceLevels? compilation = null,
      SourceLevels? execution = null,
      SourceLevels? input = null)
    {
      IDictionary<SourceSwitch, SourceLevels> oldLevels = sources.ToDictionary(
        source => source.Switch,
        source => source.Switch.Level);

      if (compilation.HasValue)
        Compilation.Switch.Level = compilation.Value;

      if (execution.HasValue)
        Execution.Switch.Level = execution.Value;

      if (input.HasValue)
        Input.Switch.Level = input.Value;

      return Disposable.Create(() =>
      {
        foreach (var pair in oldLevels)
          pair.Key.Level = pair.Value;
      });
    }

    /// <summary>
    /// Adds the specified <paramref name="listener"/> to <see cref="All"/> parser trace sources.
    /// </summary>
    /// <param name="listener">The <see cref="TraceListener"/> to be added.</param>
    public static void AddListener(TraceListener listener)
    {
      Contract.Requires(listener != null);

      sources.ForEach(source => source.Listeners.Add(listener));
    }

    /// <summary>
    /// Removes the specified <paramref name="listener"/> from <see cref="All"/> parser trace sources.
    /// </summary>
    /// <param name="listener">The <see cref="TraceListener"/> to be removed.</param>
    public static void RemoveListeners(TraceListener listener)
    {
      Contract.Requires(listener != null);

      sources.ForEach(source => source.Listeners.Remove(listener));
    }

    /// <summary>
    /// Clears the <see cref="TraceListener"/> collections of <see cref="All"/> parser trace sources.
    /// </summary>
    public static void ClearListeners()
    {
      sources.ForEach(source => source.Listeners.Clear());
    }

    internal static string FormatName(string name)
    {
      Contract.Ensures(Contract.Result<string>() == null || Contract.Result<string>().Length > 0);

      if (name == null)
      {
        return null;
      }
      else
      {
        return " [" + name + "]";
      }
    }

    [DebuggerStepThrough]
    internal static IDisposable TraceGrammarCompilation()
    {
      if (Compilation.Switch.ShouldTrace(TraceEventType.Information))
      {
        var id = Interlocked.Increment(ref lastTracedParserGrammarId);

        Compilation.TraceInformation("Parser {0} compiling grammar", id);

        var stopwatch = Stopwatch.StartNew();

        return Disposable.Create(() =>
        {
          stopwatch.Stop();

          Compilation.TraceInformation("Parser {0} compiled grammar in {1}", id, stopwatch.Elapsed);
        });
      }

      return null;
    }

    /// <summary>
    /// Traces the start of a query compilation and provides an <see cref="IDisposable"/> that traces the 
    /// end when it's disposed.
    /// </summary>
    /// <param name="name">Optional name of the parser to be traced.</param>
    /// <remarks>
    /// <alert type="warning">
    /// Compiling a query often depends upon the next value in the source sequence, 
    /// which means that if the cursor is at the head of the source sequence then calling 
    /// MoveNext may block the compilation until the next value is available.  This could
    /// lead to misleading profiling output; it may appears as though query compilation is 
    /// taking a long time for source sequences that are cold, when in fact the compilation
    /// time includes the time it takes to generate one or more values in the source sequence.
    /// </alert>
    /// </remarks>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, traces the end of a query compilation.</returns>
    [DebuggerStepThrough]
    internal static IDisposable TraceQueryCompilation(string name = null)
    {
      Contract.Requires(name == null || name.Length > 0);

      if (Compilation.Switch.ShouldTrace(TraceEventType.Verbose))
      {
        var id = Interlocked.Increment(ref lastTracedParserQueryId);

        name = FormatName(name);

        Compilation.TraceEvent(TraceEventType.Verbose, 0, "Parser {0}{1} compiling query", id, name);

        var stopwatch = Stopwatch.StartNew();

        return Disposable.Create(() =>
        {
          stopwatch.Stop();

          Compilation.TraceEvent(TraceEventType.Verbose, 0, "Parser {0}{1} compiled query in {2}", id, name, stopwatch.Elapsed);
        });
      }

      return null;
    }

    [DebuggerStepThrough]
    internal static ParserTraceExecutionContext TraceExecution(IParserCursorState cursor, string name = null)
    {
      Contract.Requires(cursor != null);
      Contract.Requires(name == null || name.Length > 0);

      return new ParserTraceExecutionContext(name, cursor);
    }

    [DebuggerStepThrough]
    [Conditional("TRACE")]
    internal static void TraceInput(object value)
    {
      if (Input.Switch.ShouldTrace(TraceEventType.Verbose))
      {
        var listeners = Input.Listeners;

        Contract.Assume(listeners != null);

        foreach (TraceListener listener in listeners)
        {
          Contract.Assume(listener != null);

          listener.Write(value);
        }
      }
    }
    #endregion
  }
}