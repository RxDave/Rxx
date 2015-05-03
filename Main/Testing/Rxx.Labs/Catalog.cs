using System;
using System.Collections.Generic;
using System.Linq;
using DaveSexton.Labs;

namespace Rxx.Labs
{
  internal sealed partial class RxxLabCatalog : LabCatalog
  {
    public override LabActivationStrategies Activation
    {
      get
      {
        return LabActivationStrategies.AllWithExclusions;
      }
    }

    public override IEnumerable<Type> LabTypes
    {
      get
      {
        // Return lab types that should not be activated

        yield break;
      }
    }

    public override IEnumerable<ILab> PriorityLabs
    {
      get
      {
        // Anonymous experiments
        //
        // See the Catalog - Anonymous.cs file.

        yield return new AnonymousLab(enabled: anonymousEnabled, main: Anonymous);

        // Labs specified here will not be executed again when they are 
        // discovered by MEF.
        // 
        // See the Catalog - Priority.cs file.

        foreach (var lab in GetPriorityLabs() ?? Enumerable.Empty<ILab>())
        {
          yield return lab;
        }
      }
    }
  }
}
