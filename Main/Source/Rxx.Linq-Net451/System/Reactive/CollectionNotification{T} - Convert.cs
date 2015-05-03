using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  public partial class CollectionNotification<T>
  {
    /// <summary>
    /// Converts a <see cref="CollectionNotification{T}"/> to a list of <see cref="CollectionModification{T}"/>.
    /// </summary>
    /// <returns>A list of <see cref="CollectionModification{T}"/> containing
    /// <see cref="CollectionModificationKind.Add"/> when the notification is <see cref="CollectionNotificationKind.Exists"/> or <see cref="CollectionNotificationKind.OnAdded"/>, 
    /// <see cref="CollectionModificationKind.Remove"/> followed by <see cref="CollectionModificationKind.Add"/> when the notification is <see cref="CollectionNotificationKind.OnReplaced"/>, 
    /// <see cref="CollectionModificationKind.Remove"/> when the notification is <see cref="CollectionNotificationKind.OnRemoved"/>, or 
    /// <see cref="CollectionModificationKind.Clear"/> when the notification is <see cref="CollectionNotificationKind.OnCleared"/>.</returns>
    public IList<CollectionModification<T>> ToModifications()
    {
      Contract.Ensures(Contract.Result<IList<CollectionModification<T>>>() != null);
      Contract.Ensures(Contract.Result<IList<CollectionModification<T>>>().IsReadOnly);

      var list = new List<CollectionModification<T>>();

      switch (Kind)
      {
        case CollectionNotificationKind.Exists:
          list.Add(CollectionModification.CreateAdd(ExistingValues));
          break;
        case CollectionNotificationKind.OnAdded:
          list.Add(CollectionModification.CreateAdd(Value));
          break;
        case CollectionNotificationKind.OnRemoved:
          list.Add(CollectionModification.CreateRemove(Value));
          break;
        case CollectionNotificationKind.OnReplaced:
          list.Add(CollectionModification.CreateRemove(ReplacedValue));
          list.Add(CollectionModification.CreateAdd(Value));
          break;
        case CollectionNotificationKind.OnCleared:
          list.Add(CollectionModification.CreateClear<T>());
          break;
      }

      IList<CollectionModification<T>> result = list.AsReadOnly();

      Contract.Assume(result.IsReadOnly);

      return result;
    }
  }
}