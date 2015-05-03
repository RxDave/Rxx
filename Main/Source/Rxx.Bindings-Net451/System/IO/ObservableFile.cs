using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;

namespace System.IO
{
  /// <summary>
  /// Provides <see langword="static" /> methods for monitoring files.
  /// </summary>
  public static class ObservableFile
  {
    /// <summary>
    /// Creates an observable sequence of bytes by asynchronously reading from the specified <paramref name="file"/> when data is appended.
    /// </summary>
    /// <param name="file">The path to a file from which bytes are read as data becomes available.</param>
    /// <remarks>
    /// The generated sequence is intended to match the contents of the specified file;
    /// however, specific changes to the file are not detected.  Only the date/time of the last write is monitored for changes.
    /// All changes to the date/time of the last write to the file are assumed to indicate that new data has been appended to the 
    /// end of the file.  Data that is overwritten in the file after it has already been read does not affect the generated sequence
    /// as long as the overwrite doesn't exceed the end of the file.  It is the responsibility of consumers to prevent the file from 
    /// being deleted, truncated or mutated in any other way that may cause I/O errors or may cause the generated sequence to contain 
    /// unexpected data.
    /// </remarks>
    /// <returns>An observable sequence of byte arrays of a default maximum size read from the specified <paramref name="file"/>.</returns>
    public static IObservable<byte[]> Watch(string file)
    {
      Contract.Requires(file != null);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return Observable.Using(
        () => File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
        stream => stream.ToObservable());
    }

    /// <summary>
    /// Creates an observable sequence of bytes by asynchronously reading from the specified <paramref name="file"/> when data is appended.
    /// </summary>
    /// <param name="file">The path to a file from which bytes are read as data becomes available.</param>
    /// <param name="bufferSize">The maximum length of each byte array that is read.</param>
    /// <remarks>
    /// The generated sequence is intended to match the contents of the specified file;
    /// however, specific changes to the file are not detected.  Only the date/time of the last write is monitored for changes.
    /// All changes to the date/time of the last write to the file are assumed to indicate that new data has been appended to the 
    /// end of the file.  Data that is overwritten in the file after it has already been read does not affect the generated sequence
    /// as long as the overwrite doesn't exceed the end of the file.  It is the responsibility of consumers to prevent the file from 
    /// being deleted, truncated or mutated in any other way that may cause I/O errors or may cause the generated sequence to contain 
    /// unexpected data.
    /// </remarks>
    /// <returns>An observable sequence of byte arrays of the specified maximum size read from the specified <paramref name="file"/>.</returns>
    public static IObservable<byte[]> Watch(string file, int bufferSize)
    {
      Contract.Requires(file != null);
      Contract.Requires(bufferSize > 0);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return Observable.Using(
        () => File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
        stream => stream.ToObservable(bufferSize));
    }

    /// <summary>
    /// Creates an observable sequence by reading from the specified <paramref name="file"/> when data is appended.
    /// </summary>
    /// <param name="file">The path to a file from which text is read as it becomes available.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <remarks>
    /// The generated sequence is intended to match the contents of the specified file;
    /// however, specific changes to the file are not detected.  Only the date/time of the last write is monitored for changes.
    /// All changes to the date/time of the last write to the file are assumed to indicate that new data has been appended to the 
    /// end of the file.  Data that is overwritten in the file after it has already been read does not affect the generated sequence
    /// as long as the overwrite doesn't exceed the end of the file.  It is the responsibility of consumers to prevent the file from 
    /// being deleted, truncated or mutated in any other way that may cause I/O errors or may cause the generated sequence to contain 
    /// unexpected data.
    /// </remarks>
    /// <returns>An observable sequence of strings read from the specified <paramref name="file"/>.</returns>
    public static IObservable<string> Watch(string file, Encoding encoding)
    {
      Contract.Requires(file != null);
      Contract.Requires(encoding != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return Observable.Using(
        () => File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
        stream => stream.ToObservable(encoding));
    }

    /// <summary>
    /// Creates an observable sequence by reading from the specified <paramref name="file"/> when data is appended.
    /// </summary>
    /// <param name="file">The path to a file from which text is read as it becomes available.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="scheduler">An object used to schedule reads.</param>
    /// <remarks>
    /// The generated sequence is intended to match the contents of the specified file;
    /// however, specific changes to the file are not detected.  Only the date/time of the last write is monitored for changes.
    /// All changes to the date/time of the last write to the file are assumed to indicate that new data has been appended to the 
    /// end of the file.  Data that is overwritten in the file after it has already been read does not affect the generated sequence
    /// as long as the overwrite doesn't exceed the end of the file.  It is the responsibility of consumers to prevent the file from 
    /// being deleted, truncated or mutated in any other way that may cause I/O errors or may cause the generated sequence to contain 
    /// unexpected data.
    /// </remarks>
    /// <returns>An observable sequence of strings read from the specified <paramref name="file"/>.</returns>
    public static IObservable<string> Watch(string file, Encoding encoding, IScheduler scheduler)
    {
      Contract.Requires(file != null);
      Contract.Requires(encoding != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return Observable.Using(
        () => File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
        stream => stream.ToObservable(encoding, scheduler));
    }

    /// <summary>
    /// Creates an observable sequence by reading lines from the specified <paramref name="file"/> when data is appended.
    /// </summary>
    /// <param name="file">The path to a file from which lines are read as they become available.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <remarks>
    /// <alert type="warning">
    /// <para>
    /// This method never calls <strong>OnCompleted</strong> since there's no indication of when writing to the file has ended.
    /// As a result, the last line in the file is not read unless it ends with new line characters.
    /// </para>
    /// <para>
    /// Until new line characters are read, the last line remains buffered without any indication of when the file has ended.
    /// Having an observable parameter or function that indicates when writing to the stream has completed is unreliable because of
    /// race conditions between the internal <see cref="FileSystemWatcher"/> and the caller's notion of completion.  For example, if the
    /// caller notifies that writing has completed as soon as the last line is written, then the <see cref="FileSystemWatcher"/> may not 
    /// have a chance to read the last line.
    /// </para>
    /// </alert>
    /// <para>
    /// The generated sequence is intended to match the contents of the specified file;
    /// however, specific changes to the file are not detected.  Only the date/time of the last write is monitored for changes.
    /// All changes to the date/time of the last write to the file are assumed to indicate that new data has been appended to the 
    /// end of the file.  Data that is overwritten in the file after it has already been read does not affect the generated sequence
    /// as long as the overwrite doesn't exceed the end of the file.  It is the responsibility of consumers to prevent the file from 
    /// being deleted, truncated or mutated in any other way that may cause I/O errors or may cause the generated sequence to contain 
    /// unexpected data.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence of lines read from the specified <paramref name="file"/>.</returns>
    public static IObservable<string> WatchLines(string file, Encoding encoding)
    {
      Contract.Requires(file != null);
      Contract.Requires(encoding != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return Observable.Using(
        () => File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
        stream => stream.ToObservableLines(encoding));
    }

    /// <summary>
    /// Creates an observable sequence by reading lines from the specified <paramref name="file"/> when data is appended.
    /// </summary>
    /// <param name="file">The path to a file from which lines are read as they become available.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="scheduler">An object used to schedule reads.</param>
    /// <remarks>
    /// <alert type="warning">
    /// <para>
    /// This method never calls <strong>OnCompleted</strong> since there's no indication of when writing to the file has ended.
    /// As a result, the last line in the file is not read unless it ends with new line characters.
    /// </para>
    /// <para>
    /// Until new line characters are read, the last line remains buffered without any indication of when the file has ended.
    /// Having an observable parameter or function that indicates when writing to the stream has completed is also unreliable because of
    /// race conditions between the internal <see cref="FileSystemWatcher"/> and the caller's notion of completion.  For example, if the
    /// caller notifies that writing has completed as soon as the last line is written, then the <see cref="FileSystemWatcher"/> may not 
    /// have a chance to read the last line.
    /// </para>
    /// </alert>
    /// <para>
    /// The generated sequence is intended to match the contents of the specified file;
    /// however, specific changes to the file are not detected.  Only the date/time of the last write is monitored for changes.
    /// All changes to the date/time of the last write to the file are assumed to indicate that new data has been appended to the 
    /// end of the file.  Data that is overwritten in the file after it has already been read does not affect the generated sequence
    /// as long as the overwrite doesn't exceed the end of the file.  It is the responsibility of consumers to prevent the file from 
    /// being deleted, truncated or mutated in any other way that may cause I/O errors or may cause the generated sequence to contain 
    /// unexpected data.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence of lines read from the specified <paramref name="file"/>.</returns>
    public static IObservable<string> WatchLines(string file, Encoding encoding, IScheduler scheduler)
    {
      Contract.Requires(file != null);
      Contract.Requires(encoding != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return Observable.Using(
        () => File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
        stream => stream.ToObservableLines(encoding, scheduler));
    }
  }
}