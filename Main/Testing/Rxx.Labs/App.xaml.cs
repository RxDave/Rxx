using System.Windows;
using DaveSexton.Labs;
using Rxx.Parsers;

namespace Rxx.Labs
{
  /// <summary>
  /// The lab application's entry point.
  /// </summary>
  public partial class App : Application
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The controller is needed throughout the duration of the application as a listener for System.Diagnostics.Trace.")]
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      var controller = new WindowsLabController(new RxxLabCatalog());

      foreach (var trace in ParserTraceSources.All)
      {
        trace.Listeners.Add(controller);
      }

      controller.Show();
    }
  }
}