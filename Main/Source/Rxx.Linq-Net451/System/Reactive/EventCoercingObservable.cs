using System.Diagnostics.Contracts;

namespace System.Reactive
{
  internal sealed class EventCoercingObservable<TSource, TTarget> : CoercingObservable<EventPattern<TSource>, EventPattern<TTarget>>
    where TSource : EventArgs
    where TTarget : EventArgs
  {
    #region Constructors
    public EventCoercingObservable(IObservable<EventPattern<TSource>> source)
      : base(source)
    {
      Contract.Requires(source != null);
    }
    #endregion

    #region Methods
    protected override CoercingObserver<EventPattern<TSource>, EventPattern<TTarget>> CreateObserver(IObserver<EventPattern<TTarget>> observer)
    {
      return new EventCoercingObserver<TSource, TTarget>(observer);
    }
    #endregion
  }
}