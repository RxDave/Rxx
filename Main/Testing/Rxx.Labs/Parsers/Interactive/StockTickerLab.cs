using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Rxx.Labs.Properties;
using Rxx.Parsers.Linq;

namespace Rxx.Labs.Parsers.Interactive
{
  [DisplayName("Stock Ticker")]
  [Description("Defines a grammar that analyzes a sequence of stock ticker values that you enter and "
             + "yields alerts for reversals beyond certain thresholds, following 2 or more consecutive "
             + "ticks in the same direction.")]
  public sealed class StockTickerLab : StockTickerBaseLab
  {
    private IEnumerable<int> UserInputTicks()
    {
      var random = new Random();

      do
      {
        string line = UserInput(Text.PromptFormat, Text.Tick);

        if (string.Equals(line, "RND", StringComparison.OrdinalIgnoreCase))
        {
          for (int i = 0; i < 5; i++)
          {
            yield return random.Next(1, 50);
          }
        }
        else
        {
          int value;
          bool isValid = int.TryParse(line, NumberStyles.Integer, CultureInfo.CurrentCulture, out value);

          if (isValid && value > -1)
          {
            yield return value;
          }
          else
            break;
        }
      }
      while (true);
    }

    protected override void Main()
    {
      TraceDescription(Instructions.ParserStockTickerLab);

      IEnumerable<StockTick> ticks = UserInputTicks()
        .Scan(StockTick.Empty, (acc, cur) => new StockTick(cur, cur - acc.Value));

      ticks = ticks.Skip(1).Do(WriteTick);

      IEnumerable<StockAlert> alerts = ticks.Parse(parser =>
        from next in parser
        let ups = next.Where(tick => tick.Change > 0)
        let downs = next.Where(tick => tick.Change < 0)
        let downAlert = from manyUps in ups.AtLeast(2)
                        from reversalDown in downs.NonGreedy()
                        where reversalDown.Change <= -11
                        select new StockAlert(manyUps, reversalDown)
        let upAlert = from manyDowns in downs.AtLeast(2)
                      from reversalUp in ups.NonGreedy()
                      where reversalUp.Change >= 21
                      select new StockAlert(manyDowns, reversalUp)
        select downAlert.Or(upAlert).Ambiguous(untilCount: 1));

      alerts.ForEach(WriteAlert);
    }
  }
}