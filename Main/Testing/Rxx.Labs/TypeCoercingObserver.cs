using System;
using System.Diagnostics.Contracts;
using System.Reactive;

namespace Rxx.Labs
{
  internal sealed class TypeCoercingObserver<TIn, TOut> : ObserverBase<TIn>
  {
    private readonly IObserver<TOut> observer;

    public TypeCoercingObserver(IObserver<TOut> observer)
    {
      Contract.Assume(observer != null);

      this.observer = observer;
    }

    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(observer != null);
    }

    protected override void OnNextCore(TIn value)
    {
      var obj = (object)value;

      if (obj == null)
      {
        observer.OnNext(default(TOut));
      }
      else
      {
        observer.OnNext((TOut)obj);
      }
    }

    protected override void OnErrorCore(Exception error)
    {
      observer.OnError(error);
    }

    protected override void OnCompletedCore()
    {
      observer.OnCompleted();
    }
  }
}