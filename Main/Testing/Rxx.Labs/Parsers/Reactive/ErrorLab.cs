using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Rxx.Labs.Properties;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.Labs.Parsers.Reactive
{
  [DisplayName("Errors")]
  [Description("Marks a single rule in a grammar as required and supplies invalid "
             + "input to illustrate failure behavior.")]
  public sealed class ErrorLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForError);

      IObservable<char> input = "1a2-3c_d".ToObservable();

      IObservable<string> groups = input.Parse(parser =>
        from next in parser
        let letter = next.Where(char.IsLetter)
        let number = next.Where(char.IsNumber).Required("A number is expected.")
        select number.And(letter).Join());

      groups.ForEach(ConsoleOutput);
    }
  }
}