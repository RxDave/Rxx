using System.Diagnostics.Contracts;
using System.Globalization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Rxx.Server.Properties;

namespace System.ServiceModel.Reactive
{
  /// <summary>
  /// Converts a service operation that returns <see cref="System.IObservable{T}"/> into an asynchronous operation.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="ObservableOperationAttribute"/> is optional.  Use it to control the <see cref="ReturnAsList"/>
  /// behavior for an observable operation; otherwise, if the default <see cref="ReturnAsList"/> behavior is 
  /// desired, then it is safe to omit <see cref="ObservableOperationAttribute"/> from the operation's method.
  /// </para>
  /// <para>
  /// <see cref="ObservableServiceAttribute"/> must be applied to a service interface or implementation to 
  /// provide contract support for operations that return <see cref="System.IObservable{T}"/>.  While updating the 
  /// contracts, it adds <see cref="ObservableOperationAttribute"/> to any observable operation that does not 
  /// already have it defined.  The <see cref="ReturnAsList"/> property is assigned to the value specified 
  /// by the <see cref="ObservableServiceAttribute.ReturnAsListDefault"/> property.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class ObservableOperationAttribute : Attribute, IOperationBehavior
  {
    #region Public Properties
    /// <summary>
    /// Gets or sets a value indicating whether the elements in the <see cref="System.IObservable{T}"/> must be 
    /// gathered and returned to the client as a list.
    /// </summary>
    /// <value>
    /// <see langword="True"/> to return <see cref="System.IObservable{T}"/> as an <see cref="System.Collections.Generic.IList{T}"/>; otherwise, 
    /// <see langword="false"/> to return the last value of <see cref="System.IObservable{T}"/> as a scalar result.
    /// The default value is <see langword="false"/> when <see cref="ObservableOperationAttribute"/> is applied
    /// explicitly to an operation's method; otherwise, the default value is <see cref="ObservableServiceAttribute.ReturnAsListDefault"/>.
    /// </value>
    public bool ReturnAsList
    {
      get;
      set;
    }
    #endregion

    #region Private / Protected
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="ObservableOperationAttribute" /> class.
    /// </summary>
    public ObservableOperationAttribute()
    {
    }
    #endregion

    #region Methods
    /// <summary>
    /// This method is not used.
    /// </summary>
    /// <param name="operationDescription">The parameter is not used.</param>
    /// <param name="bindingParameters">The parameter is not used.</param>
    public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
    {
    }

    /// <summary>
    /// This method is not used.
    /// </summary>
    /// <param name="operationDescription">The parameter is not used.</param>
    /// <param name="clientOperation">The parameter is not used.</param>
    public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
    {
    }

    /// <summary>
    /// Sets the <see cref="DispatchOperation.Invoker"/> to an object that represents an asynchronous operation by first
    /// invoking the method synchronously to acquire an <see cref="IObservable{T}"/> and then subscribing to it.
    /// </summary>
    /// <param name="operationDescription">The operation being examined.  Use for examination only.  If the operation
    /// description is modified, the results are undefined.</param>
    /// <param name="dispatchOperation">The run-time object that exposes customization properties for the operation
    /// described by <paramref name="operationDescription"/>.</param>
    public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
    {
      Contract.Assume(operationDescription != null);
      Contract.Assume(operationDescription.SyncMethod != null);
      Contract.Assume(dispatchOperation != null);

      dispatchOperation.Invoker = new ObservableMethodInvoker(operationDescription.SyncMethod, ReturnAsList);
    }

    /// <summary>
    /// Ensures that the method to which this attribute is applied is synchronous and returns <see cref="System.IObservable{T}"/>.
    /// </summary>
    /// <param name="operationDescription">The operation being examined.  Use for examination only.  If the operation
    /// description is modified, the results are undefined.</param>
    public void Validate(OperationDescription operationDescription)
    {
      Contract.Assume(operationDescription != null);

      if (operationDescription.SyncMethod == null)
      {
        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Errors.OperationIsNotSynchronous, operationDescription.Name));
      }

      if (!ObservableMethodInvoker.IsObservableMethod(operationDescription.SyncMethod))
      {
        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Errors.OperationIsNotObservable, operationDescription.Name));
      }
    }
    #endregion
  }
}