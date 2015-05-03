using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Reactive
{
  /// <summary>
  /// Represents a modifying notification to a collection.
  /// </summary>
  /// <typeparam name="T">The object that provides modification information.</typeparam>
  [ContractClass(typeof(CollectionModificationContract<>))]
  public abstract partial class CollectionModification<T> : IEquatable<CollectionModification<T>>
  {
    #region Public Properties
    /// <summary>
    /// Gets the kind of modification that is represented.
    /// </summary>
    public abstract CollectionModificationKind Kind
    {
      get;
    }

    /// <summary>
    /// Gets a value indicating whether the modification has <see cref="Values"/>.
    /// </summary>
    /// <value><see langword="True"/> when <see cref="Kind"/> is <see cref="CollectionModificationKind.Add"/>
    /// or <see cref="CollectionModificationKind.Remove"/>; otherwise, <see langword="false"/>.</value>
    public abstract bool HasValues
    {
      get;
    }

    /// <summary>
    /// Gets the values of an <see cref="CollectionModificationKind.Add"/> or <see cref="CollectionModificationKind.Remove"/> modification.
    /// </summary>
    /// <value>The values when <see cref="Kind"/> is <see cref="CollectionModificationKind.Add"/> or 
    /// <see cref="CollectionModificationKind.Remove"/>; otherwise, throws an exception.</value>
    public abstract IList<T> Values
    {
      get;
    }
    #endregion

    #region Private / Protected
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="CollectionModification{T}" /> class for derived classes.
    /// </summary>
    internal CollectionModification()
    {
    }
    #endregion

    #region Methods
    /// <summary>
    /// Invokes the collection's method corresponding to the modification.
    /// </summary>
    /// <param name="collection">Collection on which to invoke the modification.</param>
    public abstract void Accept(ICollection<T> collection);

    /// <summary>
    /// Invokes the delegate corresponding to the modification.
    /// </summary>
    /// <param name="add">Delegate to invoke for an <see cref="Add"/> modification.</param>
    /// <param name="remove">Delegate to invoke for a <see cref="Remove"/> modification.</param>
    /// <param name="clear">Delegate to invoke for a <see cref="Clear"/> modification.</param>
    public abstract void Accept(Action<IList<T>> add, Action<IList<T>> remove, Action clear);

    /// <summary>
    /// Invokes the delegate corresponding to the modification and returns the produced result.
    /// </summary>
    /// <typeparam name="TResult">Type of the produced result.</typeparam>
    /// <param name="add">Delegate to invoke for an <see cref="Add"/> modification.</param>
    /// <param name="remove">Delegate to invoke for a <see cref="Remove"/> modification.</param>
    /// <param name="clear">Delegate to invoke for a <see cref="Clear"/> modification.</param>
    /// <returns>Result produced by the observation.</returns>
    public abstract TResult Accept<TResult>(Func<IList<T>, TResult> add, Func<IList<T>, TResult> remove, Func<TResult> clear);

    /// <summary>
    /// Generates a hash code that is suitable for use when keying a dictionary.
    /// </summary>
    /// <returns>A hash code for the modification.</returns>
    public sealed override int GetHashCode()
    {
      return Kind.GetHashCode() ^ (HasValues ? -Values.Count : 0);
    }

    /// <summary>
    /// Compares the specified modification for <see cref="Kind"/> and <see cref="Values"/> equality.
    /// </summary>
    /// <param name="other">The other modification to compare.</param>
    /// <returns><see langword="True"/> if the <see cref="Kind"/> and <see cref="Values"/> of each modification are equal;
    /// otherwise, <see langword="false" />.</returns>
    public bool Equals(CollectionModification<T> other)
    {
      return !object.ReferenceEquals(other, null)
          && other.Kind == Kind
          && (!HasValues || other.Values.SequenceEqual(Values));
    }

    /// <summary>
    /// Compares the specified modification for <see cref="Kind"/> and <see cref="Values"/> equality.
    /// </summary>
    /// <param name="obj">The other modification to compare.</param>
    /// <returns><see langword="True"/> if the specified object is an instance of <see cref="CollectionModification{T}"/> and if
    /// the <see cref="Kind"/> and <see cref="Values"/> of each modification are equal; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object obj)
    {
      return Equals(obj as CollectionModification<T>);
    }

    /// <summary>
    /// Compares the specified modifications for <see cref="Kind"/> and <see cref="Values"/> equality.
    /// </summary>
    /// <param name="first">The first modification.</param>
    /// <param name="second">The second modification.</param>
    /// <returns><see langword="True"/> if the <see cref="Kind"/> and <see cref="Values"/> of each modification are equal;
    /// otherwise, <see langword="false" />.</returns>
    public static bool operator ==(CollectionModification<T> first, CollectionModification<T> second)
    {
      return first.Equals(second);
    }

    /// <summary>
    /// Compares the specified modifications for <see cref="Kind"/> and <see cref="Values"/> inequality.
    /// </summary>
    /// <param name="first">The first modification.</param>
    /// <param name="second">The second modification.</param>
    /// <returns><see langword="False"/> if the <see cref="Kind"/> and <see cref="Values"/> of each modification are equal;
    /// otherwise, <see langword="true" />.</returns>
    public static bool operator !=(CollectionModification<T> first, CollectionModification<T> second)
    {
      return !first.Equals(second);
    }
    #endregion
  }

  [ContractClassFor(typeof(CollectionModification<>))]
  internal abstract class CollectionModificationContract<T> : CollectionModification<T>
  {
    public override CollectionModificationKind Kind
    {
      get
      {
        return default(CollectionModificationKind);
      }
    }

    public override bool HasValues
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>() == (Kind != CollectionModificationKind.Clear));
        return false;
      }
    }

    public override IList<T> Values
    {
      get
      {
        Contract.Requires(HasValues);
        Contract.Ensures(Contract.Result<IList<T>>() != null);
        Contract.Ensures(Contract.Result<IList<T>>().IsReadOnly);
        return null;
      }
    }

    public override void Accept(ICollection<T> collection)
    {
      Contract.Requires(collection != null);
    }

    public override void Accept(Action<IList<T>> add, Action<IList<T>> remove, Action clear)
    {
      Contract.Requires(add != null);
      Contract.Requires(remove != null);
      Contract.Requires(clear != null);
    }

    public override TResult Accept<TResult>(Func<IList<T>, TResult> add, Func<IList<T>, TResult> remove, Func<TResult> clear)
    {
      Contract.Requires(add != null);
      Contract.Requires(remove != null);
      Contract.Requires(clear != null);
      return default(TResult);
    }
  }
}