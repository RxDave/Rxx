using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Reactive
{
  /// <summary>
  /// Converts service operations that return <see cref="System.IObservable{T}"/> into asynchronous operations.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="ObservableServiceAttribute"/> must be applied to a service interface or implementation to 
  /// provide contract support for operations that return <see cref="System.IObservable{T}"/>.  It's required
  /// to adjust operations' contracts so that they return the correct types in the service's WSDL document.
  /// </para>
  /// <para>
  /// While updating the contracts, <see cref="ObservableOperationAttribute"/> is added to any observable operation 
  /// that does not already have it defined.  The <see cref="ObservableOperationAttribute.ReturnAsList"/> property
  /// is assigned to the value specified by the <see cref="ReturnAsListDefault"/> property.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
  public sealed class ObservableServiceAttribute : Attribute, IContractBehavior
  {
    #region Public Properties
    /// <summary>
    /// Gets or sets the default value of the <see cref="ObservableOperationAttribute.ReturnAsList"/> property for
    /// operations that do not have the <see cref="ObservableOperationAttribute"/> behavior applied explicitly.
    /// </summary>
    /// <value>
    /// <see langword="True"/> to return <see cref="System.IObservable{T}"/> as an <see cref="IList{T}"/>; otherwise, 
    /// <see langword="false"/> to return the last value of <see cref="System.IObservable{T}"/> as a scalar result.
    /// The default value is <see langword="false"/>.
    /// </value>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors",
      Justification = "Indicates a default value.  The documentation for ReturnAsList provides the actual details.")]
    public bool ReturnAsListDefault
    {
      get;
      set;
    }
    #endregion

    #region Private / Protected
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="ObservableServiceAttribute" /> class.
    /// </summary>
    public ObservableServiceAttribute()
    {
    }
    #endregion

    #region Methods
    private static void InitializeObservableOperation(OperationDescription operation, bool returnAsListDefault)
    {
      Contract.Requires(operation != null);
      Contract.Requires(operation.SyncMethod != null);

      var hasObservableOperationBehavior = false;
      var returnAsList = false;

      foreach (var behavior in operation.Behaviors)
      {
        var observableOperationAttribute = behavior as ObservableOperationAttribute;

        if (observableOperationAttribute != null)
        {
          returnAsList = observableOperationAttribute.ReturnAsList;
          hasObservableOperationBehavior = true;
        }
      }

      if (!hasObservableOperationBehavior)
      {
        operation.Behaviors.Add(
          new ObservableOperationAttribute()
          {
            ReturnAsList = returnAsList = returnAsListDefault
          });
      }

      Contract.Assume(operation.SyncMethod != null);

      var dataType = ObservableMethodInvoker.GetObservableDataType(operation.SyncMethod.ReturnType);

      var returnType = returnAsList ? Rxx.GeneralReflection.MakeGenericType(typeof(IList<>), dataType) : dataType;

      foreach (var message in operation.Messages)
      {
        Contract.Assume(message != null);

        if (message.Direction == MessageDirection.Output)
        {
          Contract.Assume(message.Body.ReturnValue != null);

          message.Body.ReturnValue.Type = returnType;
        }
      }
    }

    /// <summary>
    /// This method is not used.
    /// </summary>
    /// <param name="contractDescription">The parameter is not used.</param>
    /// <param name="endpoint">The parameter is not used.</param>
    /// <param name="bindingParameters">The parameter is not used.</param>
    public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
    }

    /// <summary>
    /// This method is not used.
    /// </summary>
    /// <param name="contractDescription">The parameter is not used.</param>
    /// <param name="endpoint">The parameter is not used.</param>
    /// <param name="clientRuntime">The parameter is not used.</param>
    public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
    }

    /// <summary>
    /// Adjusts the contract and behavior of each operation that returns <see cref="System.IObservable{T}"/>.
    /// </summary>
    /// <param name="contractDescription">The contract description to be modified.</param>
    /// <param name="endpoint">The endpoint that exposes the contract.</param>
    /// <param name="dispatchRuntime">The dispatch runtime that controls service execution.</param>
    public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
    {
      Contract.Assume(contractDescription != null);

      foreach (var operation in contractDescription.Operations)
      {
        Contract.Assume(operation != null);

        if (ObservableMethodInvoker.IsObservableMethod(operation.SyncMethod))
        {
          InitializeObservableOperation(operation, ReturnAsListDefault);
        }
      }
    }

    /// <summary>
    /// This method is not used.
    /// </summary>
    /// <param name="contractDescription">The parameter is not used.</param>
    /// <param name="endpoint">The parameter is not used.</param>
    public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
    {
    }
    #endregion
  }
}