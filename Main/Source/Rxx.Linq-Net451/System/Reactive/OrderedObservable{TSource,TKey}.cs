using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;

namespace System.Reactive
{
  internal sealed class OrderedObservable<TSource, TKey> : OrderedObservable<TSource>
  {
    #region Public Properties
    public override bool IsReactiveSort
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>() == (observableKeySelector != null));

        return observableKeySelector != null;
      }
    }
    #endregion

    #region Private / Protected
    private static readonly Func<IOrderedEnumerable<TSource>, IOrderedEnumerable<TSource>> thenByNop = x => x;

    private readonly OrderedObservable<TSource> parent;
    private readonly Func<TSource, IObservable<TKey>> observableKeySelector;
    private readonly Func<TSource, TKey> keySelector;
    private readonly IComparer<TKey> comparer;
    private readonly bool descending;
    #endregion

    #region Constructors
    public OrderedObservable(
      IObservable<TSource> source,
      Func<TSource, IObservable<TKey>> observableKeySelector,
      bool descending)
      : base(source)
    {
      Contract.Requires(source != null);
      Contract.Requires(observableKeySelector != null);

      this.observableKeySelector = observableKeySelector;
      this.descending = descending;
    }

    public OrderedObservable(
      IObservable<TSource> source,
      Func<TSource, IObservable<TKey>> observableKeySelector,
      bool descending,
      OrderedObservable<TSource> parent)
      : this(source, observableKeySelector, descending)
    {
      Contract.Requires(source != null);
      Contract.Requires(observableKeySelector != null);
      Contract.Requires(parent != null);

      this.parent = parent;
    }

    public OrderedObservable(
      IObservable<TSource> source,
      Func<TSource, TKey> keySelector,
      IComparer<TKey> comparer,
      bool descending)
      : base(source)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);

      this.keySelector = keySelector;
      this.comparer = comparer ?? Comparer<TKey>.Default;
      this.descending = descending;
    }

    public OrderedObservable(
      IObservable<TSource> source,
      Func<TSource, TKey> keySelector,
      IComparer<TKey> comparer,
      bool descending,
      OrderedObservable<TSource> parent)
      : this(source, keySelector, comparer, descending)
    {
      Contract.Requires(source != null);
      Contract.Requires(keySelector != null);
      Contract.Requires(parent != null);

      this.parent = parent;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant((keySelector == null) != (observableKeySelector == null));
      Contract.Invariant((keySelector == null) == (comparer == null));
    }

    public override IObservable<TSource> Sort(
      IObservable<TSource> query,
      Func<IEnumerable<TSource>, IOrderedEnumerable<TSource>> orderBy,
      Func<IOrderedEnumerable<TSource>, IOrderedEnumerable<TSource>> thenBy)
    {
      if (IsReactiveSort)
      {
        Contract.Assert(thenBy == null);

        return ReactiveSort(query, orderBy);
      }
      else
      {
        Contract.Assert(orderBy == null);

        return InteractiveSort(query, thenBy);
      }
    }

    private IObservable<TSource> ReactiveSort(
      IObservable<TSource> query,
      Func<IEnumerable<TSource>, IOrderedEnumerable<TSource>> orderBy)
    {
      Contract.Requires(IsReactiveSort);
      Contract.Requires(query != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      var observable =
        from value in parent == null ? query : parent.Sort(query, null, null)
        from _ in Observable.Create<Unit>(observer =>
          observableKeySelector(value).SubscribeSafe(
            _ =>
            {
            },
            observer.OnError,
            () =>
            {
              observer.OnNext(Unit.Default);
              observer.OnCompleted();
            }))
        select value;

      if (descending)
      {
        if (orderBy == null)
        {
          observable = observable.ToList().SelectMany(Enumerable.Reverse);
        }
        else
        {
          observable = observable.ToList().SelectMany(list => orderBy(list.Reverse()));
        }
      }
      else if (orderBy != null)
      {
        observable = observable.ToList().SelectMany(list => orderBy(list));
      }

      return observable;
    }

    private IObservable<TSource> InteractiveSort(
      IObservable<TSource> query,
      Func<IOrderedEnumerable<TSource>, IOrderedEnumerable<TSource>> thenBy)
    {
      Contract.Requires(!IsReactiveSort);
      Contract.Requires(query != null);
      Contract.Ensures(Contract.Result<IObservable<TSource>>() != null);

      if (thenBy == null)
      {
        thenBy = thenByNop;
      }

      if (parent == null)
      {
        return descending
          ? query.ToList().SelectMany(list => thenBy(list.OrderByDescending(keySelector, comparer)))
          : query.ToList().SelectMany(list => thenBy(list.OrderBy(keySelector, comparer)));
      }
      else if (parent.IsReactiveSort)
      {
        return descending
          ? parent.Sort(query, list => thenBy(list.OrderByDescending(keySelector, comparer)), null)
          : parent.Sort(query, list => thenBy(list.OrderBy(keySelector, comparer)), null);
      }
      else
      {
        return descending
          ? parent.Sort(query, null, list => thenBy(list.ThenByDescending(keySelector, comparer)))
          : parent.Sort(query, null, list => thenBy(list.ThenBy(keySelector, comparer)));
      }
    }
    #endregion
  }
}