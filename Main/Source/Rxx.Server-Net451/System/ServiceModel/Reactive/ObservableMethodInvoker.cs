using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;
using Rxx.Server.Properties;

namespace System.ServiceModel.Reactive
{
  internal sealed class ObservableMethodInvoker : IOperationInvoker
  {
    #region Public Properties
    public bool IsSynchronous
    {
      get
      {
        return false;
      }
    }
    #endregion

    #region Private / Protected
    private const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly object[] emptyArray = new object[0];
    private static readonly MethodInfo toListTemplate = Rxx.GeneralReflection.GetMethodWithParamsOfGenericArgs(typeof(Observable), "ToList", typeof(IObservable<>));
    private static readonly MethodInfo toTaskTemplate = Rxx.GeneralReflection.GetMethodWithParamsOfGenericArgs(typeof(TaskObservableExtensions), "ToTask", typeof(IObservable<>), typeof(object));

    private readonly MethodInfo method, toList, toTask, getResult, continueWith;
    private readonly ParameterInfo[] parameters;
    private readonly int targetInputParameterCount, targetOutputParameterCount;
    private readonly bool returnAsList;
    #endregion

    #region Constructors
    public ObservableMethodInvoker(MethodInfo method, bool returnAsList)
    {
      Contract.Requires(method != null);

      Contract.Assume(IsObservableMethod(method));

      this.method = method;
      this.returnAsList = returnAsList;

      var observableType = method.ReturnType;

      var dataType = GetObservableDataType(observableType);
      var returnType = returnAsList ? Rxx.GeneralReflection.MakeGenericType(typeof(IList<>), dataType) : dataType;
      var taskType = Rxx.GeneralReflection.MakeGenericType(typeof(Task<>), returnType);

      toList = toListTemplate.MakeGenericMethod(dataType);
      toTask = toTaskTemplate.MakeGenericMethod(returnType);
      continueWith = Rxx.GeneralReflection.GetMethodWithParamsOfGenericArgs(taskType, "ContinueWith", Rxx.GeneralReflection.MakeGenericType(typeof(Action<>), taskType));

      var resultProperty = taskType.GetProperty("Result");

      Contract.Assume(resultProperty != null);

      getResult = resultProperty.GetGetMethod();

      Contract.Assume(getResult != null);

      parameters = method.GetParameters();

      foreach (var parameter in parameters)
      {
        Contract.Assume(parameter != null);

        if (IsInput(parameter))
        {
          targetInputParameterCount++;
        }

        if (IsOutput(parameter))
        {
          targetOutputParameterCount++;
        }
      }

      Contract.Assume(targetInputParameterCount <= parameters.Length);
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(method != null);
      Contract.Invariant(toList != null);
      Contract.Invariant(toTask != null);
      Contract.Invariant(getResult != null);
      Contract.Invariant(continueWith != null);
      Contract.Invariant(parameters != null);
      Contract.Invariant(targetInputParameterCount >= 0);
      Contract.Invariant(targetOutputParameterCount >= 0);
      Contract.Invariant(targetInputParameterCount <= parameters.Length);
      Contract.Invariant(targetOutputParameterCount <= parameters.Length);
    }

    [Pure]
    public static bool IsObservableMethod(MethodInfo method)
    {
      Contract.Ensures(!Contract.Result<bool>() || method != null);

      return method != null
          && method.ReturnType.IsGenericType
          && method.ReturnType.GetGenericTypeDefinition() == typeof(IObservable<>);
    }

    public static Type GetObservableDataType(Type observableType)
    {
      Contract.Requires(observableType != null);
      Contract.Ensures(Contract.Result<Type>() != null);

      var args = observableType.GetGenericArguments();

      if (args.Length != 1)
      {
        throw new InvalidOperationException();
      }

      var dataType = args[0];

      Contract.Assume(dataType != null);

      return dataType;
    }

    private static bool IsInput(ParameterInfo parameter)
    {
      Contract.Requires(parameter != null);

      return !parameter.IsOut;
    }

    private static bool IsOutput(ParameterInfo parameter)
    {
      Contract.Requires(parameter != null);

      return parameter.ParameterType.IsByRef;
    }

    public object[] AllocateInputs()
    {
      return targetInputParameterCount == 0 ? emptyArray : new object[targetInputParameterCount];
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "TestAlwaysEvaluatingToAConstant",
      Justification = "False positive on 'arguments' local.")]
    private object[] CreateArguments(object[] inputs)
    {
      Contract.Requires(inputs == null || inputs.Length <= parameters.Length);
      Contract.Ensures((Contract.Result<object[]>() == null) == (parameters.Length == 0));
      Contract.Ensures(Contract.Result<object[]>() == null || Contract.Result<object[]>().Length == parameters.Length);

      var arguments = parameters.Length == 0 ? null : new object[parameters.Length];

      if (targetInputParameterCount > 0 && inputs != null)
      {
        Contract.Assert(arguments != null);

        for (int i = 0, p = 0; p < parameters.Length; p++)
        {
          var param = parameters[p];

          Contract.Assume(param != null);

          if (IsInput(param))
          {
            Contract.Assume(p < arguments.Length);
            Contract.Assume(i < inputs.Length);

            arguments[p] = inputs[i++];
          }
        }
      }

      Contract.Assume((arguments == null) == (parameters.Length == 0));
      Contract.Assume(arguments == null || arguments.Length == parameters.Length);

      return arguments;
    }

    private object[] CreateOutputs(object[] arguments)
    {
      Contract.Requires((arguments == null) == (parameters.Length == 0));
      Contract.Requires(arguments == null || arguments.Length == parameters.Length);
      Contract.Ensures(Contract.Result<object[]>() != null);

      var outputs = targetOutputParameterCount == 0 ? emptyArray : new object[targetOutputParameterCount];

      if (targetOutputParameterCount > 0)
      {
        Contract.Assert(arguments != null);
        Contract.Assert(arguments.Length == parameters.Length);

        for (int o = 0, p = 0; p < parameters.Length; p++)
        {
          var param = parameters[p];

          Contract.Assume(param != null);

          if (IsOutput(param))
          {
            Contract.Assume(o < outputs.Length);

            outputs[o++] = arguments[p];
          }
        }
      }

      return outputs;
    }

    public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
    {
      if (instance == null)
      {
        throw new ArgumentNullException("instance");
      }

      int inputCount = inputs == null ? 0 : inputs.Length;

      if (inputCount != targetInputParameterCount)
      {
        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Errors.OperationMissingInputParameters, method.Name, targetInputParameterCount, inputCount));
      }

      var arguments = CreateArguments(inputs);

      var observable = method.Invoke(instance, bindingFlags, Type.DefaultBinder, arguments, null);

      var outputs = CreateOutputs(arguments);

      if (returnAsList)
      {
        observable = toList.Invoke(null, new[] { observable });
      }

      var task = toTask.Invoke(null, new[] { observable, state });

      Contract.Assume(task != null);

      var result = new ServiceAsyncResult((IAsyncResult)task, outputs);

      /* Testing has revealed that WCF passes the result object that is supplied to the callback 
       * to the InvokeEnd method instead of passing the object that is returned by InvokeBegin.
       * 
       * Therefore, to ensure that the strong-typed result is passed to InvokeEnd, it must be 
       * passed directly to the callback.  Just in case this behavior changes in the future, the
       * strong-typed object is also being returned from InvokeBegin instead of the Task that
       * is returned by the ContinueWith method.
       */
      Action<IAsyncResult> invokeCallback = _ => callback(result);

      continueWith.Invoke(task, new[] { invokeCallback });

      return result;
    }

    public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
    {
      var serviceResult = (ServiceAsyncResult)result;

      Contract.Assume(serviceResult != null);

      outputs = serviceResult.Outputs;

      var task = serviceResult.ActualResult;

      return getResult.Invoke(task, null);
    }

    public object Invoke(object instance, object[] inputs, out object[] outputs)
    {
      throw new NotSupportedException();
    }
    #endregion
  }
}