using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Rxx.Labs.Interactive
{
  [DisplayName("Operator Overloads")]
  [Description("Interactive AsOperational extensions.")]
  public sealed class OperationalLab : BaseConsoleLab
  {
    protected override void Main()
    {
      RunExperiments();
    }

    [Description("Operator overload methods are combinators.  Rxx provides an AsOperational extension method that "
               + "converts sequences of the primitive numeric types into sequences that can be combined using the "
               + "basic C# operators.  Currently included are the binary +, -, *, / operators and the unary +, - "
               + "operators, for all of the primitive numeric types in which C# implicitly defines them.")]
    public void BasicExperiment()
    {
      IEnumerable<int> xs = new[] { 1, 2, 3, 4 };
      IEnumerable<int> ys = new[] { 5, 6, 7, 8 };
      IEnumerable<int> zs = new[] { 2, 3, 4, 5 };

      IEnumerable<int> query = xs.AsOperational() + ys - zs;

      query.ForEach(ConsoleOutput);
    }

    [Description("The AsOperational extension method allows you to define binary and unary operator overloads as "
               + "functions for any type, not just the primitive numeric types.  You can also specify the binary "
               + "strategy for combining sequences operationally; although, this experiment does not show that "
               + "feature in particular.")]
    public void AdvancedExperiment()
    {
      // Define an operational factory so that operators are only specified once.
      var o = new Func<IEnumerable<int>, OperationalEnumerable<int>>(
        source => source.AsOperational(
          add: (left, right) => left + (right * 5),
          subtract: (left, right) => left - right,
          multiply: (left, right) => left * right,
          divide: (left, right) => (left * 2) / right,
          negative: value => -value));

      IEnumerable<int> xs = new[] { 1, 2, 3 };
      IEnumerable<int> ys = new[] { 5, 6, 7 };
      IEnumerable<int> zs = new[] { 4, 8, 12 };

      IEnumerable<int> query = (-o(xs) * 2) + ys - (o(zs) / 4);

      query.ForEach(ConsoleOutput);
    }

    [Description("Concatenating elements with separators.")]
    public void ConcatExperiment()
    {
      IEnumerable<int> xs = new[] { 1, 4, 7 };
      IEnumerable<int> ys = new[] { 2, 5, 8 };
      IEnumerable<int> zs = new[] { 3, 6, 9 };

      IEnumerable<string> query =
          xs.Select(v => v.ToString(CultureInfo.CurrentCulture)).AsOperational()
        + ","
        + ys.Select(v => v.ToString(CultureInfo.CurrentCulture))
        + "-"
        + zs.Select(v => v.ToString(CultureInfo.CurrentCulture));

      query.ForEach(ConsoleOutput);
    }

    [Description("Comparing elements in sequences for equality.")]
    public void EqualityComparisonExperiment()
    {
      var a = new object();
      var b = new object();
      var c = new object();

      IEnumerable<object> xs = new[] { a, c, b, c, b, a };
      IEnumerable<object> ys = new[] { a, b, c, c, b, c };
      IEnumerable<object> zs = new[] { c, a, b, a, a, c };

      IEnumerable<bool> query = xs.AsOperational() == ys | zs.AsOperational() == b;

      query.ForEach(ConsoleOutput);
    }

    [Description("Comparing elements in sequences.")]
    public void ComparisonExperiment()
    {
      IEnumerable<int> xs = new[] { 1, 4, 7, 5, 6, 3, 3 };
      IEnumerable<int> ys = new[] { 2, 5, 8, 1, 6, 4, 4 };
      IEnumerable<int> zs = new[] { 3, 6, 4, 5, 8, 2, 6 };

      IEnumerable<bool> query = xs.AsOperational() < ys & zs.AsOperational() >= 5;

      query.ForEach(ConsoleOutput);
    }
  }
}