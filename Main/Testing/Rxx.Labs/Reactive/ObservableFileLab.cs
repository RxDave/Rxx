using System;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("Observable File")]
  [Description("Observing bytes as they are appended to a file.")]
  public sealed class ObservableFileLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);

#if SILVERLIGHT
			const string file = "ObservableFileLab.bin";
#else
      string file = Path.GetTempFileName();
#endif

      try
      {
#if SILVERLIGHT
				using (var writer = Storage.OpenFile(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				using (Stream reader = Storage.OpenFile(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				using (reader.ToObservable(Observable.Interval(TimeSpan.FromSeconds(1.1)))
					.Subscribe(
						ConsoleOutputOnNext<byte[]>(Text.Reader, bytes => bytes[0].ToString()),
						ConsoleOutputOnError()))
#else
        using (FileStream writer = File.Open(file, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        using (ObservableFile.Watch(file).Subscribe(
          ConsoleOutputOnNext<byte[]>(Text.Reader, bytes => bytes[0].ToString()),
          ConsoleOutputOnError()))
#endif
        using (Observable.Interval(TimeSpan.FromSeconds(1))
          .Take(5)
          .Subscribe(
            ConsoleOutputOnNext<long>(
              Text.Writer,
              value =>
              {
                writer.WriteByte((byte)value);

#if WINDOWS_PHONE
								writer.Flush();
#else
                writer.Flush(flushToDisk: true);
#endif

                return value.ToString();
              }),
            ConsoleOutputOnError(Text.Writer),
            ConsoleOutputOnCompleted(Text.Writer)))
        {
          WaitForKey();
        }
      }
      finally
      {
#if SILVERLIGHT
				Storage.DeleteFile(file);
#else
        File.Delete(file);
#endif
      }
    }
  }
}