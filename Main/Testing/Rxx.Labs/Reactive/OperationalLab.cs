using System;
using System.ComponentModel;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rxx.Labs.Reactive
{
  [DisplayName("Operator Overloads")]
  [Description("Reactive AsOperational extensions.")]
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
      var xs = new Subject<int>();
      var ys = new Subject<int>();
      var zs = new Subject<int>();

      IObservable<int> query = xs.AsOperational() + ys - zs;

      using (query.Subscribe(ConsoleOutput))
      {
        xs.OnNext(1);
        ys.OnNext(5);
        zs.OnNext(2);

        xs.OnNext(2);
        ys.OnNext(6);
        zs.OnNext(3);

        xs.OnNext(3);
        xs.OnNext(4);

        zs.OnNext(4);
        zs.OnNext(5);

        ys.OnNext(7);
        ys.OnNext(8);

        WaitForKey();
      }
    }

    [Description("The AsOperational extension method allows you to define binary and unary operator overloads as "
               + "functions for any type, not just the primitive numeric types.  You can also specify the binary "
               + "strategy for combining sequences operationally; although, this experiment does not show that "
               + "feature in particular.")]
    public void AdvancedExperiment()
    {
      var xs = new Subject<int>();
      var ys = new Subject<int>();
      var zs = new Subject<int>();

      // Define an operational factory so that operators are only specified once.
      var o = (Func<IObservable<int>, OperationalObservable<int>>)
        (source => source.AsOperational(
          add: (left, right) => left + (right * 5),
          subtract: (left, right) => left - right,
          multiply: (left, right) => left * right,
          divide: (left, right) => (left * 2) / right,
          negative: value => -value));

      IObservable<int> query = (-o(xs) * 2) + ys - (o(zs) / 4);

      using (query.Subscribe(ConsoleOutput))
      {
        xs.OnNext(1);
        ys.OnNext(5);
        zs.OnNext(4);

        xs.OnNext(2);
        ys.OnNext(6);
        zs.OnNext(8);

        xs.OnNext(3);
        ys.OnNext(7);
        zs.OnNext(12);

        WaitForKey();
      }
    }

    [Description("Concatenating elements with separators.")]
    public void ConcatExperiment()
    {
      var xs = new Subject<int>();
      var ys = new Subject<int>();
      var zs = new Subject<int>();

      IObservable<string> query =
          xs.Select(v => v.ToString(CultureInfo.CurrentCulture)).AsOperational()
        + ","
        + ys.Select(v => v.ToString(CultureInfo.CurrentCulture))
        + "-"
        + zs.Select(v => v.ToString(CultureInfo.CurrentCulture));

      using (query.Subscribe(ConsoleOutput))
      {
        xs.OnNext(1);
        ys.OnNext(2);
        zs.OnNext(3);

        xs.OnNext(4);
        ys.OnNext(5);
        zs.OnNext(6);

        xs.OnNext(7);
        ys.OnNext(8);
        zs.OnNext(9);

        WaitForKey();
      }
    }

    [Description("Reevaluating an expression whenever any of its elements change.")]
    public void ReactiveExpressionExperiment()
    {
      var xs = new Subject<int>();
      var ys = new Subject<int>();
      var zs = new Subject<int>();
      var suffix = new Subject<string>();

      var os = xs.AsOperational(Observable.CombineLatest);

      IObservable<int> math = (os * 2) + ys - zs;
      IObservable<string> format = math
        .Select(v => v.ToString(CultureInfo.CurrentCulture))
        .AsOperational(Observable.CombineLatest)
        + suffix;

      using (format.Subscribe(ConsoleOutput))
      {
        xs.OnNext(1);
        ys.OnNext(1);
        zs.OnNext(1);
        suffix.OnNext("!");

        zs.OnNext(4);
        ys.OnNext(0);
        zs.OnNext(2);
        xs.OnNext(5);
        suffix.OnNext("?");

        xs.OnNext(10);
        suffix.OnNext(".");

        WaitForKey();
      }
    }

    [Description("Comparing elements in sequences for equality.")]
    public void EqualityComparisonExperiment()
    {
      var a = new object();
      var b = new object();
      var c = new object();

      var xs = new Subject<object>();
      var ys = new Subject<object>();
      var zs = new Subject<object>();

      IObservable<bool> query = xs.AsOperational() == ys | zs.AsOperational() == b;

      using (query.Subscribe(ConsoleOutput))
      {
        xs.OnNext(a);
        ys.OnNext(a);
        zs.OnNext(c);

        xs.OnNext(c);
        ys.OnNext(b);
        zs.OnNext(a);

        xs.OnNext(b);
        ys.OnNext(c);
        zs.OnNext(b);

        xs.OnNext(c);
        ys.OnNext(c);
        zs.OnNext(a);

        xs.OnNext(b);
        ys.OnNext(b);
        zs.OnNext(a);

        xs.OnNext(a);
        ys.OnNext(c);
        zs.OnNext(c);

        WaitForKey();
      }
    }

    [Description("Comparing elements in sequences.")]
    public void ComparisonExperiment()
    {
      var xs = new Subject<int>();
      var ys = new Subject<int>();
      var zs = new Subject<int>();

      IObservable<bool> query = xs.AsOperational() < ys & zs.AsOperational() >= 5;

      using (query.Subscribe(ConsoleOutput))
      {
        xs.OnNext(1);
        ys.OnNext(2);
        zs.OnNext(3);

        xs.OnNext(4);
        ys.OnNext(5);
        zs.OnNext(6);

        xs.OnNext(7);
        ys.OnNext(8);
        zs.OnNext(4);

        xs.OnNext(5);
        ys.OnNext(1);
        zs.OnNext(5);

        xs.OnNext(6);
        ys.OnNext(6);
        zs.OnNext(8);

        xs.OnNext(3);
        ys.OnNext(4);
        zs.OnNext(2);

        xs.OnNext(3);
        ys.OnNext(4);
        zs.OnNext(6);

        WaitForKey();
      }
    }
  }
}