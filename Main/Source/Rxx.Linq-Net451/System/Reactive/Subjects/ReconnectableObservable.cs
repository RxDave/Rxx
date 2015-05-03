using System.Diagnostics.Contracts;
using System.Reactive.Disposables;

namespace System.Reactive.Subjects
{
  [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
    Justification = "It returns the disposable to the caller, who controls the lifetime of the subscription.")]
  internal sealed class ReconnectableObservable<TSource, TResult> : IConnectableObservable<TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private ISubject<TSource, TResult> Subject
    {
      get
      {
        Contract.Ensures(Contract.Result<ISubject<TSource, TResult>>() != null);

        if (subject == null)
        {
          subject = factory();

          Contract.Assume(subject != null);
        }

        return subject;
      }
    }

    private readonly object gate = new object();
    private readonly IObservable<TSource> source;
    private readonly Func<ISubject<TSource, TResult>> factory;

    private ISubject<TSource, TResult> subject;
    private IDisposable subscription;
    #endregion

    #region Constructors
    public ReconnectableObservable(IObservable<TSource> source, Func<ISubject<TSource, TResult>> factory)
    {
      Contract.Requires(source != null);
      Contract.Requires(factory != null);

      this.source = source;
      this.factory = factory;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(gate != null);
      Contract.Invariant(source != null);
      Contract.Invariant(factory != null);
    }

    public IDisposable Connect()
    {
      lock (gate)
      {
        if (subscription == null)
        {
          subscription = new CompositeDisposable(
            source.SubscribeSafe(Subject),
            Disposable.Create(() =>
            {
              lock (gate)
              {
                subscription = null;
                subject = null;
              }
            }));
        }

        return subscription;
      }
    }

    public IDisposable Subscribe(IObserver<TResult> observer)
    {
      lock (gate)
      {
        return Subject.Subscribe(observer);
      }
    }
    #endregion
  }
}