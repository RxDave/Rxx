using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

namespace System.IO
{
  /// <summary>
  /// Provides <see langword="static" /> extension methods for <see cref="DirectoryInfo"/> objects.
  /// </summary>
  public static class DirectoryInfoExtensions
  {
    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/>, and also responds to changes by adding the full paths of files that are created 
    /// and removing those that are deleted.
    /// </summary>
    /// <param name="directory">The path to be watched, not including any subdirectories.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "This method only applies to a directory.")]
    public static ReadOnlyListSubject<string> Collect(this DirectoryInfo directory)
    {
      Contract.Requires(directory != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return ObservableDirectory.Collect(directory.FullName);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/>, and also responds to changes by adding the full paths of files that are created 
    /// and removing those that are deleted.
    /// </summary>
    /// <param name="directory">The path to be watched, not including any subdirectories.</param>
    /// <param name="scheduler">Schedules changes to the <see cref="ReadOnlyListSubject{T}"/> and notifications to its subscribers.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "This method only applies to a directory.")]
    public static ReadOnlyListSubject<string> Collect(this DirectoryInfo directory, IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return ObservableDirectory.Collect(directory.FullName, scheduler);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/> and match the specified <paramref name="filter"/>, and also responds to changes by 
    /// adding the full paths of files that are created and removing those that are deleted.
    /// </summary>
    /// <param name="directory">The path to be watched, not including any subdirectories.</param>
    /// <param name="filter">The type of files to watch.  For example, "*.txt" includes all text files.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "This method only applies to a directory.")]
    public static ReadOnlyListSubject<string> Collect(this DirectoryInfo directory, string filter)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return ObservableDirectory.Collect(directory.FullName, filter);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/> and match the specified <paramref name="filter"/>, and also responds to changes by 
    /// adding the full paths of files that are created and removing those that are deleted.
    /// </summary>
    /// <param name="directory">The path to be watched, not including any subdirectories.</param>
    /// <param name="filter">The type of files to watch.  For example, "*.txt" includes all text files.</param>
    /// <param name="scheduler">Schedules changes to the <see cref="ReadOnlyListSubject{T}"/> and notifications to its subscribers.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "This method only applies to a directory.")]
    public static ReadOnlyListSubject<string> Collect(this DirectoryInfo directory, string filter, IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return ObservableDirectory.Collect(directory.FullName, filter, scheduler);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/> and match the specified <paramref name="filter"/>, and also responds to changes by 
    /// adding the full paths of files that are created and removing those that are deleted.
    /// </summary>
    /// <param name="directory">The path to be watched.</param>
    /// <param name="filter">The type of files to watch.  For example, "*.txt" includes all text files.</param>
    /// <param name="includeSubdirectories">Specifies whether to include and watch files from subdirectories.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "This method only applies to a directory.")]
    public static ReadOnlyListSubject<string> Collect(this DirectoryInfo directory, string filter, bool includeSubdirectories)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return ObservableDirectory.Collect(directory.FullName, filter, includeSubdirectories);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/> and match the specified <paramref name="filter"/>, and also responds to changes by 
    /// adding the full paths of files that are created and removing those that are deleted.
    /// </summary>
    /// <param name="directory">The path to be watched.</param>
    /// <param name="filter">The type of files to watch.  For example, "*.txt" includes all text files.</param>
    /// <param name="includeSubdirectories">Specifies whether to include and watch files from subdirectories.</param>
    /// <param name="scheduler">Schedules changes to the <see cref="ReadOnlyListSubject{T}"/> and notifications to its subscribers.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "This method only applies to a directory.")]
    public static ReadOnlyListSubject<string> Collect(this DirectoryInfo directory, string filter, bool includeSubdirectories, IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return ObservableDirectory.Collect(directory.FullName, filter, includeSubdirectories, scheduler);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with elements projected from the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/>, and also responds to changes by adding elements projected from the full paths of files that are created 
    /// and removing those that are deleted.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected elements in the list.</typeparam>
    /// <param name="directory">The path to be watched, not including any subdirectories.</param>
    /// <param name="selector">Projects a sequence of file change notifications into a sequence from which the list is populated.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "This method only applies to a directory.")]
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      this DirectoryInfo directory,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector)
    {
      Contract.Requires(directory != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return ObservableDirectory.Collect(directory.FullName, selector);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with elements projected from the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/>, and also responds to changes by adding elements projected from the full paths of files that are created 
    /// and removing those that are deleted.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected elements in the list.</typeparam>
    /// <param name="directory">The path to be watched, not including any subdirectories.</param>
    /// <param name="selector">Projects a sequence of file change notifications into a sequence from which the list is populated.</param>
    /// <param name="scheduler">Schedules changes to the <see cref="ReadOnlyListSubject{T}"/> and notifications to its subscribers.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "This method only applies to a directory.")]
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      this DirectoryInfo directory,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector,
      IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(selector != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return ObservableDirectory.Collect(directory.FullName, selector, scheduler);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with elements projected from the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/> and match the specified <paramref name="filter"/>, and also responds to changes by 
    /// adding elements projected from the full paths of files that are created and removing those that are deleted.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected elements in the list.</typeparam>
    /// <param name="directory">The path to be watched, not including any subdirectories.</param>
    /// <param name="filter">The type of files to watch.  For example, "*.txt" includes all text files.</param>
    /// <param name="selector">Projects a sequence of file change notifications into a sequence from which the list is populated.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "This method only applies to a directory.")]
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      this DirectoryInfo directory,
      string filter,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return ObservableDirectory.Collect(directory.FullName, filter, selector);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with elements projected from the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/> and match the specified <paramref name="filter"/>, and also responds to changes by 
    /// adding elements projected from the full paths of files that are created and removing those that are deleted.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected elements in the list.</typeparam>
    /// <param name="directory">The path to be watched, not including any subdirectories.</param>
    /// <param name="filter">The type of files to watch.  For example, "*.txt" includes all text files.</param>
    /// <param name="selector">Projects a sequence of file change notifications into a sequence from which the list is populated.</param>
    /// <param name="scheduler">Schedules changes to the <see cref="ReadOnlyListSubject{T}"/> and notifications to its subscribers.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "This method only applies to a directory.")]
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      this DirectoryInfo directory,
      string filter,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector,
      IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return ObservableDirectory.Collect(directory.FullName, filter, selector, scheduler);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with elements projected from the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/> and match the specified <paramref name="filter"/>, and also responds to changes by 
    /// adding elements projected from the full paths of files that are created and removing those that are deleted.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected elements in the list.</typeparam>
    /// <param name="directory">The path to be watched.</param>
    /// <param name="filter">The type of files to watch.  For example, "*.txt" includes all text files.</param>
    /// <param name="includeSubdirectories">Specifies whether to include and watch files from subdirectories.</param>
    /// <param name="selector">Projects a sequence of file change notifications into a sequence from which the list is populated.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "This method only applies to a directory.")]
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      this DirectoryInfo directory,
      string filter,
      bool includeSubdirectories,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return ObservableDirectory.Collect(directory.FullName, filter, includeSubdirectories, selector);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with elements projected from the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/> and match the specified <paramref name="filter"/>, and also responds to changes by 
    /// adding elements projected from the full paths of files that are created and removing those that are deleted.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected elements in the list.</typeparam>
    /// <param name="directory">The path to be watched.</param>
    /// <param name="filter">The type of files to watch.  For example, "*.txt" includes all text files.</param>
    /// <param name="includeSubdirectories">Specifies whether to include and watch files from subdirectories.</param>
    /// <param name="selector">Projects a sequence of file change notifications into a sequence from which the list is populated.</param>
    /// <param name="scheduler">Schedules changes to the <see cref="ReadOnlyListSubject{T}"/> and notifications to its subscribers.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
      Justification = "This method only applies to a directory.")]
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      this DirectoryInfo directory,
      string filter,
      bool includeSubdirectories,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector,
      IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return ObservableDirectory.Collect(directory.FullName, filter, includeSubdirectories, selector, scheduler);
    }
  }
}