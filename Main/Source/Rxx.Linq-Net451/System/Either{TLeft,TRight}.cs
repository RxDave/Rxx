using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System
{
  /// <summary>
  /// Represents one of two possible values.
  /// </summary>
  /// <typeparam name="TLeft">Type of the left value.</typeparam>
  /// <typeparam name="TRight">Type of the right value.</typeparam>
  [ContractClass(typeof(EitherContract<,>))]
  public abstract partial class Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>, IStructuralEquatable
  {
    /// <summary>
    /// Gets a value indicating whether the object holds the left value or the right value.
    /// </summary>
    /// <value><see langword="true" /> if the object holds the left value; otherwise, <see langword="false"/>.</value>
    public abstract bool IsLeft { get; }

    /// <summary>
    /// Gets the left value when <see cref="IsLeft"/> is <see langword="true"/>.
    /// </summary>
    /// <value>The left value when <see cref="IsLeft"/> is <see langword="true"/>.</value>
    public abstract TLeft Left { get; }

    /// <summary>
    /// Gets the right value when <see cref="IsLeft"/> is <see langword="false"/>.
    /// </summary>
    /// <value>The right value when <see cref="IsLeft"/> is <see langword="false"/>.</value>
    public abstract TRight Right { get; }

    /// <summary>
    /// Constructs a new instance of the <see cref="Either{TLeft,TRight}" /> class for derived classes.
    /// </summary>
    protected Either()
    {
    }

    /// <summary>
    /// Invokes the specified <paramref name="left"/> or <paramref name="right"/> action depending upon 
    /// the value of <see cref="IsLeft"/>.
    /// </summary>
    /// <param name="left">The action to be invoked when <see cref="IsLeft"/> is <see langword="true" />.</param>
    /// <param name="right">The action to be invoked when <see cref="IsLeft"/> is <see langword="false" />.</param>
    public abstract void Switch(Action<TLeft> left, Action<TRight> right);

    /// <summary>
    /// Invokes the specified <paramref name="left"/> or <paramref name="right"/> function depending upon 
    /// the value of <see cref="IsLeft"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="left">The function to be invoked when <see cref="IsLeft"/> is <see langword="true" />.</param>
    /// <param name="right">The function to be invoked when <see cref="IsLeft"/> is <see langword="false" />.</param>
    /// <returns>The return value of either the <paramref name="left"/> or <paramref name="right"/> function
    /// depending upon the value of <see cref="IsLeft"/>.</returns>
    public abstract TResult Switch<TResult>(Func<TLeft, TResult> left, Func<TRight, TResult> right);

    /// <summary>
    /// Returns a <see cref="string"/> that represents the <see cref="Left"/> or <see cref="Right"/> side 
    /// of the current <see cref="Either{TLeft,TRight}"/> object.
    /// </summary>
    /// <returns>The <see cref="Left"/> object represented as a string if <see cref="IsLeft"/> is 
    /// <see langword="true"/>; otherwise, the <see cref="Right"/> object represented as a string.</returns>
    public override string ToString()
    {
      if (IsLeft)
      {
        return "{Left:" + (Left == null ? null : Left.ToString()) + '}';
      }
      else
      {
        return "{Right:" + (Right == null ? null : Right.ToString()) + '}';
      }
    }

    /// <summary>
    /// Gets a value indicating whether the specified objects are equal.
    /// </summary>
    /// <param name="first">An object to be compared.</param>
    /// <param name="second">The other object to be compared.</param>
    /// <returns><see langword="True"/> if the objects are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(Either<TLeft, TRight> first, Either<TLeft, TRight> second)
    {
      return object.ReferenceEquals(first, null)
           ? object.ReferenceEquals(second, null)
           : first.Equals(second);
    }

    /// <summary>
    /// Gets a value indicating whether the specified objects are not equal.
    /// </summary>
    /// <param name="first">An object to be compared.</param>
    /// <param name="second">The other object to be compared.</param>
    /// <returns><see langword="True"/> if the objects are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(Either<TLeft, TRight> first, Either<TLeft, TRight> second)
    {
      return object.ReferenceEquals(first, null)
           ? !object.ReferenceEquals(second, null)
           : !first.Equals(second);
    }

    /// <summary>
    /// Gets a value indicating whether this object is equal to the specified object.
    /// </summary>
    /// <param name="obj">An object to be compared to this object.</param>
    /// <returns><see langword="True"/> if <paramref name="obj"/> is of type <see cref="Either{TLeft,TRight}"/> and the objects are equal; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object obj)
    {
      return Equals(obj as Either<TLeft, TRight>, EqualityComparer<TLeft>.Default, EqualityComparer<TRight>.Default);
    }

    /// <summary>
    /// Gets a value indicating whether this object is equal to the specified object.
    /// </summary>
    /// <param name="other">An object to be compared to this object.</param>
    /// <param name="comparer">The object used to compare the <see cref="Left"/> or <see cref="Right"/> values.</param>
    /// <returns><see langword="True"/> if <paramref name="other"/> is of type <see cref="Either{TLeft,TRight}"/> and the objects are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(object other, IEqualityComparer comparer)
    {
      return Equals(other as Either<TLeft, TRight>, (IEqualityComparer<TLeft>)comparer, (IEqualityComparer<TRight>)comparer);
    }

    /// <summary>
    /// Gets a value indicating whether this object is equal to the specified object.
    /// </summary>
    /// <param name="other">An object to be compared to this object.</param>
    /// <param name="left">The object used to compare the <see cref="Left"/> value if <see cref="IsLeft"/> equals <see langword="true"/>.</param>
    /// <param name="right">The object used to compare the <see cref="Right"/> value if <see cref="IsLeft"/> equals <see langword="false"/>.</param>
    /// <returns><see langword="True"/> if <paramref name="other"/> is of type <see cref="Either{TLeft,TRight}"/> and the objects are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(object other, IEqualityComparer<TLeft> left, IEqualityComparer<TRight> right)
    {
      return Equals(other as Either<TLeft, TRight>, left, right);
    }

    /// <summary>
    /// Gets a value indicating whether this object is equal to the specified object.
    /// </summary>
    /// <param name="other">An object to be compared to this object.</param>
    /// <param name="left">The object used to compare the <see cref="Left"/> value if <see cref="IsLeft"/> equals <see langword="true"/>.</param>
    /// <param name="right">The object used to compare the <see cref="Right"/> value if <see cref="IsLeft"/> equals <see langword="false"/>.</param>
    /// <returns><see langword="True"/> if the objects are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(Either<TLeft, TRight> other, IEqualityComparer<TLeft> left, IEqualityComparer<TRight> right)
    {
      return !object.ReferenceEquals(other, null)
          && IsLeft == other.IsLeft
          && (IsLeft ? left.Equals(Left, other.Left) : right.Equals(Right, other.Right));
    }

    /// <summary>
    /// Gets a value indicating whether this object is equal to the specified object.
    /// </summary>
    /// <param name="other">An object to be compared to this object.</param>
    /// <returns><see langword="True"/> if the objects are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(Either<TLeft, TRight> other)
    {
      return Equals(other, EqualityComparer<TLeft>.Default, EqualityComparer<TRight>.Default);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return GetHashCode(EqualityComparer<TLeft>.Default, EqualityComparer<TRight>.Default);
    }

    /// <summary>
    /// Returns a hash code for the object using the specified <paramref name="comparer"/>.
    /// </summary>
    /// <param name="comparer">The object used to generate a hash code for the current object.</param>
    /// <returns>A hash code generated using the specified <paramref name="comparer"/>.</returns>
    public int GetHashCode(IEqualityComparer comparer)
    {
      return GetHashCode((IEqualityComparer<TLeft>)comparer, (IEqualityComparer<TRight>)comparer);
    }

    /// <summary>
    /// Returns a hash code for the object using one of the specified comparers.
    /// </summary>
    /// <param name="left">The object used to generate a hash code for the current object if <see cref="IsLeft"/> equals <see langword="true"/>.</param>
    /// <param name="right">The object used to generate a hash code for the current object if <see cref="IsLeft"/> equals <see langword="false"/>.</param>
    /// <returns>A hash code generated using one of the specified comparers.</returns>
    public int GetHashCode(IEqualityComparer<TLeft> left, IEqualityComparer<TRight> right)
    {
      return IsLeft ? (Left == null ? 0 : left.GetHashCode(Left)) : (Right == null ? 0 : right.GetHashCode(Right));
    }
  }

  [ContractClassFor(typeof(Either<,>))]
  internal abstract class EitherContract<TLeft, TRight> : Either<TLeft, TRight>
  {
    public override bool IsLeft
    {
      get
      {
        return false;
      }
    }

    public override TLeft Left
    {
      get
      {
        Contract.Requires(IsLeft);

        return default(TLeft);
      }
    }

    public override TRight Right
    {
      get
      {
        Contract.Requires(!IsLeft);

        return default(TRight);
      }
    }

    public override void Switch(Action<TLeft> left, Action<TRight> right)
    {
      Contract.Requires(left != null);
      Contract.Requires(right != null);
    }

    public override TResult Switch<TResult>(Func<TLeft, TResult> left, Func<TRight, TResult> right)
    {
      Contract.Requires(left != null);
      Contract.Requires(right != null);
      return default(TResult);
    }
  }
}