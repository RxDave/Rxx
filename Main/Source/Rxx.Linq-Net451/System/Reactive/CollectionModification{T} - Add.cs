using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  public partial class CollectionModification<T>
  {
    internal sealed class Add : CollectionModification<T>
    {
      #region Public Properties
      public override CollectionModificationKind Kind
      {
        get
        {
          Contract.Ensures(Contract.Result<CollectionModificationKind>() == CollectionModificationKind.Add);

          return CollectionModificationKind.Add;
        }
      }

      public override bool HasValues
      {
        get
        {
          Contract.Assert(Kind != CollectionModificationKind.Clear);

          return true;
        }
      }

      public override IList<T> Values
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
      public Add(IList<T> values)
      {
        Contract.Requires(values != null);
        Contract.Ensures(Kind == CollectionModificationKind.Add);

        this.values = values.IsReadOnly ? values : new List<T>(values).AsReadOnly();

        Contract.Assume(this.values.IsReadOnly);
      }
      #endregion

      #region Methods
      [ContractInvariantMethod]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
      private void ObjectInvariant()
      {
        Contract.Invariant(Kind == CollectionModificationKind.Add);
        Contract.Invariant(values != null);
        Contract.Invariant(values.IsReadOnly);
      }

      public override void Accept(ICollection<T> collection)
      {
        foreach (var value in values)
        {
          collection.Add(value);
        }
      }

      public override void Accept(Action<IList<T>> add, Action<IList<T>> remove, Action clear)
      {
        add(values);
      }

      public override TResult Accept<TResult>(Func<IList<T>, TResult> add, Func<IList<T>, TResult> remove, Func<TResult> clear)
      {
        return add(values);
      }

      public override string ToString()
      {
        return "{" + Kind + ": " + Values.Count + "}";
      }
      #endregion
    }
  }
}