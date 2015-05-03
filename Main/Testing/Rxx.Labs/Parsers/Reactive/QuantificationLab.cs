using System;
using System.ComponentModel;
using System.Reactive.Linq;
using DaveSexton.Labs;
using Rxx.Labs.Properties;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.Labs.Parsers.Reactive
{
  [DisplayName("Quantification")]
  [Description("Matching repetition by quantifying parsers.")]
  public sealed class QuantificationLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForCompletion);
      TraceLine();

      RunExperiments();
    }

    [Description("Matches 5 consecutive integers.")]
    public void ExactlyExperiment()
    {
      IObservable<char> input = "123   12345   12".ToObservable();

      IObservable<string> query = input.ParseString(parser =>
        from next in parser
        let number = parser.Character(char.IsNumber)
        select number.Exactly(5).Join());

      query.ForEach(ConsoleOutput);
    }

    [Description("Matches an optional sign and the following integer.")]
    public void MaybeExperiment()
    {
      IObservable<char> input = "+9  4  -2  @6".ToObservable();

      IObservable<string> query = input.ParseString(parser =>
        from next in parser
        let number = parser.Character(char.IsNumber)
        let sign = parser.Character('+').Or(parser.Character('-'))
        select from s in sign.Maybe().Join()
               from n in number
               select s + n);

      query.ForEach(ConsoleOutput);
    }

    [Description("Matches a letter followed by another letter.")]
    public void NoneExperiment()
    {
      IObservable<char> input = "ab1cd2ef3".ToObservable();

      IObservable<char> query = input.ParseString(parser =>
        from next in parser
        let number = parser.Character(char.IsNumber)
        let letter = parser.Character(char.IsLetter)
        select from l in letter
               from _ in number.None()
               select l);

      query.ForEach(ConsoleOutput);
    }

    [Description("Matches a letter followed by zero or more consecutive integers.")]
    public void NoneOrMoreExperiment()
    {
      IObservable<char> input = "a b1 c23 d456".ToObservable();

      IObservable<string> query = input.ParseString(parser =>
        from next in parser
        let letter = parser.Character(char.IsLetter)
        let number = parser.Character(char.IsNumber)
        select from l in letter
               from n in number.NoneOrMore().Join()
               select l + n);

      query.ForEach(ConsoleOutput);
    }

    [Description("Matches a letter followed by one or more consecutive integers.")]
    public void OneOrMoreExperiment()
    {
      IObservable<char> input = "a b1 c23 d456".ToObservable();

      IObservable<string> query = input.ParseString(parser =>
        from next in parser
        let letter = parser.Character(char.IsLetter)
        let number = parser.Character(char.IsNumber)
        select from l in letter
               from n in number.OneOrMore().Join()
               select l + n);

      query.ForEach(ConsoleOutput);
    }

    [Description("Matches a letter followed by at least two consecutive integers.")]
    public void AtLeastExperiment()
    {
      IObservable<char> input = "a b1 c23 d456".ToObservable();

      IObservable<string> query = input.ParseString(parser =>
        from next in parser
        let letter = parser.Character(char.IsLetter)
        let number = parser.Character(char.IsNumber)
        select from l in letter
               from n in number.AtLeast(2).Join()
               select l + n);

      query.ForEach(ConsoleOutput);
    }

    [Description("Matches a letter followed by at least two consecutive integers "
               + "followed by an integer that is greater than eight.")]
    public void AtLeastNonGreedyExperiment()
    {
      IObservable<char> input = "a19 b129 c12939 d123949".ToObservable();

      IObservable<string> query = input.ParseString(parser =>
        from next in parser
        let letter = parser.Character(char.IsLetter)
        let number = parser.Character(char.IsNumber).Select(char.GetNumericValue)
        select from l in letter
               from n in number.AtLeastNonGreedy(2).Join()
               from g in number.Where(value => value > 8)
               select l + n + g);

      query.ForEach(ConsoleOutput);
    }

    [Experiment("AtLeast Range")]
    [Description("Matches at least 3 and at most 4 consecutive integers.")]
    public void AtLeastRangeExperiment()
    {
      IObservable<char> input = "12 234 3456 45678 987654".ToObservable();

      IObservable<string> query = input.ParseString(parser =>
        from next in parser
        let number = parser.Character(char.IsNumber)
        select number.AtLeast(3, maximum: 4).Join());

      query.ForEach(ConsoleOutput);
    }
  }
}