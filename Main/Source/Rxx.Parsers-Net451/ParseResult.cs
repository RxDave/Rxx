using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Rxx.Parsers
{
  [DebuggerDisplay("Length = {Length}, Value = {Value}")]
  internal class ParseResult<TValue> : IParseResult<TValue>
  {
    #region Public Properties
    public TValue Value
    {
      get
      {
        return value;
      }
    }

    public int Length
    {
      get
      {
        return length;
      }
    }
    #endregion

    #region Private / Protected
    private readonly TValue value;
    private readonly int length;
    #endregion

    #region Constructors
    public ParseResult(TValue value, int length)
    {
      Contract.Requires(length >= 0);

      this.value = value;
      this.length = length;
    }

    public ParseResult(int length)
    {
      Contract.Requires(length >= 0);

      this.length = length;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(length >= 0);
    }
    #endregion
  }
}