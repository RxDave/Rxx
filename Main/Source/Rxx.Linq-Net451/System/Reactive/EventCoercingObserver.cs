using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  internal sealed class EventCoercingObserver<TSource, TTarget> : CoercingObserver<EventPattern<TSource>, EventPattern<TTarget>>
    where TSource : EventArgs
    where TTarget : EventArgs
  {
    #region Constructors
    public EventCoercingObserver(IObserver<EventPattern<TTarget>> target)
      : base(target)
    {
      Contract.Requires(target != null);
    }
    #endregion

    #region Methods
    [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly",
      Justification = "Double cast.")]
    protected override EventPattern<TTarget> Convert(EventPattern<TSource> value)
    {
      Contract.Assume(value != null);

      return new EventPattern<TTarget>(value.Sender, (TTarget)(object)value.EventArgs);
    }
    #endregion
  }
}