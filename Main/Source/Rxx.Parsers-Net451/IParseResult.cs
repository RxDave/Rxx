using System.Diagnostics.Contracts;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents a single result of a parse operation over a sequence.
  /// </summary>
  /// <typeparam name="TValue">The type of the parse result's <see cref="Value"/>.</typeparam>
  [ContractClass(typeof(IParseResultContract<>))]
  public interface IParseResult<out TValue>
  {
    /// <summary>
    /// Gets the projection of the matched elements in a sequence.
    /// </summary>
    TValue Value { get; }

    /// <summary>
    /// Gets the number of elements in a sequence that were consumed to generate the <see cref="Value"/>.
    /// </summary>
    int Length { get; }
  }

  [ContractClassFor(typeof(IParseResult<>))]
  internal abstract class IParseResultContract<TValue> : IParseResult<TValue>
  {
    public TValue Value
    {
      get
      {
        return default(TValue);
      }
    }

    public int Length
    {
      get
      {
        Contract.Ensures(Contract.Result<int>() >= 0);
        return 0;
      }
    }
  }
}