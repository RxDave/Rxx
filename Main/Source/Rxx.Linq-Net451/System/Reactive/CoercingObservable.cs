using System.Diagnostics.Contracts;

namespace System.Reactive
{
  internal class CoercingObservable<TSource, TTarget> : ObservableBase<TTarget>
  {
    private readonly IObservable<TSource> source;

    public CoercingObservable(IObservable<TSource> source)
    {
      Contract.Requires(source != null);

      this.source = source;
    }

    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(source != null);
    }

    protected override IDisposable SubscribeCore(IObserver<TTarget> observer)
    {
      return source.SubscribeSafe(CreateObserver(observer));
    }

    protected virtual CoercingObserver<TSource, TTarget> CreateObserver(IObserver<TTarget> observer)
    {
      Contract.Requires(observer != null);
      Contract.Ensures(Contract.Result<CoercingObserver<TSource, TTarget>>() != null);

      return new CoercingObserver<TSource, TTarget>(observer);
    }
  }
}