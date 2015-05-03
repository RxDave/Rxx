using System.Diagnostics.Contracts;
#if PORT_40
using System.Linq;
#endif
using System.Reflection;

namespace System.ComponentModel
{
  internal sealed class PropertyDescriptor : MemberDescriptor
  {
    #region Public Properties
    public override string Name
    {
      get
      {
        var name = property.Name;

        Contract.Assume(!string.IsNullOrEmpty(name));

        return name;
      }
    }

    public Type PropertyType
    {
      get
      {
        Contract.Ensures(Contract.Result<Type>() != null);

        return property.PropertyType;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return !property.CanWrite;
      }
    }

    public bool SupportsChangeEvents
    {
      get
      {
        return EnsureChangeEvent();
      }
    }
    #endregion

    #region Private / Protected
    private readonly PropertyInfo property;
    private EventInfo changeEvent;
    private bool changeEventInitialized;
    #endregion

    #region Constructors
    internal PropertyDescriptor(PropertyInfo property)
    {
      Contract.Requires(property != null);

      this.property = property;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(property != null);
    }

    public object GetValue(object source)
    {
      return property.GetValue(source, null);
    }

    public void SetValue(object source, object value)
    {
      property.SetValue(source, value, null);
    }

    public void AddValueChanged(object source, EventHandler handler)
    {
      if (!EnsureChangeEvent())
      {
        throw new InvalidOperationException();
      }

      changeEvent.AddEventHandler(source, handler);
    }

    public void RemoveValueChanged(object source, EventHandler handler)
    {
      if (!EnsureChangeEvent())
      {
        throw new InvalidOperationException();
      }

      changeEvent.RemoveEventHandler(source, handler);
    }

    private bool EnsureChangeEvent()
    {
      Contract.Ensures(Contract.Result<bool>() == (changeEvent != null));

      if (!changeEventInitialized)
      {
        var type = property.DeclaringType;

        Contract.Assume(type != null);

#if PORT_40
        var isNotifier = typeof(INotifyPropertyChanged).IsAssignableFrom(type);
#else
        var info = type.GetTypeInfo();

        var isNotifier = typeof(INotifyPropertyChanged).GetTypeInfo().IsAssignableFrom(info);
#endif

        if (isNotifier)
        {
#if PORT_40
          var map = type.GetInterfaces().First(i => i == typeof(INotifyPropertyChanged));
          var methods = map.GetMethods();

          Contract.Assert(methods != null);
          Contract.Assume(methods.Length > 0);

          var propertyChangedAddOrRemove = methods[0];
#else
          var map = info.GetRuntimeInterfaceMap(typeof(INotifyPropertyChanged));

          Contract.Assume(map.TargetMethods != null);
          Contract.Assume(map.TargetMethods.Length > 0);

          var propertyChangedAddOrRemove = map.TargetMethods[0];
#endif

#if PORT_40
          foreach (var eventInfo in type.GetEvents())
          {
            Contract.Assert(eventInfo != null);

            var addMethod = eventInfo.GetAddMethod();
            var removeMethod = eventInfo.GetRemoveMethod();
#else
          foreach (var eventInfo in type.GetRuntimeEvents())
          {
            Contract.Assume(eventInfo != null);

            var addMethod = eventInfo.AddMethod;
            var removeMethod = eventInfo.RemoveMethod;
#endif

#if PORT_40
            if (Rxx.ComponentReflection.IsInterfaceMap(addMethod, propertyChangedAddOrRemove)
             || Rxx.ComponentReflection.IsInterfaceMap(removeMethod, propertyChangedAddOrRemove))
#else
            if (propertyChangedAddOrRemove == addMethod
             || propertyChangedAddOrRemove == removeMethod)
#endif
            {
              changeEvent = eventInfo;
              break;
            }
          }
        }
        else
        {
#if PORT_40
          changeEvent = type.GetEvent(property.Name + "Changed");
#else
          changeEvent = type.GetRuntimeEvent(property.Name + "Changed");
#endif
        }

        changeEventInitialized = true;
      }

      return changeEvent != null;
    }
    #endregion
  }
}