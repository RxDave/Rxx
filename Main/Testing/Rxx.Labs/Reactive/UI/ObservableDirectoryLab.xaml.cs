using System;
using System.ComponentModel;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Interop;

namespace Rxx.Labs.Reactive.UI
{
  [DisplayName("Observable Directory")]
  [Description("Binding a ListBox to a directory and its changes.")]
  public partial class ObservableDirectoryLab : BaseLab, System.Windows.Forms.IWin32Window
  {
    public static readonly DependencyPropertyKey DirectoryProperty = DependencyProperty.RegisterReadOnly(
      "Directory", typeof(ReadOnlyListSubject<string>), typeof(ObservableDirectoryLab), new UIPropertyMetadata());

    public static readonly DependencyPropertyKey PathProperty = DependencyProperty.RegisterReadOnly(
      "Path", typeof(string), typeof(ObservableDirectoryLab), new UIPropertyMetadata());

    public ReadOnlyListSubject<string> Directory
    {
      get
      {
        return (ReadOnlyListSubject<string>)GetValue(DirectoryProperty.DependencyProperty);
      }
      private set
      {
        SetValue(DirectoryProperty, value);
      }
    }

    public string Path
    {
      get
      {
        return (string)GetValue(PathProperty.DependencyProperty);
      }
      private set
      {
        SetValue(PathProperty, value);
      }
    }

    public IntPtr Handle
    {
      get
      {
        return new WindowInteropHelper(App.Current.MainWindow).Handle;
      }
    }

    public ObservableDirectoryLab()
    {
      InitializeComponent();
    }

    private void SyncDirectory()
    {
      try
      {
        Directory = ObservableDirectory.Collect(Path, DispatcherScheduler.Current);
      }
      catch (Exception ex)
      {
        Directory = null;

        MessageBox.Show(
          App.Current.MainWindow,
          ex.Message,
          ex.GetType().FullName,
          MessageBoxButton.OK,
          MessageBoxImage.Error);
      }
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
      using (var dialog = new System.Windows.Forms.FolderBrowserDialog()
        {
          SelectedPath = Path,
          ShowNewFolderButton = false
        })
      {
        if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
        {
          if (!string.Equals(Path, dialog.SelectedPath, StringComparison.OrdinalIgnoreCase))
          {
            Path = dialog.SelectedPath;

            SyncDirectory();
          }
        }
      }
    }
  }
}