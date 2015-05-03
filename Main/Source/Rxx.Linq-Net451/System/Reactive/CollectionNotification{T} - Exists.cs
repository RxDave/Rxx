using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  public partial class CollectionNotification<T>
  {
    internal sealed class Exists : CollectionNotification<T>
    {
      #region Public Properties
      public override CollectionNotificationKind Kind
      {
        get
        {
          Contract.Ensures(Contract.Result<CollectionNotificationKind>() == CollectionNotificationKind.Exists);

          return CollectionNotificationKind.Exists;
        }
      }

      public override bool HasValue
      {
        get
        {
          Contract.Assert(Kind == CollectionNotificationKind.Exists);

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

      public override IList<T> ExistingValues
      {
        get
        {
          return values;
        }
      }
      #endregion

      #region Private / Protected
      private readonly IList<T> values;
      #endregion

      #region Constructors
      public Exists(IList<T> values)
      {
        Contract.Requires(values != null);
        Contract.Ensures(Kind == CollectionNotificationKind.Exists);

        this.values = values.IsReadOnly ? values : new List<T>(values).AsReadOnly();

        Contract.Assume(this.values.IsReadOnly);
      }
      #endregion

      #region Methods
      [ContractInvariantMethod]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
      private void ObjectInvariant()
      {
        Contract.Invariant(Kind == CollectionNotificationKind.Exists);
        Contract.Invariant(values != null);
        Contract.Invariant(values.IsReadOnly);
      }

      public override void Accept(Action<IList<T>> exists, Action<T> onAdded, Action<T, T> onReplaced, Action<T> onRemoved, Action onCleared)
      {
        exists(values);
      }

      public override TResult Accept<TResult>(Func<IList<T>, TResult> exists, Func<T, TResult> onAdded, Func<T, T, TResult> onReplaced, Func<T, TResult> onRemoved, Func<TResult> onCleared)
      {
        return exists(values);
      }

      public override string ToString()
      {
        return "{" + Kind + ": " + values.Count + "}";
      }
      #endregion
    }
  }
}