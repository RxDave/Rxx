using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Collections.Generic
{
#pragma warning disable 0660, 0661
  /// <summary>
  /// Represents an enumerable that uses its unary and binary operator overloads as query operators.
  /// </summary>
  /// <typeparam name="TIn">Input type.</typeparam>
  /// <typeparam name="TOut">Output type.</typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification = "This class is not intended to be consumed publicly.  It's public only so that the compiler can resolve operator overloads.")]
  [SuppressMessage("Microsoft.Usage", "CA2224:OverrideEqualsOnOverloadingOperatorEquals",
    Justification = "Reference equality is preferred.  The == and != operator overloads apply to the elements within the sequence.")]
  [SuppressMessage("Microsoft.Design", "CA1046:DoNotOverloadOperatorEqualsOnReferenceTypes",
    Justification = "The == and != operator overloads apply to the elements within the sequence.")]
  public class OperationalEnumerable<TIn, TOut> : IEnumerable<TIn>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private readonly IEnumerable<TIn> source;
    private readonly Func<IEnumerable<TOut>, OperationalEnumerable<TOut>> resultSelector;
    private readonly Func<IEnumerable<TIn>, IEnumerable<TIn>, Func<TIn, TIn, TOut>, IEnumerable<TOut>> binaryOperation;
    private readonly Func<IEnumerable<TIn>, IEnumerable<TIn>, Func<TIn, TIn, bool>, IEnumerable<bool>> comparisonOperation;
    private readonly Func<IEnumerable<TIn>, IEnumerable<int>, Func<TIn, int, TOut>, IEnumerable<TOut>> shiftOperation;
    private readonly Func<TIn, TIn, TOut> add, subtract, multiply, divide, remainder;
    private readonly Func<TIn, int, TOut> leftShift, rightShift;
    private readonly Func<TIn, TIn, TOut> and, or, xor;
    private readonly Func<TIn, TOut> positive, negative, complement;
    private readonly Func<TIn, bool> not;
    private readonly Func<TIn, TIn, bool> equals, notEquals, lessThan, lessThanOrEqual, greaterThan, greaterThanOrEqual;
    #endregion

    #region Constructors
    internal OperationalEnumerable(
      IEnumerable<TIn> source,
      Func<IEnumerable<TOut>, OperationalEnumerable<TOut>> resultSelector,
      Func<IEnumerable<TIn>, IEnumerable<TIn>, Func<TIn, TIn, TOut>, IEnumerable<TOut>> binaryOperation,
      Func<IEnumerable<TIn>, IEnumerable<TIn>, Func<TIn, TIn, bool>, IEnumerable<bool>> comparisonOperation,
      Func<IEnumerable<TIn>, IEnumerable<int>, Func<TIn, int, TOut>, IEnumerable<TOut>> shiftOperation,
      Func<TIn, TIn, TOut> add,
      Func<TIn, TIn, TOut> subtract,
      Func<TIn, TIn, TOut> multiply,
      Func<TIn, TIn, TOut> divide,
      Func<TIn, TIn, TOut> remainder,
      Func<TIn, int, TOut> leftShift,
      Func<TIn, int, TOut> rightShift,
      Func<TIn, TOut> positive,
      Func<TIn, TOut> negative,
      Func<TIn, TOut> complement,
      Func<TIn, bool> not,
      Func<TIn, TIn, bool> equals,
      Func<TIn, TIn, bool> notEquals,
      Func<TIn, TIn, bool> lessThan,
      Func<TIn, TIn, bool> lessThanOrEqual,
      Func<TIn, TIn, bool> greaterThan,
      Func<TIn, TIn, bool> greaterThanOrEqual,
      Func<TIn, TIn, TOut> and,
      Func<TIn, TIn, TOut> or,
      Func<TIn, TIn, TOut> xor)
    {
      Contract.Requires(source != null);
      Contract.Requires(resultSelector != null);

      this.source = source;
      this.resultSelector = resultSelector;

      this.binaryOperation = binaryOperation ?? DefaultBinaryOperation;
      this.comparisonOperation = comparisonOperation ?? DefaultBinaryOperation;
      this.shiftOperation = shiftOperation ?? DefaultBinaryOperation;

      this.add = add ?? UnsupportedBinaryOperation;
      this.subtract = subtract ?? UnsupportedBinaryOperation;
      this.multiply = multiply ?? UnsupportedBinaryOperation;
      this.divide = divide ?? UnsupportedBinaryOperation;
      this.remainder = remainder ?? UnsupportedBinaryOperation;
      this.leftShift = leftShift ?? UnsupportedShiftOperation;
      this.rightShift = rightShift ?? UnsupportedShiftOperation;

      this.positive = positive ?? UnsupportedUnaryOperation;
      this.negative = negative ?? UnsupportedUnaryOperation;
      this.complement = complement ?? UnsupportedUnaryOperation;
      this.not = not ?? UnsupportedUnaryBooleanOperation;

      this.equals = equals ?? DefaultEqualsOperation;
      this.notEquals = notEquals ?? DefaultNotEqualsOperation;
      this.lessThan = lessThan ?? UnsupportedComparisonOperation;
      this.lessThanOrEqual = lessThanOrEqual ?? UnsupportedComparisonOperation;
      this.greaterThan = greaterThan ?? UnsupportedComparisonOperation;
      this.greaterThanOrEqual = greaterThanOrEqual ?? UnsupportedComparisonOperation;

      this.and = and ?? UnsupportedBinaryOperation;
      this.or = or ?? UnsupportedBinaryOperation;
      this.xor = xor ?? UnsupportedBinaryOperation;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(source != null);
      Contract.Invariant(resultSelector != null);
      Contract.Invariant(binaryOperation != null);
      Contract.Invariant(comparisonOperation != null);
      Contract.Invariant(shiftOperation != null);
      Contract.Invariant(add != null);
      Contract.Invariant(subtract != null);
      Contract.Invariant(multiply != null);
      Contract.Invariant(divide != null);
      Contract.Invariant(remainder != null);
      Contract.Invariant(leftShift != null);
      Contract.Invariant(rightShift != null);
      Contract.Invariant(positive != null);
      Contract.Invariant(negative != null);
      Contract.Invariant(complement != null);
      Contract.Invariant(not != null);
      Contract.Invariant(equals != null);
      Contract.Invariant(notEquals != null);
      Contract.Invariant(lessThan != null);
      Contract.Invariant(lessThanOrEqual != null);
      Contract.Invariant(greaterThan != null);
      Contract.Invariant(greaterThanOrEqual != null);
      Contract.Invariant(and != null);
      Contract.Invariant(or != null);
      Contract.Invariant(xor != null);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
    public IEnumerator<TIn> GetEnumerator()
    {
      return source.GetEnumerator();
    }

    IEnumerator Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    [ContractVerification(false)]
    private static IEnumerable<TResult> DefaultBinaryOperation<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> operation)
    {
      Contract.Requires(first != null);
      Contract.Requires(second != null);
      Contract.Requires(operation != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      return first.Zip(second, operation);
    }

    private static bool DefaultEqualsOperation(TIn first, TIn second)
    {
      return EqualityComparer<TIn>.Default.Equals(first, second);
    }

    private static bool DefaultNotEqualsOperation(TIn first, TIn second)
    {
      return !EqualityComparer<TIn>.Default.Equals(first, second);
    }

    private static TOut UnsupportedBinaryOperation(TIn first, TIn second)
    {
      throw new NotSupportedException();
    }

    private static TOut UnsupportedUnaryOperation(TIn value)
    {
      throw new NotSupportedException();
    }

    private static bool UnsupportedUnaryBooleanOperation(TIn value)
    {
      throw new NotSupportedException();
    }

    private static bool UnsupportedComparisonOperation(TIn first, TIn second)
    {
      throw new NotSupportedException();
    }

    private static TOut UnsupportedShiftOperation(TIn first, int second)
    {
      throw new NotSupportedException();
    }

    private OperationalEnumerable<TOut> BinaryOperation(IEnumerable<TIn> second, Func<TIn, TIn, TOut> operation)
    {
      Contract.Requires(second != null);
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      var result = resultSelector(binaryOperation(this, second, operation));

      Contract.Assume(!object.Equals(result, null));

      return result;
    }

    private OperationalEnumerable<TOut> BinaryOperation(TIn second, Func<TIn, TIn, TOut> operation)
    {
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      var result = resultSelector(this.Select(first => operation(first, second)));

      Contract.Assume(!object.Equals(result, null));

      return result;
    }

    private OperationalEnumerable<TOut> ShiftOperation(IEnumerable<int> second, Func<TIn, int, TOut> operation)
    {
      Contract.Requires(second != null);
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      var result = resultSelector(shiftOperation(this, second, operation));

      Contract.Assume(!object.Equals(result, null));

      return result;
    }

    private OperationalEnumerable<TOut> ShiftOperation(int second, Func<TIn, int, TOut> operation)
    {
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      var result = resultSelector(this.Select(first => operation(first, second)));

      Contract.Assume(!object.Equals(result, null));

      return result;
    }

    private OperationalEnumerable<TOut> UnaryOperation(Func<TIn, TOut> operation)
    {
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      var result = resultSelector(this.Select(operation));

      Contract.Assume(!object.Equals(result, null));

      return result;
    }

    private OperationalEnumerable<bool> UnaryBooleanOperation(Func<TIn, bool> operation)
    {
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return this.Select(operation).AsOperational();
    }

    private OperationalEnumerable<bool> ComparisonOperation(IEnumerable<TIn> second, Func<TIn, TIn, bool> operation)
    {
      Contract.Requires(second != null);
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return comparisonOperation(this, second, operation).AsOperational();
    }

    private OperationalEnumerable<bool> ComparisonOperation(TIn second, Func<TIn, TIn, bool> operation)
    {
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return this.Select(first => operation(first, second)).AsOperational();
    }
    #endregion

    #region Binary Operators
    #region Math
    /// <summary>
    /// Creates a new operational enumerable that adds the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator +(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.add);
    }

    /// <summary>
    /// Creates a new operational enumerable that adds the values in this enumerable to the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Add(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(second, add);
    }

    /// <summary>
    /// Creates a new operational enumerable that adds the values in the specified enumerable to the specified value
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value that is added to each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator +(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.add);
    }

    /// <summary>
    /// Creates a new operational enumerable that adds the values in this enumerable to the specified value.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value that is added to each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Add(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(value, add);
    }

    /// <summary>
    /// Creates a new operational enumerable that subtracts the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator -(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.subtract);
    }

    /// <summary>
    /// Creates a new operational enumerable that subtracts the values in the specified enumerable from the values in this enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Subtract(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(second, subtract);
    }

    /// <summary>
    /// Creates a new operational enumerable that subtracts the specified value from the values in the specified enumerable
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value that is subtracted from each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator -(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.subtract);
    }

    /// <summary>
    /// Creates a new operational enumerable that subtracts the specified value from the values in this enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value that is subtracted from each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Subtract(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(value, subtract);
    }

    /// <summary>
    /// Creates a new operational enumerable that multiplies the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator *(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.multiply);
    }

    /// <summary>
    /// Creates a new operational enumerable that multiplies the values in this enumerable by the values in the specified enumerable.
    /// </summary>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Multiply(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(second, multiply);
    }

    /// <summary>
    /// Creates a new operational enumerable that multiplies the values in the specified enumerable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value that is multiplied against each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator *(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.multiply);
    }

    /// <summary>
    /// Creates a new operational enumerable that multiplies the values in this enumerable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value that is multiplied against each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Multiply(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(value, multiply);
    }

    /// <summary>
    /// Creates a new operational enumerable that divides the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator /(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.divide);
    }

    /// <summary>
    /// Creates a new operational enumerable that divides the values in this enumerable by the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Divide(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(second, divide);
    }

    /// <summary>
    /// Creates a new operational enumerable that divides the values in the specified enumerable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value that divides each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator /(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.divide);
    }

    /// <summary>
    /// Creates a new operational enumerable that divides the values in this enumerable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value that divides each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Divide(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(value, divide);
    }

    /// <summary>
    /// Creates a new operational enumerable that calculates the remainder when dividing the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator %(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.remainder);
    }

    /// <summary>
    /// Creates a new operational enumerable that calculates the remainder of the values in this enumerable divided by the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Remainder(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(second, remainder);
    }

    /// <summary>
    /// Creates a new operational enumerable that calculates the remainder of the values in the specified enumerable divided by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value that divides each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator %(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.remainder);
    }

    /// <summary>
    /// Creates a new operational enumerable that calculates the remainder of the values in this enumerable divided by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value that divides each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Remainder(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(value, remainder);
    }

    /// <summary>
    /// Creates a new operational enumerable that left-shifts the values in this enumerable by the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> LeftShift(IEnumerable<int> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return ShiftOperation(second, leftShift);
    }

    /// <summary>
    /// Creates a new operational enumerable that left-shifts the values in the specified enumerable by the specified value 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">The number of bits to left-shift each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator <<(OperationalEnumerable<TIn, TOut> first, int second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.ShiftOperation(second, first.leftShift);
    }

    /// <summary>
    /// Creates a new operational enumerable that left-shifts the values in this enumerable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">The number of bits to left-shift each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> LeftShift(int value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return ShiftOperation(value, leftShift);
    }

    /// <summary>
    /// Creates a new operational enumerable that right-shifts the values in this enumerable by the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> RightShift(IEnumerable<int> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return ShiftOperation(second, rightShift);
    }

    /// <summary>
    /// Creates a new operational enumerable that right-shifts the values in the specified enumerable by the specified value 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">The number of bits to right-shift each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator >>(OperationalEnumerable<TIn, TOut> first, int second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.ShiftOperation(second, first.rightShift);
    }

    /// <summary>
    /// Creates a new operational enumerable that right-shifts the values in this enumerable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">The number of bits to right-shift each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> RightShift(int value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return ShiftOperation(value, rightShift);
    }
    #endregion

    #region Comparison
    /// <summary>
    /// Creates a new operational enumerable that compares the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<bool> operator ==(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return first.ComparisonOperation(second, first.equals);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in this enumerable with the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<bool> Equals(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return ComparisonOperation(second, equals);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in the specified enumerable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value to be compared to each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<bool> operator ==(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return first.ComparisonOperation(second, first.equals);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in this enumerable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be compared to each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<bool> Equals(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return ComparisonOperation(value, equals);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<bool> operator !=(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return first.ComparisonOperation(second, first.notEquals);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in the specified enumerable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value to be compared to each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<bool> operator !=(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return first.ComparisonOperation(second, first.notEquals);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalEnumerable<bool> operator <(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return first.ComparisonOperation(second, first.lessThan);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in this enumerable with the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<bool> LessThan(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return ComparisonOperation(second, lessThan);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in the specified enumerable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value to be compared to each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalEnumerable<bool> operator <(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return first.ComparisonOperation(second, first.lessThan);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in this enumerable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be compared to each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<bool> LessThan(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return ComparisonOperation(value, lessThan);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalEnumerable<bool> operator <=(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return first.ComparisonOperation(second, first.lessThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in this enumerable with the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<bool> LessThanOrEqual(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return ComparisonOperation(second, lessThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in the specified enumerable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value to be compared to each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalEnumerable<bool> operator <=(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return first.ComparisonOperation(second, first.lessThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in this enumerable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be compared to each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<bool> LessThanOrEqual(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return ComparisonOperation(value, lessThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalEnumerable<bool> operator >(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return first.ComparisonOperation(second, first.greaterThan);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in this enumerable with the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<bool> GreaterThan(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return ComparisonOperation(second, greaterThan);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in the specified enumerable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value to be compared to each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalEnumerable<bool> operator >(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return first.ComparisonOperation(second, first.greaterThan);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in this enumerable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be compared to each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<bool> GreaterThan(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return ComparisonOperation(value, greaterThan);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalEnumerable<bool> operator >=(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return first.ComparisonOperation(second, first.greaterThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in this enumerable with the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<bool> GreaterThanOrEqual(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return ComparisonOperation(second, greaterThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in the specified enumerable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value to be compared to each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalEnumerable<bool> operator >=(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return first.ComparisonOperation(second, first.greaterThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational enumerable that compares the values in this enumerable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be compared to each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<bool> GreaterThanOrEqual(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return ComparisonOperation(value, greaterThanOrEqual);
    }
    #endregion

    #region Logical
    /// <summary>
    /// Creates a new operational enumerable that performs a logical or bitwise AND operation on the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator &(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.and);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a logical or bitwise AND operation on the values in this enumerable 
    /// and the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> BitwiseAnd(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(second, and);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a logical or bitwise AND operation on the values in the specified enumerable 
    /// and the specified value based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value to be logically or bitwise ANDed to each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator &(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.and);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a logical or bitwise AND operation on the values in this enumerable and the 
    /// specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be logically or bitwise ANDed to each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> BitwiseAnd(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(value, and);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a logical or bitwise OR operation on the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator |(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.or);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a logical or bitwise OR operation on the values in this enumerable 
    /// and the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> BitwiseOr(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(second, or);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a logical or bitwise OR operation on the values in the specified enumerable 
    /// and the specified value based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value to be logically or bitwise ORed to each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator |(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.or);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a logical or bitwise OR operation on the values in this enumerable and the 
    /// specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be logically or bitwise ORed to each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> BitwiseOr(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(value, or);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a logical or bitwise exclusive-OR operation on the values in the specified enumerables 
    /// based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first enumerable.</param>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator ^(OperationalEnumerable<TIn, TOut> first, IEnumerable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.xor);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a logical or bitwise exclusive-OR operation on the values in this enumerable 
    /// and the values in the specified enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Xor(IEnumerable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(second, xor);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a logical or bitwise exclusive-OR operation on the values in the specified enumerable 
    /// and the specified value based on the binary operation logic of the <paramref name="first"/> enumerable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The enumerable.</param>
    /// <param name="second">A value to be exclusive-ORed with each value in the <paramref name="first"/> enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator ^(OperationalEnumerable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return first.BinaryOperation(second, first.xor);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a logical or bitwise exclusive-OR operation on the values in this enumerable and the 
    /// specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be exclusive-ORed with each value in this enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Xor(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return BinaryOperation(value, xor);
    }
    #endregion
    #endregion

    #region Unary Operators
    /// <summary>
    /// Creates a new operational enumerable that negates the specified enumerable's values.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<bool> operator !(OperationalEnumerable<TIn, TOut> enumerable)
    {
      Contract.Requires(!object.Equals(enumerable, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return enumerable.UnaryBooleanOperation(enumerable.not);
    }

    /// <summary>
    /// Creates a new operational enumerable that negates this enumerable's values.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<bool> LogicalNot()
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<bool>>(), null));

      return UnaryBooleanOperation(not);
    }

    /// <summary>
    /// Creates a new operational enumerable that ensures the sign of the specified enumerable's values are positive.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator +(OperationalEnumerable<TIn, TOut> enumerable)
    {
      Contract.Requires(!object.Equals(enumerable, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return enumerable.UnaryOperation(enumerable.positive);
    }

    /// <summary>
    /// Creates a new operational enumerable that ensures the sign of this enumerable's values are positive.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Plus()
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return UnaryOperation(positive);
    }

    /// <summary>
    /// Creates a new operational enumerable that ensures the sign of the specified enumerable's values are negative.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator -(OperationalEnumerable<TIn, TOut> enumerable)
    {
      Contract.Requires(!object.Equals(enumerable, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return enumerable.UnaryOperation(enumerable.negative);
    }

    /// <summary>
    /// Creates a new operational enumerable that ensures the sign of this enumerable's values are negative.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> Negate()
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return UnaryOperation(negative);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a bitwise complement on the specified enumerable's values.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalEnumerable<TOut> operator ~(OperationalEnumerable<TIn, TOut> enumerable)
    {
      Contract.Requires(!object.Equals(enumerable, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return enumerable.UnaryOperation(enumerable.complement);
    }

    /// <summary>
    /// Creates a new operational enumerable that performs a bitwise complement on this enumerable's values.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Linq.OperationalEnumerable.AsOperational(IEnumerable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <returns>An operational enumerable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalEnumerable<TOut> OnesComplement()
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalEnumerable<TOut>>(), null));

      return UnaryOperation(complement);
    }
    #endregion
  }
#pragma warning restore 0660, 0661
}