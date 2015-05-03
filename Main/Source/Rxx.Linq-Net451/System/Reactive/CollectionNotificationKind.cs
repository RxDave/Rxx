namespace System.Reactive
{
  /// <summary>
  /// Indicates the type of a collection notification.
  /// </summary>
  public enum CollectionNotificationKind
  {
    /// <summary>
    /// Indicates that an item exists in the collection.
    /// </summary>
    Exists,

    /// <summary>
    /// Indicates that an item was added to the collection.
    /// </summary>
    OnAdded,

    /// <summary>
    /// Indicates that an item was replaced in the collection.
    /// </summary>
    OnReplaced,

    /// <summary>
    /// Indicates that an item was removed from the collection.
    /// </summary>
    OnRemoved,

    /// <summary>
    /// Indicates that all items were removed from the collection.
    /// </summary>
    OnCleared
  }
}