#pragma warning disable 0436
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
#if PORT_40
using System.Reflection;
#endif

namespace Rxx
{
  /// <summary>
  /// Provides methods for accessing an object's members through the <see cref="TypeDescriptor"/> and 
  /// <see cref="PropertyDescriptor"/> classes.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The benefits of using <see cref="TypeDescriptor"/> instead of direct reflection is that types can define
  /// what members they expose in various ways, even dynamically at runtime via component services, for classes
  /// that implement <strong>IComponent</strong>, or through self-description via the <strong>ICustomTypeDescriptor</strong> 
  /// interface.
  /// </para>
  /// <para>
  /// This is much more powerful than direct reflection is alone, but it's not a replacement and shouldn't be 
  /// used everywhere.  Before using this API instead of direct reflection, consider whether allowing types
  /// to describe themselves or applications to describe their types is actually the desired behavior when
  /// reflecting.
  /// </para>
  /// </remarks>
  /// <seealso href="http://msdn.microsoft.com/en-us/library/system.componentmodel.typedescriptor.aspx">
  /// TypeDescriptor Class, Remarks
  /// </seealso>
  internal static class ComponentReflection
  {
    public static PropertyDescriptor GetProperty(object source, string propertyName, StringComparison comparison)
    {
      Contract.Requires(source != null);
      Contract.Requires(propertyName != null);

      return (from property in TypeDescriptor.GetProperties(source).Cast<PropertyDescriptor>()
              where string.Equals(property.Name, propertyName, comparison)
              select property)
              .SingleOrDefault();
    }

    public static EventDescriptor GetEvent(object source, string eventName, StringComparison comparison)
    {
      Contract.Requires(source != null);
      Contract.Requires(eventName != null);

      return (from @event in TypeDescriptor.GetEvents(source).Cast<EventDescriptor>()
              where string.Equals(@event.Name, eventName, comparison)
              select @event)
              .SingleOrDefault();
    }

    public static IEnumerable<PropertyDescriptor> GetProperties(object source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<PropertyDescriptor>>() != null);

      return TypeDescriptor.GetProperties(source).Cast<PropertyDescriptor>();
    }

    public static IEnumerable<EventDescriptor> GetEvents(object source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<EventDescriptor>>() != null);

      return TypeDescriptor.GetEvents(source).Cast<EventDescriptor>();
    }

    public static IEnumerable<MemberDescriptor> GetMembers(object source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IEnumerable<MemberDescriptor>>() != null);

      return GetProperties(source).Cast<MemberDescriptor>()
        .Concat(GetEvents(source).Cast<MemberDescriptor>());
    }

#if PORT_40
    public static bool IsInterfaceMap(MethodInfo implementation, MethodInfo interfaceMethod)
    {
      Contract.Requires(implementation != null);
      Contract.Requires(interfaceMethod != null);

      return Unbound(implementation.ReturnParameter.ParameterType) == Unbound(interfaceMethod.ReturnParameter.ParameterType)
          && implementation.GetParameters().Select(p => Unbound(p.ParameterType)).SequenceEqual(
              interfaceMethod.GetParameters().Select(p => Unbound(p.ParameterType)));
    }

    private static Type Unbound(Type type)
    {
      Contract.Requires(type != null);
      Contract.Ensures(Contract.Result<Type>() != null);

      return type.IsGenericType && !type.IsGenericTypeDefinition ? type.GetGenericTypeDefinition() : type;
    }
#endif
  }
}