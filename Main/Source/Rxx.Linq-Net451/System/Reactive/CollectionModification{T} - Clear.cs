using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  public partial class CollectionModification<T>
  {
    internal sealed class Clear : CollectionModification<T>
    {
      #region Public Properties
      public override CollectionModificationKind Kind
      {
        get
        {
          Contract.Ensures(Contract.Result<CollectionModificationKind>() == CollectionModificationKind.Clear);

          return CollectionModificationKind.Clear;
        }
      }

      public override bool HasValues
      {
        get
        {
          Contract.Assert(Kind == CollectionModificationKind.Clear);

          return false;
        }
      }

      [ContractVerification(false)]
      public override IList<T> Values
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
      public Clear()
      {
        Contract.Ensures(Kind == CollectionModificationKind.Clear);
      }
      #endregion

      #region Methods
      public override void Accept(ICollection<T> collection)
      {
        collection.Clear();
      }

      public override void Accept(Action<IList<T>> add, Action<IList<T>> remove, Action clear)
      {
        clear();
      }

      public override TResult Accept<TResult>(Func<IList<T>, TResult> add, Func<IList<T>, TResult> remove, Func<TResult> clear)
      {
        return clear();
      }

      public override string ToString()
      {
        return "{" + Kind + "}";
      }
      #endregion
    }
  }
}