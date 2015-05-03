using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Rxx.Bindings.Properties;

namespace System.IO
{
  /// <summary>
  /// Provides <see langword="static" /> methods for generating in-memory lists that reactively reflect the state of file system directories.
  /// </summary>
  public static class ObservableDirectory
  {
    private const string defaultFilter = "*.*";
    private const bool defaultIncludeSubdirectories = false;

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/>, and also responds to changes by adding the full paths of files that are created 
    /// and removing those that are deleted.
    /// </summary>
    /// <param name="directory">The path to be watched, not including any subdirectories.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
      Justification = "Already does this indirectly.")]
    public static ReadOnlyListSubject<string> Collect(string directory)
    {
      Contract.Requires(directory != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return Collect(directory, defaultFilter, defaultIncludeSubdirectories);
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
      Justification = "Already does this indirectly.")]
    public static ReadOnlyListSubject<string> Collect(string directory, IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return Collect(directory, defaultFilter, defaultIncludeSubdirectories, scheduler);
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
      Justification = "Already does this indirectly.")]
    public static ReadOnlyListSubject<string> Collect(string directory, string filter)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return Collect(directory, filter, defaultIncludeSubdirectories);
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
      Justification = "Already does this indirectly.")]
    public static ReadOnlyListSubject<string> Collect(string directory, string filter, IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return Collect(directory, filter, defaultIncludeSubdirectories, scheduler);
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
    public static ReadOnlyListSubject<string> Collect(string directory, string filter, bool includeSubdirectories)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      var url = new Uri(directory);

      if (url.IsAbsoluteUri && !url.IsFile)
      {
        throw new ArgumentException(Errors.InvalidFormatDirectoryPath, "directory");
      }

      return Collect(url, filter, includeSubdirectories);
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
    public static ReadOnlyListSubject<string> Collect(string directory, string filter, bool includeSubdirectories, IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      var url = new Uri(directory);

      if (url.IsAbsoluteUri && !url.IsFile)
      {
        throw new ArgumentException(Errors.InvalidFormatDirectoryPath, "directory");
      }

      return Collect(url, filter, includeSubdirectories, scheduler);
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
      Justification = "Already does this indirectly.")]
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      string directory,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector)
    {
      Contract.Requires(directory != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return Collect(directory, defaultFilter, defaultIncludeSubdirectories, selector);
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
      Justification = "Already does this indirectly.")]
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      string directory,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector,
      IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(selector != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return Collect(directory, defaultFilter, defaultIncludeSubdirectories, selector, scheduler);
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
      Justification = "Already does this indirectly.")]
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      string directory,
      string filter,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return Collect(directory, filter, defaultIncludeSubdirectories, selector);
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
      Justification = "Already does this indirectly.")]
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      string directory,
      string filter,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector,
      IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return Collect(directory, filter, defaultIncludeSubdirectories, selector, scheduler);
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
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      string directory,
      string filter,
      bool includeSubdirectories,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector)
    {
      Contract.Requires(directory != null);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      var url = new Uri(directory);

      if (url.IsAbsoluteUri && !url.IsFile)
      {
        throw new ArgumentException(Errors.InvalidFormatDirectoryPath, "directory");
      }

      return Collect(url, filter, includeSubdirectories, selector);
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
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      string directory,
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

      var url = new Uri(directory);

      if (url.IsAbsoluteUri && !url.IsFile)
      {
        throw new ArgumentException(Errors.InvalidFormatDirectoryPath, "directory");
      }

      return Collect(url, filter, includeSubdirectories, selector, scheduler);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with the full paths to all of the files that currently exist within 
    /// the specified <paramref name="directory"/>, and also responds to changes by adding the full paths of files that are created 
    /// and removing those that are deleted.
    /// </summary>
    /// <param name="directory">The path to be watched, not including any subdirectories.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes in the specified <paramref name="directory"/>
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the specified <paramref name="directory"/>.</returns>
    public static ReadOnlyListSubject<string> Collect(Uri directory)
    {
      Contract.Requires(directory != null);
      Contract.Requires(!directory.IsAbsoluteUri || directory.IsFile);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return Collect(directory, defaultFilter, defaultIncludeSubdirectories);
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
    public static ReadOnlyListSubject<string> Collect(Uri directory, IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(!directory.IsAbsoluteUri || directory.IsFile);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return Collect(directory, defaultFilter, defaultIncludeSubdirectories, scheduler);
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
    public static ReadOnlyListSubject<string> Collect(Uri directory, string filter)
    {
      Contract.Requires(directory != null);
      Contract.Requires(!directory.IsAbsoluteUri || directory.IsFile);
      Contract.Requires(filter != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return Collect(directory, filter, defaultIncludeSubdirectories);
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
    public static ReadOnlyListSubject<string> Collect(Uri directory, string filter, IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(!directory.IsAbsoluteUri || directory.IsFile);
      Contract.Requires(filter != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return Collect(directory, filter, defaultIncludeSubdirectories, scheduler);
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
    public static ReadOnlyListSubject<string> Collect(Uri directory, string filter, bool includeSubdirectories)
    {
      Contract.Requires(directory != null);
      Contract.Requires(!directory.IsAbsoluteUri || directory.IsFile);
      Contract.Requires(filter != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return Collect(directory, filter, includeSubdirectories, Scheduler.CurrentThread);
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "FileSystemWatcher is composited into the ListSubject that is created.")]
    public static ReadOnlyListSubject<string> Collect(Uri directory, string filter, bool includeSubdirectories, IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(!directory.IsAbsoluteUri || directory.IsFile);
      Contract.Requires(filter != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      string path = directory.IsAbsoluteUri ? directory.LocalPath : directory.ToString();

      return new FileSystemWatcher(path, filter)
        {
          IncludeSubdirectories = includeSubdirectories
        }
        .CollectInternal(true, all => all.SelectMany(n => n.ToModifications()), scheduler);
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
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      Uri directory,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector)
    {
      Contract.Requires(directory != null);
      Contract.Requires(!directory.IsAbsoluteUri || directory.IsFile);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return Collect(directory, defaultFilter, defaultIncludeSubdirectories, selector);
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
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      Uri directory,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector,
      IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(!directory.IsAbsoluteUri || directory.IsFile);
      Contract.Requires(selector != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return Collect(directory, defaultFilter, defaultIncludeSubdirectories, selector, scheduler);
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
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      Uri directory,
      string filter,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector)
    {
      Contract.Requires(directory != null);
      Contract.Requires(!directory.IsAbsoluteUri || directory.IsFile);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return Collect(directory, filter, defaultIncludeSubdirectories, selector);
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
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      Uri directory,
      string filter,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector,
      IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(!directory.IsAbsoluteUri || directory.IsFile);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return Collect(directory, filter, defaultIncludeSubdirectories, selector, scheduler);
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
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      Uri directory,
      string filter,
      bool includeSubdirectories,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector)
    {
      Contract.Requires(directory != null);
      Contract.Requires(!directory.IsAbsoluteUri || directory.IsFile);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return Collect(directory, filter, includeSubdirectories, selector, Scheduler.CurrentThread);
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "FileSystemWatcher is composited into the ListSubject that is created.")]
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      Uri directory,
      string filter,
      bool includeSubdirectories,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector,
      IScheduler scheduler)
    {
      Contract.Requires(directory != null);
      Contract.Requires(!directory.IsAbsoluteUri || directory.IsFile);
      Contract.Requires(filter != null);
      Contract.Requires(selector != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      string path = directory.IsAbsoluteUri ? directory.LocalPath : directory.ToString();

      return new FileSystemWatcher(path, filter)
        {
          IncludeSubdirectories = includeSubdirectories
        }
        .CollectInternal(true, selector, scheduler);
    }
  }
}