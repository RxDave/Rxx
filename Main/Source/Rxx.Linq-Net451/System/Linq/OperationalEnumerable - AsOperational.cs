using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System.Linq
{
  /// <summary>
  /// Provides extension methods that convert an <see cref="IEnumerable{T}"/> into an <see cref="OperationalEnumerable{TIn,TOut}"/>.
  /// </summary>
  public static partial class OperationalEnumerable
  {
    /// <summary>
    /// Creates an <see cref="OperationalEnumerable{T}"/> with comparison operators for the specified <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects to observe.</typeparam>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{T}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    public static OperationalEnumerable<T> AsOperational<T>(this IEnumerable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<T>>(), null));

      return AsOperational(source, null, null, null);
    }

    /// <summary>
    /// Creates an <see cref="OperationalEnumerable{T}"/> with comparison operators for the specified <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects to observe.</typeparam>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for ordering.</param>
    /// <param name="equalityComparer">An object that compares values for equality.</param>
    /// <returns>An <see cref="OperationalEnumerable{T}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<T> AsOperational<T>(
      this IEnumerable<T> source,
      Func<IEnumerable<T>, IEnumerable<T>, Func<T, T, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<T> comparer = null,
      IEqualityComparer<T> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<T>>(), null));

      if (comparer == null)
      {
        comparer = Comparer<T>.Default;
      }

      if (equalityComparer == null)
      {
        equalityComparer = EqualityComparer<T>.Default;
      }

      return new OperationalEnumerable<T>(
        source,
        null,
        comparisonOperation,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        equalityComparer.Equals,
        (left, right) => !equalityComparer.Equals(left, right),
        (left, right) => comparer.Compare(left, right) < 0,
        (left, right) => comparer.Compare(left, right) <= 0,
        (left, right) => comparer.Compare(left, right) > 0,
        (left, right) => comparer.Compare(left, right) >= 0,
        null,
        null,
        null);
    }

    /// <summary>
    /// Creates an <see cref="OperationalEnumerable{T}"/> for the specified <paramref name="source"/> from the specified operators.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="shiftOperation">The join behavior for bitwise shift operations.</param>
    /// <param name="add">The addition operator.</param>
    /// <param name="subtract">The subtraction operator.</param>
    /// <param name="multiply">The multiplication operator.</param>
    /// <param name="divide">The division operator.</param>
    /// <param name="remainder">The remainder operator.</param>
    /// <param name="leftShift">The left shift operator.</param>
    /// <param name="rightShift">The right shift operator.</param>
    /// <param name="positive">The positive operator.</param>
    /// <param name="negative">The numeric negation operator.</param>
    /// <param name="complement">The bitwise complement operator.</param>
    /// <param name="not">The logical negation operator.</param>
    /// <param name="equals">The equals operator.</param>
    /// <param name="notEquals">The not equals operator.</param>
    /// <param name="lessThan">The less than operator.</param>
    /// <param name="lessThanOrEqual">The less than or equal to operator.</param>
    /// <param name="greaterThan">The greater than operator.</param>
    /// <param name="greaterThanOrEqual">The greater than or equal to operator.</param>
    /// <param name="and">The logical/bitwise AND operator.</param>
    /// <param name="or">The logical/bitwise OR operator.</param>
    /// <param name="xor">The logical/bitwise exclusive-OR operator.</param>
    /// <returns>An <see cref="OperationalEnumerable{T}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<T> AsOperational<T>(
      this IEnumerable<T> source,
      Func<IEnumerable<T>, IEnumerable<T>, Func<T, T, T>, IEnumerable<T>> binaryOperation = null,
      Func<IEnumerable<T>, IEnumerable<T>, Func<T, T, bool>, IEnumerable<bool>> comparisonOperation = null,
      Func<IEnumerable<T>, IEnumerable<int>, Func<T, int, T>, IEnumerable<T>> shiftOperation = null,
      Func<T, T, T> add = null,
      Func<T, T, T> subtract = null,
      Func<T, T, T> multiply = null,
      Func<T, T, T> divide = null,
      Func<T, T, T> remainder = null,
      Func<T, int, T> leftShift = null,
      Func<T, int, T> rightShift = null,
      Func<T, T> positive = null,
      Func<T, T> negative = null,
      Func<T, T> complement = null,
      Func<T, bool> not = null,
      Func<T, T, bool> equals = null,
      Func<T, T, bool> notEquals = null,
      Func<T, T, bool> lessThan = null,
      Func<T, T, bool> lessThanOrEqual = null,
      Func<T, T, bool> greaterThan = null,
      Func<T, T, bool> greaterThanOrEqual = null,
      Func<T, T, T> and = null,
      Func<T, T, T> or = null,
      Func<T, T, T> xor = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<T>>(), null));

      return new OperationalEnumerable<T>(
        source,
        binaryOperation,
        comparisonOperation,
        shiftOperation,
        add,
        subtract,
        multiply,
        divide,
        remainder,
        leftShift,
        rightShift,
        positive,
        negative,
        complement,
        not,
        equals,
        notEquals,
        lessThan,
        lessThanOrEqual,
        greaterThan,
        greaterThanOrEqual,
        and,
        or,
        xor);
    }

    /// <summary>
    /// Creates an <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <paramref name="source"/> from the specified operators.
    /// </summary>
    /// <typeparam name="TIn">The type of input to enumerate.</typeparam>
    /// <typeparam name="TOut">The type of output that each operation generates.</typeparam>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="resultSelector">Projects the result sequence into an <see cref="OperationalEnumerable{T}"/>.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="shiftOperation">The join behavior for bitwise shift operations.</param>
    /// <param name="add">The addition operator.</param>
    /// <param name="subtract">The subtraction operator.</param>
    /// <param name="multiply">The multiplication operator.</param>
    /// <param name="divide">The division operator.</param>
    /// <param name="remainder">The remainder operator.</param>
    /// <param name="leftShift">The left shift operator.</param>
    /// <param name="rightShift">The right shift operator.</param>
    /// <param name="positive">The positive operator.</param>
    /// <param name="negative">The numeric negation operator.</param>
    /// <param name="complement">The bitwise complement operator.</param>
    /// <param name="not">The logical negation operator.</param>
    /// <param name="equals">The equals operator.</param>
    /// <param name="notEquals">The not equals operator.</param>
    /// <param name="lessThan">The less than operator.</param>
    /// <param name="lessThanOrEqual">The less than or equal to operator.</param>
    /// <param name="greaterThan">The greater than operator.</param>
    /// <param name="greaterThanOrEqual">The greater than or equal to operator.</param>
    /// <param name="and">The logical/bitwise AND operator.</param>
    /// <param name="or">The logical/bitwise OR operator.</param>
    /// <param name="xor">The logical/bitwise exclusive-OR operator.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<TIn, TOut> AsOperational<TIn, TOut>(
      this IEnumerable<TIn> source,
      Func<IEnumerable<TOut>, OperationalEnumerable<TOut>> resultSelector,
      Func<IEnumerable<TIn>, IEnumerable<TIn>, Func<TIn, TIn, TOut>, IEnumerable<TOut>> binaryOperation = null,
      Func<IEnumerable<TIn>, IEnumerable<TIn>, Func<TIn, TIn, bool>, IEnumerable<bool>> comparisonOperation = null,
      Func<IEnumerable<TIn>, IEnumerable<int>, Func<TIn, int, TOut>, IEnumerable<TOut>> shiftOperation = null,
      Func<TIn, TIn, TOut> add = null,
      Func<TIn, TIn, TOut> subtract = null,
      Func<TIn, TIn, TOut> multiply = null,
      Func<TIn, TIn, TOut> divide = null,
      Func<TIn, TIn, TOut> remainder = null,
      Func<TIn, int, TOut> leftShift = null,
      Func<TIn, int, TOut> rightShift = null,
      Func<TIn, TOut> positive = null,
      Func<TIn, TOut> negative = null,
      Func<TIn, TOut> complement = null,
      Func<TIn, bool> not = null,
      Func<TIn, TIn, bool> equals = null,
      Func<TIn, TIn, bool> notEquals = null,
      Func<TIn, TIn, bool> lessThan = null,
      Func<TIn, TIn, bool> lessThanOrEqual = null,
      Func<TIn, TIn, bool> greaterThan = null,
      Func<TIn, TIn, bool> greaterThanOrEqual = null,
      Func<TIn, TIn, TOut> and = null,
      Func<TIn, TIn, TOut> or = null,
      Func<TIn, TIn, TOut> xor = null)
    {
      Contract.Requires(source != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TIn, TOut>>(), null));

      return new OperationalEnumerable<TIn, TOut>(
        source,
        resultSelector,
        binaryOperation,
        comparisonOperation,
        shiftOperation,
        add,
        subtract,
        multiply,
        divide,
        remainder,
        leftShift,
        rightShift,
        positive,
        negative,
        complement,
        not,
        equals,
        notEquals,
        lessThan,
        lessThanOrEqual,
        greaterThan,
        greaterThanOrEqual,
        and,
        or,
        xor);
    }
  }
}