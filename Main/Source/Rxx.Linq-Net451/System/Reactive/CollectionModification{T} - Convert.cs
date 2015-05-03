using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  public partial class CollectionModification<T>
  {
    /// <summary>
    /// Converts a <see cref="CollectionModification{T}"/> to a list of <see cref="CollectionNotification{T}"/>.
    /// </summary>
    /// <returns>A list of <see cref="CollectionNotification{T}"/> containing 
    /// <see cref="CollectionNotificationKind.OnAdded"/> when the modification is <see cref="CollectionModificationKind.Add"/>, 
    /// <see cref="CollectionNotificationKind.OnRemoved"/> when the modification is <see cref="CollectionModificationKind.Remove"/>, or
    /// <see cref="CollectionNotificationKind.OnCleared"/> when the modification is <see cref="CollectionModificationKind.Clear"/>.</returns>
    public IList<CollectionNotification<T>> ToNotifications()
    {
      Contract.Ensures(Contract.Result<IList<CollectionNotification<T>>>() != null);
      Contract.Ensures(Contract.Result<IList<CollectionNotification<T>>>().IsReadOnly);

      var list = new List<CollectionNotification<T>>();

      IList<T> values;

      switch (Kind)
      {
        case CollectionModificationKind.Add:
          values = Values;

          for (int i = 0; i < values.Count; i++)
          {
            list.Add(CollectionNotification.CreateOnAdded(values[i]));
          }
          break;
        case CollectionModificationKind.Remove:
          values = Values;

          for (int i = 0; i < values.Count; i++)
          {
            list.Add(CollectionNotification.CreateOnRemoved(values[i]));
          }
          break;
        case CollectionModificationKind.Clear:
          list.Add(CollectionNotification.CreateOnCleared<T>());
          break;
      }

      IList<CollectionNotification<T>> result = list.AsReadOnly();

      Contract.Assume(result.IsReadOnly);

      return result;
    }
  }
}