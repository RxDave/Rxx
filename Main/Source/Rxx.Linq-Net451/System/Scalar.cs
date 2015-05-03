using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System
{
  /// <summary>
  /// Provides <see langword="static"/> factory methods for creating instances of <see cref="Scalar{T}"/>.
  /// </summary>
  public static class Scalar
  {
    /// <summary>
    /// Returns a new <see cref="Scalar{T}"/> object that represents the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value to be amplified.</param>
    /// <returns>A new <see cref="Scalar{T}"/> object that represents the specified <paramref name="value"/>.</returns>
    public static Scalar<T> Return<T>(T value)
    {
      Contract.Ensures(Contract.Result<Scalar<T>>() != null);
      Contract.Ensures(object.Equals(Contract.Result<Scalar<T>>().Value, value));

      return new Scalar<T>(value);
    }

    /// <summary>
    /// Returns an enumerable sequence containing the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value to be amplified.</param>
    /// <returns>An enumerable sequence containing the specified <paramref name="value"/>.</returns>
    public static IEnumerable<T> Enumerable<T>(T value)
    {
      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

      return new Scalar<T>(value);
    }

    /// <summary>
    /// Returns an observable sequence containing the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value to be amplified.</param>
    /// <returns>An observable sequence containing the specified <paramref name="value"/>.</returns>
    public static IObservable<T> Observable<T>(T value)
    {
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return new Scalar<T>(value);
    }
  }
}