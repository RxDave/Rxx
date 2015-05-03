using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Disposables;

namespace System
{
  /* I've decided to make Scalar<T> a class instead of a struct simply because its primary usage is 
   * to represent scalar-valued interfaces, such as IEnumerable<T> and IObservable<T>.  A boxing 
   * operation to these interfaces is perhaps slightly more costly than making Scalar<T> a class to 
   * begin with, because boxing requires the additional construction of a wrapper object (though I 
   * suspect that allocation costs are the same).  Being an immutable class also means that a Scalar<T>
   * object can be allocated once and shared many times without the overhead of boxing many times.
   * 
   * Furthermore, it's unlikely for Scalar<T> to be statically enumerated except perhaps in lab or 
   * accademic scenarios, thus the duck-typing optimizations provided by C# foreach loops to avoid 
   * boxing don't generally apply to instances of Scalar<T> as a value type.
   * 
   * A downside of being a class is that a Scalar<T> variable can be null, though this might make sense
   * anyway; however, its primary usage is to represent scalar-valued interfaces, which are treated as
   * reference types and thus can be null.  The lifetime of a variable of type Scalar<T> is typically 
   * ephemeral, given that most of the time Scalar<T> is returned as an interface from a function or is 
   * used as a local variable to be passed into a method that requires one of its interfaces, so the 
   * cost of having to deal with null references should be pretty low outside of the implementation of 
   * this class.
   */

  /// <summary>
  /// Represents a single value as a read-only list, collection and sequence of size 1.
  /// </summary>
  /// <typeparam name="T">The type of the value.</typeparam>
  /// <remarks>
  /// <see cref="Scalar{T}"/> is a reference type, though it's treated like a value type for the sake 
  /// of equality comparisons between two <see cref="Scalar{T}"/> objects and unwrapped instances of 
  /// <typeparamref name="T"/>.
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
#if !SILVERLIGHT && !PORT_45 && !PORT_40
  [Serializable]
#endif
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification = "It's many things.")]
  public sealed class Scalar<T> : ObservableBase<T>, IList<T>, IEquatable<Scalar<T>>, IEquatable<T>, IStructuralEquatable
  {
    /// <summary>
    /// Represents a single default value of <typeparamref name="T"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes",
      Justification = "This is a convenience field that should be clear semantically; e.g., Scalar<int>.Default.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
      Justification = "Scalar<T> is immutable.")]
    public static readonly Scalar<T> Default = new Scalar<T>(default(T));

    /// <summary>
    /// Gets the value of this <see cref="Scalar{T}"/>.
    /// </summary>
    /// <value>The value of this <see cref="Scalar{T}"/>.</value>
    public T Value
    {
      get
      {
        Contract.Ensures(object.Equals(Contract.Result<T>(), value));

        return value;
      }
    }

    bool ICollection<T>.IsReadOnly
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>() == true);

        return true;
      }
    }

    int ICollection<T>.Count
    {
      get
      {
        Contract.Ensures(Contract.Result<int>() == 1);

        return 1;
      }
    }

    T IList<T>.this[int index]
    {
      get
      {
        return value;
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    private readonly T value;
    private readonly IEqualityComparer<T> comparer;

    /// <summary>
    /// Constructs a new instance of the <see cref="Scalar{T}" /> class with the specified <paramref name="value"/> 
    /// and the default comparer for <typeparamref name="T"/>.
    /// </summary>
    /// <param name="value">The value of the <see cref="Scalar{T}"/>.</param>
    public Scalar(T value)
      : this(value, EqualityComparer<T>.Default)
    {
      Contract.Ensures(object.Equals(Value, value));
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="Scalar{T}" /> class with the specified <paramref name="value"/>
    /// and <paramref name="comparer"/>.
    /// </summary>
    /// <param name="value">The value of the <see cref="Scalar{T}"/>.</param>
    /// <param name="comparer">An object that can determine whether the specified <paramref name="value"/> is equal 
    /// to other instances of <typeparamref name="T"/>.</param>
    public Scalar(T value, IEqualityComparer<T> comparer)
    {
      Contract.Requires(comparer != null);
      Contract.Ensures(object.Equals(Value, value));

      this.value = value;
      this.comparer = comparer;
    }

    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(comparer != null);
      Contract.Invariant(((ICollection<T>)this).Count == 1);
    }

    /// <inheritdoc />
    protected override IDisposable SubscribeCore(IObserver<T> observer)
    {
      observer.OnNext(value);
      observer.OnCompleted();

      return Disposable.Empty;
    }

    /// <summary>
    /// Gets an object that represents a scalar sequence containing <see cref="Value"/>.
    /// </summary>
    /// <returns>An enumerator that moves one time and yields <see cref="Value"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "It's returned to the caller.")]
    public IEnumerator<T> GetEnumerator()
    {
      return new Enumerator(value);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "It's returned to the caller.")]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new Enumerator(value);
    }

    /// <summary>
    /// Extracts <see cref="Value"/> from the specified <paramref name="scalar"/>.
    /// </summary>
    /// <param name="scalar">The scalar from which to extract <see cref="Value"/>.</param>
    /// <returns>The <see cref="Value"/> of the specified <paramref name="scalar"/>.</returns>
    public static implicit operator T(Scalar<T> scalar)
    {
      return scalar == null ? default(T) : scalar.value;
    }

    /// <summary>
    /// Amplifies the specified <paramref name="value"/> into the <see cref="Value"/> of a new <see cref="Scalar{T}"/>.
    /// </summary>
    /// <param name="value">The value to be amplified.</param>
    /// <returns>A new <see cref="Scalar{T}"/> containing the specified <paramref name="value"/>.</returns>
    public static implicit operator Scalar<T>(T value)
    {
      Contract.Ensures(Contract.Result<Scalar<T>>() != null);

      return new Scalar<T>(value);
    }

    /// <summary>
    /// Creates a <see cref="Scalar{T}"/> containing the value from the specified <paramref name="tuple"/>.
    /// </summary>
    /// <param name="tuple">The tuple from which to extract <see cref="Tuple{T}.Item1"/> into a <see cref="Scalar{T}"/>.</param>
    /// <returns>A <see cref="Scalar{T}"/> containing the value from the specified <paramref name="tuple"/> if it's not 
    /// <see langword="null"/>; otherwise, returns <see langword="null"/>.</returns>
    public static implicit operator Scalar<T>(Tuple<T> tuple)
    {
      Contract.Ensures((Contract.Result<Scalar<T>>() == null) == (tuple == null));

      return tuple == null ? null : new Scalar<T>(tuple.Item1);
    }

    /// <summary>
    /// Creates a <see cref="Tuple{T}"/> containing the <see cref="Value"/> of the specified <paramref name="scalar"/>.
    /// </summary>
    /// <param name="scalar">The scalar from which to extract <see cref="Value"/> into a <see cref="Tuple{T}"/>.</param>
    /// <returns>A <see cref="Tuple{T}"/> containing the <see cref="Value"/> of the specified <paramref name="scalar"/> if it's not 
    /// <see langword="null"/>; otherwise, returns <see langword="null"/>.</returns>
    public static implicit operator Tuple<T>(Scalar<T> scalar)
    {
      Contract.Ensures((Contract.Result<Tuple<T>>() == null) == (scalar == null));

      return scalar == null ? null : new Tuple<T>(scalar.value);
    }

    /// <summary>
    /// Gets a value indicating whether the specified scalars' values are equal according to the equality comparer of 
    /// the <paramref name="first"/> scalar.
    /// </summary>
    /// <param name="first">A scalar to be compared to the <paramref name="second"/> value.</param>
    /// <param name="second">A scalar to be compared to the <paramref name="first"/> value.</param>
    /// <returns><see langword="True"/> if the specified scalars' values are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(Scalar<T> first, Scalar<T> second)
    {
      return object.ReferenceEquals(first, null)
           ? object.ReferenceEquals(second, null)
           : first.Equals(second);
    }

    /// <summary>
    /// Gets a value indicating whether the specified values are equal according to the equality comparer of 
    /// the <paramref name="first"/> scalar.
    /// </summary>
    /// <param name="first">A scalar to be compared to the specified <paramref name="value"/>.</param>
    /// <param name="value">A value to be compared to the <paramref name="first"/> value.</param>
    /// <returns><see langword="True"/> if the specified values are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(Scalar<T> first, T value)
    {
      Contract.Assume(object.ReferenceEquals(first, null) || first.comparer != null);

      return object.ReferenceEquals(first, null)
           ? object.ReferenceEquals(value, null)
           : first.comparer.Equals(first.value, value);
    }

    /// <summary>
    /// Gets a value indicating whether the specified scalars' values are not equal according to the equality comparer of 
    /// the <paramref name="first"/> scalar.
    /// </summary>
    /// <param name="first">A scalar to be compared to the <paramref name="second"/> value for inequality.</param>
    /// <param name="second">A scalar to be compared to the <paramref name="first"/> value for inequality.</param>
    /// <returns><see langword="False"/> if the specified scalars' values are equal; otherwise, <see langword="true"/>.</returns>
    public static bool operator !=(Scalar<T> first, Scalar<T> second)
    {
      return object.ReferenceEquals(first, null)
           ? !object.ReferenceEquals(second, null)
           : !first.Equals(second);
    }

    /// <summary>
    /// Gets a value indicating whether the specified values are not equal according to the equality comparer of 
    /// the <paramref name="first"/> scalar.
    /// </summary>
    /// <param name="first">A scalar to be compared to the specified <paramref name="value"/> for inequality.</param>
    /// <param name="value">A value to be compared to the <paramref name="first"/> value for inequality.</param>
    /// <returns><see langword="False"/> if the specified values are equal; otherwise, <see langword="true"/>.</returns>
    public static bool operator !=(Scalar<T> first, T value)
    {
      Contract.Assume(object.ReferenceEquals(first, null) || first.comparer != null);

      return object.ReferenceEquals(first, null)
           ? !object.ReferenceEquals(value, null)
           : !first.comparer.Equals(first.value, value);
    }

    /// <summary>
    /// Gets a value indicating whether the current scalar's <see cref="Value"/> is equal to the specified scalar's <see cref="Value"/>.
    /// </summary>
    /// <param name="other">A scalar to be compared to the current <see cref="Value"/>.</param>
    /// <returns><see langword="True"/> if the current <see cref="Value"/> is equal to the specified scalar's <see cref="Value"/>; 
    /// otherwise, <see langword="false"/>.</returns>
    public bool Equals(Scalar<T> other)
    {
      return !object.ReferenceEquals(other, null) && comparer.Equals(value, other.value);
    }

    /// <summary>
    /// Gets a value indicating whether the current scalar's <see cref="Value"/> is equal to the specified value.
    /// </summary>
    /// <param name="other">A value to be compared to the current <see cref="Value"/>.</param>
    /// <returns><see langword="True"/> if the current <see cref="Value"/> is equal to the specified value; 
    /// otherwise, <see langword="false"/>.</returns>
    public bool Equals(T other)
    {
      return comparer.Equals(value, other);
    }

    /// <summary>
    /// Gets a value indicating whether the current scalar's <see cref="Value"/> is equal to the specified object.
    /// </summary>
    /// <param name="obj">An object to be compared to the current <see cref="Value"/>.</param>
    /// <returns><see langword="True"/> if the current <see cref="Value"/> is equal to the specified object; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object obj)
    {
      var s = obj as Scalar<T>;

      return s != null
           ? Equals(s)
           : obj is T && comparer.Equals(value, (T)obj);
    }

    /// <summary>
    /// Returns a hash code for <see cref="Value"/>.
    /// </summary>
    /// <returns>A hash code for <see cref="Value"/>.</returns>
    public override int GetHashCode()
    {
      return value == null ? 0 : comparer.GetHashCode(value);
    }

    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
    {
      if (comparer == null)
      {
        throw new ArgumentNullException("comparer");
      }

      var s = other as Scalar<T>;

      return s != null
        ? comparer.Equals(value, s.value)
        : comparer.Equals(value, other);
    }

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
      if (comparer == null)
      {
        throw new ArgumentNullException("comparer");
      }

      return value == null ? 0 : comparer.GetHashCode(value);
    }

    /// <summary>
    /// Returns a string that represents <see cref="Value"/>.
    /// </summary>
    /// <returns>A string that represents <see cref="Value"/>.</returns>
    public override string ToString()
    {
      return value == null ? string.Empty : value.ToString();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures",
      Justification = "Static checker proves assertion but warns that the result might not be less than Count.")]
    int IList<T>.IndexOf(T item)
    {
      Contract.Assert(((ICollection<T>)this).Count == 1);

      return comparer.Equals(value, item) ? 0 : -1;
    }

    void IList<T>.Insert(int index, T item)
    {
      throw new NotSupportedException();
    }

    void IList<T>.RemoveAt(int index)
    {
      throw new NotSupportedException();
    }

    bool ICollection<T>.Remove(T item)
    {
      throw new NotSupportedException();
    }

    void ICollection<T>.Add(T item)
    {
      throw new NotSupportedException();
    }

    void ICollection<T>.Clear()
    {
      throw new NotSupportedException();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures",
      Justification = "Static checker proves assertion but warns that Count might not be greater than zero.")]
    bool ICollection<T>.Contains(T item)
    {
      Contract.Assert(((ICollection<T>)this).Count == 1);

      return comparer.Equals(value, item);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
      Contract.Assume(arrayIndex < array.Length);

      array[arrayIndex] = value;
    }

    private sealed class Enumerator : IEnumerator<T>
    {
      public T Current
      {
        get
        {
          return value;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return value;
        }
      }

      private readonly T value;
      private bool moved;

      internal Enumerator(T value)
      {
        this.value = value;
      }

      public bool MoveNext()
      {
        if (moved)
        {
          return false;
        }

        moved = true;
        return true;
      }

      void IEnumerator.Reset()
      {
        moved = false;
      }

      public void Dispose()
      {
      }
    }
  }
}