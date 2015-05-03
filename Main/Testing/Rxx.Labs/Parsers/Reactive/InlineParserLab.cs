using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using Rxx.Labs.Properties;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.Labs.Parsers.Reactive
{
  [DisplayName("In-line")]
  [Description("Defines an in-line grammar that yields the sum of each pair of even "
             + "numbers that follows an odd number, filtering out sums less than 21.")]
  public sealed class InlineParserLab : BaseConsoleLab
  {
    private static IEnumerable<int> values =
      new[] { 1, 20, 40, 16, 3, 10, 12, 2, 5, 22, 9, 8, 4, 1, 18, 6 };

    protected override void Main()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);

      IObservable<int> xs = Observable.Generate(
        values.GetEnumerator(),
        e => e.MoveNext(),
        e => e,
        e => e.Current,
        e => TimeSpan.FromSeconds(.5))
        .Do(ConsoleOutput(Text.Generated));

      IObservable<int> parsed =
        from sum in
          xs.Parse(parser =>
            from next in parser
            let odd = next.Where(i => i % 2 != 0)
            let sum = from _ in odd
                      from firstEven in next.Not(odd)
                      from secondEven in next.Not(odd)
                      select firstEven + secondEven
            select sum)
        where sum > 20
        select sum;

      using (parsed.Subscribe(ConsoleOutput))
      {
        WaitForKey();
      }
    }
  }
}