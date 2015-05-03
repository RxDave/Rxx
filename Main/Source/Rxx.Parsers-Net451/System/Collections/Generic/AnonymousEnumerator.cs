using System.Diagnostics.Contracts;

namespace System.Collections.Generic
{
  internal sealed class AnonymousEnumerator<T> : IEnumerator<T>
  {
    #region Public Properties
    public T Current
    {
      get
      {
        return current();
      }
    }

    object Collections.IEnumerator.Current
    {
      get
      {
        return current();
      }
    }
    #endregion

    #region Private / Protected
    private readonly Func<bool> moveNext;
    private readonly Func<T> current;
    private readonly Action reset, dispose;
    #endregion

    #region Constructors
    public AnonymousEnumerator(Func<bool> moveNext, Func<T> current, Action reset, Action dispose)
    {
      Contract.Requires(moveNext != null);
      Contract.Requires(current != null);
      Contract.Requires(reset != null);
      Contract.Requires(dispose != null);

      this.moveNext = moveNext;
      this.current = current;
      this.reset = reset;
      this.dispose = dispose;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(moveNext != null);
      Contract.Invariant(current != null);
      Contract.Invariant(reset != null);
      Contract.Invariant(dispose != null);
    }

    public bool MoveNext()
    {
      return moveNext();
    }

    public void Reset()
    {
      reset();
    }

    public void Dispose()
    {
      dispose();
    }
    #endregion
  }
}