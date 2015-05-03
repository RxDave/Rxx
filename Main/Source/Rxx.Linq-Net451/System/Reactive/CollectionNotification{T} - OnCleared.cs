using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  public partial class CollectionNotification<T>
  {
    internal sealed class OnCleared : CollectionNotification<T>
    {
      #region Public Properties
      public override CollectionNotificationKind Kind
      {
        get
        {
          Contract.Ensures(Contract.Result<CollectionNotificationKind>() == CollectionNotificationKind.OnCleared);

          return CollectionNotificationKind.OnCleared;
        }
      }

      public override bool HasValue
      {
        get
        {
          Contract.Assert(Kind == CollectionNotificationKind.OnCleared);

          return false;
        }
      }

      [ContractVerification(false)]
      public override T Value
      {
        get
        {
          return default(T);
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
      #endregion

      #region Constructors
      public OnCleared()
      {
        Contract.Ensures(Kind == CollectionNotificationKind.OnCleared);
      }
      #endregion

      #region Methods
      public override void Accept(Action<IList<T>> exists, Action<T> onAdded, Action<T, T> onReplaced, Action<T> onRemoved, Action onCleared)
      {
        onCleared();
      }

      public override TResult Accept<TResult>(Func<IList<T>, TResult> exists, Func<T, TResult> onAdded, Func<T, T, TResult> onReplaced, Func<T, TResult> onRemoved, Func<TResult> onCleared)
      {
        return onCleared();
      }

      public override string ToString()
      {
        return "{" + Kind + "}";
      }
      #endregion
    }
  }
}