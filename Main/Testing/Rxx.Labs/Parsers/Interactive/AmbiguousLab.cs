using System.Collections.Generic;
using System.ComponentModel;
using Rxx.Labs.Properties;
using Rxx.Parsers.Linq;

namespace Rxx.Labs.Parsers.Interactive
{
  [DisplayName("Ambiguous")]
  [Description("Parses a string with an ambiguous grammar that matches two consecutive letters.")]
  public sealed class AmbiguousLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForCompletion);

      IEnumerable<char> input = "abcde|fgh";

      IEnumerable<string> doubles = input.Parse(parser =>
        from next in parser
        let letter = next.Where(char.IsLetter)
        let twoLetters = letter.And(letter).Join()
        select twoLetters.Ambiguous());

      doubles.ForEach(ConsoleOutput);
    }
  }
}