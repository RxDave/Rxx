using System.Collections.Generic;
using System.ComponentModel;
using Rxx.Labs.Properties;
using Rxx.Parsers.Linq;

namespace Rxx.Labs.Parsers.Interactive
{
  [DisplayName("Mini ML")]
  [Description("Parses a simple functional programming language into an Abstract Syntax Tree (AST).")]
  public sealed class MiniMLParserLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForCompletion);

      IEnumerable<char> program = @"
				let true = \x.\y.x in 
				let false = \x.\y.y in 
				let if = \b.\l.\r.(b l) r in
				if true then false else true;";

      IEnumerable<Term> parsed = program.Parse(new MiniMLParser());

      parsed.ForEach(ConsoleOutput);
    }
  }
}