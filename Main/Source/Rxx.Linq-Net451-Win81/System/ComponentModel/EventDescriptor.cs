using System.Diagnostics.Contracts;
using System.Reflection;

namespace System.ComponentModel
{
  internal sealed class EventDescriptor : MemberDescriptor
  {
    #region Public Properties
    public override string Name
    {
      get
      {
        var name = @event.Name;

        Contract.Assume(!string.IsNullOrEmpty(name));

        return name;
      }
    }

    public Type EventType
    {
      get
      {
        Contract.Ensures(Contract.Result<Type>() != null);

        return @event.EventHandlerType;
      }
    }
    #endregion

    #region Private / Protected
    private readonly EventInfo @event;
    #endregion

    #region Constructors
    internal EventDescriptor(EventInfo @event)
    {
      Contract.Requires(@event != null);

      this.@event = @event;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(@event != null);
    }

    public void AddEventHandler(object source, Delegate handler)
    {
      @event.AddEventHandler(source, handler);
    }

    public void RemoveEventHandler(object source, Delegate handler)
    {
      @event.RemoveEventHandler(source, handler);
    }
    #endregion
  }
}