using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System.Reactive
{
  internal class CoercingObserver<TSource, TTarget> : ObserverBase<TSource>
  {
    private readonly IObserver<TTarget> target;

    public CoercingObserver(IObserver<TTarget> target)
    {
      Contract.Requires(target != null);

      this.target = target;
    }

    [ContractInvariantMethod]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(target != null);
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly",
      Justification = "Double cast.")]
    protected override void OnNextCore(TSource value)
    {
      if (typeof(TTarget) == typeof(Unit))
      {
        target.OnNext((TTarget)(object)Unit.Default);
      }
      else
      {
        target.OnNext(Convert(value));
      }
    }

    protected override void OnErrorCore(Exception error)
    {
      target.OnError(error);
    }

    protected override void OnCompletedCore()
    {
      target.OnCompleted();
    }

    [ContractVerification(false)]
    [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly",
      Justification = "Double cast.")]
    protected virtual TTarget Convert(TSource value)
    {
      return (TTarget)(object)value;
    }
  }
}