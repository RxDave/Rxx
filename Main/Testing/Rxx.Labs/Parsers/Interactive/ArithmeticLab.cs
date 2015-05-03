using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Rxx.Labs.Properties;
using Rxx.Parsers.Linq;

namespace Rxx.Labs.Parsers.Interactive
{
  [DisplayName("Arithmetic")]
  [Description("Parses and evaluates a string of simple mathematical expressions.")]
  public sealed class ArithmeticLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForCompletion);

      IEnumerable<char> expressions = @"
				-15 +5
				100 / 5   13+8
				6 *-2";

      IEnumerable<int> parsed = expressions.ParseString(parser =>
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

      parsed.ForEach(ConsoleOutput);
    }
  }
}