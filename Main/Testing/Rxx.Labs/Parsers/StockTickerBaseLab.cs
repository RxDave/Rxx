using System;
using System.Diagnostics.Contracts;

namespace Rxx.Labs.Parsers
{
  public abstract class StockTickerBaseLab : BaseConsoleLab
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private static readonly object gate = new object();
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="StockTickerBaseLab" /> class for derived classes.
    /// </summary>
    protected StockTickerBaseLab()
    {
    }
    #endregion

    #region Methods
    protected void WriteTick(StockTick tick)
    {
      lock (gate)
      {
        var output = new System.Text.StringBuilder();

        Action<string> traceStrategy;

        if (tick.Change > 0)
        {
          output.Append('↑');
          output.Append(' ');

          traceStrategy = TraceStatus;
        }
        else if (tick.Change < 0)
        {
          output.Append('↓');
          output.Append(' ');

          traceStrategy = TraceWarning;
        }
        else
        {
          output.Append('-');
          output.Append(' ');

          traceStrategy = TraceLine;
        }

        output.Append(tick);

        traceStrategy(output.ToString());
      }
    }

    protected void WriteAlert(StockAlert alert)
    {
      Contract.Requires(alert != null);

      lock (gate)
      {
        var output = new System.Text.StringBuilder();

        output.Append('*');

        Action<string> traceStrategy;

        if (alert.Tick.Change > 0)
        {
          output.Append('↑');

          traceStrategy = TraceSuccess;
        }
        else
        {
          output.Append('↓');

          traceStrategy = TraceFailure;
        }

        output.Append(' ');
        output.Append(alert);

        traceStrategy(output.ToString());
      }
    }
    #endregion
  }
}