using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Reactive
{
  [ContractClass(typeof(OrderedObservableContract<>))]
  internal abstract class OrderedObservable<TSource> : ObservableBase<TSource>, IOrderedObservable<TSource>
  {
    public abstract bool IsReactiveSort { get; }

    private readonly IObservable<TSource> source;

    protected OrderedObservable(IObservable<TSource> source)
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

    public IOrderedObservable<TSource> CreateOrderedObservable<TKey>(
      Func<TSource, TKey> keySelector,
      IComparer<TKey> comparer,
      bool descending)
    {
      return new OrderedObservable<TSource, TKey>(source, keySelector, comparer, descending, parent: this);
    }

    public IOrderedObservable<TSource> CreateOrderedObservable<TOther>(
      Func<TSource, IObservable<TOther>> keySelector,
      bool descending)
    {
      return new OrderedObservable<TSource, TOther>(source, keySelector, descending, parent: this);
    }

    public abstract IObservable<TSource> Sort(
      IObservable<TSource> query,
      Func<IEnumerable<TSource>, IOrderedEnumerable<TSource>> orderBy,
      Func<IOrderedEnumerable<TSource>, IOrderedEnumerable<TSource>> thenBy);

    protected override IDisposable SubscribeCore(IObserver<TSource> observer)
    {
      return Sort(source, null, null).Subscribe(observer);
    }
  }

  [ContractClassFor(typeof(OrderedObservable<>))]
  internal abstract class OrderedObservableContract<TSource> : OrderedObservable<TSource>
  {
    public override bool IsReactiveSort
    {
      get
      {
        return false;
      }
    }

    public OrderedObservableContract(IObservable<TSource> source)
      : base(source)
    {
    }

    public override IObservable<TSource> Sort(
      IObservable<TSource> query,
      Func<IEnumerable<TSource>, IOrderedEnumerable<TSource>> orderBy,
      Func<IOrderedEnumerable<TSource>, IOrderedEnumerable<TSource>> thenBy)
    {
      Contract.Requires(query != null);
      Contract.Requires(IsReactiveSort || orderBy == null);
      Contract.Requires(!IsReactiveSort || thenBy == null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);
      return null;
    }
  }
}