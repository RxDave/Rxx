using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Rxx.Parsers.Linq;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents a parser over an enumerable sequence of bytes.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of bytes.</typeparam>
  public abstract class BinaryParser<TResult> : Parser<byte, TResult>, IBinaryParser<TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    /// <summary>
    /// Gets a parser with a grammar that matches a single byte and converts it into a boolean value.
    /// </summary>
    protected IParser<byte, bool> Boolean
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, bool>>() != null);

        return Next.Select(value => BitConverter.ToBoolean(new[] { value }, 0));
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches a single byte.
    /// </summary>
    protected IParser<byte, byte> Byte
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, byte>>() != null);

        return Next;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches a single byte as a signed byte.
    /// </summary>
    [CLSCompliant(false)]
    protected IParser<byte, sbyte> SByte
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, sbyte>>() != null);

        return Next.Select(b => (sbyte)b);
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into an integer.
    /// </summary>
    protected IParser<byte, short> Int16
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, short>>() != null);

        return Next.Exactly(2).Select(bytes => BitConverter.ToInt16(Order(bytes).ToArray(), 0));
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into an unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    protected IParser<byte, ushort> UInt16
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, ushort>>() != null);

        return Next.Exactly(2).Select(bytes => BitConverter.ToUInt16(Order(bytes).ToArray(), 0));
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into an integer.
    /// </summary>
    protected IParser<byte, int> Int32
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, int>>() != null);

        return Next.Exactly(4).Select(bytes => BitConverter.ToInt32(Order(bytes).ToArray(), 0));
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into an unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    protected IParser<byte, uint> UInt32
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, uint>>() != null);

        return Next.Exactly(4).Select(bytes => BitConverter.ToUInt32(Order(bytes).ToArray(), 0));
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into an integer.
    /// </summary>
    protected IParser<byte, long> Int64
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, long>>() != null);

        return Next.Exactly(8).Select(bytes => BitConverter.ToInt64(Order(bytes).ToArray(), 0));
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into an unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    protected IParser<byte, ulong> UInt64
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, ulong>>() != null);

        return Next.Exactly(8).Select(bytes => BitConverter.ToUInt64(Order(bytes).ToArray(), 0));
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into a floating point number.
    /// </summary>
    protected IParser<byte, float> Single
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, float>>() != null);

        return Next.Exactly(4).Select(bytes => BitConverter.ToSingle(Order(bytes).ToArray(), 0));
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into a floating point number.
    /// </summary>
    protected IParser<byte, double> Double
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, double>>() != null);

        return Next.Exactly(8).Select(bytes => BitConverter.ToDouble(Order(bytes).ToArray(), 0));
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into a unicode character.
    /// </summary>
    protected IParser<byte, char> Char
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, char>>() != null);

        return Next.Exactly(2).Select(bytes => BitConverter.ToChar(Order(bytes).ToArray(), 0));
      }
    }

    private readonly bool bigEndian;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="BinaryParser{TResult}" /> class for derived classes
    /// to parse sequences with little endian byte order.
    /// </summary>
    protected BinaryParser()
    {
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="BinaryParser{TResult}" /> class for derived classes
    /// to parse sequences with the specified byte order.
    /// </summary>
    /// <param name="bigEndian">Specifies whether the byte order of sequences being parsed is big endian 
    /// or little endian.  The default value is <see langword="false"/>, which indicates that sequences are
    /// encoded with a little endian byte order, which is the default byte order in .NET when running on 
    /// a Windows OS.</param>
    protected BinaryParser(bool bigEndian)
    {
      this.bigEndian = bigEndian;
    }
    #endregion

    #region Methods
    internal static IEnumerable<byte> Order(IEnumerable<byte> bytes, bool bigEndian)
    {
      Contract.Requires(bytes != null);
      Contract.Ensures(Contract.Result<IEnumerable<byte>>() != null);
      Contract.Ensures(Contract.Result<IEnumerable<byte>>().Count() == bytes.Count());

      return bigEndian == !BitConverter.IsLittleEndian ? bytes : bytes.Reverse();
    }

    /// <summary>
    /// Orders the specified sequence of bytes according to the endianness of the parser and the 
    /// endianness of the host platform.
    /// </summary>
    /// <param name="bytes">The sequence of bytes to be ordered.</param>
    /// <returns>The specified <paramref name="bytes"/> in reverse order, if the endianness of the 
    /// parser does not match the host platform; otherwise, <paramref name="bytes"/> is returned 
    /// unmodified.</returns>
    protected IEnumerable<byte> Order(IEnumerable<byte> bytes)
    {
      Contract.Requires(bytes != null);
      Contract.Ensures(Contract.Result<IEnumerable<byte>>() != null);
      Contract.Ensures(Contract.Result<IEnumerable<byte>>().Count() == bytes.Count());

      return Order(bytes, bigEndian);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string of hyphen-delimited hexadecimal pairs representing each byte.
    /// </summary>
    /// <param name="length">The number of bytes to be converted.</param>
    /// <returns>A parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string of hyphen-delimited hexadecimal pairs representing each byte..</returns>
    protected IParser<byte, string> HexString(int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParser<byte, string>>() != null);

      return Next.Exactly(length).Select(bytes => BitConverter.ToString(Order(bytes).ToArray()));
    }

    /// <summary>
    /// Creates a parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string in the specified <paramref name="encoding"/>.
    /// </summary>
    /// <param name="encoding">The character encoding for converting the bytes into a string.</param>
    /// <param name="length">The number of bytes to be converted.</param>
    /// <returns>A parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string in the specified <paramref name="encoding"/>.</returns>
    protected IParser<byte, string> String(Encoding encoding, int length)
    {
      Contract.Requires(encoding != null);
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParser<byte, string>>() != null);

      return from bytes in Next.Exactly(length)
             let array = Order(bytes).ToArray()
             select encoding.GetString(array, 0, array.Length);
    }

    /// <summary>
    /// Creates a parser with a grammar that returns a string in the specified <paramref name="encoding"/>
    /// and with a length that is specified by a 7-bit encoded prefix, which is written by the 
    /// <see cref="System.IO.BinaryWriter.Write7BitEncodedInt"/> method.
    /// </summary>
    /// <param name="encoding">The character encoding for converting the bytes into a string.</param>
    /// <returns>A parser with a grammar that returns a string in the specified <paramref name="encoding"/>
    /// and with a length that is specified by a 7-bit encoded prefix.</returns>
    protected IParser<byte, string> String(Encoding encoding)
    {
      Contract.Requires(encoding != null);
      Contract.Ensures(Contract.Result<IParser<byte, string>>() != null);

      return from length in Parse7BitEncodedInt32()
             from bytes in Next.Exactly(length)
             let array = Order(bytes).ToArray()
             select encoding.GetString(array, 0, array.Length);
    }

    private IParser<byte, int> Parse7BitEncodedInt32()
    {
      Contract.Ensures(Contract.Result<IParser<byte, int>>() != null);

      return from b1 in Next
             from length in
               (b1 & 0x80) != 0
               ? from b2 in Next
                 from length2 in
                   (b2 & 0x80) != 0
                   ? from b3 in Next
                     from length3 in
                       (b3 & 0x80) != 0
                       ? from b4 in Next
                         where (b4 & 0x80) == 0
                         select (b1 & 0x7f) | ((b2 & 0x7f) << 7) | ((b3 & 0x7f) << 14) | (b4 << 21)
                       : Yield((b1 & 0x7f) | ((b2 & 0x7f) << 7) | (b3 << 14))
                     select length3
                   : Yield((b1 & 0x7f) | (b2 << 7))
                 select length2
               : Yield(b1)
             select length;
    }

    private IParser<byte, int> Yield(int value)
    {
      Contract.Ensures(Contract.Result<IParser<byte, int>>() != null);

      return this.Yield(_ => ParseResult.Return(value, 0));
    }

    /// <summary>
    /// Creates a parser with a grammar that matches the number of bytes in the specified 
    /// enum type and converts them into an instance of the enum.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum value to be parsed.</typeparam>
    /// <returns>A parser with a grammar that matches the number of bytes in the specified 
    /// enum type and converts them into an instance of the enum.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "RequiresAtCall-typeof(TEnum).IsEnum",
      Justification = "The static checker doesn't understand typeof(TEnum).IsEnum.")]
    protected IParser<byte, TEnum> Enum<TEnum>()
      where TEnum : struct
    {
#if !PORT_45 && !UNIVERSAL
      Contract.Requires(typeof(TEnum).IsEnum);
#endif
      Contract.Ensures(Contract.Result<IParser<byte, TEnum>>() != null);

      var type = typeof(TEnum);
      var underlyingType = System.Enum.GetUnderlyingType(type);

      if (underlyingType == typeof(int) || underlyingType == typeof(uint))
      {
        return this.Int32.Select(value => (TEnum)System.Enum.ToObject(type, value));
      }
      else if (underlyingType == typeof(long) || underlyingType == typeof(ulong))
      {
        return this.Int64.Select(value => (TEnum)System.Enum.ToObject(type, value));
      }
      else if (underlyingType == typeof(short) || underlyingType == typeof(ushort))
      {
        return this.Int16.Select(value => (TEnum)System.Enum.ToObject(type, value));
      }
      else if (underlyingType == typeof(byte) || underlyingType == typeof(sbyte))
      {
        return this.Byte.Select(value => (TEnum)System.Enum.ToObject(type, value));
      }
      else
      {
        throw new NotSupportedException();
      }
    }
    #endregion

    #region IBinaryParser<TResult> Members
    IParser<byte, bool> IBinaryParser<TResult>.Boolean
    {
      get
      {
        return Boolean;
      }
    }

    IParser<byte, byte> IBinaryParser<TResult>.Byte
    {
      get
      {
        return Byte;
      }
    }

    IParser<byte, sbyte> IBinaryParser<TResult>.SByte
    {
      get
      {
        return SByte;
      }
    }

    IParser<byte, short> IBinaryParser<TResult>.Int16
    {
      get
      {
        return Int16;
      }
    }

    IParser<byte, ushort> IBinaryParser<TResult>.UInt16
    {
      get
      {
        return UInt16;
      }
    }

    IParser<byte, int> IBinaryParser<TResult>.Int32
    {
      get
      {
        return Int32;
      }
    }

    IParser<byte, uint> IBinaryParser<TResult>.UInt32
    {
      get
      {
        return UInt32;
      }
    }

    IParser<byte, long> IBinaryParser<TResult>.Int64
    {
      get
      {
        return Int64;
      }
    }

    IParser<byte, ulong> IBinaryParser<TResult>.UInt64
    {
      get
      {
        return UInt64;
      }
    }

    IParser<byte, float> IBinaryParser<TResult>.Single
    {
      get
      {
        return Single;
      }
    }

    IParser<byte, double> IBinaryParser<TResult>.Double
    {
      get
      {
        return Double;
      }
    }

    IParser<byte, char> IBinaryParser<TResult>.Char
    {
      get
      {
        return Char;
      }
    }

    IEnumerable<byte> IBinaryParser<TResult>.Order(IEnumerable<byte> bytes)
    {
      return Order(bytes);
    }

    IParser<byte, string> IBinaryParser<TResult>.HexString(int length)
    {
      return HexString(length);
    }

    IParser<byte, string> IBinaryParser<TResult>.String(Encoding encoding, int length)
    {
      return String(encoding, length);
    }

    IParser<byte, string> IBinaryParser<TResult>.String(Encoding encoding)
    {
      return String(encoding);
    }

    IParser<byte, TEnum> IBinaryParser<TResult>.Enum<TEnum>()
    {
      return Enum<TEnum>();
    }
    #endregion
  }
}