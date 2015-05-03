using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Reactive
{
  /// <summary>
  /// Represents a collection-modifying notification to an observer.
  /// </summary>
  /// <typeparam name="T">The object that provides notification information.</typeparam>
  [ContractClass(typeof(CollectionNotificationContract<>))]
  public abstract partial class CollectionNotification<T> : IEquatable<CollectionNotification<T>>
  {
    #region Public Properties
    /// <summary>
    /// Gets the kind of notification that is represented.
    /// </summary>
    public abstract CollectionNotificationKind Kind
    {
      get;
    }

    /// <summary>
    /// Gets a value indicating whether the notification has a <see cref="Value"/> for
    /// <see cref="CollectionNotificationKind.OnAdded"/>, <see cref="CollectionNotificationKind.OnRemoved"/> or
    /// <see cref="CollectionNotificationKind.OnReplaced"/>.
    /// </summary>
    /// <value><see langword="True"/> when <see cref="Kind"/> is <see cref="CollectionNotificationKind.OnAdded"/>, 
    /// <see cref="CollectionNotificationKind.OnRemoved"/> or <see cref="CollectionNotificationKind.OnReplaced"/>; 
    /// otherwise <see langword="false"/>.</value>
    public abstract bool HasValue
    {
      get;
    }

    /// <summary>
    /// Gets the value of an <see cref="CollectionNotificationKind.OnAdded"/>, <see cref="CollectionNotificationKind.OnRemoved"/> or
    /// <see cref="CollectionNotificationKind.OnReplaced"/> notification.
    /// </summary>
    /// <value>The value when <see cref="Kind"/> is <see cref="CollectionNotificationKind.OnAdded"/>, 
    /// <see cref="CollectionNotificationKind.OnRemoved"/> or <see cref="CollectionNotificationKind.OnReplaced"/>; 
    /// otherwise, throws an exception.</value>
    public abstract T Value
    {
      get;
    }

    /// <summary>
    /// Gets the value of an <see cref="CollectionNotificationKind.OnReplaced"/> notification that was 
    /// replaced by a new <see cref="Value"/>.
    /// </summary>
    /// <value>The value that was replaced by <see cref="Value"/> when <see cref="Kind"/> is 
    /// <see cref="CollectionNotificationKind.OnReplaced"/>; otherwise, throws an exception.</value>
    public abstract T ReplacedValue
    {
      get;
    }

    /// <summary>
    /// Gets the values of an <see cref="CollectionNotificationKind.Exists"/> notification.
    /// </summary>
    /// <value>The list of existing values when <see cref="Kind"/> is <see cref="CollectionNotificationKind.Exists"/>; 
    /// otherwise, throws an exception.</value>
    public abstract IList<T> ExistingValues
    {
      get;
    }
    #endregion

    #region Private / Protected
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="CollectionNotification{T}" /> class for derived classes.
    /// </summary>
    internal CollectionNotification()
    {
    }
    #endregion

    #region Methods
    /// <summary>
    /// Invokes the delegate corresponding to the notification.
    /// </summary>
    /// <param name="exists">Delegate to invoke for an <see cref="Exists"/> notification.</param>
    /// <param name="onAdded">Delegate to invoke for an <see cref="OnAdded"/> notification.</param>
    /// <param name="onReplaced">Delegate to invoke for an <see cref="OnReplaced"/> notification.
    /// The first argument is the <see cref="ReplacedValue"/> and the second argument is the new <see cref="Value"/>.</param>
    /// <param name="onRemoved">Delegate to invoke for an <see cref="OnRemoved"/> notification.</param>
    /// <param name="onCleared">Delegate to invoke for an <see cref="OnCleared"/> notification.</param>
    public abstract void Accept(Action<IList<T>> exists, Action<T> onAdded, Action<T, T> onReplaced, Action<T> onRemoved, Action onCleared);

    /// <summary>
    /// Invokes the delegate corresponding to the notification and returns the produced result.
    /// </summary>
    /// <typeparam name="TResult">Type of the produced result.</typeparam>
    /// <param name="exists">Delegate to invoke for an <see cref="Exists"/> notification.</param>
    /// <param name="onAdded">Delegate to invoke for an <see cref="OnAdded"/> notification.</param>
    /// <param name="onReplaced">Delegate to invoke for an <see cref="OnReplaced"/> notification.
    /// The first argument is the <see cref="ReplacedValue"/> and the second argument is the new <see cref="Value"/>.</param>
    /// <param name="onRemoved">Delegate to invoke for an <see cref="OnRemoved"/> notification.</param>
    /// <param name="onCleared">Delegate to invoke for an <see cref="OnCleared"/> notification.</param>
    /// <returns>Result produced by the observation.</returns>
    public abstract TResult Accept<TResult>(Func<IList<T>, TResult> exists, Func<T, TResult> onAdded, Func<T, T, TResult> onReplaced, Func<T, TResult> onRemoved, Func<TResult> onCleared);

    /// <summary>
    /// Generates a hash code that is suitable for use when keying a dictionary.
    /// </summary>
    /// <returns>A hash code for the notification.</returns>
    public sealed override int GetHashCode()
    {
      return Kind.GetHashCode()
        ^ (HasValue ? EqualityComparer<T>.Default.GetHashCode(Value) : 0)
        ^ (Kind == CollectionNotificationKind.OnReplaced ? EqualityComparer<T>.Default.GetHashCode(ReplacedValue) : 0)
        ^ (Kind == CollectionNotificationKind.Exists ? -ExistingValues.Count : 0);
    }

    /// <summary>
    /// Compares the specified notification for <see cref="Kind"/> and <see cref="Value"/> equality.
    /// </summary>
    /// <param name="other">The other notification to compare.</param>
    /// <returns><see langword="True"/> if the <see cref="Kind"/> and <see cref="Value"/> of each notification are equal;
    /// otherwise, <see langword="false" />.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Requires",
      Justification = "other.HasValue must be true if this.HasValue is true because other.Kind == this.Kind.")]
    public bool Equals(CollectionNotification<T> other)
    {
      return !object.ReferenceEquals(other, null)
          && other.Kind == Kind
          && (!HasValue || EqualityComparer<T>.Default.Equals(other.Value, Value))
          && (Kind != CollectionNotificationKind.OnReplaced || EqualityComparer<T>.Default.Equals(other.ReplacedValue, ReplacedValue))
          && (Kind != CollectionNotificationKind.Exists || other.ExistingValues.SequenceEqual(ExistingValues));
    }

    /// <summary>
    /// Compares the specified notification for <see cref="Kind"/> and <see cref="Value"/> equality.
    /// </summary>
    /// <param name="obj">The other notification to compare.</param>
    /// <returns><see langword="True"/> if the specified object is an instance of <see cref="CollectionNotification{T}"/> and if
    /// the <see cref="Kind"/> and <see cref="Value"/> of each notification are equal; otherwise, <see langword="false" />.</returns>
    public sealed override bool Equals(object obj)
    {
      return Equals(obj as CollectionNotification<T>);
    }

    /// <summary>
    /// Compares the specified notifications for <see cref="Kind"/> and <see cref="Value"/> equality.
    /// </summary>
    /// <param name="first">The first notification.</param>
    /// <param name="second">The second notification.</param>
    /// <returns><see langword="True"/> if the <see cref="Kind"/> and <see cref="Value"/> of each notification are equal;
    /// otherwise, <see langword="false" />.</returns>
    public static bool operator ==(CollectionNotification<T> first, CollectionNotification<T> second)
    {
      return first.Equals(second);
    }

    /// <summary>
    /// Compares the specified notifications for <see cref="Kind"/> and <see cref="Value"/> inequality.
    /// </summary>
    /// <param name="first">The first notification.</param>
    /// <param name="second">The second notification.</param>
    /// <returns><see langword="False"/> if the <see cref="Kind"/> and <see cref="Value"/> of each notification are equal;
    /// otherwise, <see langword="true" />.</returns>
    public static bool operator !=(CollectionNotification<T> first, CollectionNotification<T> second)
    {
      return !first.Equals(second);
    }
    #endregion
  }

  [ContractClassFor(typeof(CollectionNotification<>))]
  internal abstract class CollectionNotificationContract<T> : CollectionNotification<T>
  {
    public override CollectionNotificationKind Kind
    {
      get
      {
        return default(CollectionNotificationKind);
      }
    }

    public override bool HasValue
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>() == (Kind != CollectionNotificationKind.OnCleared && Kind != CollectionNotificationKind.Exists));
        return false;
      }
    }

    public override T Value
    {
      get
      {
        Contract.Requires(HasValue);

        return default(T);
      }
    }

    public override T ReplacedValue
    {
      get
      {
        Contract.Requires(Kind == CollectionNotificationKind.OnReplaced);

        return default(T);
      }
    }

    public override IList<T> ExistingValues
    {
      get
      {
        Contract.Requires(Kind == CollectionNotificationKind.Exists);
        Contract.Ensures(Contract.Result<IList<T>>() != null);
        Contract.Ensures(Contract.Result<IList<T>>().IsReadOnly);
        return null;
      }
    }

    public override void Accept(Action<IList<T>> exists, Action<T> onAdded, Action<T, T> onReplaced, Action<T> onRemoved, Action onCleared)
    {
      Contract.Requires(exists != null);
      Contract.Requires(onAdded != null);
      Contract.Requires(onReplaced != null);
      Contract.Requires(onRemoved != null);
      Contract.Requires(onCleared != null);
    }

    public override TResult Accept<TResult>(Func<IList<T>, TResult> exists, Func<T, TResult> onAdded, Func<T, T, TResult> onReplaced, Func<T, TResult> onRemoved, Func<TResult> onCleared)
    {
      Contract.Requires(exists != null);
      Contract.Requires(onAdded != null);
      Contract.Requires(onReplaced != null);
      Contract.Requires(onRemoved != null);
      Contract.Requires(onCleared != null);

      return default(TResult);
    }
  }
}