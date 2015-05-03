using System.Collections.Generic;
using System.ComponentModel;
using Rxx.Labs.Properties;
using Rxx.Parsers.Linq;

namespace Rxx.Labs.Parsers.Interactive
{
  [DisplayName("Grouping")]
  [Description("Ambiguously matches characters within [square brackets].")]
  public sealed class GroupLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForCompletion);

      IEnumerable<char> input = "[a[b[c]d][#]e]";

      IEnumerable<string> groups = input.Parse(parser =>
        from next in parser
        let open = next.Where(c => c == '[')
        let close = next.Where(c => c == ']')
        select open.AmbiguousGroup(close).Join(v => "[" + v + "]"));

      groups.ForEach(ConsoleOutput);
    }
  }
}