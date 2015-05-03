using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Reactive
{
#pragma warning disable 0660, 0661
  /// <summary>
  /// Represents an observable that uses its unary and binary operator overloads as observable query operators.
  /// </summary>
  /// <typeparam name="TIn">Input type.</typeparam>
  /// <typeparam name="TOut">Output type.</typeparam>
  [SuppressMessage("Microsoft.Usage", "CA2224:OverrideEqualsOnOverloadingOperatorEquals",
    Justification = "Reference equality is preferred.  The == and != operator overloads apply to the elements within the sequence.")]
  [SuppressMessage("Microsoft.Design", "CA1046:DoNotOverloadOperatorEqualsOnReferenceTypes",
    Justification = "The == and != operator overloads apply to the elements within the sequence.")]
  public class OperationalObservable<TIn, TOut> : ObservableBase<TIn>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private readonly IObservable<TIn> source;
    private readonly Func<IObservable<TOut>, OperationalObservable<TOut>> resultSelector;
    private readonly Func<IObservable<TIn>, IObservable<TIn>, Func<TIn, TIn, TOut>, IObservable<TOut>> binaryOperation;
    private readonly Func<IObservable<TIn>, IObservable<TIn>, Func<TIn, TIn, bool>, IObservable<bool>> comparisonOperation;
    private readonly Func<IObservable<TIn>, IObservable<int>, Func<TIn, int, TOut>, IObservable<TOut>> shiftOperation;
    private readonly Func<TIn, TIn, TOut> add, subtract, multiply, divide, remainder;
    private readonly Func<TIn, int, TOut> leftShift, rightShift;
    private readonly Func<TIn, TIn, TOut> and, or, xor;
    private readonly Func<TIn, TOut> positive, negative, complement;
    private readonly Func<TIn, bool> not;
    private readonly Func<TIn, TIn, bool> equals, notEquals, lessThan, lessThanOrEqual, greaterThan, greaterThanOrEqual;
    #endregion

    #region Constructors
    internal OperationalObservable(
      IObservable<TIn> source,
      Func<IObservable<TOut>, OperationalObservable<TOut>> resultSelector,
      Func<IObservable<TIn>, IObservable<TIn>, Func<TIn, TIn, TOut>, IObservable<TOut>> binaryOperation,
      Func<IObservable<TIn>, IObservable<TIn>, Func<TIn, TIn, bool>, IObservable<bool>> comparisonOperation,
      Func<IObservable<TIn>, IObservable<int>, Func<TIn, int, TOut>, IObservable<TOut>> shiftOperation,
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

    /// <inheritdoc />
    protected override IDisposable SubscribeCore(IObserver<TIn> observer)
    {
      return source.Subscribe(observer);
    }

    [ContractVerification(false)]
    private static IObservable<TResult> DefaultBinaryOperation<TFirst, TSecond, TResult>(IObservable<TFirst> first, IObservable<TSecond> second, Func<TFirst, TSecond, TResult> operation)
    {
      Contract.Requires(first != null);
      Contract.Requires(second != null);
      Contract.Requires(operation != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      return Observable.When(first.And(second).Then(operation));
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

    private OperationalObservable<TOut> BinaryOperation(IObservable<TIn> second, Func<TIn, TIn, TOut> operation)
    {
      Contract.Requires(second != null);
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      var result = resultSelector(binaryOperation(this, second, operation));

      Contract.Assume(!object.Equals(result, null));

      return result;
    }

    private OperationalObservable<TOut> BinaryOperation(TIn second, Func<TIn, TIn, TOut> operation)
    {
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      var result = resultSelector(this.Select(first => operation(first, second)));

      Contract.Assume(!object.Equals(result, null));

      return result;
    }

    private OperationalObservable<TOut> ShiftOperation(IObservable<int> second, Func<TIn, int, TOut> operation)
    {
      Contract.Requires(second != null);
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      var result = resultSelector(shiftOperation(this, second, operation));

      Contract.Assume(!object.Equals(result, null));

      return result;
    }

    private OperationalObservable<TOut> ShiftOperation(int second, Func<TIn, int, TOut> operation)
    {
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      var result = resultSelector(this.Select(first => operation(first, second)));

      Contract.Assume(!object.Equals(result, null));

      return result;
    }

    private OperationalObservable<TOut> UnaryOperation(Func<TIn, TOut> operation)
    {
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      var result = resultSelector(this.Select(operation));

      Contract.Assume(!object.Equals(result, null));

      return result;
    }

    private OperationalObservable<bool> UnaryBooleanOperation(Func<TIn, bool> operation)
    {
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return this.Select(operation).AsOperational();
    }

    private OperationalObservable<bool> ComparisonOperation(IObservable<TIn> second, Func<TIn, TIn, bool> operation)
    {
      Contract.Requires(second != null);
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return comparisonOperation(this, second, operation).AsOperational();
    }

    private OperationalObservable<bool> ComparisonOperation(TIn second, Func<TIn, TIn, bool> operation)
    {
      Contract.Requires(operation != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return this.Select(first => operation(first, second)).AsOperational();
    }
    #endregion

    #region Binary Operators
    #region Math
    /// <summary>
    /// Creates a new operational observable that adds the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator +(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.add);
    }

    /// <summary>
    /// Creates a new operational observable that adds the values in this observable to the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Add(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(second, add);
    }

    /// <summary>
    /// Creates a new operational observable that adds the values in the specified observable to the specified value
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value that is added to each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator +(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.add);
    }

    /// <summary>
    /// Creates a new operational observable that adds the values in this observable to the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value that is added to each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Add(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(value, add);
    }

    /// <summary>
    /// Creates a new operational observable that subtracts the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator -(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.subtract);
    }

    /// <summary>
    /// Creates a new operational observable that subtracts the values in the specified observable from the values in this observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Subtract(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(second, subtract);
    }

    /// <summary>
    /// Creates a new operational observable that subtracts the specified value from the values in the specified observable
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value that is subtracted from each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator -(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.subtract);
    }

    /// <summary>
    /// Creates a new operational observable that subtracts the specified value from the values in this observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value that is subtracted from each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Subtract(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(value, subtract);
    }

    /// <summary>
    /// Creates a new operational observable that multiplies the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator *(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.multiply);
    }

    /// <summary>
    /// Creates a new operational observable that multiplies the values in this observable with the values in the specified observable.
    /// </summary>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Multiply(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(second, multiply);
    }

    /// <summary>
    /// Creates a new operational observable that multiplies the values in the specified observable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value that is multiplied against each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator *(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.multiply);
    }

    /// <summary>
    /// Creates a new operational observable that multiplies the values in the specified observable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value that is multiplied against each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Multiply(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(value, multiply);
    }

    /// <summary>
    /// Creates a new operational observable that divides the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator /(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.divide);
    }

    /// <summary>
    /// Creates a new operational observable that divides the values in this observable with the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Divide(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(second, divide);
    }

    /// <summary>
    /// Creates a new operational observable that divides the values in the specified observable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value that divides each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator /(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.divide);
    }

    /// <summary>
    /// Creates a new operational observable that divides the values in this observable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value that divides each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Divide(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(value, divide);
    }

    /// <summary>
    /// Creates a new operational observable that calculates the remainder when dividing the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator %(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.remainder);
    }

    /// <summary>
    /// Creates a new operational observable that calculates the remainder of the values in this observable divided by the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Remainder(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(second, remainder);
    }

    /// <summary>
    /// Creates a new operational observable that calculates the remainder of the values in the specified observable divided by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value that divides each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator %(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.remainder);
    }

    /// <summary>
    /// Creates a new operational observable that calculates the remainder of the values in this observable divided by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value that divides each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Remainder(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(value, remainder);
    }

    /// <summary>
    /// Creates a new operational observable that left-shifts the values in this observable by the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> LeftShift(IObservable<int> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return ShiftOperation(second, leftShift);
    }

    /// <summary>
    /// Creates a new operational observable that left-shifts the values in the specified observable by the specified value 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">The number of bits to left-shift each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator <<(OperationalObservable<TIn, TOut> first, int second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.ShiftOperation(second, first.leftShift);
    }

    /// <summary>
    /// Creates a new operational observable that left-shifts the values in this observable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">The number of bits to left-shift each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> LeftShift(int value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return ShiftOperation(value, leftShift);
    }

    /// <summary>
    /// Creates a new operational observable that right-shifts the values in this observable by the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> RightShift(IObservable<int> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return ShiftOperation(second, rightShift);
    }

    /// <summary>
    /// Creates a new operational observable that right-shifts the values in the specified observable by the specified value 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">The number of bits to right-shift each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator >>(OperationalObservable<TIn, TOut> first, int second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.ShiftOperation(second, first.rightShift);
    }

    /// <summary>
    /// Creates a new operational observable that right-shifts the values in this observable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">The number of bits to right-shift each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> RightShift(int value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return ShiftOperation(value, rightShift);
    }
    #endregion

    #region Comparison
    /// <summary>
    /// Creates a new operational observable that compares the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<bool> operator ==(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return first.ComparisonOperation(second, first.equals);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in this observable with the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<bool> Equals(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return ComparisonOperation(second, equals);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in the specified observable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value to be compared to each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<bool> operator ==(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return first.ComparisonOperation(second, first.equals);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in this observable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be compared to each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<bool> Equals(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return ComparisonOperation(value, equals);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<bool> operator !=(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return first.ComparisonOperation(second, first.notEquals);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in the specified observable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value to be compared to each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<bool> operator !=(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return first.ComparisonOperation(second, first.notEquals);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalObservable<bool> operator <(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return first.ComparisonOperation(second, first.lessThan);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in this observable with the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<bool> LessThan(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return ComparisonOperation(second, lessThan);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in the specified observable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value to be compared to each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalObservable<bool> operator <(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return first.ComparisonOperation(second, first.lessThan);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in this observable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be compared to each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<bool> LessThan(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return ComparisonOperation(value, lessThan);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalObservable<bool> operator <=(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return first.ComparisonOperation(second, first.lessThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in this observable with the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<bool> LessThanOrEqual(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return ComparisonOperation(second, lessThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in the specified observable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value to be compared to each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalObservable<bool> operator <=(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return first.ComparisonOperation(second, first.lessThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in this observable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be compared to each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<bool> LessThanOrEqual(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return ComparisonOperation(value, lessThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalObservable<bool> operator >(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return first.ComparisonOperation(second, first.greaterThan);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in this observable with the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<bool> GreaterThan(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return ComparisonOperation(second, greaterThan);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in the specified observable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value to be compared to each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalObservable<bool> operator >(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return first.ComparisonOperation(second, first.greaterThan);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in this observable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be compared to each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<bool> GreaterThan(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return ComparisonOperation(value, greaterThan);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalObservable<bool> operator >=(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return first.ComparisonOperation(second, first.greaterThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in this observable with the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<bool> GreaterThanOrEqual(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return ComparisonOperation(second, greaterThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in the specified observable by the specified value
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value to be compared to each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
      Justification = "Compare is inappropriate.  This operator is used to compare the elements within the sequence.  "
                    + "A different named alternative has been provided.")]
    public static OperationalObservable<bool> operator >=(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return first.ComparisonOperation(second, first.greaterThanOrEqual);
    }

    /// <summary>
    /// Creates a new operational observable that compares the values in this observable by the specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be compared to each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<bool> GreaterThanOrEqual(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return ComparisonOperation(value, greaterThanOrEqual);
    }
    #endregion

    #region Logical
    /// <summary>
    /// Creates a new operational observable that performs a logical or bitwise AND operation on the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator &(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.and);
    }

    /// <summary>
    /// Creates a new operational observable that performs a logical or bitwise AND operation on the values in this observable 
    /// and the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> BitwiseAnd(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(second, and);
    }

    /// <summary>
    /// Creates a new operational observable that performs a logical or bitwise AND operation on the values in the specified observable 
    /// and the specified value based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value to be logically or bitwise ANDed to each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator &(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.and);
    }

    /// <summary>
    /// Creates a new operational observable that performs a logical or bitwise AND operation on the values in this observable and the 
    /// specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be logically or bitwise ANDed to each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> BitwiseAnd(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(value, and);
    }

    /// <summary>
    /// Creates a new operational observable that performs a logical or bitwise OR operation on the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator |(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.or);
    }

    /// <summary>
    /// Creates a new operational observable that performs a logical or bitwise OR operation on the values in this observable 
    /// and the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> BitwiseOr(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(second, or);
    }

    /// <summary>
    /// Creates a new operational observable that performs a logical or bitwise OR operation on the values in the specified observable 
    /// and the specified value based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value to be logically or bitwise ORed to each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator |(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.or);
    }

    /// <summary>
    /// Creates a new operational observable that performs a logical or bitwise OR operation on the values in this observable and the 
    /// specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be logically or bitwise ORed to each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> BitwiseOr(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(value, or);
    }

    /// <summary>
    /// Creates a new operational observable that performs a logical or bitwise exclusive-OR operation on the values in the specified observables 
    /// based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The first observable.</param>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator ^(OperationalObservable<TIn, TOut> first, IObservable<TIn> second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.xor);
    }

    /// <summary>
    /// Creates a new operational observable that performs a logical or bitwise exclusive-OR operation on the values in this observable 
    /// and the values in the specified observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="second">The second observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Xor(IObservable<TIn> second)
    {
      Contract.Requires(second != null);
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(second, xor);
    }

    /// <summary>
    /// Creates a new operational observable that performs a logical or bitwise exclusive-OR operation on the values in the specified observable 
    /// and the specified value based on the binary operation logic of the <paramref name="first"/> observable.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="first">The observable.</param>
    /// <param name="second">A value to be exclusive-ORed with each value in the <paramref name="first"/> observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator ^(OperationalObservable<TIn, TOut> first, TIn second)
    {
      Contract.Requires(!object.Equals(first, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return first.BinaryOperation(second, first.xor);
    }

    /// <summary>
    /// Creates a new operational observable that performs a logical or bitwise exclusive-OR operation on the values in this observable and the 
    /// specified <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="value">A value to be exclusive-ORed with each value in this observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Xor(TIn value)
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return BinaryOperation(value, xor);
    }
    #endregion
    #endregion

    #region Unary Operators
    /// <summary>
    /// Creates a new operational observable that negates the specified observable's values.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="observable">The observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<bool> operator !(OperationalObservable<TIn, TOut> observable)
    {
      Contract.Requires(!object.Equals(observable, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return observable.UnaryBooleanOperation(observable.not);
    }

    /// <summary>
    /// Creates a new operational observable that negates this observable's values.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<bool> LogicalNot()
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<bool>>(), null));

      return UnaryBooleanOperation(not);
    }

    /// <summary>
    /// Creates a new operational observable that ensures the sign of the specified observable's values are positive.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="observable">The observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator +(OperationalObservable<TIn, TOut> observable)
    {
      Contract.Requires(!object.Equals(observable, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return observable.UnaryOperation(observable.positive);
    }

    /// <summary>
    /// Creates a new operational observable that ensures the sign of this observable's values are positive.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Plus()
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return UnaryOperation(positive);
    }

    /// <summary>
    /// Creates a new operational observable that ensures the sign of the specified observable's values are negative.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="observable">The observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator -(OperationalObservable<TIn, TOut> observable)
    {
      Contract.Requires(!object.Equals(observable, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return observable.UnaryOperation(observable.negative);
    }

    /// <summary>
    /// Creates a new operational observable that ensures the sign of this observable's values are negative.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> Negate()
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return UnaryOperation(negative);
    }

    /// <summary>
    /// Creates a new operational observable that performs a bitwise complement on the specified observable's values.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <param name="observable">The observable.</param>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public static OperationalObservable<TOut> operator ~(OperationalObservable<TIn, TOut> observable)
    {
      Contract.Requires(!object.Equals(observable, null));
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return observable.UnaryOperation(observable.complement);
    }

    /// <summary>
    /// Creates a new operational observable that performs a bitwise complement on this observable's values.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// The actual behavior of this operator is determined by <see cref="System.Reactive.Linq.OperationalObservable.AsOperational(IObservable{int})"/>.
    /// </alert>
    /// </remarks>
    /// <returns>An operational observable that generates the output of the operation.</returns>
    [ContractVerification(false)]
    public OperationalObservable<TOut> OnesComplement()
    {
      Contract.Ensures(!object.Equals(Contract.Result<OperationalObservable<TOut>>(), null));

      return UnaryOperation(complement);
    }
    #endregion
  }
#pragma warning restore 0660, 0661
}