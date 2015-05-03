using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Rxx.Labs.Parsers
{
  public sealed class StockAlert
  {
    #region Public Properties
    public StockTick Tick
    {
      get
      {
        return tick;
      }
    }
    #endregion

    #region Private / Protected
    private readonly StockTick tick;
    private readonly string prefixAsString;
    #endregion

    #region Constructors
    public StockAlert(IEnumerable<StockTick> prefix, StockTick tick)
    {
      Contract.Requires(prefix != null);

      this.tick = tick;
      this.prefixAsString = prefix.Aggregate(
        new StringBuilder(),
        (builder, t) => builder.Append(t.Value).Append(','),
        builder => (builder.Length == 0) ? string.Empty : builder.ToString(0, builder.Length - 1));
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(prefixAsString != null);
    }

    public override string ToString()
    {
      return string.Format(
        System.Globalization.CultureInfo.CurrentCulture,
        "{{{0}}} -> {1}",
        prefixAsString,
        tick);
    }
    #endregion
  }
}
