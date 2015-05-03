using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  /// <summary>
  /// Provides a set of <see langword="static"/> methods for query operations over observable sequences of <see cref="System.Maybe{T}" /> objects.
  /// </summary>
  public static partial class MaybeObservable
  {
    /// <summary>
    /// Returns the elements of the specified sequence as a sequence of <see cref="System.Maybe{T}"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be projected into <see cref="System.Maybe{T}"/> objects.</param>
    /// <returns>A sequence of <see cref="System.Maybe{T}"/> objects that contain the values from the specified observable.</returns>
    public static IObservable<Maybe<TSource>> Always<TSource>(this IObservable<TSource> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<Maybe<TSource>>>() != null);

      return source.Select(System.Maybe.Return);
    }

    /// <summary>
    /// Returns the elements of the specified sequence as a sequence of <see cref="System.Maybe{T}"/>, starting with <paramref name="defaultValue"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be projected into <see cref="System.Maybe{T}"/> objects.</param>
    /// <param name="defaultValue">The default value to be wrapped in <see cref="System.Maybe{T}"/> and pushed to an observer immediately upon subscription.</param>
    /// <returns>A sequence of <see cref="System.Maybe{T}"/> objects that contain the values from the specified
    /// observable, starting with <paramref name="defaultValue"/>.</returns>
    public static IObservable<Maybe<TSource>> Always<TSource>(this IObservable<TSource> source, TSource defaultValue)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<Maybe<TSource>>>() != null);

      return source.Select(System.Maybe.Return).StartWith(System.Maybe.Return(defaultValue));
    }

    /// <summary>
    /// Returns the elements of the specified sequence of <see cref="Nullable{TSource}"/> values as a sequence of <see cref="System.Maybe{T}"/>, 
    /// starting with <see cref="System.Maybe.Empty{T}()"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be projected into <see cref="System.Maybe{T}"/> objects.</param>
    /// <returns>A sequence of <see cref="System.Maybe{T}"/> objects that contain the <see cref="Nullable{TSource}"/> values from the specified
    /// observable, starting with <see cref="System.Maybe.Empty{T}()"/>.</returns>
    public static IObservable<Maybe<TSource>> Maybe<TSource>(this IObservable<TSource?> source)
      where TSource : struct
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<Maybe<TSource>>>() != null);

      return source.Select(value => value.HasValue ? System.Maybe.Return(value.Value) : System.Maybe.Empty<TSource>())
                   .StartWith(System.Maybe.Empty<TSource>());
    }

    /// <summary>
    /// Returns the elements of the specified sequence of <see cref="Nullable{TSource}"/> values as a sequence of <see cref="System.Maybe{T}"/>, starting with <paramref name="defaultValue"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be projected into <see cref="System.Maybe{T}"/> objects.</param>
    /// <param name="defaultValue">The default value to be wrapped in <see cref="System.Maybe{T}"/> and pushed to an observer immediately upon subscription.</param>
    /// <returns>A sequence of <see cref="System.Maybe{T}"/> objects that contain the <see cref="Nullable{TSource}"/> values from the specified
    /// observable, starting with <paramref name="defaultValue"/>.</returns>
    public static IObservable<Maybe<TSource>> Maybe<TSource>(this IObservable<TSource?> source, TSource? defaultValue)
      where TSource : struct
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<Maybe<TSource>>>() != null);

      return source.Select(value => value.HasValue ? System.Maybe.Return(value.Value) : System.Maybe.Empty<TSource>())
                   .StartWith(defaultValue.HasValue ? System.Maybe.Return(defaultValue.Value) : System.Maybe.Empty<TSource>());
    }

    /// <summary>
    /// Returns the elements of the specified sequence of objects as a sequence of <see cref="System.Maybe{T}"/> based on whether each reference is <see langword="null"/>, 
    /// starting with <see cref="System.Maybe.Empty{T}()"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be projected into <see cref="System.Maybe{T}"/> objects.</param>
    /// <returns>A sequence of <see cref="System.Maybe{T}"/> objects that contain the objects from the specified
    /// observable, starting with <see cref="System.Maybe.Empty{T}()"/>.</returns>
    public static IObservable<Maybe<TSource>> MaybeNull<TSource>(this IObservable<TSource> source)
      where TSource : class
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<Maybe<TSource>>>() != null);

      return source.Select(value => value != null ? System.Maybe.Return(value) : System.Maybe.Empty<TSource>())
                   .StartWith(System.Maybe.Empty<TSource>());
    }

    /// <summary>
    /// Returns the elements of the specified sequence of objects as a sequence of <see cref="System.Maybe{T}"/> based on whether each reference is <see langword="null"/>, 
    /// starting with <paramref name="defaultValue"/>.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable to be projected into <see cref="System.Maybe{T}"/> objects.</param>
    /// <param name="defaultValue">The default value to be wrapped in <see cref="System.Maybe{T}"/> and pushed to an observer immediately upon subscription.</param>
    /// <returns>A sequence of <see cref="System.Maybe{T}"/> objects that contain the objects from the specified
    /// observable, starting with <paramref name="defaultValue"/>.</returns>
    public static IObservable<Maybe<TSource>> MaybeNull<TSource>(this IObservable<TSource> source, TSource defaultValue)
      where TSource : class
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<Maybe<TSource>>>() != null);

      return source.Select(value => value != null ? System.Maybe.Return(value) : System.Maybe.Empty<TSource>())
                   .StartWith(defaultValue != null ? System.Maybe.Return(defaultValue) : System.Maybe.Empty<TSource>());
    }

    /// <summary>
    /// Filters the specified sequence of <see cref="System.Maybe{T}"/> objects where <see cref="System.Maybe{T}.HasValue"/> is <see langword="true" />.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <param name="source">The observable of <see cref="System.Maybe{T}"/> objects from which to extract values.</param>
    /// <returns>A sequence of values from the specified observable of <see cref="System.Maybe{T}"/> objects.</returns>
    public static IObservable<TSource> WhereHasValue<TSource>(this IObservable<Maybe<TSource>> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      return from value in source
             where value.HasValue
             select value.Value;
    }
  }
}