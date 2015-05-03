#pragma warning disable 0436
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
#if !PORT_40
using System.Reflection;
#endif

namespace System.ComponentModel
{
  internal static class TypeDescriptor
  {
    public static IEnumerable<PropertyDescriptor> GetProperties(object source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<PropertyDescriptor>>() != null);

#if PORT_40
      return source.GetType().GetProperties().Select(property => new PropertyDescriptor(property));
#else
      return source.GetType().GetRuntimeProperties().Select(property => new PropertyDescriptor(property));
#endif
    }

    public static IEnumerable<EventDescriptor> GetEvents(object source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<EventDescriptor>>() != null);

#if PORT_40
      return source.GetType().GetEvents().Select(@event => new EventDescriptor(@event));
#else
      return source.GetType().GetRuntimeEvents().Select(@event => new EventDescriptor(@event));
#endif
    }
  }
}