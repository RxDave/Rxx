using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace System.IO
{
  /// <summary>
  /// Provides extension methods for <see cref="FileSystemWatcher"/>.
  /// </summary>
  public static class FileSystemWatcherExtensions
  {
    // http://msdn.microsoft.com/en-us/library/system.io.filesystemwatcher.internalbuffersize.aspx
    private const int minBufferSize = 4096;
    private const int maxBufferSize = 65536;

    /// <summary>
    /// Creates an observable sequence of file system change notifications of the specified types.
    /// </summary>
    /// <param name="watcher">Watches the file system for changes.</param>
    /// <param name="changes">Specifies the types of changes to watch.</param>
    /// <remarks>
    /// <see cref="FileSystemWatcher"/> events are raised on a thread-pool thread by default.
    /// See <see href="http://msdn.microsoft.com/en-us/library/system.io.filesystemwatcher.synchronizingobject.aspx">
    /// FileSystemNotification.SynchronizingObject
    /// </see> for more information.
    /// </remarks>
    /// <returns>An observable sequence of file system change notifications.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
      Justification = "This method is designed to trade internal maintainability for the simplification of consumption.  Furthermore, it only relies on highly cohesive types in the FCL that cannot be changed or decoupled.")]
    public static IObservable<FileSystemNotification> Watch(
      this FileSystemWatcher watcher,
      WatcherChangeTypes changes)
    {
      Contract.Requires(watcher != null);
      Contract.Ensures(Contract.Result<IObservable<FileSystemNotification>>() != null);

      return Observable.Defer<FileSystemNotification>(() =>
        {
          var allEvents = CreateObservableEvents(watcher, changes);

          var error = Observable.FromEventPattern<ErrorEventHandler, ErrorEventArgs>(
            eh => eh.Invoke,
            eh => watcher.Error += eh,
            eh => watcher.Error -= eh);

          allEvents = allEvents.Merge(
            error.SelectMany(
              e => Observable.Throw<FileSystemNotification>(new FileSystemWatcherBufferOverflowException(e.EventArgs.GetException()))));

          watcher.EnableRaisingEvents = true;

          bool firstTime = true;

          return Observable.Create<FileSystemNotification>(
            observer =>
            {
              return Scheduler.CurrentThread.Schedule(
                self =>
                {
                  if (!firstTime)
                  {
                    int size = watcher.InternalBufferSize;
                    int newSize = Math.Min(size + minBufferSize, maxBufferSize);

                    Trace.TraceWarning(Rxx.Bindings.Properties.Text.FileSystemWatcherBufferChangeFormat, size, newSize);

                    watcher.InternalBufferSize = newSize;
                  }

                  firstTime = false;

                  allEvents.SubscribeSafe(
                    observer.OnNext,
                    ex =>
                    {
                      if (ex is FileSystemWatcherBufferOverflowException)
                      {
                        if (watcher.InternalBufferSize < maxBufferSize)
                        {
                          Trace.TraceWarning(Rxx.Bindings.Properties.Text.FileSystemWatcherBufferOverflowFormat, ex.Message);

                          self();
                        }
                        else
                        {
                          Trace.TraceError(Rxx.Bindings.Properties.Text.FileSystemWatcherBufferOverflowFormat, ex.Message);
                        }
                      }
                      else
                      {
                        observer.OnError(ex);
                      }
                    },
                    observer.OnCompleted);
                });
            });
        });
    }

    private static IObservable<FileSystemNotification> CreateObservableEvents(
      FileSystemWatcher watcher,
      WatcherChangeTypes changes)
    {
      Contract.Requires(watcher != null);
      Contract.Ensures(Contract.Result<IObservable<FileSystemNotification>>() != null);

      IObservable<FileSystemNotification> allEvents = null;

      var events = CreateObservableEventsWithoutRenamed(watcher, changes);

      if (events != null)
      {
        allEvents = from e in events
                    select new FileSystemNotification(
                      e.EventArgs.ChangeType,
                      e.EventArgs.Name,
                      e.EventArgs.FullPath);
      }

      if (changes.HasFlag(WatcherChangeTypes.Renamed))
      {
        var renamed = Observable
          .FromEventPattern<RenamedEventHandler, RenamedEventArgs>(
            eh => eh.Invoke,
            eh => watcher.Renamed += eh,
            eh => watcher.Renamed -= eh)
          .Select(e => new FileSystemNotification(
            e.EventArgs.OldName,
            e.EventArgs.OldFullPath,
            e.EventArgs.Name,
            e.EventArgs.FullPath));

        allEvents = allEvents == null ? renamed : allEvents.Merge(renamed);
      }

      if (allEvents == null)
        throw new InvalidEnumArgumentException("changes", (int)changes, typeof(WatcherChangeTypes));

      return allEvents;
    }

    private static IObservable<EventPattern<FileSystemEventArgs>> CreateObservableEventsWithoutRenamed(
      FileSystemWatcher watcher,
      WatcherChangeTypes changes)
    {
      Contract.Requires(watcher != null);

      IObservable<EventPattern<FileSystemEventArgs>> events = null;

      if (changes.HasFlag(WatcherChangeTypes.Changed))
      {
        events = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
          eh => eh.Invoke,
          eh => watcher.Changed += eh,
          eh => watcher.Changed -= eh);
      }

      if (changes.HasFlag(WatcherChangeTypes.Created))
      {
        var created = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
          eh => eh.Invoke,
          eh => watcher.Created += eh,
          eh => watcher.Created -= eh);

        events = events == null ? created : events.Merge(created);
      }

      if (changes.HasFlag(WatcherChangeTypes.Deleted))
      {
        var deleted = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
          eh => eh.Invoke,
          eh => watcher.Deleted += eh,
          eh => watcher.Deleted -= eh);

        events = events == null ? deleted : events.Merge(deleted);
      }

      return events;
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with the full paths to all of the files that currently exist within 
    /// the scope of the specified <see cref="FileSystemWatcher"/>, and also responds to changes by adding the full paths of files 
    /// that are created and removing those that are deleted.
    /// </summary>
    /// <param name="watcher">The object that specifies the directory to be watched and the files to be included.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes from the specified <see cref="FileSystemWatcher"/> 
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the scope of the specified <see cref="FileSystemWatcher"/>.</returns>
    public static ReadOnlyListSubject<string> Collect(this FileSystemWatcher watcher)
    {
      Contract.Requires(watcher != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return watcher.CollectInternal(false, all => all.SelectMany(n => n.ToModifications()), Scheduler.CurrentThread);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with the full paths to all of the files that currently exist within 
    /// the scope of the specified <see cref="FileSystemWatcher"/>, and also responds to changes by adding the full paths of files 
    /// that are created and removing those that are deleted.
    /// </summary>
    /// <param name="watcher">The object that specifies the directory to be watched and the files to be included.</param>
    /// <param name="scheduler">Schedules changes to the <see cref="ReadOnlyListSubject{T}"/> and notifications to its subscribers.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes from the specified <see cref="FileSystemWatcher"/> 
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the scope of the specified <see cref="FileSystemWatcher"/>.</returns>
    public static ReadOnlyListSubject<string> Collect(this FileSystemWatcher watcher, IScheduler scheduler)
    {
      Contract.Requires(watcher != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<string>>() != null);

      return watcher.CollectInternal(false, all => all.SelectMany(n => n.ToModifications()), scheduler);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with elements projected from the full paths to all of the files that currently exist within 
    /// the scope of the specified <see cref="FileSystemWatcher"/>, and also responds to changes by adding elements projected from the full paths of files 
    /// that are created and removing those that are deleted.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected elements in the list.</typeparam>
    /// <param name="watcher">The object that specifies the directory to be watched and the files to be included.</param>
    /// <param name="selector">Projects a sequence of file change notifications into a sequence from which the list is populated.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes from the specified <see cref="FileSystemWatcher"/> 
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the scope of the specified <see cref="FileSystemWatcher"/>.</returns>
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      this FileSystemWatcher watcher,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector)
    {
      Contract.Requires(watcher != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return watcher.CollectInternal(false, selector, Scheduler.CurrentThread);
    }

    /// <summary>
    /// Populates a <see cref="ReadOnlyListSubject{T}"/> with elements projected from the full paths to all of the files that currently exist within 
    /// the scope of the specified <see cref="FileSystemWatcher"/>, and also responds to changes by adding elements projected from the full paths of files 
    /// that are created and removing those that are deleted.
    /// </summary>
    /// <typeparam name="TResult">The type of the projected elements in the list.</typeparam>
    /// <param name="watcher">The object that specifies the directory to be watched and the files to be included.</param>
    /// <param name="selector">Projects a sequence of file change notifications into a sequence from which the list is populated.</param>
    /// <param name="scheduler">Schedules changes to the <see cref="ReadOnlyListSubject{T}"/> and notifications to its subscribers.</param>
    /// <returns>A <see cref="ReadOnlyListSubject{T}"/> that responds to changes from the specified <see cref="FileSystemWatcher"/> 
    /// by adding the full paths of files that are created and removing those that are deleted, and also adds the full paths to all 
    /// of the files that initially exist within the scope of the specified <see cref="FileSystemWatcher"/>.</returns>
    public static ReadOnlyListSubject<TResult> Collect<TResult>(
      this FileSystemWatcher watcher,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector,
      IScheduler scheduler)
    {
      Contract.Requires(watcher != null);
      Contract.Requires(selector != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      return watcher.CollectInternal(false, selector, scheduler);
    }

    internal static ReadOnlyListSubject<TResult> CollectInternal<TResult>(
      this FileSystemWatcher watcher,
      bool composited,
      Func<IObservable<CollectionNotification<string>>, IObservable<CollectionModification<TResult>>> selector,
      IScheduler scheduler)
    {
      Contract.Requires(watcher != null);
      Contract.Requires(selector != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<ReadOnlyListSubject<TResult>>() != null);

      var existing = Directory
        .EnumerateFiles(
          watcher.Path,
          watcher.Filter,
          watcher.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

      var changes = watcher
        .Watch(WatcherChangeTypes.Created | WatcherChangeTypes.Deleted | WatcherChangeTypes.Renamed)
        .SelectMany(notification => notification.Change == WatcherChangeTypes.Created
          ? Observable.Return(CollectionModification.CreateAdd(notification.FullPath))
          : notification.Change == WatcherChangeTypes.Deleted
            ? Observable.Return(CollectionModification.CreateRemove(notification.FullPath))
            : Observable.Return(CollectionModification.CreateRemove(notification.OldFullPath)).Concat(Observable.Return(CollectionModification.CreateAdd(notification.FullPath))));

      if (composited)
      {
        changes = changes.Finally(watcher.Dispose);
      }

      return existing.ToObservable(scheduler).ToListObservable(changes.ObserveOn(scheduler), selector, StringComparer.OrdinalIgnoreCase);
    }
  }
}