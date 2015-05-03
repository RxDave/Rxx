using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rxx.Parsers.Linq;

namespace Rxx.UnitTests.Parsers.Interactive.UseCases
{
  [TestClass]
  public class StockAlerts : RxxTests
  {
    private static IEnumerable<int> Alerts(IEnumerable<int> ticks)
    {
      return EnumerableEx.Concat(
        ticks.AsStockTicks().Parse(parser =>
          from next in parser
          let ups = next.Where(tick => tick.Change > 0)
          let downs = next.Where(tick => tick.Change < 0)
          select (from up in ups.AtLeast(2)
                  from down in downs.NonGreedy()
                  where down.Change <= StockTicks.ReversalAlertDown
                  select up.Concat(new[] { down }))
                 .Or(
                  from down in downs.AtLeast(2)
                  from up in ups.NonGreedy()
                  where up.Change >= StockTicks.ReversalAlertUp
                  select down.Concat(new[] { up }))
                 .Ambiguous(untilCount: 1)))
        .Select(tick => tick.Value);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksNonAlertDown()
    {
      AssertEqual(Alerts(StockTicks.NonAlertDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksNonAlertUp()
    {
      AssertEqual(Alerts(StockTicks.NonAlertUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksLongNonAlertDown()
    {
      AssertEqual(Alerts(StockTicks.LongNonAlertDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksLongNonAlertUp()
    {
      AssertEqual(Alerts(StockTicks.LongNonAlertUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksAlertDown()
    {
      AssertEqual(Alerts(StockTicks.AlertDown), StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksAlertUp()
    {
      AssertEqual(Alerts(StockTicks.AlertUp), StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksLongAlertDown()
    {
      AssertEqual(Alerts(StockTicks.LongAlertDown), StockTicks.LongAlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksLongAlertUp()
    {
      AssertEqual(Alerts(StockTicks.LongAlertUp), StockTicks.LongAlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksAmbiguousAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.AmbiguousAlertsDownUp), StockTicks.AlertDown, StockTicks.Neutral, StockTicks.AmbiguousAlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksAmbiguousAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.AmbiguousAlertsUpDown), StockTicks.AlertUp, StockTicks.Neutral, StockTicks.AmbiguousAlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksAmbiguousLongAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.AmbiguousLongAlertsDownUp), StockTicks.LongAlertDown, StockTicks.Neutral, StockTicks.AmbiguousAlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksAmbiguousLongAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.AmbiguousLongAlertsUpDown), StockTicks.LongAlertUp, StockTicks.Neutral, StockTicks.AmbiguousAlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoNonAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.TwoNonAlertsDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoNonAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.TwoNonAlertsUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyNonAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.ManyNonAlertsDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyNonAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.ManyNonAlertsUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoLongNonAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.TwoLongNonAlertsDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoLongNonAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.TwoLongNonAlertsUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyLongNonAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.ManyLongNonAlertsDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyLongNonAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.ManyLongNonAlertsUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.TwoAlertsDown), StockTicks.AlertDown, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.TwoAlertsUp), StockTicks.AlertUp, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.ManyAlertsDown), StockTicks.AlertDown, StockTicks.AlertDown, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.ManyAlertsUp), StockTicks.AlertUp, StockTicks.AlertUp, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoLongAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.TwoLongAlertsDown), StockTicks.LongAlertDown, StockTicks.LongAlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoLongAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.TwoLongAlertsUp), StockTicks.LongAlertUp, StockTicks.LongAlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyLongAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.ManyLongAlertsDown), StockTicks.LongAlertDown, StockTicks.LongAlertDown, StockTicks.LongAlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyLongAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.ManyLongAlertsUp), StockTicks.LongAlertUp, StockTicks.LongAlertUp, StockTicks.LongAlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksNonAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.NonAlertsDownUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksNonAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.NonAlertsUpDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoNonAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.TwoNonAlertsDownUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoNonAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.TwoNonAlertsUpDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyNonAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.ManyNonAlertsDownUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyNonAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.ManyNonAlertsUpDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksNonAlertDownAlertUp()
    {
      AssertEqual(Alerts(StockTicks.NonAlertDownAlertUp), StockTicks.Neutral, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksNonAlertUpAlertDown()
    {
      AssertEqual(Alerts(StockTicks.NonAlertUpAlertDown), StockTicks.Neutral, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoNonAlertsDownAlertUp()
    {
      AssertEqual(Alerts(StockTicks.TwoNonAlertsDownAlertUp), StockTicks.Neutral, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoNonAlertsUpAlertDown()
    {
      AssertEqual(Alerts(StockTicks.TwoNonAlertsUpAlertDown), StockTicks.Neutral, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyNonAlertsDownAlertUp()
    {
      AssertEqual(Alerts(StockTicks.ManyNonAlertsDownAlertUp), StockTicks.Neutral, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyNonAlertsUpAlertDown()
    {
      AssertEqual(Alerts(StockTicks.ManyNonAlertsUpAlertDown), StockTicks.Neutral, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.AlertsDownUp), StockTicks.AlertDown, StockTicks.Neutral, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.AlertsUpDown), StockTicks.AlertUp, StockTicks.Neutral, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.TwoAlertsDownUp), StockTicks.AlertDown, StockTicks.AlertDown, StockTicks.Neutral, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.TwoAlertsUpDown), StockTicks.AlertUp, StockTicks.AlertUp, StockTicks.Neutral, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.ManyAlertsDownUp), StockTicks.AlertDown, StockTicks.AlertDown, StockTicks.AlertDown, StockTicks.Neutral, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.ManyAlertsUpDown), StockTicks.AlertUp, StockTicks.AlertUp, StockTicks.AlertUp, StockTicks.Neutral, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksAlertDownNonAlertUp()
    {
      AssertEqual(Alerts(StockTicks.AlertDownNonAlertUp), StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksAlertUpNonAlertDown()
    {
      AssertEqual(Alerts(StockTicks.AlertUpNonAlertDown), StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoAlertsDownNonAlertUp()
    {
      AssertEqual(Alerts(StockTicks.TwoAlertsDownNonAlertUp), StockTicks.AlertDown, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksTwoAlertsUpNonAlertDown()
    {
      AssertEqual(Alerts(StockTicks.TwoAlertsUpNonAlertDown), StockTicks.AlertUp, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyAlertsDownNonAlertUp()
    {
      AssertEqual(Alerts(StockTicks.ManyAlertsDownNonAlertUp), StockTicks.AlertDown, StockTicks.AlertDown, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksManyAlertsUpNonAlertDown()
    {
      AssertEqual(Alerts(StockTicks.ManyAlertsUpNonAlertDown), StockTicks.AlertUp, StockTicks.AlertUp, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksComplex()
    {
      AssertEqual(Alerts(
              StockTicks.Filler
      .Concat(StockTicks.NonAlertDown)
      .Concat(StockTicks.Filler)
      .Concat(StockTicks.NonAlertUp)
      .Concat(StockTicks.LongNonAlertDown)
      .Concat(StockTicks.LongNonAlertUp)
      .Concat(StockTicks.Filler)
      .Concat(StockTicks.AlertDown)
      .Concat(StockTicks.Filler)
      .Concat(StockTicks.AlertUp)
      .Concat(StockTicks.LongAlertDown)
      .Concat(StockTicks.LongAlertUp)
      .Concat(StockTicks.AmbiguousAlertsDownUp)
      .Concat(StockTicks.AmbiguousAlertsUpDown)
      .Concat(StockTicks.AmbiguousLongAlertsDownUp)
      .Concat(StockTicks.AmbiguousLongAlertsUpDown)
      .Concat(StockTicks.NonAlertsDownUp)
      .Concat(StockTicks.NonAlertsUpDown)
      .Concat(StockTicks.NonAlertDownAlertUp)
      .Concat(StockTicks.NonAlertUpAlertDown)
      .Concat(StockTicks.AlertsDownUp)
      .Concat(StockTicks.AlertsUpDown)
      .Concat(StockTicks.AlertDownNonAlertUp)
      .Concat(StockTicks.AlertUpNonAlertDown)
      .Concat(StockTicks.Filler)),
      expected:
              StockTicks.AlertDown
      .Concat(StockTicks.AlertUp)
      .Concat(StockTicks.Neutral).Concat(StockTicks.LongAlertDown)
      .Concat(StockTicks.Neutral).Concat(StockTicks.LongAlertUp)
      .Concat(StockTicks.Neutral).Concat(StockTicks.AlertDown).Concat(StockTicks.Neutral).Concat(StockTicks.AmbiguousAlertUp)
      .Concat(StockTicks.AlertUp).Concat(StockTicks.Neutral).Concat(StockTicks.AmbiguousAlertDown)
      .Concat(StockTicks.LongAlertDown).Concat(StockTicks.Neutral).Concat(StockTicks.AmbiguousAlertUp)
      .Concat(StockTicks.LongAlertUp).Concat(StockTicks.Neutral).Concat(StockTicks.AmbiguousAlertDown)
      .Concat(StockTicks.Neutral).Concat(StockTicks.AlertUp)
      .Concat(StockTicks.Neutral).Concat(StockTicks.AlertDown)
      .Concat(StockTicks.AlertDown)
      .Concat(StockTicks.Neutral).Concat(StockTicks.AlertUp)
      .Concat(StockTicks.AlertUp)
      .Concat(StockTicks.Neutral).Concat(StockTicks.AlertDown)
      .Concat(StockTicks.AlertDown)
      .Concat(StockTicks.AlertUp));
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxStockTicksAlertDownWithoutStackOverflow()
    {
      AssertEqual(Alerts(StockTicks.AlertDownStackOverflowTest), StockTicks.AlertDownStackOverflowTest);
    }
  }
}