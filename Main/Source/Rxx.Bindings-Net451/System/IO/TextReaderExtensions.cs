using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;

namespace System.IO
{
  /// <summary>
  /// Provides <see langword="static" /> extension methods for <see cref="TextReader"/> objects.
  /// </summary>
  public static class TextReaderExtensions
  {
    /// <summary>
    /// Creates an observable sequence by reading to the end of the specified <paramref name="reader"/> each time 
    /// the <paramref name="textAvailable"/> sequence notifies that additional text is available to be read
    /// and advances the position within the reader to the end of the stream.
    /// </summary>
    /// <typeparam name="TOther">The type of elements in the sequence that notifies when text is available to be read.</typeparam>
    /// <param name="reader">The object from which text is read as it becomes available.</param>
    /// <param name="textAvailable">An observable sequence that notifies when additional text is available to be read.</param>
    /// <remarks>
    /// The generated sequence is intended to match the underlying stream; however, this behavior 
    /// depends on whether the reader is well-behaved and whether the reader is not being shared.  Reading always starts from the 
    /// current position of the reader in the underlying stream.  The reader is expected to increment its position in the stream
    /// when it's read.  Each time that the <paramref name="textAvailable"/> sequence notifies that additional text is available, 
    /// reading is expected to begin at the previous position in the stream, but if the reader is shared or it's not well-behaved, 
    /// then the generated sequence may contain unexpected data.
    /// </remarks>
    /// <returns>An observable sequence of strings read from the specified <paramref name="reader"/>.</returns>
    public static IObservable<string> ToObservable<TOther>(this TextReader reader, IObservable<TOther> textAvailable)
    {
      Contract.Requires(reader != null);
      Contract.Requires(textAvailable != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return reader.ToObservable(textAvailable, Scheduler.CurrentThread);
    }

    /// <summary>
    /// Creates an observable sequence by reading to the end of the specified <paramref name="reader"/> each time 
    /// the <paramref name="textAvailable"/> sequence notifies that additional text is available to be read
    /// and advances the position within the reader to the end of the stream.
    /// </summary>
    /// <typeparam name="TOther">The type of elements in the sequence that notifies when text is available to be read.</typeparam>
    /// <param name="reader">The object from which text is read as it becomes available.</param>
    /// <param name="textAvailable">An observable sequence that notifies when additional text is available to be read.</param>
    /// <param name="scheduler">An object used to schedule reads.</param>
    /// <remarks>
    /// The generated sequence is intended to match the underlying stream; however, this behavior 
    /// depends on whether the reader is well-behaved and whether the reader is not being shared.  Reading always starts from the 
    /// current position of the reader in the underlying stream.  The reader is expected to increment its position in the stream
    /// when it's read.  Each time that the <paramref name="textAvailable"/> sequence notifies that additional text is available, 
    /// reading is expected to begin at the previous position in the stream, but if the reader is shared or it's not well-behaved, 
    /// then the generated sequence may contain unexpected data.
    /// </remarks>
    /// <returns>An observable sequence of strings read from the specified <paramref name="reader"/>.</returns>
    public static IObservable<string> ToObservable<TOther>(this TextReader reader, IObservable<TOther> textAvailable, IScheduler scheduler)
    {
      Contract.Requires(reader != null);
      Contract.Requires(textAvailable != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return textAvailable.Consume(
        Observable.Defer(() =>
          Observable.Start(() => reader.ReadToEnd(), scheduler)
            .Where(value => !string.IsNullOrEmpty(value))));
    }

    /// <summary>
    /// Creates an observable sequence by reading lines to the end of the specified <paramref name="reader"/> each time 
    /// the <paramref name="textAvailable"/> sequence notifies that additional text is available to be read
    /// and advances the position within the reader to the end of the stream.
    /// </summary>
    /// <typeparam name="TOther">The type of elements in the sequence that notifies when text is available to be read.</typeparam>
    /// <param name="reader">The object from which lines are read as they become available.</param>
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
    /// The generated sequence is intended to match the underlying stream; however, this behavior 
    /// depends on whether the reader is well-behaved and whether the reader is not being shared.  Reading always starts from the 
    /// current position of the reader in the underlying stream.  The reader is expected to increment its position in the stream
    /// when it's read.  Each time that the <paramref name="textAvailable"/> sequence notifies that additional text is available, 
    /// reading is expected to begin at the previous position in the stream, but if the reader is shared or it's not well-behaved, 
    /// then the generated sequence may contain unexpected data.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence of lines read from the specified <paramref name="reader"/>.</returns>
    public static IObservable<string> ToObservableLines<TOther>(this TextReader reader, IObservable<TOther> textAvailable)
    {
      Contract.Requires(reader != null);
      Contract.Requires(textAvailable != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      return reader.ToObservableLines(textAvailable, Scheduler.CurrentThread);
    }

    /// <summary>
    /// Creates an observable sequence by reading lines to the end of the specified <paramref name="reader"/> each time 
    /// the <paramref name="textAvailable"/> sequence notifies that additional text is available to be read
    /// and advances the position within the reader to the end of the stream.
    /// </summary>
    /// <typeparam name="TOther">The type of elements in the sequence that notifies when text is available to be read.</typeparam>
    /// <param name="reader">The object from which lines are read as they become available.</param>
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
    /// The generated sequence is intended to match the underlying stream; however, this behavior 
    /// depends on whether the reader is well-behaved and whether the reader is not being shared.  Reading always starts from the 
    /// current position of the reader in the underlying stream.  The reader is expected to increment its position in the stream
    /// when it's read.  Each time that the <paramref name="textAvailable"/> sequence notifies that additional text is available, 
    /// reading is expected to begin at the previous position in the stream, but if the reader is shared or it's not well-behaved, 
    /// then the generated sequence may contain unexpected data.
    /// </para>
    /// </remarks>
    /// <returns>An observable sequence of lines read from the specified <paramref name="reader"/>.</returns>
    public static IObservable<string> ToObservableLines<TOther>(this TextReader reader, IObservable<TOther> textAvailable, IScheduler scheduler)
    {
      Contract.Requires(reader != null);
      Contract.Requires(textAvailable != null);
      Contract.Requires(scheduler != null);
      Contract.Ensures(Contract.Result<IObservable<string>>() != null);

      var buffer = new char[1024];
      var remainder = new StringBuilder(1024);
      var lines = new Queue<Tuple<int, int>>();
      var ignoreLeadingNewLine = false;

      return textAvailable.Consume(
        _ =>
        {
          if (lines.Count > 0)
          {
            var line = lines.Dequeue();

            return Maybe.Return(new string(buffer, line.Item1, line.Item2));
          }

          int read = reader.Read(buffer, 0, buffer.Length);
          int nextLineStart = 0;

          if (read > 0 && ignoreLeadingNewLine)
          {
            if (buffer[0] == '\n')
            {
              nextLineStart = 1;
            }

            ignoreLeadingNewLine = false;
          }

          for (int i = nextLineStart; i < read; i++)
          {
            var c = buffer[i];

            switch (c)
            {
              case '\r':
              case '\n':
                lines.Enqueue(Tuple.Create(nextLineStart, i - nextLineStart));

                if (c == '\r')
                {
                  if (i + 1 < read)
                  {
                    if (buffer[i + 1] == '\n')
                    {
                      i++;
                    }
                  }
                  else
                  {
                    ignoreLeadingNewLine = true;
                  }
                }

                nextLineStart = i + 1;
                break;
            }
          }

          Maybe<string> value;

          if (lines.Count > 0)
          {
            var line = lines.Dequeue();

            if (remainder.Length > 0)
            {
              remainder.Append(buffer, line.Item1, line.Item2);

              value = Maybe.Return(remainder.ToString());

              remainder.Length = 0;
            }
            else
            {
              value = Maybe.Return(new string(buffer, line.Item1, line.Item2));
            }
          }
          else
          {
            value = Maybe.Empty<string>();
          }

          if (nextLineStart < read)
          {
            remainder.Append(buffer, nextLineStart, read - nextLineStart);
          }

          return value;
        },
        scheduler)
        .Concat(Observable.Defer(() =>
          remainder.Length > 0
          ? Observable.Return(remainder.ToString(), scheduler)
          : Observable.Empty<string>(scheduler)));
    }
  }
}