using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using Rxx.Parsers.Properties;

namespace Rxx.Parsers
{
  [ContractClass(typeof(ParserStartBaseContract<,>))]
  internal abstract class ParserStartBase<TSource, TResult> : IParser<TSource, TResult>
  {
    #region Public Properties
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "DeferredParserCursor must not be disposed since it's the return value.")]
    public IParser<TSource, TSource> Next
    {
      get
      {
        if (cursor == null)
        {
          return new DeferredParserCursor<TSource>(() =>
            {
              if (cursor == null)
              {
                throw new InvalidOperationException(Errors.ParserNextNotReady);
              }

              return cursor;
            });
        }
        else
        {
          return cursor;
        }
      }
    }
    #endregion

    #region Private / Protected
    protected virtual string TraceName
    {
      get
      {
        return null;
      }
    }

    protected ParserCursor<TSource> Cursor
    {
      get
      {
        Contract.Ensures(Contract.Result<ParserCursor<TSource>>() == cursor);

        return cursor;
      }
    }

    private ParserCursor<TSource> cursor;
    private int parsing;
    #endregion

    #region Constructors
    protected ParserStartBase()
    {
    }
    #endregion

    #region Methods
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "This is a factory method.")]
    protected virtual ParserCursor<TSource> CreateCursor(ICursor<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Ensures(Contract.Result<ParserCursor<TSource>>() != null);

      return new ParserCursor<TSource>(source);
    }

#if !SILVERLIGHT && !PORT_45 && !PORT_40
    protected abstract IEnumerable<IParseResult<TResult>> Parse(ParserTraceExecutionContext traceContext);
#else
    protected abstract IEnumerable<IParseResult<TResult>> Parse();
#endif

    public IEnumerable<IParseResult<TResult>> Parse(ICursor<TSource> source)
    {
      if (Interlocked.Exchange(ref parsing, 1) == 1)
      {
        throw new InvalidOperationException(Errors.ParseCalledWhileParsing);
      }

      if (cursor != null)
      {
        cursor.Dispose();
      }

      cursor = CreateCursor(source);

      return ParseIterator();
    }

    private IEnumerable<IParseResult<TResult>> ParseIterator()
    {
      Contract.Requires(Cursor != null);
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<TResult>>>() != null);

      try
      {
#if !SILVERLIGHT && !PORT_45 && !PORT_40
        using (var traceContext = ParserTraceSources.TraceExecution(Cursor, TraceName))
        {
          foreach (var result in Parse(traceContext))
          {
            yield return result;
          }
        }
#else
        foreach (var result in Parse())
        {
          yield return result;
        }
#endif
      }
      finally
      {
        Interlocked.Exchange(ref parsing, 0);
      }

      cursor.Reset();
    }

    public override string ToString()
    {
      return (TraceName == null ? string.Empty : "(" + TraceName + ") ")
           + (cursor == null ? "?" : cursor.ToString());
    }
    #endregion
  }

  [ContractClassFor(typeof(ParserStartBase<,>))]
  internal abstract class ParserStartBaseContract<TSource, TResult> : ParserStartBase<TSource, TResult>
  {
#if !SILVERLIGHT && !PORT_45 && !PORT_40
    protected override IEnumerable<IParseResult<TResult>> Parse(ParserTraceExecutionContext traceContext)
    {
#else
    protected override IEnumerable<IParseResult<TResult>> Parse()
    {
#endif
      Contract.Requires(Cursor != null);
#if !SILVERLIGHT && !PORT_45 && !PORT_40
      Contract.Requires(traceContext != null);
#endif
      Contract.Ensures(Contract.Result<IEnumerable<IParseResult<TResult>>>() != null);
      return null;
    }
  }
}