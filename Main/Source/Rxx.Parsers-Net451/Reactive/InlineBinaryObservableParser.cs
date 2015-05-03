using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;

namespace Rxx.Parsers.Reactive
{
  internal sealed class InlineBinaryObservableParser<TResult> : BinaryObservableParser<TResult>, IBinaryObservableParser<byte>
  {
    #region Public Properties
    protected override IObservableParser<byte, TResult> Start
    {
      get
      {
        return start;
      }
    }
    #endregion

    #region Private / Protected
    private IObservableParser<byte, TResult> start;
    #endregion

    #region Constructors
    public InlineBinaryObservableParser()
    {
      start = new AnonymousObservableParser<byte, TResult>(
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

    public IObservable<TResult> Parse(IObservableCursor<byte> source, IObservableParser<byte, TResult> grammar)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Requires(grammar != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      this.start = grammar;

      return base.Parse(source);
    }
    #endregion

    #region IObservableParser<byte,byte> Members
    IObservableParser<byte, byte> IObservableParser<byte, byte>.Next
    {
      get
      {
        return Next;
      }
    }

    IObservable<IParseResult<byte>> IObservableParser<byte, byte>.Parse(IObservableCursor<byte> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion

    #region IBinaryObservableParser<byte> Members
    IObservableParser<byte, bool> IBinaryObservableParser<byte>.Boolean
    {
      get
      {
        return Boolean;
      }
    }

    IObservableParser<byte, byte> IBinaryObservableParser<byte>.Byte
    {
      get
      {
        return Byte;
      }
    }

    IObservableParser<byte, sbyte> IBinaryObservableParser<byte>.SByte
    {
      get
      {
        return SByte;
      }
    }

    IObservableParser<byte, short> IBinaryObservableParser<byte>.Int16
    {
      get
      {
        return Int16;
      }
    }

    IObservableParser<byte, ushort> IBinaryObservableParser<byte>.UInt16
    {
      get
      {
        return UInt16;
      }
    }

    IObservableParser<byte, int> IBinaryObservableParser<byte>.Int32
    {
      get
      {
        return Int32;
      }
    }

    IObservableParser<byte, uint> IBinaryObservableParser<byte>.UInt32
    {
      get
      {
        return UInt32;
      }
    }

    IObservableParser<byte, long> IBinaryObservableParser<byte>.Int64
    {
      get
      {
        return Int64;
      }
    }

    IObservableParser<byte, ulong> IBinaryObservableParser<byte>.UInt64
    {
      get
      {
        return UInt64;
      }
    }

    IObservableParser<byte, float> IBinaryObservableParser<byte>.Single
    {
      get
      {
        return Single;
      }
    }

    IObservableParser<byte, double> IBinaryObservableParser<byte>.Double
    {
      get
      {
        return Double;
      }
    }

    IObservableParser<byte, char> IBinaryObservableParser<byte>.Char
    {
      get
      {
        return Char;
      }
    }

    IEnumerable<byte> IBinaryObservableParser<byte>.Order(IEnumerable<byte> bytes)
    {
      return Order(bytes);
    }

    IObservableParser<byte, string> IBinaryObservableParser<byte>.HexString(int length)
    {
      return HexString(length);
    }

    IObservableParser<byte, string> IBinaryObservableParser<byte>.String(System.Text.Encoding encoding, int length)
    {
      return String(encoding, length);
    }

    IObservableParser<byte, string> IBinaryObservableParser<byte>.String(System.Text.Encoding encoding)
    {
      return String(encoding);
    }

    IObservableParser<byte, TEnum> IBinaryObservableParser<byte>.Enum<TEnum>()
    {
      return Enum<TEnum>();
    }
    #endregion
  }
}