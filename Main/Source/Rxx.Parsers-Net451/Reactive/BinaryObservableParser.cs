using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
#if PORT_45 || UNIVERSAL
using System.Reflection;
#endif
using System.Text;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Represents a parser over an observable sequence of bytes.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of bytes.</typeparam>
  public abstract class BinaryObservableParser<TResult> : ObservableParser<byte, TResult>, IBinaryObservableParser<TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    /// <summary>
    /// Gets a parser with a grammar that matches a single byte and converts it into a boolean value.
    /// </summary>
    protected IObservableParser<byte, bool> Boolean
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, bool>>() != null);

        return Next.Select(value => BitConverter.ToBoolean(new[] { value }, 0));
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches a single byte.
    /// </summary>
    protected IObservableParser<byte, byte> Byte
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, byte>>() != null);

        return Next;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches a single byte as a signed byte.
    /// </summary>
    [CLSCompliant(false)]
    protected IObservableParser<byte, sbyte> SByte
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, sbyte>>() != null);

        return Next.Select(b => (sbyte)b);
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into an integer.
    /// </summary>
    protected IObservableParser<byte, short> Int16
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, short>>() != null);

        return from bytes in Next.Exactly(2)
               from array in bytes.ToArray()
               select BitConverter.ToInt16(Order(array).ToArray(), 0);
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into an unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    protected IObservableParser<byte, ushort> UInt16
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, ushort>>() != null);

        return from bytes in Next.Exactly(2)
               from array in bytes.ToArray()
               select BitConverter.ToUInt16(Order(array).ToArray(), 0);
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into an integer.
    /// </summary>
    protected IObservableParser<byte, int> Int32
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, int>>() != null);

        return from bytes in Next.Exactly(4)
               from array in bytes.ToArray()
               select BitConverter.ToInt32(Order(array).ToArray(), 0);
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into an unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    protected IObservableParser<byte, uint> UInt32
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, uint>>() != null);

        return from bytes in Next.Exactly(4)
               from array in bytes.ToArray()
               select BitConverter.ToUInt32(Order(array).ToArray(), 0);
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into an integer.
    /// </summary>
    protected IObservableParser<byte, long> Int64
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, long>>() != null);

        return from bytes in Next.Exactly(8)
               from array in bytes.ToArray()
               select BitConverter.ToInt64(Order(array).ToArray(), 0);
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into an unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    protected IObservableParser<byte, ulong> UInt64
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, ulong>>() != null);

        return from bytes in Next.Exactly(8)
               from array in bytes.ToArray()
               select BitConverter.ToUInt64(Order(array).ToArray(), 0);
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into a floating point number.
    /// </summary>
    protected IObservableParser<byte, float> Single
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, float>>() != null);

        return from bytes in Next.Exactly(4)
               from array in bytes.ToArray()
               select BitConverter.ToSingle(Order(array).ToArray(), 0);
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into a floating point number.
    /// </summary>
    protected IObservableParser<byte, double> Double
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, double>>() != null);

        return from bytes in Next.Exactly(8)
               from array in bytes.ToArray()
               select BitConverter.ToDouble(Order(array).ToArray(), 0);
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into a unicode character.
    /// </summary>
    protected IObservableParser<byte, char> Char
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, char>>() != null);

        return from bytes in Next.Exactly(2)
               from array in bytes.ToArray()
               select BitConverter.ToChar(Order(array).ToArray(), 0);
      }
    }

    private readonly bool bigEndian;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="BinaryParser{TResult}" /> class for derived classes
    /// to parse sequences with little endian byte order.
    /// </summary>
    protected BinaryObservableParser()
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
    protected BinaryObservableParser(bool bigEndian)
    {
      this.bigEndian = bigEndian;
    }
    #endregion

    #region Methods
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

      return BinaryParser<TResult>.Order(bytes, bigEndian);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string of hyphen-delimited hexadecimal pairs representing each byte.
    /// </summary>
    /// <param name="length">The number of bytes to be converted.</param>
    /// <returns>A parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string of hyphen-delimited hexadecimal pairs representing each byte..</returns>
    protected IObservableParser<byte, string> HexString(int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IObservableParser<byte, string>>() != null);

      return from bytes in Next.Exactly(length)
             from array in bytes.ToArray()
             select BitConverter.ToString(Order(array).ToArray());
    }

    /// <summary>
    /// Creates a parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string in the specified <paramref name="encoding"/>.
    /// </summary>
    /// <param name="encoding">The character encoding for converting the bytes into a string.</param>
    /// <param name="length">The number of bytes to be converted.</param>
    /// <returns>A parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string in the specified <paramref name="encoding"/>.</returns>
    protected IObservableParser<byte, string> String(Encoding encoding, int length)
    {
      Contract.Requires(encoding != null);
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IObservableParser<byte, string>>() != null);

      return from bytes in Next.Exactly(length)
             from array in bytes.ToArray()
             select encoding.GetString(Order(array).ToArray(), 0, array.Length);
    }

    /// <summary>
    /// Creates a parser with a grammar that returns a string in the specified <paramref name="encoding"/>
    /// and with a length that is specified by a 7-bit encoded prefix, which is written by the 
    /// <see cref="System.IO.BinaryWriter.Write7BitEncodedInt"/> method.
    /// </summary>
    /// <param name="encoding">The character encoding for converting the bytes into a string.</param>
    /// <returns>A parser with a grammar that returns a string in the specified <paramref name="encoding"/>
    /// and with a length that is specified by a 7-bit encoded prefix.</returns>
    protected IObservableParser<byte, string> String(Encoding encoding)
    {
      Contract.Requires(encoding != null);
      Contract.Ensures(Contract.Result<IObservableParser<byte, string>>() != null);

      return from length in Parse7BitEncodedInt32()
             from bytes in Next.Exactly(length)
             from array in bytes.ToArray()
             select encoding.GetString(Order(array).ToArray(), 0, array.Length);
    }

    private IObservableParser<byte, int> Parse7BitEncodedInt32()
    {
      Contract.Ensures(Contract.Result<IObservableParser<byte, int>>() != null);

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

    private IObservableParser<byte, int> Yield(int value)
    {
      Contract.Ensures(Contract.Result<IObservableParser<byte, int>>() != null);

      return this.Yield(_ => ObservableParseResult.Return(value, 0));
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
    protected IObservableParser<byte, TEnum> Enum<TEnum>()
      where TEnum : struct
    {
#if !PORT_45 && !UNIVERSAL
      Contract.Requires(typeof(TEnum).IsEnum);
#else
      Contract.Requires(typeof(TEnum).GetTypeInfo().IsEnum);
#endif
      Contract.Ensures(Contract.Result<IObservableParser<byte, TEnum>>() != null);

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

    #region IBinaryObservableParser<TResult> Members
    IObservableParser<byte, bool> IBinaryObservableParser<TResult>.Boolean
    {
      get
      {
        return Boolean;
      }
    }

    IObservableParser<byte, byte> IBinaryObservableParser<TResult>.Byte
    {
      get
      {
        return Byte;
      }
    }

    IObservableParser<byte, sbyte> IBinaryObservableParser<TResult>.SByte
    {
      get
      {
        return SByte;
      }
    }

    IObservableParser<byte, short> IBinaryObservableParser<TResult>.Int16
    {
      get
      {
        return Int16;
      }
    }

    IObservableParser<byte, ushort> IBinaryObservableParser<TResult>.UInt16
    {
      get
      {
        return UInt16;
      }
    }

    IObservableParser<byte, int> IBinaryObservableParser<TResult>.Int32
    {
      get
      {
        return Int32;
      }
    }

    IObservableParser<byte, uint> IBinaryObservableParser<TResult>.UInt32
    {
      get
      {
        return UInt32;
      }
    }

    IObservableParser<byte, long> IBinaryObservableParser<TResult>.Int64
    {
      get
      {
        return Int64;
      }
    }

    IObservableParser<byte, ulong> IBinaryObservableParser<TResult>.UInt64
    {
      get
      {
        return UInt64;
      }
    }

    IObservableParser<byte, float> IBinaryObservableParser<TResult>.Single
    {
      get
      {
        return Single;
      }
    }

    IObservableParser<byte, double> IBinaryObservableParser<TResult>.Double
    {
      get
      {
        return Double;
      }
    }

    IObservableParser<byte, char> IBinaryObservableParser<TResult>.Char
    {
      get
      {
        return Char;
      }
    }

    IEnumerable<byte> IBinaryObservableParser<TResult>.Order(IEnumerable<byte> bytes)
    {
      return Order(bytes);
    }

    IObservableParser<byte, string> IBinaryObservableParser<TResult>.HexString(int length)
    {
      return HexString(length);
    }

    IObservableParser<byte, string> IBinaryObservableParser<TResult>.String(Encoding encoding, int length)
    {
      return String(encoding, length);
    }

    IObservableParser<byte, string> IBinaryObservableParser<TResult>.String(Encoding encoding)
    {
      return String(encoding);
    }

    IObservableParser<byte, TEnum> IBinaryObservableParser<TResult>.Enum<TEnum>()
    {
      return Enum<TEnum>();
    }
    #endregion
  }
}