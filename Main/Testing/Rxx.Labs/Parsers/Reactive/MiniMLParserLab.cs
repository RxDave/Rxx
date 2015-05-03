using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.Labs.Parsers.Reactive
{
  [DisplayName("Mini ML")]
  [Description("Parses a simple functional programming language into an Abstract Syntax Tree (AST).")]
  public sealed class MiniMLParserLab : BaseConsoleLab
  {
    protected override void Main()
    {
      IObservable<char> program = Observable.Return(@"
				let true = \x.\y.x in 
				let false = \x.\y.y in 
				let if = \b.\l.\r.(b l) r in
				if true then false else true;")
        .SelectMany(c => c);

      IObservable<Term> parsed = program.Parse(new MiniMLObservableParser());

      parsed.ForEach(ConsoleOutput);
    }
  }
}