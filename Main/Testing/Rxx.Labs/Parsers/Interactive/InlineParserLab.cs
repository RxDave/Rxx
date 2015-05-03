using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Rxx.Labs.Properties;
using Rxx.Parsers.Linq;

namespace Rxx.Labs.Parsers.Interactive
{
  [DisplayName("In-line")]
  [Description("Yields pairs of repeated letters that follow a number, excluding all b's.")]
  public sealed class InlineParserLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForCompletion);

      IEnumerable<char> source = new[]
				{
					'f', '3', 'a', 'a', '4', 'b', 'b', 'j', 'd', 'e', '1', 'c', 'c', '9'
				}
        .Do(ConsoleOutput(Text.Generated));

      IEnumerable<string> parsed =
        from word in
          source.Parse(parser =>
            from next in parser
            let letter = next.Where(char.IsLetter)
            let number = next.Where(char.IsNumber)
            let word = from _ in number
                       from twoInARow in letter.And(letter).Join()
                       select twoInARow
            select word)
        where !word.Contains('b')
        select word;

      parsed.ForEach(ConsoleOutput);
    }
  }
}
