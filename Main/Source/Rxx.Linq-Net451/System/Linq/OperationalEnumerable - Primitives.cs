using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System.Linq
{
  public static partial class OperationalEnumerable
  {
    #region Public
    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="SByte"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [CLSCompliant(false)]
    public static OperationalEnumerable<sbyte, int> AsOperational(
      this IEnumerable<sbyte> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<sbyte, int>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="SByte"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<sbyte, int> AsOperational(
      this IEnumerable<sbyte> source,
      Func<IEnumerable<sbyte>, IEnumerable<sbyte>, Func<sbyte, sbyte, int>, IEnumerable<int>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<sbyte, int>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="SByte"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="resultBinaryOperation">The join behavior for binary operations on the resulting <see cref="OperationalEnumerable{TIn,TOut}"/>.</param>
    /// <param name="shiftOperation">The join behavior for bitwise shift operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<sbyte, int> AsOperational(
      this IEnumerable<sbyte> source,
      Func<IEnumerable<sbyte>, IEnumerable<sbyte>, Func<sbyte, sbyte, int>, IEnumerable<int>> binaryOperation = null,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> resultBinaryOperation = null,
      Func<IEnumerable<sbyte>, IEnumerable<int>, Func<sbyte, int, int>, IEnumerable<int>> shiftOperation = null,
      Func<IEnumerable<sbyte>, IEnumerable<sbyte>, Func<sbyte, sbyte, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<sbyte> comparer = null,
      IEqualityComparer<sbyte> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<sbyte, int>>(), null));

      return AsOperationalInternal(source, binaryOperation, resultBinaryOperation, shiftOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Byte"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    public static OperationalEnumerable<byte, int> AsOperational(
      this IEnumerable<byte> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<byte, int>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Byte"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<byte, int> AsOperational(
      this IEnumerable<byte> source,
      Func<IEnumerable<byte>, IEnumerable<byte>, Func<byte, byte, int>, IEnumerable<int>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<byte, int>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Byte"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="resultBinaryOperation">The join behavior for binary operations on the resulting <see cref="OperationalEnumerable{TIn,TOut}"/>.</param>
    /// <param name="shiftOperation">The join behavior for bitwise shift operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<byte, int> AsOperational(
      this IEnumerable<byte> source,
      Func<IEnumerable<byte>, IEnumerable<byte>, Func<byte, byte, int>, IEnumerable<int>> binaryOperation = null,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> resultBinaryOperation = null,
      Func<IEnumerable<byte>, IEnumerable<int>, Func<byte, int, int>, IEnumerable<int>> shiftOperation = null,
      Func<IEnumerable<byte>, IEnumerable<byte>, Func<byte, byte, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<byte> comparer = null,
      IEqualityComparer<byte> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<byte, int>>(), null));

      return AsOperationalInternal(source, binaryOperation, resultBinaryOperation, shiftOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Char"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    public static OperationalEnumerable<char, int> AsOperational(
      this IEnumerable<char> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<char, int>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Char"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<char, int> AsOperational(
      this IEnumerable<char> source,
      Func<IEnumerable<char>, IEnumerable<char>, Func<char, char, int>, IEnumerable<int>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<char, int>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Char"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="resultBinaryOperation">The join behavior for binary operations on the resulting <see cref="OperationalEnumerable{TIn,TOut}"/>.</param>
    /// <param name="shiftOperation">The join behavior for bitwise shift operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<char, int> AsOperational(
      this IEnumerable<char> source,
      Func<IEnumerable<char>, IEnumerable<char>, Func<char, char, int>, IEnumerable<int>> binaryOperation = null,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> resultBinaryOperation = null,
      Func<IEnumerable<char>, IEnumerable<int>, Func<char, int, int>, IEnumerable<int>> shiftOperation = null,
      Func<IEnumerable<char>, IEnumerable<char>, Func<char, char, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<char> comparer = null,
      IEqualityComparer<char> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<char, int>>(), null));

      return AsOperationalInternal(source, binaryOperation, resultBinaryOperation, shiftOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Int16"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    public static OperationalEnumerable<short, int> AsOperational(
      this IEnumerable<short> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<short, int>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Int16"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<short, int> AsOperational(
      this IEnumerable<short> source,
      Func<IEnumerable<short>, IEnumerable<short>, Func<short, short, int>, IEnumerable<int>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<short, int>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Int16"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="resultBinaryOperation">The join behavior for binary operations on the resulting <see cref="OperationalEnumerable{TIn,TOut}"/>.</param>
    /// <param name="shiftOperation">The join behavior for bitwise shift operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<short, int> AsOperational(
      this IEnumerable<short> source,
      Func<IEnumerable<short>, IEnumerable<short>, Func<short, short, int>, IEnumerable<int>> binaryOperation = null,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> resultBinaryOperation = null,
      Func<IEnumerable<short>, IEnumerable<int>, Func<short, int, int>, IEnumerable<int>> shiftOperation = null,
      Func<IEnumerable<short>, IEnumerable<short>, Func<short, short, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<short> comparer = null,
      IEqualityComparer<short> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<short, int>>(), null));

      return AsOperationalInternal(source, binaryOperation, resultBinaryOperation, shiftOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="UInt16"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [CLSCompliant(false)]
    public static OperationalEnumerable<ushort, int> AsOperational(
      this IEnumerable<ushort> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<ushort, int>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="UInt16"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<ushort, int> AsOperational(
      this IEnumerable<ushort> source,
      Func<IEnumerable<ushort>, IEnumerable<ushort>, Func<ushort, ushort, int>, IEnumerable<int>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<ushort, int>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="UInt16"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="resultBinaryOperation">The join behavior for binary operations on the resulting <see cref="OperationalEnumerable{TIn,TOut}"/>.</param>
    /// <param name="shiftOperation">The join behavior for bitwise shift operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<ushort, int> AsOperational(
      this IEnumerable<ushort> source,
      Func<IEnumerable<ushort>, IEnumerable<ushort>, Func<ushort, ushort, int>, IEnumerable<int>> binaryOperation = null,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> resultBinaryOperation = null,
      Func<IEnumerable<ushort>, IEnumerable<int>, Func<ushort, int, int>, IEnumerable<int>> shiftOperation = null,
      Func<IEnumerable<ushort>, IEnumerable<ushort>, Func<ushort, ushort, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<ushort> comparer = null,
      IEqualityComparer<ushort> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<ushort, int>>(), null));

      return AsOperationalInternal(source, binaryOperation, resultBinaryOperation, shiftOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Int32"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    public static OperationalEnumerable<int> AsOperational(
      this IEnumerable<int> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<int>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Int32"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<int> AsOperational(
      this IEnumerable<int> source,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<int>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Int32"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<int> AsOperational(
      this IEnumerable<int> source,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> binaryOperation = null,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<int> comparer = null,
      IEqualityComparer<int> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<int>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="UInt32"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [CLSCompliant(false)]
    public static OperationalEnumerable<uint> AsOperational(
      this IEnumerable<uint> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<uint>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="UInt32"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<uint> AsOperational(
      this IEnumerable<uint> source,
      Func<IEnumerable<uint>, IEnumerable<uint>, Func<uint, uint, uint>, IEnumerable<uint>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<uint>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="UInt32"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<uint> AsOperational(
      this IEnumerable<uint> source,
      Func<IEnumerable<uint>, IEnumerable<uint>, Func<uint, uint, uint>, IEnumerable<uint>> binaryOperation = null,
      Func<IEnumerable<uint>, IEnumerable<uint>, Func<uint, uint, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<uint> comparer = null,
      IEqualityComparer<uint> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<uint>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Int64"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    public static OperationalEnumerable<long> AsOperational(
      this IEnumerable<long> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<long>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Int64"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<long> AsOperational(
      this IEnumerable<long> source,
      Func<IEnumerable<long>, IEnumerable<long>, Func<long, long, long>, IEnumerable<long>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<long>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Int64"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<long> AsOperational(
      this IEnumerable<long> source,
      Func<IEnumerable<long>, IEnumerable<long>, Func<long, long, long>, IEnumerable<long>> binaryOperation = null,
      Func<IEnumerable<long>, IEnumerable<long>, Func<long, long, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<long> comparer = null,
      IEqualityComparer<long> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<long>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="UInt64"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [CLSCompliant(false)]
    public static OperationalEnumerable<ulong> AsOperational(
      this IEnumerable<ulong> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<ulong>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="UInt64"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<ulong> AsOperational(
      this IEnumerable<ulong> source,
      Func<IEnumerable<ulong>, IEnumerable<ulong>, Func<ulong, ulong, ulong>, IEnumerable<ulong>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<ulong>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="UInt64"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<ulong> AsOperational(
      this IEnumerable<ulong> source,
      Func<IEnumerable<ulong>, IEnumerable<ulong>, Func<ulong, ulong, ulong>, IEnumerable<ulong>> binaryOperation = null,
      Func<IEnumerable<ulong>, IEnumerable<ulong>, Func<ulong, ulong, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<ulong> comparer = null,
      IEqualityComparer<ulong> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<ulong>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Single"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    public static OperationalEnumerable<float> AsOperational(
      this IEnumerable<float> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<float>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Single"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<float> AsOperational(
      this IEnumerable<float> source,
      Func<IEnumerable<float>, IEnumerable<float>, Func<float, float, float>, IEnumerable<float>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<float>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Single"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<float> AsOperational(
      this IEnumerable<float> source,
      Func<IEnumerable<float>, IEnumerable<float>, Func<float, float, float>, IEnumerable<float>> binaryOperation = null,
      Func<IEnumerable<float>, IEnumerable<float>, Func<float, float, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<float> comparer = null,
      IEqualityComparer<float> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<float>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Double"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    public static OperationalEnumerable<double> AsOperational(
      this IEnumerable<double> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<double>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Double"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<double> AsOperational(
      this IEnumerable<double> source,
      Func<IEnumerable<double>, IEnumerable<double>, Func<double, double, double>, IEnumerable<double>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<double>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Double"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<double> AsOperational(
      this IEnumerable<double> source,
      Func<IEnumerable<double>, IEnumerable<double>, Func<double, double, double>, IEnumerable<double>> binaryOperation = null,
      Func<IEnumerable<double>, IEnumerable<double>, Func<double, double, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<double> comparer = null,
      IEqualityComparer<double> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<double>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Decimal"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    public static OperationalEnumerable<decimal> AsOperational(
      this IEnumerable<decimal> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<decimal>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Decimal"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<decimal> AsOperational(
      this IEnumerable<decimal> source,
      Func<IEnumerable<decimal>, IEnumerable<decimal>, Func<decimal, decimal, decimal>, IEnumerable<decimal>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<decimal>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="Decimal"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<decimal> AsOperational(
      this IEnumerable<decimal> source,
      Func<IEnumerable<decimal>, IEnumerable<decimal>, Func<decimal, decimal, decimal>, IEnumerable<decimal>> binaryOperation = null,
      Func<IEnumerable<decimal>, IEnumerable<decimal>, Func<decimal, decimal, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<decimal> comparer = null,
      IEqualityComparer<decimal> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<decimal>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="bool"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    public static OperationalEnumerable<bool> AsOperational(
      this IEnumerable<bool> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="bool"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<bool> AsOperational(
      this IEnumerable<bool> source,
      Func<IEnumerable<bool>, IEnumerable<bool>, Func<bool, bool, bool>, IEnumerable<bool>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return AsOperationalInternal(source, binaryOperation, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="bool"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<bool> AsOperational(
      this IEnumerable<bool> source,
      Func<IEnumerable<bool>, IEnumerable<bool>, Func<bool, bool, bool>, IEnumerable<bool>> binaryOperation = null,
      IComparer<bool> comparer = null,
      IEqualityComparer<bool> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return AsOperationalInternal(source, binaryOperation, binaryOperation, comparer, equalityComparer);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="string"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    public static OperationalEnumerable<string> AsOperational(
      this IEnumerable<string> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<string>>(), null));

      return AsOperationalInternal(source);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="string"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<string> AsOperational(
      this IEnumerable<string> source,
      Func<IEnumerable<string>, IEnumerable<string>, Func<string, string, string>, IEnumerable<string>> binaryOperation = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<string>>(), null));

      return AsOperationalInternal(source, binaryOperation);
    }

    /// <summary>
    /// Creates a standard <see cref="OperationalEnumerable{TIn,TOut}"/> for the specified <see cref="string"/> <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The enumerable to be converted.</param>
    /// <param name="binaryOperation">The join behavior for binary operations.</param>
    /// <param name="comparisonOperation">The join behavior for binary comparison operations.</param>
    /// <param name="comparer">An object that compares values for comparison operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <param name="equalityComparer">An object that determines the equality of values for equality operations.  Specify <see langword="null"/> to use the default comparer.</param>
    /// <returns>An <see cref="OperationalEnumerable{TIn,TOut}"/> that applies the specified operations to the specified <paramref name="source"/> 
    /// when combined with another enumerable.</returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
      Justification = "In this case, optional parameters are more flexible than defining only a subset of all possible combinations.")]
    public static OperationalEnumerable<string> AsOperational(
      this IEnumerable<string> source,
      Func<IEnumerable<string>, IEnumerable<string>, Func<string, string, string>, IEnumerable<string>> binaryOperation = null,
      Func<IEnumerable<string>, IEnumerable<string>, Func<string, string, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<string> comparer = null,
      IEqualityComparer<string> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<string>>(), null));

      return AsOperationalInternal(source, binaryOperation, comparisonOperation, comparer, equalityComparer);
    }
    #endregion

    #region Implementations
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There aren't any simpler alternatives.")]
    private static OperationalEnumerable<sbyte, int> AsOperationalInternal(
      IEnumerable<sbyte> source,
      Func<IEnumerable<sbyte>, IEnumerable<sbyte>, Func<sbyte, sbyte, int>, IEnumerable<int>> binaryOperation = null,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> resultBinaryOperation = null,
      Func<IEnumerable<sbyte>, IEnumerable<int>, Func<sbyte, int, int>, IEnumerable<int>> shiftOperation = null,
      Func<IEnumerable<sbyte>, IEnumerable<sbyte>, Func<sbyte, sbyte, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<sbyte> comparer = null,
      IEqualityComparer<sbyte> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<sbyte, int>>(), null));

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
    private static OperationalEnumerable<byte, int> AsOperationalInternal(
      IEnumerable<byte> source,
      Func<IEnumerable<byte>, IEnumerable<byte>, Func<byte, byte, int>, IEnumerable<int>> binaryOperation = null,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> resultBinaryOperation = null,
      Func<IEnumerable<byte>, IEnumerable<int>, Func<byte, int, int>, IEnumerable<int>> shiftOperation = null,
      Func<IEnumerable<byte>, IEnumerable<byte>, Func<byte, byte, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<byte> comparer = null,
      IEqualityComparer<byte> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<byte, int>>(), null));

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
    private static OperationalEnumerable<char, int> AsOperationalInternal(
      IEnumerable<char> source,
      Func<IEnumerable<char>, IEnumerable<char>, Func<char, char, int>, IEnumerable<int>> binaryOperation = null,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> resultBinaryOperation = null,
      Func<IEnumerable<char>, IEnumerable<int>, Func<char, int, int>, IEnumerable<int>> shiftOperation = null,
      Func<IEnumerable<char>, IEnumerable<char>, Func<char, char, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<char> comparer = null,
      IEqualityComparer<char> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<char, int>>(), null));

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
    private static OperationalEnumerable<short, int> AsOperationalInternal(
      IEnumerable<short> source,
      Func<IEnumerable<short>, IEnumerable<short>, Func<short, short, int>, IEnumerable<int>> binaryOperation = null,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> resultBinaryOperation = null,
      Func<IEnumerable<short>, IEnumerable<int>, Func<short, int, int>, IEnumerable<int>> shiftOperation = null,
      Func<IEnumerable<short>, IEnumerable<short>, Func<short, short, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<short> comparer = null,
      IEqualityComparer<short> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<short, int>>(), null));

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
    private static OperationalEnumerable<ushort, int> AsOperationalInternal(
      IEnumerable<ushort> source,
      Func<IEnumerable<ushort>, IEnumerable<ushort>, Func<ushort, ushort, int>, IEnumerable<int>> binaryOperation = null,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> resultBinaryOperation = null,
      Func<IEnumerable<ushort>, IEnumerable<int>, Func<ushort, int, int>, IEnumerable<int>> shiftOperation = null,
      Func<IEnumerable<ushort>, IEnumerable<ushort>, Func<ushort, ushort, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<ushort> comparer = null,
      IEqualityComparer<ushort> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<ushort, int>>(), null));

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
    private static OperationalEnumerable<int> AsOperationalInternal(
      IEnumerable<int> source,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, int>, IEnumerable<int>> binaryOperation = null,
      Func<IEnumerable<int>, IEnumerable<int>, Func<int, int, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<int> comparer = null,
      IEqualityComparer<int> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<int>>(), null));

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
    private static OperationalEnumerable<uint> AsOperationalInternal(
      IEnumerable<uint> source,
      Func<IEnumerable<uint>, IEnumerable<uint>, Func<uint, uint, uint>, IEnumerable<uint>> binaryOperation = null,
      Func<IEnumerable<uint>, IEnumerable<uint>, Func<uint, uint, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<uint> comparer = null,
      IEqualityComparer<uint> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<uint>>(), null));

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
    private static OperationalEnumerable<long> AsOperationalInternal(
      IEnumerable<long> source,
      Func<IEnumerable<long>, IEnumerable<long>, Func<long, long, long>, IEnumerable<long>> binaryOperation = null,
      Func<IEnumerable<long>, IEnumerable<long>, Func<long, long, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<long> comparer = null,
      IEqualityComparer<long> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<long>>(), null));

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
    private static OperationalEnumerable<ulong> AsOperationalInternal(
      IEnumerable<ulong> source,
      Func<IEnumerable<ulong>, IEnumerable<ulong>, Func<ulong, ulong, ulong>, IEnumerable<ulong>> binaryOperation = null,
      Func<IEnumerable<ulong>, IEnumerable<ulong>, Func<ulong, ulong, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<ulong> comparer = null,
      IEqualityComparer<ulong> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<ulong>>(), null));

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
    private static OperationalEnumerable<float> AsOperationalInternal(
      IEnumerable<float> source,
      Func<IEnumerable<float>, IEnumerable<float>, Func<float, float, float>, IEnumerable<float>> binaryOperation = null,
      Func<IEnumerable<float>, IEnumerable<float>, Func<float, float, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<float> comparer = null,
      IEqualityComparer<float> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<float>>(), null));

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
    private static OperationalEnumerable<double> AsOperationalInternal(
      IEnumerable<double> source,
      Func<IEnumerable<double>, IEnumerable<double>, Func<double, double, double>, IEnumerable<double>> binaryOperation = null,
      Func<IEnumerable<double>, IEnumerable<double>, Func<double, double, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<double> comparer = null,
      IEqualityComparer<double> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<double>>(), null));

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
    private static OperationalEnumerable<decimal> AsOperationalInternal(
      IEnumerable<decimal> source,
      Func<IEnumerable<decimal>, IEnumerable<decimal>, Func<decimal, decimal, decimal>, IEnumerable<decimal>> binaryOperation = null,
      Func<IEnumerable<decimal>, IEnumerable<decimal>, Func<decimal, decimal, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<decimal> comparer = null,
      IEqualityComparer<decimal> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<decimal>>(), null));

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

    private static OperationalEnumerable<bool> AsOperationalInternal(
      IEnumerable<bool> source,
      Func<IEnumerable<bool>, IEnumerable<bool>, Func<bool, bool, bool>, IEnumerable<bool>> binaryOperation = null,
      Func<IEnumerable<bool>, IEnumerable<bool>, Func<bool, bool, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<bool> comparer = null,
      IEqualityComparer<bool> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

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
    private static OperationalEnumerable<string> AsOperationalInternal(
      IEnumerable<string> source,
      Func<IEnumerable<string>, IEnumerable<string>, Func<string, string, string>, IEnumerable<string>> binaryOperation = null,
      Func<IEnumerable<string>, IEnumerable<string>, Func<string, string, bool>, IEnumerable<bool>> comparisonOperation = null,
      IComparer<string> comparer = null,
      IEqualityComparer<string> equalityComparer = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<string>>(), null));

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