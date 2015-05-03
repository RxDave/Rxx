using System.Collections.Generic;
using System.ComponentModel;
using Rxx.Labs.Properties;
using Rxx.Parsers.Linq;

namespace Rxx.Labs.Parsers.Interactive
{
  [DisplayName("Errors")]
  [Description("Marks a single rule in a grammar as required and supplies invalid "
             + "input to illustrate failure behavior.")]
  public sealed class ErrorLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForError);

      IEnumerable<char> input = "1a2-3c_d";

      IEnumerable<string> groups = input.ParseString(parser =>
        from next in parser
        let letter = parser.Character(char.IsLetter)
        let number = parser.Character(char.IsNumber).Required("A number is expected.")
        select number.And(letter).Join());

      groups.ForEach(ConsoleOutput);
    }
  }
}