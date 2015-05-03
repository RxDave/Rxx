using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.Labs.Parsers.Reactive
{
  [DisplayName("Ambiguous")]
  [Description("Parses a small observable string with an ambiguous grammar that matches two consecutive characters.")]
  public sealed class AmbiguousLab : BaseConsoleLab
  {
    protected override void Main()
    {
      IObservable<char> input = Observable.Return(
        "abcde|fgh")
        .SelectMany(c => c);

      IObservable<string> doubles = input.Parse(parser =>
        from next in parser
        let letter = next.Where(char.IsLetter)
        let twoLetters = letter.And(letter).Join()
        select twoLetters.Ambiguous());

      doubles.ForEach(ConsoleOutput);
    }
  }
}