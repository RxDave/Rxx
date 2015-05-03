using System.Diagnostics.Contracts;
using System.Threading;

namespace System.ServiceModel.Reactive
{
  internal sealed class ServiceAsyncResult : IAsyncResult
  {
    #region Public Properties
    public IAsyncResult ActualResult
    {
      get
      {
        Contract.Ensures(Contract.Result<IAsyncResult>() != null);

        return result;
      }
    }

    public object[] Outputs
    {
      get
      {
        return outputs;
      }
    }

    public object AsyncState
    {
      get
      {
        return result.AsyncState;
      }
    }

    public WaitHandle AsyncWaitHandle
    {
      get
      {
        return result.AsyncWaitHandle;
      }
    }

    public bool CompletedSynchronously
    {
      get
      {
        return result.CompletedSynchronously;
      }
    }

    public bool IsCompleted
    {
      get
      {
        return result.IsCompleted;
      }
    }
    #endregion

    #region Private / Protected
    private readonly object[] outputs;
    private readonly IAsyncResult result;
    #endregion

    #region Constructors
    public ServiceAsyncResult(IAsyncResult result, object[] outputs)
    {
      Contract.Requires(result != null);

      this.result = result;
      this.outputs = outputs;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(result != null);
    }
    #endregion
  }
}