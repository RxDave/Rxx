using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace System.Collections.Generic
{
  internal sealed class AnonymousEnumerable<T> : IEnumerable<T>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    private readonly Func<IEnumerator<T>> getEnumerator;
    #endregion

    #region Constructors
    public AnonymousEnumerable(Func<IEnumerator<T>> getEnumerator)
    {
      Contract.Requires(getEnumerator != null);

      this.getEnumerator = getEnumerator;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(getEnumerator != null);
    }

    public IEnumerator<T> GetEnumerator()
    {
      var e = getEnumerator();

      Contract.Assume(e != null);

      return e;
    }

    Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
    {
      var e = getEnumerator();

      Contract.Assume(e != null);

      return e;
    }
    #endregion
  }
}