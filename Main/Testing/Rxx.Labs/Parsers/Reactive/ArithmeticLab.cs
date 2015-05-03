using System;
using System.ComponentModel;
using System.Globalization;
using System.Reactive.Linq;
using Rxx.Labs.Properties;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.Labs.Parsers.Reactive
{
  [DisplayName("Arithmetic")]
  [Description("Reactively evaluates simple mathematical expressions as they are entered.")]
  public sealed class ArithmeticLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceDescription(Instructions.ParserArithmeticLab);

      IObservable<string> expressions = Observable.Defer(() =>
        Observable.Return(UserInput(Text.PromptFormat, Text.Expression)));

      bool stop = false;

      IObservable<char> chars = expressions
        .Do(expression => stop = string.IsNullOrEmpty(expression))
        .Where(_ => !stop)
        .SelectMany(c => c);

      IObservable<int> parsed = chars.ParseString(parser =>
        from next in parser
        let sign = parser.Character('-').Or(parser.Character('+')).WithDefault('+')
        let number = sign.And(parser.Character(char.IsNumber).OneOrMore())
          .Join(value => int.Parse(value, NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture))
        let padding = parser.InsignificantWhiteSpace
        let paddedNumber = padding.IgnoreBefore(number).IgnoreTrailing(padding)
        let operationSymbol = new[]
					{
						parser.Character('+'), 
						parser.Character('-'), 
						parser.Character('*'), 
						parser.Character('/')
					}.Any()
        select from left in paddedNumber
               from symbol in operationSymbol
               from right in paddedNumber
               select new { left, symbol, right })
        .Select(op =>
        {
          switch (op.symbol)
          {
            case '+':
              return op.left + op.right;
            case '-':
              return op.left - op.right;
            case '*':
              return op.left * op.right;
            default:
              return (op.right == 0 || op.left == int.MinValue) ? 0 : op.left / op.right;
          }
        });

      do
      {
        parsed.ForEach(ConsoleOutput);
      }
      while (!stop);
    }
  }
}