using System.Diagnostics.Contracts;

namespace System.Reactive
{
  /// <summary>
  /// Represents an observable that uses its unary and binary operator overloads as observable query operators.
  /// </summary>
  /// <typeparam name="T">Input and output type.</typeparam>
  public sealed class OperationalObservable<T> : OperationalObservable<T, T>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    #endregion

    #region Constructors
    internal OperationalObservable(
      IObservable<T> source,
      Func<IObservable<T>, IObservable<T>, Func<T, T, T>, IObservable<T>> binaryOperation,
      Func<IObservable<T>, IObservable<T>, Func<T, T, bool>, IObservable<bool>> comparisonOperation,
      Func<IObservable<T>, IObservable<int>, Func<T, int, T>, IObservable<T>> shiftOperation,
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
          result => new OperationalObservable<T>(
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