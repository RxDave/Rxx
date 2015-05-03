using System.Diagnostics.Contracts;

namespace System.Collections.Generic
{
  /// <summary>
  /// Represents an enumerable that uses its unary and binary operator overloads as query operators.
  /// </summary>
  /// <typeparam name="T">Input and output type.</typeparam>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification = "This class is not intended to be consumed publicly.  It's public only so that the compiler can resolve operator overloads.")]
  public sealed class OperationalEnumerable<T> : OperationalEnumerable<T, T>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    #endregion

    #region Constructors
    internal OperationalEnumerable(
      IEnumerable<T> source,
      Func<IEnumerable<T>, IEnumerable<T>, Func<T, T, T>, IEnumerable<T>> binaryOperation,
      Func<IEnumerable<T>, IEnumerable<T>, Func<T, T, bool>, IEnumerable<bool>> comparisonOperation,
      Func<IEnumerable<T>, IEnumerable<int>, Func<T, int, T>, IEnumerable<T>> shiftOperation,
      Func<T, T, T> add,
      Func<T, T, T> subtract,
      Func<T, T, T> multiply,
      Func<T, T, T> divide,
      Func<T, T, T> remainder,
      Func<T, int, T> leftShift,
      Func<T, int, T> rightShift,
      Func<T, T> positive,
      Func<T, T> negative,
      Func<T, T> complement,
      Func<T, bool> not,
      Func<T, T, bool> equals,
      Func<T, T, bool> notEquals,
      Func<T, T, bool> lessThan,
      Func<T, T, bool> lessThanOrEqual,
      Func<T, T, bool> greaterThan,
      Func<T, T, bool> greaterThanOrEqual,
      Func<T, T, T> and,
      Func<T, T, T> or,
      Func<T, T, T> xor)
      : base(
          source,
          result => new OperationalEnumerable<T>(
            result,
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
            xor),
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
          xor)
    {
      Contract.Requires(source != null);
    }
    #endregion

    #region Methods
    #endregion
  }
}