namespace System.Reactive
{
  /// <summary>
  /// Indicates the type of a collection modification.
  /// </summary>
  public enum CollectionModificationKind
  {
    /// <summary>
    /// Indicates that an item is to be added to the collection.
    /// </summary>
    Add,

    /// <summary>
    /// Indicates that an item is to be removed from the collection.
    /// </summary>
    Remove,

    /// <summary>
    /// Indicates that all items are to be removed from the collection.
    /// </summary>
    Clear
  }
}