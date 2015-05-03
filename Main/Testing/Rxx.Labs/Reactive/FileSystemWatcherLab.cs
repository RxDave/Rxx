using System;
using System.ComponentModel;
using System.IO;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("FileSystemWatcher")]
  [Description("Observing file system change events.")]
  public sealed class FileSystemWatcherLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);

      string path = Environment.ExpandEnvironmentVariables(@"%systemdrive%\");

      using (var watcher = new FileSystemWatcher(path))
      {
        watcher.InternalBufferSize = 0;		// Not recommended; used here for testing only.
        watcher.IncludeSubdirectories = true;
        watcher.NotifyFilter =
            NotifyFilters.FileName
          | NotifyFilters.DirectoryName
          | NotifyFilters.Size
          | NotifyFilters.CreationTime
          | NotifyFilters.LastWrite
          | NotifyFilters.LastAccess
          | NotifyFilters.Attributes
          | NotifyFilters.Security;

        using (watcher
          .Watch(WatcherChangeTypes.All)
          .Subscribe(ConsoleOutput()))
        {
          WaitForKey();
        }
      }
    }
  }
}
