using System.Diagnostics.Contracts;

namespace System.IO
{
  /// <summary>
  /// Holds the information for a file system change event.
  /// </summary>
  public sealed class FileSystemNotification
  {
    #region Public Properties
    /// <summary>
    /// Gets the kind of change that the <see cref="FileSystemNotification"/> represents.
    /// </summary>
    public WatcherChangeTypes Change
    {
      get
      {
        Contract.Ensures(Enum.IsDefined(typeof(WatcherChangeTypes), Contract.Result<WatcherChangeTypes>()));

        return change;
      }
    }

    /// <summary>
    /// Gets the name of the file or directory that changed.
    /// </summary>
    public string Name
    {
      get
      {
        Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

        return name;
      }
    }

    /// <summary>
    /// Gets the full path of the file or directory that changed.
    /// </summary>
    public string FullPath
    {
      get
      {
        Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

        return fullPath;
      }
    }

    /// <summary>
    /// Gets the previous name of the file or directory, or <see langword="null" />.
    /// </summary>
    /// <value>Returns the previous name of the file or directory when <see cref="Change"/> is <see cref="WatcherChangeTypes.Renamed"/>;
    /// otherwise, returns <see langword="null"/>.</value>
    public string OldName
    {
      get
      {
        Contract.Ensures(Change != WatcherChangeTypes.Renamed || !string.IsNullOrEmpty(Contract.Result<string>()));
        Contract.Ensures(Change == WatcherChangeTypes.Renamed || Contract.Result<string>() == null);

        return oldName;
      }
    }

    /// <summary>
    /// Gets the previous full path of the file or directory, or <see langword="null" />.
    /// </summary>
    /// <value>Returns the previous full path of the file or directory when <see cref="Change"/> is <see cref="WatcherChangeTypes.Renamed"/>;
    /// otherwise, returns <see langword="null"/>.</value>
    public string OldFullPath
    {
      get
      {
        Contract.Ensures(Change != WatcherChangeTypes.Renamed || !string.IsNullOrEmpty(Contract.Result<string>()));
        Contract.Ensures(Change == WatcherChangeTypes.Renamed || Contract.Result<string>() == null);

        return oldFullPath;
      }
    }
    #endregion

    #region Private / Protected
    [ContractPublicPropertyName("Change")]
    private readonly WatcherChangeTypes change;
    private readonly string name, fullPath, oldName, oldFullPath;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="FileSystemNotification" /> class for all change types other than <see cref="WatcherChangeTypes.Renamed"/>.
    /// </summary>
    /// <param name="change">The type of change that occurred.</param>
    /// <param name="name">The name of the file or directory that changed.</param>
    /// <param name="fullPath">The full path to the file or directory that changed.</param>
    public FileSystemNotification(WatcherChangeTypes change, string name, string fullPath)
    {
      Contract.Requires(change != WatcherChangeTypes.Renamed);
      Contract.Requires(Enum.IsDefined(typeof(WatcherChangeTypes), change));
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Requires(!string.IsNullOrEmpty(fullPath));

      this.change = change;
      this.name = name;
      this.fullPath = fullPath;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="FileSystemNotification" /> class for <see cref="WatcherChangeTypes.Renamed"/>.
    /// </summary>
    /// <param name="oldName">The previous name of the file or directory that was renamed.</param>
    /// <param name="oldFullPath">The previous full path of the file or directory that was renamed.</param>
    /// <param name="name">The new name of the file or directory.</param>
    /// <param name="fullPath">The new full path of the file or directory.</param>
    public FileSystemNotification(string oldName, string oldFullPath, string name, string fullPath)
    {
      Contract.Requires(!string.IsNullOrEmpty(oldName));
      Contract.Requires(!string.IsNullOrEmpty(oldFullPath));
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Requires(!string.IsNullOrEmpty(fullPath));
      Contract.Ensures(change == WatcherChangeTypes.Renamed);

      this.change = WatcherChangeTypes.Renamed;
      this.name = name;
      this.fullPath = fullPath;
      this.oldName = oldName;
      this.oldFullPath = oldFullPath;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(Enum.IsDefined(typeof(WatcherChangeTypes), change));
      Contract.Invariant(!string.IsNullOrEmpty(name));
      Contract.Invariant(!string.IsNullOrEmpty(fullPath));
      Contract.Invariant(change != WatcherChangeTypes.Renamed || !string.IsNullOrEmpty(oldName));
      Contract.Invariant(change == WatcherChangeTypes.Renamed || oldName == null);
      Contract.Invariant(change != WatcherChangeTypes.Renamed || !string.IsNullOrEmpty(oldFullPath));
      Contract.Invariant(change == WatcherChangeTypes.Renamed || oldFullPath == null);
    }

    /// <summary>
    /// Gets a string representation of the file system change notification.
    /// </summary>
    /// <returns>String the represents the notification.</returns>
    public override string ToString()
    {
      if (change == WatcherChangeTypes.Renamed)
      {
        return "Renamed \"" + oldName + "\" to \"" + name + "\"";
      }
      else
      {
        return string.Concat(change, " ", name);
      }
    }
    #endregion
  }
}
