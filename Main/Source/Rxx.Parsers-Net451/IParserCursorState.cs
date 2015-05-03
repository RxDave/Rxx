using System.Diagnostics.Contracts;

namespace Rxx.Parsers
{
  [ContractClass(typeof(IParserCursorStateContract))]
  internal interface IParserCursorState
  {
    int CurrentIndex
    {
      get;
    }

    bool AtEndOfSequence
    {
      get;
    }
  }

  [ContractClassFor(typeof(IParserCursorState))]
  internal abstract class IParserCursorStateContract : IParserCursorState
  {
    public int CurrentIndex
    {
      get
      {
        Contract.Ensures(Contract.Result<int>() >= -1);
        return 0;
      }
    }

    public bool AtEndOfSequence
    {
      get
      {
        return false;
      }
    }
  }
}