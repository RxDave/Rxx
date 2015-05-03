using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Interactive
{
  [DisplayName("Tracing")]
  [Description("One example of the many interactive Trace extensions.")]
  public sealed class TraceLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.IxTraceLab);

      IEnumerable<string> lines = GetLinesFromUser()
        .TraceOnNext(value => "OnNext: " + value)
        .TraceOnCompleted(Text.Done);

      foreach (var line in lines)
      {
        // do nothing
      }
    }

    private IEnumerable<string> GetLinesFromUser()
    {
      TraceLine();

      do
      {
        string line = UserInput(Text.PromptFormat, Instructions.Input);

        if (string.IsNullOrWhiteSpace(line))
          break;

        yield return line;
      }
      while (true);
    }
  }
}