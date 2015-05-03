using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Rxx.Labs.Parsers;

namespace Rxx.UnitTests.Parsers
{
  internal static partial class StockTicks
  {
    public const int ReversalAlertDown = -20;
    public const int ReversalAlertUp = 20;

    /* The initial neutral tick is always 50, provided by the AsStockTicks extension.
     * All tick sequences must end with 50 so that they may be concatenated with the
     * expectation that the previous value is always 50.
     * 
     * Some tick sequences must end with 50 twice as a nuetralizer for when a sequence 
     * is concatenated onto it; e.g., when a single neutral value may be part of an 
     * alert due to ambiguity.
     * 
     * Alerts/non-alerts followed by an alert in the opposite direction will actually 
     * include the neutral tick separating them in the alert.  This must be expected 
     * when making assertions.
     */

    public static readonly IEnumerable<int> Neutral = ReadOnly(50);
    public static readonly IEnumerable<int> Neutralizer = Neutral.Repeat(2);

    public static readonly IEnumerable<int> Filler = Neutral.Concat(ReadOnly(91, 12)).Concat(Neutralizer);

    public static readonly IEnumerable<int> NonAlertDown = ReadOnly(61, 62).Concat(Neutral);
    public static readonly IEnumerable<int> NonAlertUp = ReadOnly(39, 38).Concat(Neutral);
    public static readonly IEnumerable<int> LongNonAlertDown = ReadOnly(61, 62, 63, 64, 65).Concat(Neutral);
    public static readonly IEnumerable<int> LongNonAlertUp = ReadOnly(39, 38, 37, 36, 35).Concat(Neutral);

    public static readonly IEnumerable<int> AlertDown = ReadOnly(91, 92).Concat(Neutral);
    public static readonly IEnumerable<int> AlertUp = ReadOnly(19, 18).Concat(Neutral);
    public static readonly IEnumerable<int> LongAlertDown = ReadOnly(91, 92, 93, 94, 95).Concat(Neutral);
    public static readonly IEnumerable<int> LongAlertUp = ReadOnly(19, 18, 17, 16, 15).Concat(Neutral);

    public static readonly IEnumerable<int> AmbiguousAlertUp = ReadOnly(49, 90);
    public static readonly IEnumerable<int> AmbiguousAlertDown = ReadOnly(51, 10);
    public static readonly IEnumerable<int> AmbiguousAlertsDownUp = AlertDown.Concat(AmbiguousAlertUp).Concat(Neutralizer);
    public static readonly IEnumerable<int> AmbiguousAlertsUpDown = AlertUp.Concat(AmbiguousAlertDown).Concat(Neutralizer);
    public static readonly IEnumerable<int> AmbiguousLongAlertsDownUp = LongAlertDown.Concat(AmbiguousAlertUp).Concat(Neutralizer);
    public static readonly IEnumerable<int> AmbiguousLongAlertsUpDown = LongAlertUp.Concat(AmbiguousAlertDown).Concat(Neutralizer);

    public static readonly IEnumerable<int> TwoNonAlertsDown = NonAlertDown.Repeat(2);
    public static readonly IEnumerable<int> TwoNonAlertsUp = NonAlertUp.Repeat(2);
    public static readonly IEnumerable<int> ManyNonAlertsDown = NonAlertDown.Repeat(3);
    public static readonly IEnumerable<int> ManyNonAlertsUp = NonAlertUp.Repeat(3);

    public static readonly IEnumerable<int> TwoLongNonAlertsDown = LongNonAlertDown.Repeat(2);
    public static readonly IEnumerable<int> TwoLongNonAlertsUp = LongNonAlertUp.Repeat(2);
    public static readonly IEnumerable<int> ManyLongNonAlertsDown = LongNonAlertDown.Repeat(3);
    public static readonly IEnumerable<int> ManyLongNonAlertsUp = LongNonAlertUp.Repeat(3);

    public static readonly IEnumerable<int> TwoAlertsDown = AlertDown.Repeat(2);
    public static readonly IEnumerable<int> TwoAlertsUp = AlertUp.Repeat(2);
    public static readonly IEnumerable<int> ManyAlertsDown = AlertDown.Repeat(3);
    public static readonly IEnumerable<int> ManyAlertsUp = AlertUp.Repeat(3);

    public static readonly IEnumerable<int> TwoLongAlertsDown = LongAlertDown.Repeat(2);
    public static readonly IEnumerable<int> TwoLongAlertsUp = LongAlertUp.Repeat(2);
    public static readonly IEnumerable<int> ManyLongAlertsDown = LongAlertDown.Repeat(3);
    public static readonly IEnumerable<int> ManyLongAlertsUp = LongAlertUp.Repeat(3);

    public static readonly IEnumerable<int> NonAlertsDownUp = NonAlertDown.Concat(NonAlertUp);
    public static readonly IEnumerable<int> NonAlertsUpDown = NonAlertUp.Concat(NonAlertDown);
    public static readonly IEnumerable<int> TwoNonAlertsDownUp = TwoNonAlertsDown.Concat(NonAlertUp);
    public static readonly IEnumerable<int> TwoNonAlertsUpDown = TwoNonAlertsUp.Concat(NonAlertDown);
    public static readonly IEnumerable<int> ManyNonAlertsDownUp = ManyNonAlertsDown.Concat(NonAlertUp);
    public static readonly IEnumerable<int> ManyNonAlertsUpDown = ManyNonAlertsUp.Concat(NonAlertDown);

    public static readonly IEnumerable<int> NonAlertDownAlertUp = NonAlertDown.Concat(AlertUp);
    public static readonly IEnumerable<int> NonAlertUpAlertDown = NonAlertUp.Concat(AlertDown);
    public static readonly IEnumerable<int> TwoNonAlertsDownAlertUp = TwoNonAlertsDown.Concat(AlertUp);
    public static readonly IEnumerable<int> TwoNonAlertsUpAlertDown = TwoNonAlertsUp.Concat(AlertDown);
    public static readonly IEnumerable<int> ManyNonAlertsDownAlertUp = ManyNonAlertsDown.Concat(AlertUp);
    public static readonly IEnumerable<int> ManyNonAlertsUpAlertDown = ManyNonAlertsUp.Concat(AlertDown);

    public static readonly IEnumerable<int> AlertsDownUp = AlertDown.Concat(AlertUp);
    public static readonly IEnumerable<int> AlertsUpDown = AlertUp.Concat(AlertDown);
    public static readonly IEnumerable<int> TwoAlertsDownUp = TwoAlertsDown.Concat(AlertUp);
    public static readonly IEnumerable<int> TwoAlertsUpDown = TwoAlertsUp.Concat(AlertDown);
    public static readonly IEnumerable<int> ManyAlertsDownUp = ManyAlertsDown.Concat(AlertUp);
    public static readonly IEnumerable<int> ManyAlertsUpDown = ManyAlertsUp.Concat(AlertDown);

    public static readonly IEnumerable<int> AlertDownNonAlertUp = AlertDown.Concat(NonAlertUp);
    public static readonly IEnumerable<int> AlertUpNonAlertDown = AlertUp.Concat(NonAlertDown);
    public static readonly IEnumerable<int> TwoAlertsDownNonAlertUp = TwoAlertsDown.Concat(NonAlertUp);
    public static readonly IEnumerable<int> TwoAlertsUpNonAlertDown = TwoAlertsUp.Concat(NonAlertDown);
    public static readonly IEnumerable<int> ManyAlertsDownNonAlertUp = ManyAlertsDown.Concat(NonAlertUp);
    public static readonly IEnumerable<int> ManyAlertsUpNonAlertDown = ManyAlertsUp.Concat(NonAlertDown);

    public static readonly IEnumerable<int> AlertDownStackOverflowTest = Enumerable.Range(Neutral.First() + 1, 10 * 1000).Concat(Neutral);

    public static IEnumerable<StockTick> AsStockTicks(this IEnumerable<int> source)
    {
      return source.Scan(new StockTick(Neutral.First(), 0), (acc, cur) => new StockTick(cur, cur - acc.Value));
    }

    public static IObservable<StockTick> AsStockTicksObservable(this IEnumerable<int> source)
    {
      return source.AsStockTicks().ToObservable();
    }

    internal static ReadOnlyCollection<T> ReadOnly<T>(params T[] objects)
    {
      return objects.ToList().AsReadOnly();
    }
  }
}