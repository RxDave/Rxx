using System;
using System.Linq;
using System.Net;
using System.Threading;
using DaveSexton.Labs;

namespace Rxx.Labs
{
  internal sealed partial class RxxLabCatalog : LabCatalog
  {
    /* Anonymous experiments (optional)
     * 
     * Enable this anonymous lab and it will be executed before all of 
     * the priority labs and discovered labs.
     */
    private const bool anonymousEnabled = false;

    private static void Anonymous()
    {
      // Define anonymous experiments here and set enabled: true
    }
  }
}
