using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  public partial class CollectionNotification<T>
  {
    internal sealed class OnAdded : CollectionNotification<T>
    {
      #region Public Properties
      public override CollectionNotificationKind Kind
      {
        get
        {
          Contract.Ensures(Contract.Result<CollectionNotificationKind>() == CollectionNotificationKind.OnAdded);

          return CollectionNotificationKind.OnAdded;
        }
      }

      public override bool HasValue
      {
        get
        {
          Contract.Assert(Kind != CollectionNotificationKind.OnCleared && Kind != CollectionNotificationKind.Exists);

          return true;
        }
      }

      public override T Value
      {
        get
        {
          return value;
        }
      }

      [ContractVerification(false)]
      public override T ReplacedValue
      {
        get
        {
          return default(T);
        }
      }

      [ContractVerification(false)]
      public override IList<T> ExistingValues
      {
        get
        {
          return null;
        }
      }
      #endregion

      #region Private / Protected
      private readonly T value;
      #endregion

      #region Constructors
      public OnAdded(T value)
      {
        Contract.Ensures(Kind == CollectionNotificationKind.OnAdded);

        this.value = value;
      }
      #endregion

      #region Methods
      public override void Accept(Action<IList<T>> exists, Action<T> onAdded, Action<T, T> onReplaced, Action<T> onRemoved, Action onCleared)
      {
        onAdded(value);
      }

      public override TResult Accept<TResult>(Func<IList<T>, TResult> exists, Func<T, TResult> onAdded, Func<T, T, TResult> onReplaced, Func<T, TResult> onRemoved, Func<TResult> onCleared)
      {
        return onAdded(value);
      }

      public override string ToString()
      {
        return "{" + Kind + ": " + value + "}";
      }
      #endregion
    }
  }
}