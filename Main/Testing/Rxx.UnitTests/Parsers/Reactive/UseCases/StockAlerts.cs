using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.UnitTests.Parsers.Reactive.UseCases
{
  [TestClass]
  public class StockAlerts : RxxTests
  {
    private static IObservable<int> Alerts(IEnumerable<int> ticks)
    {
      return Observable.Concat(
        ticks.AsStockTicksObservable().Parse(parser =>
          from next in parser
          let ups = next.Where(tick => tick.Change > 0)
          let downs = next.Where(tick => tick.Change < 0)
          select (from up in ups.AtLeast(2)
                  from down in downs.NonGreedy()
                  where down.Change <= StockTicks.ReversalAlertDown
                  select up.Concat(Observable.Return(down)))
                 .Or(
                  from down in downs.AtLeast(2)
                  from up in ups.NonGreedy()
                  where up.Change >= StockTicks.ReversalAlertUp
                  select down.Concat(Observable.Return(up)))
                 .Ambiguous(untilCount: 1)))
        .Select(tick => tick.Value);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksNonAlertDown()
    {
      AssertEqual(Alerts(StockTicks.NonAlertDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksNonAlertUp()
    {
      AssertEqual(Alerts(StockTicks.NonAlertUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksLongNonAlertDown()
    {
      AssertEqual(Alerts(StockTicks.LongNonAlertDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksLongNonAlertUp()
    {
      AssertEqual(Alerts(StockTicks.LongNonAlertUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksAlertDown()
    {
      AssertEqual(Alerts(StockTicks.AlertDown), StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksAlertUp()
    {
      AssertEqual(Alerts(StockTicks.AlertUp), StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksLongAlertDown()
    {
      AssertEqual(Alerts(StockTicks.LongAlertDown), StockTicks.LongAlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksLongAlertUp()
    {
      AssertEqual(Alerts(StockTicks.LongAlertUp), StockTicks.LongAlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksAmbiguousAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.AmbiguousAlertsDownUp), StockTicks.AlertDown, StockTicks.Neutral, StockTicks.AmbiguousAlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksAmbiguousAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.AmbiguousAlertsUpDown), StockTicks.AlertUp, StockTicks.Neutral, StockTicks.AmbiguousAlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksAmbiguousLongAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.AmbiguousLongAlertsDownUp), StockTicks.LongAlertDown, StockTicks.Neutral, StockTicks.AmbiguousAlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksAmbiguousLongAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.AmbiguousLongAlertsUpDown), StockTicks.LongAlertUp, StockTicks.Neutral, StockTicks.AmbiguousAlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoNonAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.TwoNonAlertsDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoNonAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.TwoNonAlertsUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyNonAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.ManyNonAlertsDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyNonAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.ManyNonAlertsUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoLongNonAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.TwoLongNonAlertsDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoLongNonAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.TwoLongNonAlertsUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyLongNonAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.ManyLongNonAlertsDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyLongNonAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.ManyLongNonAlertsUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.TwoAlertsDown), StockTicks.AlertDown, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.TwoAlertsUp), StockTicks.AlertUp, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.ManyAlertsDown), StockTicks.AlertDown, StockTicks.AlertDown, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.ManyAlertsUp), StockTicks.AlertUp, StockTicks.AlertUp, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoLongAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.TwoLongAlertsDown), StockTicks.LongAlertDown, StockTicks.LongAlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoLongAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.TwoLongAlertsUp), StockTicks.LongAlertUp, StockTicks.LongAlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyLongAlertsDown()
    {
      AssertEqual(Alerts(StockTicks.ManyLongAlertsDown), StockTicks.LongAlertDown, StockTicks.LongAlertDown, StockTicks.LongAlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyLongAlertsUp()
    {
      AssertEqual(Alerts(StockTicks.ManyLongAlertsUp), StockTicks.LongAlertUp, StockTicks.LongAlertUp, StockTicks.LongAlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksNonAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.NonAlertsDownUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksNonAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.NonAlertsUpDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoNonAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.TwoNonAlertsDownUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoNonAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.TwoNonAlertsUpDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyNonAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.ManyNonAlertsDownUp), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyNonAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.ManyNonAlertsUpDown), Enumerable.Empty<int>());
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksNonAlertDownAlertUp()
    {
      AssertEqual(Alerts(StockTicks.NonAlertDownAlertUp), StockTicks.Neutral, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksNonAlertUpAlertDown()
    {
      AssertEqual(Alerts(StockTicks.NonAlertUpAlertDown), StockTicks.Neutral, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoNonAlertsDownAlertUp()
    {
      AssertEqual(Alerts(StockTicks.TwoNonAlertsDownAlertUp), StockTicks.Neutral, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoNonAlertsUpAlertDown()
    {
      AssertEqual(Alerts(StockTicks.TwoNonAlertsUpAlertDown), StockTicks.Neutral, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyNonAlertsDownAlertUp()
    {
      AssertEqual(Alerts(StockTicks.ManyNonAlertsDownAlertUp), StockTicks.Neutral, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyNonAlertsUpAlertDown()
    {
      AssertEqual(Alerts(StockTicks.ManyNonAlertsUpAlertDown), StockTicks.Neutral, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.AlertsDownUp), StockTicks.AlertDown, StockTicks.Neutral, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.AlertsUpDown), StockTicks.AlertUp, StockTicks.Neutral, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.TwoAlertsDownUp), StockTicks.AlertDown, StockTicks.AlertDown, StockTicks.Neutral, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.TwoAlertsUpDown), StockTicks.AlertUp, StockTicks.AlertUp, StockTicks.Neutral, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyAlertsDownUp()
    {
      AssertEqual(Alerts(StockTicks.ManyAlertsDownUp), StockTicks.AlertDown, StockTicks.AlertDown, StockTicks.AlertDown, StockTicks.Neutral, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyAlertsUpDown()
    {
      AssertEqual(Alerts(StockTicks.ManyAlertsUpDown), StockTicks.AlertUp, StockTicks.AlertUp, StockTicks.AlertUp, StockTicks.Neutral, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksAlertDownNonAlertUp()
    {
      AssertEqual(Alerts(StockTicks.AlertDownNonAlertUp), StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksAlertUpNonAlertDown()
    {
      AssertEqual(Alerts(StockTicks.AlertUpNonAlertDown), StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoAlertsDownNonAlertUp()
    {
      AssertEqual(Alerts(StockTicks.TwoAlertsDownNonAlertUp), StockTicks.AlertDown, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksTwoAlertsUpNonAlertDown()
    {
      AssertEqual(Alerts(StockTicks.TwoAlertsUpNonAlertDown), StockTicks.AlertUp, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyAlertsDownNonAlertUp()
    {
      AssertEqual(Alerts(StockTicks.ManyAlertsDownNonAlertUp), StockTicks.AlertDown, StockTicks.AlertDown, StockTicks.AlertDown);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksManyAlertsUpNonAlertDown()
    {
      AssertEqual(Alerts(StockTicks.ManyAlertsUpNonAlertDown), StockTicks.AlertUp, StockTicks.AlertUp, StockTicks.AlertUp);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksComplex()
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

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxStockTicksAlertDownWithoutStackOverflow()
    {
      AssertEqual(Alerts(StockTicks.AlertDownStackOverflowTest), StockTicks.AlertDownStackOverflowTest);
    }
  }
}