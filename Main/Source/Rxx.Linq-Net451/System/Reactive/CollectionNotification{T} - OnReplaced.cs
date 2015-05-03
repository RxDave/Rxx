using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  public partial class CollectionNotification<T>
  {
    internal sealed class OnReplaced : CollectionNotification<T>
    {
      #region Public Properties
      public override CollectionNotificationKind Kind
      {
        get
        {
          Contract.Ensures(Contract.Result<CollectionNotificationKind>() == CollectionNotificationKind.OnReplaced);

          return CollectionNotificationKind.OnReplaced;
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

      public override T ReplacedValue
      {
        get
        {
          return replacedValue;
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
      private readonly T value, replacedValue;
      #endregion

      #region Constructors
      public OnReplaced(T oldValue, T newValue)
      {
        Contract.Ensures(Kind == CollectionNotificationKind.OnReplaced);

        this.replacedValue = oldValue;
        this.value = newValue;
      }
      #endregion

      #region Methods
      [ContractInvariantMethod]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
      private void ObjectInvariant()
      {
        Contract.Invariant(Kind == CollectionNotificationKind.OnReplaced);
      }

      public override void Accept(Action<IList<T>> exists, Action<T> onAdded, Action<T, T> onReplaced, Action<T> onRemoved, Action onCleared)
      {
        onReplaced(replacedValue, value);
      }

      public override TResult Accept<TResult>(Func<IList<T>, TResult> exists, Func<T, TResult> onAdded, Func<T, T, TResult> onReplaced, Func<T, TResult> onRemoved, Func<TResult> onCleared)
      {
        return onReplaced(replacedValue, value);
      }

      public override string ToString()
      {
        return "{" + Kind + ": " + replacedValue + " -> " + value + "}";
      }
      #endregion
    }
  }
}