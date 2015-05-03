using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers
{
  internal sealed class InlineBinaryParser<TResult> : BinaryParser<TResult>, IBinaryParser<byte>
  {
    #region Public Properties
    protected override IParser<byte, TResult> Start
    {
      get
      {
        return start;
      }
    }
    #endregion

    #region Private / Protected
    private IParser<byte, TResult> start;
    #endregion

    #region Constructors
    public InlineBinaryParser()
    {
      start = new AnonymousParser<byte, TResult>(
        "Inline",
        () => Next,
        source =>
        {
          throw new NotSupportedException(Properties.Errors.InlineParserWithoutGrammar);
        });
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(start != null);
    }

    public IEnumerable<TResult> Parse(ICursor<byte> source, IParser<byte, TResult> grammar)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Requires(grammar != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      this.start = grammar;

      return base.Parse(source);
    }
    #endregion

    #region IParser<byte,byte> Members
    IParser<byte, byte> IParser<byte, byte>.Next
    {
      get
      {
        return Next;
      }
    }

    IEnumerable<IParseResult<byte>> IParser<byte, byte>.Parse(ICursor<byte> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion

    #region IBinaryParser<byte> Members
    IParser<byte, bool> IBinaryParser<byte>.Boolean
    {
      get
      {
        return Boolean;
      }
    }

    IParser<byte, byte> IBinaryParser<byte>.Byte
    {
      get
      {
        return Byte;
      }
    }

    IParser<byte, sbyte> IBinaryParser<byte>.SByte
    {
      get
      {
        return SByte;
      }
    }

    IParser<byte, short> IBinaryParser<byte>.Int16
    {
      get
      {
        return Int16;
      }
    }

    IParser<byte, ushort> IBinaryParser<byte>.UInt16
    {
      get
      {
        return UInt16;
      }
    }

    IParser<byte, int> IBinaryParser<byte>.Int32
    {
      get
      {
        return Int32;
      }
    }

    IParser<byte, uint> IBinaryParser<byte>.UInt32
    {
      get
      {
        return UInt32;
      }
    }

    IParser<byte, long> IBinaryParser<byte>.Int64
    {
      get
      {
        return Int64;
      }
    }

    IParser<byte, ulong> IBinaryParser<byte>.UInt64
    {
      get
      {
        return UInt64;
      }
    }

    IParser<byte, float> IBinaryParser<byte>.Single
    {
      get
      {
        return Single;
      }
    }

    IParser<byte, double> IBinaryParser<byte>.Double
    {
      get
      {
        return Double;
      }
    }

    IParser<byte, char> IBinaryParser<byte>.Char
    {
      get
      {
        return Char;
      }
    }

    IEnumerable<byte> IBinaryParser<byte>.Order(IEnumerable<byte> bytes)
    {
      return Order(bytes);
    }

    IParser<byte, string> IBinaryParser<byte>.HexString(int length)
    {
      return HexString(length);
    }

    IParser<byte, string> IBinaryParser<byte>.String(System.Text.Encoding encoding, int length)
    {
      return String(encoding, length);
    }

    IParser<byte, string> IBinaryParser<byte>.String(System.Text.Encoding encoding)
    {
      return String(encoding);
    }

    IParser<byte, TEnum> IBinaryParser<byte>.Enum<TEnum>()
    {
      return Enum<TEnum>();
    }
    #endregion
  }
}