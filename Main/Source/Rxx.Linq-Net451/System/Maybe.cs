using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace System
{
  /// <summary>
  /// Provides <see langword="static"/> methods that construct instances of <see cref="Maybe{T}"/> and extension methods that extract its value.
  /// </summary>
  public static class Maybe
  {
    /// <summary>
    /// Gets a <see cref="Maybe{T}"/> that represents a missing instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <returns>A <see cref="Maybe{T}"/> with <see cref="Maybe{T}.HasValue"/> set to <see langword="false" />.</returns>
    [SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Static Empty field has no explicit contracts.")]
    public static Maybe<T> Empty<T>()
    {
      Contract.Ensures(!Contract.Result<Maybe<T>>().HasValue);

      return Maybe<T>.Empty;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Maybe{T}" /> with the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">Type of <paramref name="value"/></typeparam>
    /// <param name="value">The value assigned to the <see cref="Maybe{T}.Value"/> property.</param>
    /// <returns>A new instance of <see cref="Maybe{T}"/> with the specified <paramref name="value"/> and 
    /// <see cref="Maybe{T}.HasValue"/> set to <see langword="true" />.</returns>
    public static Maybe<T> Return<T>(T value)
    {
      Contract.Ensures(Contract.Result<Maybe<T>>().HasValue);
      Contract.Ensures(object.Equals(Contract.Result<Maybe<T>>().Value, value));

      var maybe = new Maybe<T>(value);

      Contract.Assert(maybe.HasValue);

      return maybe;
    }

    /// <summary>
    /// Gets the value of the <see cref="Maybe{T}"/> structure if present when the specified asynchronous operation completes; otherwise, returns the default value of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type that may or may not be present when the asynchronous operation completes.</typeparam>
    /// <param name="task">The <see cref="Task{T}"/> representing an asynchronous operation that returns a <see cref="Maybe{T}"/> from which to extract the value.</param>
    /// <returns>A <see cref="Task{T}"/> containing the value of the <see cref="Maybe{T}"/> structure if present when <paramref name="task"/> completes; otherwise, the default value of <typeparamref name="T"/>.</returns>
    public static Task<T> ValueOrDefaultAsync<T>(this Task<Maybe<T>> task)
    {
      Contract.Requires(task != null);

      return task.ValueOrDefaultAsync(default(T));
    }

    /// <summary>
    /// Gets the value of the <see cref="Maybe{T}"/> structure if present when the specified asynchronous operation completes; otherwise, returns <paramref name="defaultValue"/>.
    /// </summary>
    /// <typeparam name="T">Type that may or may not be present when the asynchronous operation completes.</typeparam>
    /// <param name="task">The <see cref="Task{T}"/> representing an asynchronous operation that returns a <see cref="Maybe{T}"/> from which to extract the value.</param>
    /// <param name="defaultValue">The value to be returned when <paramref name="task"/> completes without a value.</param>
    /// <returns>A <see cref="Task{T}"/> containing the value of the <see cref="Maybe{T}"/> structure if present when <paramref name="task"/> completes; otherwise, <paramref name="defaultValue"/>.</returns>
    public static async Task<T> ValueOrDefaultAsync<T>(this Task<Maybe<T>> task, T defaultValue)
    {
      Contract.Requires(task != null);

      return (await task.ConfigureAwait(false)).ValueOrDefault(defaultValue);
    }
  }
}