using System.Diagnostics.Contracts;

namespace System
{
  /// <summary>
  /// Provides <see langword="static"/> factory methods for creating instances of <see cref="Either{TLeft,TRight}"/> objects.
  /// </summary>
  public static partial class Either
  {
    /// <summary>
    /// Creates a new instance of <see cref="Either{TLeft,TRight}" /> with the specified <paramref name="value"/>
    /// and <see cref="Either{TLeft,TRight}.IsLeft"/> set to <see langword="true"/>.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left value.</typeparam>
    /// <typeparam name="TRight">Type of the right value.</typeparam>
    /// <param name="value">The left value.</param>
    /// <returns>A new instance of <see cref="Either{TLeft,TRight}" /> with the specified <paramref name="value"/>
    /// and <see cref="Either{TLeft,TRight}.IsLeft"/> set to <see langword="true"/>.</returns>
    public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft value)
    {
      Contract.Ensures(Contract.Result<Either<TLeft, TRight>>() != null);

      return new LeftValue<TLeft, TRight>(value);
    }

    /// <summary>
    /// Creates a new instance of <see cref="Either{TLeft,TRight}" /> with the specified <paramref name="value"/>
    /// and <see cref="Either{TLeft,TRight}.IsLeft"/> set to <see langword="false"/>.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left value.</typeparam>
    /// <typeparam name="TRight">Type of the right value.</typeparam>
    /// <param name="value">The right value.</param>
    /// <returns>A new instance of <see cref="Either{TLeft,TRight}" /> with the specified <paramref name="value"/>
    /// and <see cref="Either{TLeft,TRight}.IsLeft"/> set to <see langword="false"/>.</returns>
    public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight value)
    {
      Contract.Ensures(Contract.Result<Either<TLeft, TRight>>() != null);

      return new RightValue<TLeft, TRight>(value);
    }
  }
}