using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
#if SILVERLIGHT || UNIVERSAL || PORT_40
using System.Reactive.Threading.Tasks;
#endif
using System.Text;
#if SILVERLIGHT || UNIVERSAL || PORT_40
using System.Threading.Tasks;
#endif

namespace System.IO
{
  /// <summary>
  /// Provides <see langword="static" /> extension methods for <see cref="Stream"/> objects.
  /// </summary>
  public static class StreamExtensions
  {
    internal const int defaultBufferSize = 4096;

    internal static IObservable<byte[]> ReadObservable(this Stream stream, byte[] buffer)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(buffer != null);
      Contract.Requires(buffer.Length > 0);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return stream.ReadObservable(buffer, 0, buffer.Length).Select(
        read =>
        {
          byte[] data;

          if (read <= 0)
          {
            data = new byte[0];
          }
          else if (read == buffer.Length)
          {
            data = (byte[])buffer.Clone();
          }
          else
          {
            data = new byte[read];

            Array.Copy(buffer, data, read);
          }

          return data;
        });
    }

    internal static IObservable<byte[]> ReadToEndObservable(this Stream stream, byte[] buffer)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(buffer != null);
      Contract.Requires(buffer.Length > 0);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return Observable.Create<byte[]>(
        observer =>
        {
          var subscription = new SerialDisposable();

          return new CompositeDisposable(
            subscription,
            Scheduler.Immediate.Schedule(
              self =>
              {
                bool continueReading = true;

                subscription.SetDisposableIndirectly(() =>
                  stream.ReadObservable(buffer).SubscribeSafe(
                    data =>
                    {
                      if (data.Length > 0)
                      {
                        observer.OnNext(data);
                      }
                      else
                      {
                        continueReading = false;
                      }
                    },
                    observer.OnError,
                    () =>
                    {
                      if (continueReading)
                      {
                        self();
                      }
                      else
                      {
                        observer.OnCompleted();
                      }
                    }));
              }));
        });
    }

    /// <summary>
    /// Converts the specified <paramref name="stream"/> into a sequence of bytes, starting from the current 
    /// position of the stream and advancing the position as the sequence is iterated.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to be read.</param>
    /// <param name="bufferSize">The number of bytes to be read as buffered blocks.</param>
    /// <returns>A sequence of bytes from the specified <paramref name="stream"/>, starting from the current
    /// position of the stream.</returns>
    public static IEnumerable<byte> ToEnumerable(this Stream stream, int bufferSize)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(bufferSize > 0);
      Contract.Ensures(Contract.Result<IEnumerable<byte>>() != null);

      var bytes = new byte[bufferSize];

      while (stream.Read(bytes, 0, bytes.Length) > 0)
      {
        for (int i = 0; i < bytes.Length; i++)
        {
          yield return bytes[i];
        }
      }
    }

    /// <summary>
    /// Converts the specified <paramref name="stream"/> into a sequence of bytes, starting from the current 
    /// position of the stream and advancing the position as the sequence is iterated.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to be read.</param>
    /// <returns>A sequence of bytes from the specified <paramref name="stream"/>, starting from the current
    /// position of the stream.</returns>
    public static IEnumerable<byte> ToEnumerable(this Stream stream)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Ensures(Contract.Result<IEnumerable<byte>>() != null);

      int value;
      while ((value = stream.ReadByte()) > -1)
      {
        yield return (byte)value;
      }
    }

    /// <summary>
    /// Writes a section of bytes asynchronously from the specified <paramref name="buffer"/> into the specified <paramref name="stream"/> 
    /// and advances the position within the stream by the number of bytes written.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to which the <paramref name="buffer"/> is written.</param>
    /// <param name="buffer">The buffer to write data from.</param>
    /// <param name="offset">The byte offset in <paramref name="buffer"/> from which to begin writing.</param>
    /// <param name="count">The maximum number of bytes to write.</param>
    /// <returns>A scalar observable sequence that indicates when the data has been written.</returns>
    public static IObservable<Unit> WriteObservable(this Stream stream, byte[] buffer, int offset, int count)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanWrite);
      Contract.Requires(buffer != null);
      Contract.Requires(offset >= 0);
      Contract.Requires(count >= 0);
      Contract.Requires(offset + count <= buffer.Length);
      Contract.Ensures(Contract.Result<IObservable<Unit>>() != null);

#if PORT_40
      return Task.Factory.FromAsync<byte[], int, int>(
        stream.BeginWrite,
        stream.EndWrite,
        buffer,
        offset,
        count,
        state: null)
        .ToObservable();
#elif !SILVERLIGHT || UNIVERSAL
      return Observable.StartAsync(cancel => stream.WriteAsync(buffer, offset, count, cancel));
#else
      return Task.Factory.FromAsync<byte[], int, int>(
        stream.BeginWrite,
        stream.EndWrite,
        buffer,
        offset,
        count,
        state: null)
        .ToObservable();
#endif
    }

    /// <summary>
    /// Reads bytes asynchronously from the specified <paramref name="stream"/> into the specified <paramref name="buffer"/> 
    /// and advances the position within the stream by the number of bytes read.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to be read.</param>
    /// <param name="buffer">The buffer to read the data into.</param>
    /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing data read from the <paramref name="stream"/>.</param>
    /// <param name="count">The maximum number of bytes to read.</param>
    /// <returns>A scalar observable sequence containing the total number of bytes read into the buffer.  This can be 
    /// less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end
    /// of the stream has been reached.</returns>
    public static IObservable<int> ReadObservable(this Stream stream, byte[] buffer, int offset, int count)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(buffer != null);
      Contract.Requires(offset >= 0);
      Contract.Requires(count >= 0);
      Contract.Requires(offset + count <= buffer.Length);
      Contract.Ensures(Contract.Result<IObservable<int>>() != null);

#if !SILVERLIGHT || UNIVERSAL
      return Observable.StartAsync(cancel => stream.ReadAsync(buffer, offset, count, cancel));
#else
      return Task.Factory.FromAsync<byte[], int, int, int>(
        stream.BeginRead,
        stream.EndRead,
        buffer,
        offset,
        count,
        state: null)
        .ToObservable();
#endif
    }

    /// <summary>
    /// Reads bytes asynchronously from the specified <paramref name="stream"/> and advances the position within the stream
    /// by the number of bytes read.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to be read.</param>
    /// <param name="count">The maximum number of bytes to read.</param>
    /// <returns>A scalar observable sequence containing the byte array that is read.  The length of the array can be 
    /// less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end 
    /// of the stream has been reached.</returns>
    public static IObservable<byte[]> ReadObservable(this Stream stream, int count)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(count > 0);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return stream.ReadObservable(new byte[count]);
    }

    /// <summary>
    /// Creates an observable sequence by asynchronously reading bytes from the current position to the end of the specified <paramref name="stream"/>
    /// and advances the position within the stream to the end.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to be read.</param>
    /// <returns>An observable sequence of byte arrays of a default maximum size read from the current position to the end of the 
    /// specified <paramref name="stream"/>.</returns>
    public static IObservable<byte[]> ReadToEndObservable(this Stream stream)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return stream.ReadToEndObservable(new byte[defaultBufferSize]);
    }

    /// <summary>
    /// Creates an observable sequence by asynchronously reading bytes from the current position to the end of the specified <paramref name="stream"/>
    /// and advances the position within the stream to the end.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to be read.</param>
    /// <param name="bufferSize">The maximum length of each byte array that is read.</param>
    /// <returns>An observable sequence of byte arrays of the specified maximum size read from the current position to the end of the 
    /// specified <paramref name="stream"/>.</returns>
    public static IObservable<byte[]> ReadToEndObservable(this Stream stream, int bufferSize)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(bufferSize > 0);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return stream.ReadToEndObservable(new byte[bufferSize]);
    }

    /// <summary>
    /// Creates an observable sequence of bytes by asynchronously reading to the end of the specified <paramref name="stream"/> each time 
    /// the <paramref name="dataAvailable"/> sequence notifies that additional data is available to be read
    /// and advances the position within the stream to the end.
    /// </summary>
    /// <typeparam name="TOther">The type of elements in the sequence that notifies when data is available to be read.</typeparam>
    /// <param name="stream">The object from which bytes are read as data becomes available.</param>
    /// <param name="dataAvailable">An observable sequence that notifies when additional data is available to be read.</param>
    /// <remarks>
    /// The generated sequence is intended to match the specified stream; however, this behavior 
    /// depends on whether the stream is well-behaved and whether the stream is not being shared.  Reading always starts from the 
    /// current position of the stream.  The stream is expected to increment its position
    /// when it's read.  Each time that the <paramref name="dataAvailable"/> sequence notifies that additional data is available, 
    /// reading is expected to begin at the previous position in the stream, but if the stream is shared or it's not well-behaved, 
    /// then the generated sequence may contain unexpected data.
    /// </remarks>
    /// <returns>An observable sequence of byte arrays of a default maximum size read from the specified <paramref name="stream"/>.</returns>
    public static IObservable<byte[]> ToObservable<TOther>(this Stream stream, IObservable<TOther> dataAvailable)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(dataAvailable != null);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return stream.ToObservable(defaultBufferSize, dataAvailable);
    }

    /// <summary>
    /// Creates an observable sequence of bytes by asynchronously reading to the end of the specified <paramref name="stream"/> each time 
    /// the <paramref name="dataAvailable"/> sequence notifies that additional data is available to be read
    /// and advances the position within the stream to the end.
    /// </summary>
    /// <typeparam name="TOther">The type of elements in the sequence that notifies when data is available to be read.</typeparam>
    /// <param name="stream">The object from which bytes are read as data becomes available.</param>
    /// <param name="bufferSize">The maximum length of each byte array that is read.</param>
    /// <param name="dataAvailable">An observable sequence that notifies when additional data is available to be read.</param>
    /// <remarks>
    /// The generated sequence is intended to match the specified stream; however, this behavior 
    /// depends on whether the stream is well-behaved and whether the stream is not being shared.  Reading always starts from the 
    /// current position of the stream.  The stream is expected to increment its position
    /// when it's read.  Each time that the <paramref name="dataAvailable"/> sequence notifies that additional data is available, 
    /// reading is expected to begin at the previous position in the stream, but if the stream is shared or it's not well-behaved, 
    /// then the generated sequence may contain unexpected data.
    /// </remarks>
    /// <returns>An observable sequence of byte arrays of the specified maximum size read from the specified <paramref name="stream"/>.</returns>
    public static IObservable<byte[]> ToObservable<TOther>(this Stream stream, int bufferSize, IObservable<TOther> dataAvailable)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(bufferSize > 0);
      Contract.Requires(dataAvailable != null);
      Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

      return Observable.Defer(() =>
        {
          var buffer = new byte[bufferSize];

          return dataAvailable.Consume(stream.ReadToEndObservable(buffer));
        });
    }

    /// <summary>
    /// Creates an observable sequence by asynchronously reading to the end of the specified <paramref name="stream"/> each time 
    /// the <paramref name="textAvailable"/> sequence notifies that additional text is available to be read
    /// and advances the position within the stream to the end.
    /// </summary>
    /// <typeparam name="TOther">The type of elements in the sequence that notifies when text is available to be read.</typeparam>
    /// <param name="stream">The object from which text is read as it becomes available.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="textAvailable">An observable sequence that notifies when additional text is available to be read.</param>
    /// <remarks>
    /// The generated sequence is intended to match the specified stream; however, this behavior 
    /// depends on whether the stream is well-behaved and whether the stream is not being shared.  Reading always starts from the 
    /// current position of the stream.  The stream is expected to increment its position
    /// when it's read.  Each time that the <paramref name="textAvailable"/> sequence notifies that additional data is available, 
    /// reading is expected to begin at the previous position in the stream, but if the stream is shared or it's not well-behaved, 
    /// then the generated sequence may contain unexpected data.
    /// </remarks>
    /// <returns>An observable sequence of strings read from the specified <paramref name="stream"/>.</returns>
    public static IObservable<string> ToObservable<TOther>(this Stream stream, Encoding encoding, IObservable<TOther> textAvailable)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(encoding != null);
      Contract.Requires(textAvailable != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return stream.ToObservable(encoding, textAvailable, Scheduler.CurrentThread);
    }

    /// <summary>
    /// Creates an observable sequence by asynchronously reading to the end of the specified <paramref name="stream"/> each time 
    /// the <paramref name="textAvailable"/> sequence notifies that additional text is available to be read
    /// and advances the position within the stream to the end.
    /// </summary>
    /// <typeparam name="TOther">The type of elements in the sequence that notifies when text is available to be read.</typeparam>
    /// <param name="stream">The object from which text is read as it becomes available.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="textAvailable">An observable sequence that notifies when additional text is available to be read.</param>
    /// <param name="scheduler">An object used to schedule reads.</param>
    /// <remarks>
    /// The generated sequence is intended to match the specified stream; however, this behavior 
    /// depends on whether the stream is well-behaved and whether the stream is not being shared.  Reading always starts from the 
    /// current position of the stream.  The stream is expected to increment its position
    /// when it's read.  Each time that the <paramref name="textAvailable"/> sequence notifies that additional data is available, 
    /// reading is expected to begin at the previous position in the stream, but if the stream is shared or it's not well-behaved, 
    /// then the generated sequence may contain unexpected data.
    /// </remarks>
    /// <returns>An observable sequence of strings read from the specified <paramref name="stream"/>.</returns>
    public static IObservable<string> ToObservable<TOther>(this Stream stream, Encoding encoding, IObservable<TOther> textAvailable, IScheduler scheduler)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(encoding != null);
      Contract.Requires(textAvailable != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return Observable.Using(
        () => new StreamReader(stream, encoding),
        reader => reader.ToObservable(textAvailable, scheduler));
    }

    /// <summary>
    /// Creates an observable sequence by reading lines to the end of the specified <paramref name="stream"/> each time 
    /// the <paramref name="textAvailable"/> sequence notifies that additional text is available to be read
    /// and advances the position within the stream to the end.
    /// </summary>
    /// <typeparam name="TOther">The type of elements in the sequence that notifies when text is available to be read.</typeparam>
    /// <param name="stream">The object from which lines are read as they become available.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="textAvailable">An observable sequence that notifies when additional text is available to be read.</param>
    /// <remarks>
    /// <para>
    /// The <paramref name="textAvailable"/> sequence does not have to notify when new lines are available.  It only must notify when 
    /// new text is available, which may or may not have new lines.  Characters that are read up to the end of the stream are automatically
    /// buffered until a new line sequence is encountered in a subsequent read.  A consequence of this behavior is that if the stream does
    /// not end with a new line sequence and it's not going to receive any more text, then the last line will not be read until 
    /// <paramref name="textAvailable"/> calls <strong>OnCompleted</strong>.
    /// </para>
    /// <para>
    /// The generated sequence is intended to match the specified stream; however, this behavior 
    /// depends on whether the stream is well-behaved and whether the stream is not being shared.  Reading always starts from the 
    /// current position of the stream.  The stream is expected to increment its position
    /// when it's read.  Each time that the <paramref name="textAvailable"/> sequence notifies that additional text is available, 
    /// reading is expected to begin at the previous position in the stream, but if the stream is shared or it's not well-behaved, 
    /// then the generated sequence may contain unexpected data.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence of lines read from the specified <paramref name="stream"/>.</returns>
    public static IObservable<string> ToObservableLines<TOther>(this Stream stream, Encoding encoding, IObservable<TOther> textAvailable)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(encoding != null);
      Contract.Requires(textAvailable != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return stream.ToObservableLines(encoding, textAvailable, Scheduler.CurrentThread);
    }

    /// <summary>
    /// Creates an observable sequence by reading lines to the end of the specified <paramref name="stream"/> each time 
    /// the <paramref name="textAvailable"/> sequence notifies that additional text is available to be read
    /// and advances the position within the stream to the end.
    /// </summary>
    /// <typeparam name="TOther">The type of elements in the sequence that notifies when text is available to be read.</typeparam>
    /// <param name="stream">The object from which lines are read as they become available.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="textAvailable">An observable sequence that notifies when additional text is available to be read.</param>
    /// <param name="scheduler">An object used to schedule reads.</param>
    /// <remarks>
    /// <para>
    /// The <paramref name="textAvailable"/> sequence does not have to notify when new lines are available.  It only must notify when 
    /// new text is available, which may or may not have new lines.  Characters that are read up to the end of the stream are automatically
    /// buffered until a new line sequence is encountered in a subsequent read.  A consequence of this behavior is that if the stream does
    /// not end with a new line sequence and it's not going to receive any more text, then the last line will not be read until 
    /// <paramref name="textAvailable"/> calls <strong>OnCompleted</strong>.
    /// </para>
    /// <para>
    /// The generated sequence is intended to match the specified stream; however, this behavior 
    /// depends on whether the stream is well-behaved and whether the stream is not being shared.  Reading always starts from the 
    /// current position of the stream.  The stream is expected to increment its position
    /// when it's read.  Each time that the <paramref name="textAvailable"/> sequence notifies that additional text is available, 
    /// reading is expected to begin at the previous position in the stream, but if the stream is shared or it's not well-behaved, 
    /// then the generated sequence may contain unexpected data.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence of lines read from the specified <paramref name="stream"/>.</returns>
    public static IObservable<string> ToObservableLines<TOther>(this Stream stream, Encoding encoding, IObservable<TOther> textAvailable, IScheduler scheduler)
    {
      Contract.Requires(stream != null);
      Contract.Requires(stream.CanRead);
      Contract.Requires(encoding != null);
      Contract.Requires(textAvailable != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return Observable.Using(
        () => new StreamReader(stream, encoding),
        reader => reader.ToObservableLines(textAvailable, scheduler));
    }
  }
}