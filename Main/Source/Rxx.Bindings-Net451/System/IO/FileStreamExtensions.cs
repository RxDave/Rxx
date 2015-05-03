using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;

namespace System.IO
{
  /// <summary>
  /// Provides <see langword="static" /> extension methods for <see cref="FileStream"/> objects.
  /// </summary>
  public static class FileStreamExtensions
  {
    /// <summary>
    /// Creates an observable sequence of bytes by asynchronously reading to the end of the specified <paramref name="stream"/>
    /// each time the underlying file is updated and advances the position within the stream to the end.
    /// </summary>
    /// <param name="stream">The object from which bytes are read as data becomes available.</param>
    /// <remarks>
    /// <para>
    /// The generated sequence is intended to match the specified stream; however, this behavior 
    /// depends on whether the stream is well-behaved and whether the stream is not being shared.  Reading always starts from the 
    /// current position of the stream.  The stream is expected to increment its position
    /// when it's read.  Each time that the underlying file is updated on disc, reading is expected to begin at the previous position 
    /// in the stream, but if the stream is shared or it's not well-behaved, then the generated sequence may contain unexpected data.
    /// </para>
    /// <para>
    /// Furthermore, specific changes to the file are not detected.  Only the date/time of the last write is monitored for changes.
    /// All changes to the date/time of the last write to the file are assumed to indicate that new data has been appended to the 
    /// end of the file.  If data is overwritten in the file after it has already been read from the specified stream, then the last
    /// write date/time will be updated causing the stream to be read again, although it will already be at the end of the file so 
    /// nothing will happen.  It is the responsibility of consumers to prevent the file from being deleted, truncated or mutated in 
    /// any other way that may cause I/O errors or may cause the generated sequence to contain unexpected data.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence of byte arrays of a default maximum size read from the specified <paramref name="stream"/>.</returns>
    public static IObservable<byte[]> ToObservable(this FileStream stream)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return stream.ToObservable(StreamExtensions.defaultBufferSize);
    }

    /// <summary>
    /// Creates an observable sequence of bytes by asynchronously reading to the end of the specified <paramref name="stream"/>
    /// each time the underlying file is updated and advances the position within the stream to the end.
    /// </summary>
    /// <param name="stream">The object from which bytes are read as data becomes available.</param>
    /// <param name="bufferSize">The maximum length of each byte array that is read.</param>
    /// <remarks>
    /// <para>
    /// The generated sequence is intended to match the specified stream; however, this behavior 
    /// depends on whether the stream is well-behaved and whether the stream is not being shared.  Reading always starts from the 
    /// current position of the stream.  The stream is expected to increment its position
    /// when it's read.  Each time that the underlying file is updated on disc, reading is expected to begin at the previous position 
    /// in the stream, but if the stream is shared or it's not well-behaved, then the generated sequence may contain unexpected data.
    /// </para>
    /// <para>
    /// Furthermore, specific changes to the file are not detected.  Only the date/time of the last write is monitored for changes.
    /// All changes to the date/time of the last write to the file are assumed to indicate that new data has been appended to the 
    /// end of the file.  If data is overwritten in the file after it has already been read from the specified stream, then the last
    /// write date/time will be updated causing the stream to be read again, although it will already be at the end of the file so 
    /// nothing will happen.  It is the responsibility of consumers to prevent the file from being deleted, truncated or mutated in 
    /// any other way that may cause I/O errors or may cause the generated sequence to contain unexpected data.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence of byte arrays of the specified maximum size read from the specified <paramref name="stream"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The FileSystemWatcher is composited in the sequence.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",
      Justification = "Reported false positive: http://stylecop.codeplex.com/discussions/275654")]
    public static IObservable<byte[]> ToObservable(this FileStream stream, int bufferSize)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(bufferSize > 0);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return Observable.Using(
        () => new FileSystemWatcher(Path.GetDirectoryName(stream.Name), Path.GetFileName(stream.Name))
        {
          NotifyFilter = NotifyFilters.LastWrite
        },
        watcher => stream.ToObservable(
          bufferSize,
          watcher.Watch(WatcherChangeTypes.Created | WatcherChangeTypes.Changed)
            .StartWith((FileSystemNotification)null)));
    }

    /// <summary>
    /// Creates an observable sequence by reading to the end of the specified <paramref name="stream"/> 
    /// each time the underlying file is updated and advances the position within the stream to the end.
    /// </summary>
    /// <param name="stream">The object from which text is read as it becomes available.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <remarks>
    /// <para>
    /// The generated sequence is intended to match the specified stream; however, this behavior 
    /// depends on whether the stream is well-behaved and whether the stream is not being shared.  Reading always starts from the 
    /// current position of the stream.  The stream is expected to increment its position
    /// when it's read.  Each time that the underlying file is updated on disc, reading is expected to begin at the previous position 
    /// in the stream, but if the stream is shared or it's not well-behaved, then the generated sequence may contain unexpected data.
    /// </para>
    /// <para>
    /// Furthermore, specific changes to the file are not detected.  Only the date/time of the last write is monitored for changes.
    /// All changes to the date/time of the last write to the file are assumed to indicate that new data has been appended to the 
    /// end of the file.  If data is overwritten in the file after it has already been read from the specified stream, then the last
    /// write date/time will be updated causing the stream to be read again, although it will already be at the end of the file so 
    /// nothing will happen.  It is the responsibility of consumers to prevent the file from being deleted, truncated or mutated in 
    /// any other way that may cause I/O errors or may cause the generated sequence to contain unexpected data.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence of strings read from the specified <paramref name="stream"/>.</returns>
    public static IObservable<string> ToObservable(this FileStream stream, Encoding encoding)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(encoding != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return stream.ToObservable(encoding, TaskPoolScheduler.Default);
    }

    /// <summary>
    /// Creates an observable sequence by reading to the end of the specified <paramref name="stream"/>
    /// each time the underlying file is updated and advances the position within the stream to the end.
    /// </summary>
    /// <param name="stream">The object from which text is read as it becomes available.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="scheduler">An object used to schedule reads.</param>
    /// <remarks>
    /// <para>
    /// The generated sequence is intended to match the specified stream; however, this behavior 
    /// depends on whether the stream is well-behaved and whether the stream is not being shared.  Reading always starts from the 
    /// current position of the stream.  The stream is expected to increment its position
    /// when it's read.  Each time that the underlying file is updated on disc, reading is expected to begin at the previous position 
    /// in the stream, but if the stream is shared or it's not well-behaved, then the generated sequence may contain unexpected data.
    /// </para>
    /// <para>
    /// Furthermore, specific changes to the file are not detected.  Only the date/time of the last write is monitored for changes.
    /// All changes to the date/time of the last write to the file are assumed to indicate that new data has been appended to the 
    /// end of the file.  If data is overwritten in the file after it has already been read from the specified stream, then the last
    /// write date/time will be updated causing the stream to be read again, although it will already be at the end of the file so 
    /// nothing will happen.  It is the responsibility of consumers to prevent the file from being deleted, truncated or mutated in 
    /// any other way that may cause I/O errors or may cause the generated sequence to contain unexpected data.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence of strings read from the specified <paramref name="stream"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The StreamReader is composited in the sequence.")]
    public static IObservable<string> ToObservable(this FileStream stream, Encoding encoding, IScheduler scheduler)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(encoding != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return Observable.Using(
        () => new StreamReader(stream, encoding),
        reader => Observable.Using(
          () => new FileSystemWatcher(Path.GetDirectoryName(stream.Name), Path.GetFileName(stream.Name))
          {
            NotifyFilter = NotifyFilters.LastWrite
          },
          watcher => reader.ToObservable(
            watcher.Watch(WatcherChangeTypes.Created | WatcherChangeTypes.Changed)
              .StartWith((FileSystemNotification)null),
            scheduler)));
    }

    /// <summary>
    /// Creates an observable sequence by reading lines to the end of the specified <paramref name="stream"/> 
    /// each time the underlying file is updated and advances the position within the stream to the end.
    /// </summary>
    /// <param name="stream">The object from which lines are read as they become available.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <remarks>
    /// <alert type="warning">
    /// <para>
    /// This method never calls <strong>OnCompleted</strong> since there's no indication of when writing to the stream has ended.
    /// As a result, the last line in the file is not read unless it ends with new line characters.
    /// </para>
    /// <para>
    /// Until new line characters are read, the last line remains buffered without any indication of when the stream has ended.
    /// Closing the <paramref name="stream"/> will result in an exception being thrown on the next attempt to read from the closed stream, 
    /// although waiting for an exception is unreliable as a completion indicator because the stream may write to the file exclusively, 
    /// in which case closing the stream some time after the last line has been read and buffered will not result in an exception later 
    /// since the internal <see cref="FileSystemWatcher"/> will not raise any additional change events and there will be no further attempts 
    /// to read from the stream.
    /// </para>
    /// <para>
    /// Having an observable parameter or function that indicates when writing to the stream has completed is also unreliable because of
    /// race conditions between the internal <see cref="FileSystemWatcher"/> and the caller's notion of completion.  For example, if the
    /// caller notifies that writing has completed as soon as the last line is written, then the <see cref="FileSystemWatcher"/> may not 
    /// have a chance to read the last line.
    /// </para>
    /// </alert>
    /// <para>
    /// The generated sequence is intended to match the specified stream; however, this behavior 
    /// depends on whether the stream is well-behaved and whether the stream is not being shared.  Reading always starts from the 
    /// current position of the stream.  The stream is expected to increment its position
    /// when it's read.  Each time that the underlying file is updated on disc, reading is expected to begin at the previous position 
    /// in the stream, but if the stream is shared or it's not well-behaved, then the generated sequence may contain unexpected data.
    /// </para>
    /// <para>
    /// Furthermore, specific changes to the file are not detected.  Only the date/time of the last write is monitored for changes.
    /// All changes to the date/time of the last write to the file are assumed to indicate that new data has been appended to the 
    /// end of the file.  If data is overwritten in the file after it has already been read from the specified stream, then the last
    /// write date/time will be updated causing the stream to be read again, although it will already be at the end of the file so 
    /// nothing will happen.  It is the responsibility of consumers to prevent the file from being deleted, truncated or mutated in 
    /// any other way that may cause I/O errors or may cause the generated sequence to contain unexpected data.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence of lines read from the specified <paramref name="stream"/>.</returns>
    public static IObservable<string> ToObservableLines(this FileStream stream, Encoding encoding)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(encoding != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return stream.ToObservableLines(encoding, TaskPoolScheduler.Default);
    }

    /// <summary>
    /// Creates an observable sequence by reading lines to the end of the specified <paramref name="stream"/> 
    /// each time the underlying file is updated and advances the position within the stream to the end.
    /// </summary>
    /// <param name="stream">The object from which lines are read as they become available.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="scheduler">An object used to schedule reads.</param>
    /// <remarks>
    /// <alert type="warning">
    /// <para>
    /// This method never calls <strong>OnCompleted</strong> since there's no indication of when writing to the stream has ended.
    /// As a result, the last line in the file is not read unless it ends with new line characters.
    /// </para>
    /// <para>
    /// Until new line characters are read, the last line remains buffered without any indication of when the stream has ended.
    /// Closing the <paramref name="stream"/> will result in an exception being thrown on the next attempt to read from the closed stream, 
    /// although waiting for an exception is unreliable as a completion indicator because the stream may write to the file exclusively, 
    /// in which case closing the stream some time after the last line has been read and buffered will not result in an exception later 
    /// since the internal <see cref="FileSystemWatcher"/> will not raise any additional change events and there will be no further attempts 
    /// to read from the stream.
    /// </para>
    /// <para>
    /// Having an observable parameter or function that indicates when writing to the stream has completed is also unreliable because of
    /// race conditions between the internal <see cref="FileSystemWatcher"/> and the caller's notion of completion.  For example, if the
    /// caller notifies that writing has completed as soon as the last line is written, then the <see cref="FileSystemWatcher"/> may not 
    /// have a chance to read the last line.
    /// </para>
    /// </alert>
    /// <para>
    /// The generated sequence is intended to match the specified stream; however, this behavior 
    /// depends on whether the stream is well-behaved and whether the stream is not being shared.  Reading always starts from the 
    /// current position of the stream.  The stream is expected to increment its position
    /// when it's read.  Each time that the underlying file is updated on disc, reading is expected to begin at the previous position 
    /// in the stream, but if the stream is shared or it's not well-behaved, then the generated sequence may contain unexpected data.
    /// </para>
    /// <para>
    /// Furthermore, specific changes to the file are not detected.  Only the date/time of the last write is monitored for changes.
    /// All changes to the date/time of the last write to the file are assumed to indicate that new data has been appended to the 
    /// end of the file.  If data is overwritten in the file after it has already been read from the specified stream, then the last
    /// write date/time will be updated causing the stream to be read again, although it will already be at the end of the file so 
    /// nothing will happen.  It is the responsibility of consumers to prevent the file from being deleted, truncated or mutated in 
    /// any other way that may cause I/O errors or may cause the generated sequence to contain unexpected data.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence of lines read from the specified <paramref name="stream"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The StreamReader is composited in the sequence.")]
    public static IObservable<string> ToObservableLines(this FileStream stream, Encoding encoding, IScheduler scheduler)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(encoding != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return Observable.Using(
        () => new StreamReader(stream, encoding),
        reader => Observable.Using(
          () => new FileSystemWatcher(Path.GetDirectoryName(stream.Name), Path.GetFileName(stream.Name))
          {
            NotifyFilter = NotifyFilters.LastWrite
          },
          watcher => reader.ToObservableLines(
            watcher.Watch(WatcherChangeTypes.Created | WatcherChangeTypes.Changed)
              .StartWith((FileSystemNotification)null),
            scheduler)));
    }
  }
}