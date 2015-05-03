using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  public static partial class OperationalObservable
  {
    #region Public
    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="SByte"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [CLSCompliant(false)]
    public static OperationalObservable<sbyte, int> AsOperational(
      this IObservable<sbyte> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<sbyte, int>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="SByte"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<sbyte, int> AsOperational(
      this IObservable<sbyte> source,
      Func<IObservable<sbyte>, IObservable<sbyte>, Func<sbyte, sbyte, int>, IObservable<int>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<sbyte, int>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="SByte"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="resultBinaryOperation">The join behavior for binary operations on the resulting <see cref="OperationalObservable{TIn,TOut}"/>.</param>
    /// <param name="shiftOperation">The join behavior for bitwise shift operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<sbyte, int> AsOperational(
      this IObservable<sbyte> source,
      Func<IObservable<sbyte>, IObservable<sbyte>, Func<sbyte, sbyte, int>, IObservable<int>> binaryOperation = null,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> resultBinaryOperation = null,
      Func<IObservable<sbyte>, IObservable<int>, Func<sbyte, int, int>, IObservable<int>> shiftOperation = null,
      Func<IObservable<sbyte>, IObservable<sbyte>, Func<sbyte, sbyte, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<sbyte> comparer = null,
      IEqualityComparer<sbyte> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<sbyte, int>>(), null));

      return AsOperationalInternal(source, binaryOperation, resultBinaryOperation, shiftOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Byte"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    public static OperationalObservable<byte, int> AsOperational(
      this IObservable<byte> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<byte, int>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Byte"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<byte, int> AsOperational(
      this IObservable<byte> source,
      Func<IObservable<byte>, IObservable<byte>, Func<byte, byte, int>, IObservable<int>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<byte, int>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Byte"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="resultBinaryOperation">The join behavior for binary operations on the resulting <see cref="OperationalObservable{TIn,TOut}"/>.</param>
    /// <param name="shiftOperation">The join behavior for bitwise shift operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<byte, int> AsOperational(
      this IObservable<byte> source,
      Func<IObservable<byte>, IObservable<byte>, Func<byte, byte, int>, IObservable<int>> binaryOperation = null,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> resultBinaryOperation = null,
      Func<IObservable<byte>, IObservable<int>, Func<byte, int, int>, IObservable<int>> shiftOperation = null,
      Func<IObservable<byte>, IObservable<byte>, Func<byte, byte, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<byte> comparer = null,
      IEqualityComparer<byte> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<byte, int>>(), null));

      return AsOperationalInternal(source, binaryOperation, resultBinaryOperation, shiftOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Char"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    public static OperationalObservable<char, int> AsOperational(
      this IObservable<char> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<char, int>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Char"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<char, int> AsOperational(
      this IObservable<char> source,
      Func<IObservable<char>, IObservable<char>, Func<char, char, int>, IObservable<int>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<char, int>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Char"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="resultBinaryOperation">The join behavior for binary operations on the resulting <see cref="OperationalObservable{TIn,TOut}"/>.</param>
    /// <param name="shiftOperation">The join behavior for bitwise shift operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<char, int> AsOperational(
      this IObservable<char> source,
      Func<IObservable<char>, IObservable<char>, Func<char, char, int>, IObservable<int>> binaryOperation = null,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> resultBinaryOperation = null,
      Func<IObservable<char>, IObservable<int>, Func<char, int, int>, IObservable<int>> shiftOperation = null,
      Func<IObservable<char>, IObservable<char>, Func<char, char, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<char> comparer = null,
      IEqualityComparer<char> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<char, int>>(), null));

      return AsOperationalInternal(source, binaryOperation, resultBinaryOperation, shiftOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Int16"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    public static OperationalObservable<short, int> AsOperational(
      this IObservable<short> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<short, int>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Int16"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<short, int> AsOperational(
      this IObservable<short> source,
      Func<IObservable<short>, IObservable<short>, Func<short, short, int>, IObservable<int>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<short, int>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Int16"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="resultBinaryOperation">The join behavior for binary operations on the resulting <see cref="OperationalObservable{TIn,TOut}"/>.</param>
    /// <param name="shiftOperation">The join behavior for bitwise shift operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<short, int> AsOperational(
      this IObservable<short> source,
      Func<IObservable<short>, IObservable<short>, Func<short, short, int>, IObservable<int>> binaryOperation = null,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> resultBinaryOperation = null,
      Func<IObservable<short>, IObservable<int>, Func<short, int, int>, IObservable<int>> shiftOperation = null,
      Func<IObservable<short>, IObservable<short>, Func<short, short, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<short> comparer = null,
      IEqualityComparer<short> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<short, int>>(), null));

      return AsOperationalInternal(source, binaryOperation, resultBinaryOperation, shiftOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="UInt16"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [CLSCompliant(false)]
    public static OperationalObservable<ushort, int> AsOperational(
      this IObservable<ushort> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<ushort, int>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="UInt16"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<ushort, int> AsOperational(
      this IObservable<ushort> source,
      Func<IObservable<ushort>, IObservable<ushort>, Func<ushort, ushort, int>, IObservable<int>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<ushort, int>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="UInt16"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="resultBinaryOperation">The join behavior for binary operations on the resulting <see cref="OperationalObservable{TIn,TOut}"/>.</param>
    /// <param name="shiftOperation">The join behavior for bitwise shift operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<ushort, int> AsOperational(
      this IObservable<ushort> source,
      Func<IObservable<ushort>, IObservable<ushort>, Func<ushort, ushort, int>, IObservable<int>> binaryOperation = null,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> resultBinaryOperation = null,
      Func<IObservable<ushort>, IObservable<int>, Func<ushort, int, int>, IObservable<int>> shiftOperation = null,
      Func<IObservable<ushort>, IObservable<ushort>, Func<ushort, ushort, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<ushort> comparer = null,
      IEqualityComparer<ushort> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<ushort, int>>(), null));

      return AsOperationalInternal(source, binaryOperation, resultBinaryOperation, shiftOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Int32"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    public static OperationalObservable<int> AsOperational(
      this IObservable<int> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<int>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Int32"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<int> AsOperational(
      this IObservable<int> source,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<int>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Int32"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<int> AsOperational(
      this IObservable<int> source,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> binaryOperation = null,
      Func<IObservable<int>, IObservable<int>, Func<int, int, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<int> comparer = null,
      IEqualityComparer<int> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<int>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="UInt32"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [CLSCompliant(false)]
    public static OperationalObservable<uint> AsOperational(
      this IObservable<uint> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<uint>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="UInt32"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<uint> AsOperational(
      this IObservable<uint> source,
      Func<IObservable<uint>, IObservable<uint>, Func<uint, uint, uint>, IObservable<uint>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<uint>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="UInt32"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<uint> AsOperational(
      this IObservable<uint> source,
      Func<IObservable<uint>, IObservable<uint>, Func<uint, uint, uint>, IObservable<uint>> binaryOperation = null,
      Func<IObservable<uint>, IObservable<uint>, Func<uint, uint, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<uint> comparer = null,
      IEqualityComparer<uint> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<uint>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Int64"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    public static OperationalObservable<long> AsOperational(
      this IObservable<long> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<long>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Int64"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<long> AsOperational(
      this IObservable<long> source,
      Func<IObservable<long>, IObservable<long>, Func<long, long, long>, IObservable<long>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<long>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Int64"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<long> AsOperational(
      this IObservable<long> source,
      Func<IObservable<long>, IObservable<long>, Func<long, long, long>, IObservable<long>> binaryOperation = null,
      Func<IObservable<long>, IObservable<long>, Func<long, long, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<long> comparer = null,
      IEqualityComparer<long> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<long>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="UInt64"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [CLSCompliant(false)]
    public static OperationalObservable<ulong> AsOperational(
      this IObservable<ulong> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<ulong>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="UInt64"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<ulong> AsOperational(
      this IObservable<ulong> source,
      Func<IObservable<ulong>, IObservable<ulong>, Func<ulong, ulong, ulong>, IObservable<ulong>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<ulong>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="UInt64"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<ulong> AsOperational(
      this IObservable<ulong> source,
      Func<IObservable<ulong>, IObservable<ulong>, Func<ulong, ulong, ulong>, IObservable<ulong>> binaryOperation = null,
      Func<IObservable<ulong>, IObservable<ulong>, Func<ulong, ulong, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<ulong> comparer = null,
      IEqualityComparer<ulong> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<ulong>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Single"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    public static OperationalObservable<float> AsOperational(
      this IObservable<float> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<float>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Single"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<float> AsOperational(
      this IObservable<float> source,
      Func<IObservable<float>, IObservable<float>, Func<float, float, float>, IObservable<float>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<float>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Single"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<float> AsOperational(
      this IObservable<float> source,
      Func<IObservable<float>, IObservable<float>, Func<float, float, float>, IObservable<float>> binaryOperation = null,
      Func<IObservable<float>, IObservable<float>, Func<float, float, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<float> comparer = null,
      IEqualityComparer<float> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<float>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Double"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    public static OperationalObservable<double> AsOperational(
      this IObservable<double> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<double>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Double"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<double> AsOperational(
      this IObservable<double> source,
      Func<IObservable<double>, IObservable<double>, Func<double, double, double>, IObservable<double>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<double>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Double"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<double> AsOperational(
      this IObservable<double> source,
      Func<IObservable<double>, IObservable<double>, Func<double, double, double>, IObservable<double>> binaryOperation = null,
      Func<IObservable<double>, IObservable<double>, Func<double, double, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<double> comparer = null,
      IEqualityComparer<double> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<double>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Decimal"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    public static OperationalObservable<decimal> AsOperational(
      this IObservable<decimal> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<decimal>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Decimal"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<decimal> AsOperational(
      this IObservable<decimal> source,
      Func<IObservable<decimal>, IObservable<decimal>, Func<decimal, decimal, decimal>, IObservable<decimal>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<decimal>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="Decimal"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<decimal> AsOperational(
      this IObservable<decimal> source,
      Func<IObservable<decimal>, IObservable<decimal>, Func<decimal, decimal, decimal>, IObservable<decimal>> binaryOperation = null,
      Func<IObservable<decimal>, IObservable<decimal>, Func<decimal, decimal, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<decimal> comparer = null,
      IEqualityComparer<decimal> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<decimal>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="bool"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    public static OperationalObservable<bool> AsOperational(
      this IObservable<bool> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="bool"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<bool> AsOperational(
      this IObservable<bool> source,
      Func<IObservable<bool>, IObservable<bool>, Func<bool, bool, bool>, IObservable<bool>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return AsOperationalInternal(source, binaryOperation, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="bool"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<bool> AsOperational(
      this IObservable<bool> source,
      Func<IObservable<bool>, IObservable<bool>, Func<bool, bool, bool>, IObservable<bool>> binaryOperation = null,
      IComparer<bool> comparer = null,
      IEqualityComparer<bool> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return AsOperationalInternal(source, binaryOperation, binaryOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="string"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    public static OperationalObservable<string> AsOperational(
      this IObservable<string> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<string>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="string"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<string> AsOperational(
      this IObservable<string> source,
      Func<IObservable<string>, IObservable<string>, Func<string, string, string>, IObservable<string>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<string>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalObservable{TIn,TOut}"/> for the specified <see cref="string"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The observable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalObservable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another observable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalObservable<string> AsOperational(
      this IObservable<string> source,
      Func<IObservable<string>, IObservable<string>, Func<string, string, string>, IObservable<string>> binaryOperation = null,
      Func<IObservable<string>, IObservable<string>, Func<string, string, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<string> comparer = null,
      IEqualityComparer<string> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<string>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }
    #endregion

    #region Implementations
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<sbyte, int> AsOperationalInternal(
      IObservable<sbyte> source,
      Func<IObservable<sbyte>, IObservable<sbyte>, Func<sbyte, sbyte, int>, IObservable<int>> binaryOperation = null,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> resultBinaryOperation = null,
      Func<IObservable<sbyte>, IObservable<int>, Func<sbyte, int, int>, IObservable<int>> shiftOperation = null,
      Func<IObservable<sbyte>, IObservable<sbyte>, Func<sbyte, sbyte, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<sbyte> comparer = null,
      IEqualityComparer<sbyte> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<sbyte, int>>(), null));

      return source.AsOperational(
        result => AsOperationalInternal(result, resultBinaryOperation),
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        shiftOperation: shiftOperation,
        add: (first, second) => first + second,
        subtract: (first, second) => first - second,
        multiply: (first, second) => first * second,
        divide: (first, second) => first / second,
        remainder: (first, second) => first % second,
        leftShift: (first, second) => first << second,
        rightShift: (first, second) => first >> second,
        positive: value => +value,
        negative: value => -value,
        complement: value => ~value,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => comparer == null ? first < second : comparer.Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => comparer == null ? first <= second : comparer.Compare(first, second) <= 0,
        greaterThan: (first, second) => comparer == null ? first > second : comparer.Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => comparer == null ? first >= second : comparer.Compare(first, second) >= 0,
        and: (first, second) => first & second,
        or: (first, second) => first | second,
        xor: (first, second) => first ^ second);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<byte, int> AsOperationalInternal(
      IObservable<byte> source,
      Func<IObservable<byte>, IObservable<byte>, Func<byte, byte, int>, IObservable<int>> binaryOperation = null,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> resultBinaryOperation = null,
      Func<IObservable<byte>, IObservable<int>, Func<byte, int, int>, IObservable<int>> shiftOperation = null,
      Func<IObservable<byte>, IObservable<byte>, Func<byte, byte, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<byte> comparer = null,
      IEqualityComparer<byte> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<byte, int>>(), null));

      return source.AsOperational(
        result => AsOperationalInternal(result, resultBinaryOperation),
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        shiftOperation: shiftOperation,
        add: (first, second) => first + second,
        subtract: (first, second) => first - second,
        multiply: (first, second) => first * second,
        divide: (first, second) => first / second,
        remainder: (first, second) => first % second,
        leftShift: (first, second) => first << second,
        rightShift: (first, second) => first >> second,
        positive: value => +value,
        negative: value => -value,
        complement: value => ~value,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => comparer == null ? first < second : comparer.Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => comparer == null ? first <= second : comparer.Compare(first, second) <= 0,
        greaterThan: (first, second) => comparer == null ? first > second : comparer.Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => comparer == null ? first >= second : comparer.Compare(first, second) >= 0,
        and: (first, second) => first & second,
        or: (first, second) => first | second,
        xor: (first, second) => first ^ second);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<char, int> AsOperationalInternal(
      IObservable<char> source,
      Func<IObservable<char>, IObservable<char>, Func<char, char, int>, IObservable<int>> binaryOperation = null,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> resultBinaryOperation = null,
      Func<IObservable<char>, IObservable<int>, Func<char, int, int>, IObservable<int>> shiftOperation = null,
      Func<IObservable<char>, IObservable<char>, Func<char, char, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<char> comparer = null,
      IEqualityComparer<char> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<char, int>>(), null));

      return source.AsOperational(
        result => AsOperationalInternal(result, resultBinaryOperation),
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        shiftOperation: shiftOperation,
        add: (first, second) => first + second,
        subtract: (first, second) => first - second,
        multiply: (first, second) => first * second,
        divide: (first, second) => first / second,
        remainder: (first, second) => first % second,
        leftShift: (first, second) => first << second,
        rightShift: (first, second) => first >> second,
        positive: value => +value,
        negative: value => -value,
        complement: value => ~value,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => comparer == null ? first < second : comparer.Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => comparer == null ? first <= second : comparer.Compare(first, second) <= 0,
        greaterThan: (first, second) => comparer == null ? first > second : comparer.Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => comparer == null ? first >= second : comparer.Compare(first, second) >= 0,
        and: (first, second) => first & second,
        or: (first, second) => first | second,
        xor: (first, second) => first ^ second);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<short, int> AsOperationalInternal(
      IObservable<short> source,
      Func<IObservable<short>, IObservable<short>, Func<short, short, int>, IObservable<int>> binaryOperation = null,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> resultBinaryOperation = null,
      Func<IObservable<short>, IObservable<int>, Func<short, int, int>, IObservable<int>> shiftOperation = null,
      Func<IObservable<short>, IObservable<short>, Func<short, short, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<short> comparer = null,
      IEqualityComparer<short> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<short, int>>(), null));

      return source.AsOperational(
        result => AsOperationalInternal(result, resultBinaryOperation),
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        shiftOperation: shiftOperation,
        add: (first, second) => first + second,
        subtract: (first, second) => first - second,
        multiply: (first, second) => first * second,
        divide: (first, second) => first / second,
        remainder: (first, second) => first % second,
        leftShift: (first, second) => first << second,
        rightShift: (first, second) => first >> second,
        positive: value => +value,
        negative: value => -value,
        complement: value => ~value,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => comparer == null ? first < second : comparer.Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => comparer == null ? first <= second : comparer.Compare(first, second) <= 0,
        greaterThan: (first, second) => comparer == null ? first > second : comparer.Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => comparer == null ? first >= second : comparer.Compare(first, second) >= 0,
        and: (first, second) => first & second,
        or: (first, second) => first | second,
        xor: (first, second) => first ^ second);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<ushort, int> AsOperationalInternal(
      IObservable<ushort> source,
      Func<IObservable<ushort>, IObservable<ushort>, Func<ushort, ushort, int>, IObservable<int>> binaryOperation = null,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> resultBinaryOperation = null,
      Func<IObservable<ushort>, IObservable<int>, Func<ushort, int, int>, IObservable<int>> shiftOperation = null,
      Func<IObservable<ushort>, IObservable<ushort>, Func<ushort, ushort, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<ushort> comparer = null,
      IEqualityComparer<ushort> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<ushort, int>>(), null));

      return source.AsOperational(
        result => AsOperationalInternal(result, resultBinaryOperation),
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        shiftOperation: shiftOperation,
        add: (first, second) => first + second,
        subtract: (first, second) => first - second,
        multiply: (first, second) => first * second,
        divide: (first, second) => first / second,
        remainder: (first, second) => first % second,
        leftShift: (first, second) => first << second,
        rightShift: (first, second) => first >> second,
        positive: value => +value,
        negative: value => -value,
        complement: value => ~value,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => comparer == null ? first < second : comparer.Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => comparer == null ? first <= second : comparer.Compare(first, second) <= 0,
        greaterThan: (first, second) => comparer == null ? first > second : comparer.Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => comparer == null ? first >= second : comparer.Compare(first, second) >= 0,
        and: (first, second) => first & second,
        or: (first, second) => first | second,
        xor: (first, second) => first ^ second);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<int> AsOperationalInternal(
      IObservable<int> source,
      Func<IObservable<int>, IObservable<int>, Func<int, int, int>, IObservable<int>> binaryOperation = null,
      Func<IObservable<int>, IObservable<int>, Func<int, int, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<int> comparer = null,
      IEqualityComparer<int> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<int>>(), null));

      return source.AsOperational(
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        shiftOperation: binaryOperation,
        add: (first, second) => first + second,
        subtract: (first, second) => first - second,
        multiply: (first, second) => first * second,
        divide: (first, second) => first / second,
        remainder: (first, second) => first % second,
        leftShift: (first, second) => first << second,
        rightShift: (first, second) => first >> second,
        positive: value => +value,
        negative: value => -value,
        complement: value => ~value,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => comparer == null ? first < second : comparer.Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => comparer == null ? first <= second : comparer.Compare(first, second) <= 0,
        greaterThan: (first, second) => comparer == null ? first > second : comparer.Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => comparer == null ? first >= second : comparer.Compare(first, second) >= 0,
        and: (first, second) => first & second,
        or: (first, second) => first | second,
        xor: (first, second) => first ^ second);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<uint> AsOperationalInternal(
      IObservable<uint> source,
      Func<IObservable<uint>, IObservable<uint>, Func<uint, uint, uint>, IObservable<uint>> binaryOperation = null,
      Func<IObservable<uint>, IObservable<uint>, Func<uint, uint, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<uint> comparer = null,
      IEqualityComparer<uint> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<uint>>(), null));

      return source.AsOperational(
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        add: (first, second) => first + second,
        subtract: (first, second) => first - second,
        multiply: (first, second) => first * second,
        divide: (first, second) => first / second,
        remainder: (first, second) => first % second,
        leftShift: null,
        rightShift: null,
        positive: value => +value,
        negative: null,
        complement: value => ~value,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => comparer == null ? first < second : comparer.Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => comparer == null ? first <= second : comparer.Compare(first, second) <= 0,
        greaterThan: (first, second) => comparer == null ? first > second : comparer.Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => comparer == null ? first >= second : comparer.Compare(first, second) >= 0,
        and: (first, second) => first & second,
        or: (first, second) => first | second,
        xor: (first, second) => first ^ second);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<long> AsOperationalInternal(
      IObservable<long> source,
      Func<IObservable<long>, IObservable<long>, Func<long, long, long>, IObservable<long>> binaryOperation = null,
      Func<IObservable<long>, IObservable<long>, Func<long, long, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<long> comparer = null,
      IEqualityComparer<long> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<long>>(), null));

      return source.AsOperational(
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        add: (first, second) => first + second,
        subtract: (first, second) => first - second,
        multiply: (first, second) => first * second,
        divide: (first, second) => first / second,
        remainder: (first, second) => first % second,
        leftShift: null,
        rightShift: null,
        positive: value => +value,
        negative: value => -value,
        complement: value => ~value,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => comparer == null ? first < second : comparer.Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => comparer == null ? first <= second : comparer.Compare(first, second) <= 0,
        greaterThan: (first, second) => comparer == null ? first > second : comparer.Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => comparer == null ? first >= second : comparer.Compare(first, second) >= 0,
        and: (first, second) => first & second,
        or: (first, second) => first | second,
        xor: (first, second) => first ^ second);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<ulong> AsOperationalInternal(
      IObservable<ulong> source,
      Func<IObservable<ulong>, IObservable<ulong>, Func<ulong, ulong, ulong>, IObservable<ulong>> binaryOperation = null,
      Func<IObservable<ulong>, IObservable<ulong>, Func<ulong, ulong, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<ulong> comparer = null,
      IEqualityComparer<ulong> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<ulong>>(), null));

      return source.AsOperational(
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        add: (first, second) => first + second,
        subtract: (first, second) => first - second,
        multiply: (first, second) => first * second,
        divide: (first, second) => first / second,
        remainder: (first, second) => first % second,
        leftShift: null,
        rightShift: null,
        positive: value => +value,
        negative: null,
        complement: value => ~value,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => comparer == null ? first < second : comparer.Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => comparer == null ? first <= second : comparer.Compare(first, second) <= 0,
        greaterThan: (first, second) => comparer == null ? first > second : comparer.Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => comparer == null ? first >= second : comparer.Compare(first, second) >= 0,
        and: (first, second) => first & second,
        or: (first, second) => first | second,
        xor: (first, second) => first ^ second);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<float> AsOperationalInternal(
      IObservable<float> source,
      Func<IObservable<float>, IObservable<float>, Func<float, float, float>, IObservable<float>> binaryOperation = null,
      Func<IObservable<float>, IObservable<float>, Func<float, float, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<float> comparer = null,
      IEqualityComparer<float> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<float>>(), null));

      return source.AsOperational(
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        add: (first, second) => first + second,
        subtract: (first, second) => first - second,
        multiply: (first, second) => first * second,
        divide: (first, second) => first / second,
        remainder: (first, second) => first % second,
        leftShift: null,
        rightShift: null,
        positive: value => +value,
        negative: value => -value,
        complement: null,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => comparer == null ? first < second : comparer.Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => comparer == null ? first <= second : comparer.Compare(first, second) <= 0,
        greaterThan: (first, second) => comparer == null ? first > second : comparer.Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => comparer == null ? first >= second : comparer.Compare(first, second) >= 0,
        and: null,
        or: null,
        xor: null);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<double> AsOperationalInternal(
      IObservable<double> source,
      Func<IObservable<double>, IObservable<double>, Func<double, double, double>, IObservable<double>> binaryOperation = null,
      Func<IObservable<double>, IObservable<double>, Func<double, double, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<double> comparer = null,
      IEqualityComparer<double> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<double>>(), null));

      return source.AsOperational(
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        add: (first, second) => first + second,
        subtract: (first, second) => first - second,
        multiply: (first, second) => first * second,
        divide: (first, second) => first / second,
        remainder: (first, second) => first % second,
        leftShift: null,
        rightShift: null,
        positive: value => +value,
        negative: value => -value,
        complement: null,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => comparer == null ? first < second : comparer.Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => comparer == null ? first <= second : comparer.Compare(first, second) <= 0,
        greaterThan: (first, second) => comparer == null ? first > second : comparer.Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => comparer == null ? first >= second : comparer.Compare(first, second) >= 0,
        and: null,
        or: null,
        xor: null);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<decimal> AsOperationalInternal(
      IObservable<decimal> source,
      Func<IObservable<decimal>, IObservable<decimal>, Func<decimal, decimal, decimal>, IObservable<decimal>> binaryOperation = null,
      Func<IObservable<decimal>, IObservable<decimal>, Func<decimal, decimal, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<decimal> comparer = null,
      IEqualityComparer<decimal> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<decimal>>(), null));

      return source.AsOperational(
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        add: (first, second) => first + second,
        subtract: (first, second) => first - second,
        multiply: (first, second) => first * second,
        divide: (first, second) => first / second,
        remainder: (first, second) => first % second,
        leftShift: null,
        rightShift: null,
        positive: value => +value,
        negative: value => -value,
        complement: null,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => comparer == null ? first < second : comparer.Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => comparer == null ? first <= second : comparer.Compare(first, second) <= 0,
        greaterThan: (first, second) => comparer == null ? first > second : comparer.Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => comparer == null ? first >= second : comparer.Compare(first, second) >= 0,
        and: null,
        or: null,
        xor: null);
    }

    private static OperationalObservable<bool> AsOperationalInternal(
      IObservable<bool> source,
      Func<IObservable<bool>, IObservable<bool>, Func<bool, bool, bool>, IObservable<bool>> binaryOperation = null,
      Func<IObservable<bool>, IObservable<bool>, Func<bool, bool, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<bool> comparer = null,
      IEqualityComparer<bool> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return source.AsOperational(
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        add: null,
        subtract: null,
        multiply: null,
        divide: null,
        remainder: null,
        leftShift: null,
        rightShift: null,
        positive: null,
        negative: null,
        complement: null,
        not: value => !value,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => (comparer ?? Comparer<bool>.Default).Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => (comparer ?? Comparer<bool>.Default).Compare(first, second) <= 0,
        greaterThan: (first, second) => (comparer ?? Comparer<bool>.Default).Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => (comparer ?? Comparer<bool>.Default).Compare(first, second) >= 0,
        and: (first, second) => first & second,
        or: (first, second) => first | second,
        xor: (first, second) => first ^ second);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalObservable<string> AsOperationalInternal(
      IObservable<string> source,
      Func<IObservable<string>, IObservable<string>, Func<string, string, string>, IObservable<string>> binaryOperation = null,
      Func<IObservable<string>, IObservable<string>, Func<string, string, bool>, IObservable<bool>> comparisonOperation = null,
      IComparer<string> comparer = null,
      IEqualityComparer<string> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<string>>(), null));

      return source.AsOperational(
        binaryOperation: binaryOperation,
        comparisonOperation: comparisonOperation,
        add: (first, second) => first + second,
        subtract: null,
        multiply: null,
        divide: null,
        remainder: null,
        leftShift: null,
        rightShift: null,
        positive: null,
        negative: null,
        complement: null,
        not: null,
        equals: (first, second) => equalityComparer == null ? first == second : equalityComparer.Equals(first, second),
        notEquals: (first, second) => equalityComparer == null ? first != second : !equalityComparer.Equals(first, second),
        lessThan: (first, second) => (comparer ?? StringComparer.Ordinal).Compare(first, second) < 0,
        lessThanOrEqual: (first, second) => (comparer ?? StringComparer.Ordinal).Compare(first, second) <= 0,
        greaterThan: (first, second) => (comparer ?? StringComparer.Ordinal).Compare(first, second) > 0,
        greaterThanOrEqual: (first, second) => (comparer ?? StringComparer.Ordinal).Compare(first, second) >= 0,
        and: null,
        or: null,
        xor: null);
    }
    #endregion
  }
}