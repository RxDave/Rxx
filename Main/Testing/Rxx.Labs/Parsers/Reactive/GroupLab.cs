using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.Labs.Parsers.Reactive
{
  [DisplayName("Grouping")]
  [Description("Ambiguously matches characters within pairs of [ square brackets ].")]
  public sealed class GroupLab : BaseConsoleLab
  {
    protected override void Main()
    {
      IObservable<char> input = Observable.Return(
        "[a[b[c]d][#]e]")
        .SelectMany(c => c);

      IObservable<string> groups = input.Parse(parser =>
        from next in parser
        let open = next.Where(c => c == '[')
        let close = next.Where(c => c == ']')
        select open.AmbiguousGroup(close).Join(v => "[" + v + "]"));

      groups.ForEach(ConsoleOutput);
    }
  }
}