using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using DaveSexton.Labs;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive.Networking
{
  /* The "Hack" comment prevents the Labs Framework from showing this file as the source
   * code for the lab.  A future release will support partial classes by aggregating them
   * together.
   */
  public partial class /*Hack*/WcfLab
  {
    protected override async Task ExperimentExecutingAsync(IList<IExperiment> experiments, int index)
    {
      TraceLine();
      TraceLine(Text.ServerStarting);

      try
      {
        // This is not required when hosting the service in ASP.NET.
        using (StartService(new Uri(serviceUrl)))
        {
          await base.ExperimentExecutingAsync(experiments, index);
        }
      }
      catch (TimeoutException ex)
      {
        TraceError(ex.Message);
      }
      catch (CommunicationException ex)
      {
        TraceError(ex.Message);
      }

      TraceLine(Text.ServerStopped);
      TraceLine();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Host is returned to the caller.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "The exception message is traced and it's not that important anyway in this particular lab.")]
    private IDisposable StartService(Uri url)
    {
      ServiceHost host = null;

      try
      {
        host = new ServiceHost(typeof(ExampleService), url);
        host.Open();
      }
      catch (Exception ex)
      {
        TraceError(ex.Message);

        if (host != null)
        {
          host.Close();
          host = null;
        }
      }

      return host;
    }
  }
}