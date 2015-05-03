using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System
{
  /// <summary>
  /// Represents an instance of an object or a missing instance of an object.
  /// </summary>
  /// <typeparam name="T">Type of object.</typeparam>
#if !SILVERLIGHT && !PORT_45 && !PORT_40
  [Serializable]
#endif
  public struct Maybe<T> : IEquatable<Maybe<T>>
  {
    /// <summary>
    /// Gets a value indicating whether <see cref="Value"/> is available.
    /// </summary>
    /// <value><see langword="true"/> if <see cref="Value"/> is available; otherwise, <see langword="false" />.</value>
    public bool HasValue
    {
      get
      {
        return hasValue;
      }
    }

    /// <summary>
    /// Gets the value when <see cref="HasValue"/> is <see langword="true" />.
    /// </summary>
    public T Value
    {
      get
      {
        Contract.Requires(HasValue);

        return value;
      }
    }

    /// <summary>
    /// Indicates a missing instance of <typeparamref name="T"/>.
    /// </summary>
    internal static readonly Maybe<T> Empty = new Maybe<T>();

    [ContractPublicPropertyName("Value")]
    private readonly T value;
    [ContractPublicPropertyName("HasValue")]
    private readonly bool hasValue;

    /// <summary>
    /// Constructs a new instance of the <see cref="Maybe{T}" /> struct with the specified available <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// Constructing a <see cref="Maybe{T}"/> instance with this constructor always sets <see cref="HasValue"/> to <see langword="true" />.
    /// </remarks>
    /// <param name="value">The value assigned to the <see cref="Value"/> property.</param>
    internal Maybe(T value)
    {
      Contract.Ensures(Contract.ValueAtReturn(out hasValue));
      Contract.Ensures(object.Equals(Contract.ValueAtReturn(out this.value), value));

      this.value = value;

      hasValue = true;
    }

    /// <summary>
    /// Gets the <see cref="Value"/> if present; otherwise, returns the default value of <typeparamref name="T"/>.
    /// </summary>
    /// <returns>The <see cref="Value"/> if present; otherwise, the default value of <typeparamref name="T"/>.</returns>
    public T ValueOrDefault()
    {
      return ValueOrDefault(default(T));
    }

    /// <summary>
    /// Gets the <see cref="Value"/> if present; otherwise, returns <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="defaultValue">The value to be returned when <see cref="HasValue"/> is <see langword="false"/>.</param>
    /// <returns>The <see cref="Value"/> if present; otherwise, <paramref name="defaultValue"/>.</returns>
    public T ValueOrDefault(T defaultValue)
    {
      return hasValue ? value : defaultValue;
    }

    /// <summary>
    /// Determines the equality of two <see cref="Maybe{T}"/> values.
    /// </summary>
    /// <param name="first">The first value.</param>
    /// <param name="second">The second value.</param>
    /// <returns><see langword="true"/> if the <paramref name="first"/> value equals the <paramref name="second"/> value; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Maybe<T> first, Maybe<T> second)
    {
      return first.Equals(second);
    }

    /// <summary>
    /// Determines the inequality of two <see cref="Maybe{T}"/> values.
    /// </summary>
    /// <param name="first">The first value.</param>
    /// <param name="second">The second value.</param>
    /// <returns><see langword="true"/> if the <paramref name="first"/> value does not equal the <paramref name="second"/> value; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Maybe<T> first, Maybe<T> second)
    {
      return !first.Equals(second);
    }

    /// <summary>
    /// Determines the equality of this instance and the specified <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">The object that is compared to this instance.</param>
    /// <returns><see langword="true"/> if this instance equals the specified <paramref name="obj"/>; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object obj)
    {
      return obj is Maybe<T>
          && Equals((Maybe<T>)obj);
    }

    /// <summary>
    /// Determines the equality of this instance and the <paramref name="other"/> instance.
    /// </summary>
    /// <param name="other">The instance that is compared to this instance.</param>
    /// <returns><see langword="true"/> if this instance equals the <paramref name="other"/> instance; otherwise, <see langword="false" />.</returns>
    public bool Equals(Maybe<T> other)
    {
      return hasValue == other.hasValue
          && (!hasValue || EqualityComparer<T>.Default.Equals(value, other.value));
    }

    /// <summary>
    /// Gets the hash code of this instance.
    /// </summary>
    /// <returns>-1 if <see cref="HasValue"/> is <see langword="false" /> and 0 if <see cref="Value"/> is <see langword="null"/>; otherwise, the hash code of <see cref="Value"/>.</returns>
    public override int GetHashCode()
    {
      return hasValue
        ? value == null ? 0 : value.GetHashCode()
        : -1;
    }

    /// <summary>
    /// Returns the string representation of <see cref="Value"/>.
    /// </summary>
    /// <returns>String that represents this instance.</returns>
    public override string ToString()
    {
      return value == null ? string.Empty : value.ToString();
    }
  }
}
