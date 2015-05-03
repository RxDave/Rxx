using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
#if PORT_45 || UNIVERSAL
using System.Reflection;
#endif
using System.Text;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents a parser over an enumerable sequence of bytes.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of bytes.</typeparam>
  [CLSCompliant(false)]
  [ContractClass(typeof(IBinaryParserContract<>))]
#if !SILVERLIGHT || WINDOWS_PHONE
  public interface IBinaryParser<out TResult> : IParser<byte, TResult>
#else
	public interface IBinaryParser<TResult> : IParser<byte, TResult>
#endif
  {
    /// <summary>
    /// Gets a parser with a grammar that matches a single byte and converts it into a boolean value.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Boolean",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IParser<byte, bool> Boolean
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches a single byte.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Byte",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IParser<byte, byte> Byte
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches a single byte as a signed byte.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "SByte",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IParser<byte, sbyte> SByte
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into an integer.
    /// </summary>
    IParser<byte, short> Int16
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into an unsigned integer.
    /// </summary>
    IParser<byte, ushort> UInt16
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into an integer.
    /// </summary>
    IParser<byte, int> Int32
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into an unsigned integer.
    /// </summary>
    IParser<byte, uint> UInt32
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into an integer.
    /// </summary>
    IParser<byte, long> Int64
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into an unsigned integer.
    /// </summary>
    IParser<byte, ulong> UInt64
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into a floating point number.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Single",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IParser<byte, float> Single
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into a floating point number.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Double",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IParser<byte, double> Double
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into a unicode character.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Char",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IParser<byte, char> Char
    {
      get;
    }

    /// <summary>
    /// Orders the specified sequence of bytes according to the endianness of the parser and the 
    /// endianness of the host platform.
    /// </summary>
    /// <param name="bytes">The sequence of bytes to be ordered.</param>
    /// <returns>The specified <paramref name="bytes"/> in reverse order, if the endianness of the 
    /// parser does not match the host platform; otherwise, <paramref name="bytes"/> is returned 
    /// unmodified.</returns>
    IEnumerable<byte> Order(IEnumerable<byte> bytes);

    /// <summary>
    /// Creates a parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string of hyphen-delimited hexadecimal pairs representing each byte.
    /// </summary>
    /// <param name="length">The number of bytes to be converted.</param>
    /// <returns>A parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string of hyphen-delimited hexadecimal pairs representing each byte..</returns>
    IParser<byte, string> HexString(int length);

    /// <summary>
    /// Creates a parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string in the specified <paramref name="encoding"/>.
    /// </summary>
    /// <param name="encoding">The character encoding for converting the bytes into a string.</param>
    /// <param name="length">The number of bytes to be converted.</param>
    /// <returns>A parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string in the specified <paramref name="encoding"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "String",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IParser<byte, string> String(Encoding encoding, int length);

    /// <summary>
    /// Creates a parser with a grammar that returns a string in the specified <paramref name="encoding"/>
    /// and with a length that is specified by a 7-bit encoded prefix, which is written by the 
    /// <see cref="System.IO.BinaryWriter.Write7BitEncodedInt"/> method.
    /// </summary>
    /// <param name="encoding">The character encoding for converting the bytes into a string.</param>
    /// <returns>A parser with a grammar that returns a string in the specified <paramref name="encoding"/>
    /// and with a length that is specified by a 7-bit encoded prefix.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "String",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IParser<byte, string> String(Encoding encoding);

    /// <summary>
    /// Creates a parser with a grammar that matches the number of bytes in the specified 
    /// enum type and converts them into an instance of the enum.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum value to be parsed.</typeparam>
    /// <returns>A parser with a grammar that matches the number of bytes in the specified 
    /// enum type and converts them into an instance of the enum.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Enum",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IParser<byte, TEnum> Enum<TEnum>()
      where TEnum : struct;
  }

  [ContractClassFor(typeof(IBinaryParser<>))]
  internal abstract class IBinaryParserContract<TResult> : IBinaryParser<TResult>
  {
    public IParser<byte, bool> Boolean
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, bool>>() != null);
        return null;
      }
    }

    public IParser<byte, byte> Byte
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, byte>>() != null);
        return null;
      }
    }

    public IParser<byte, sbyte> SByte
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, sbyte>>() != null);
        return null;
      }
    }

    public IParser<byte, short> Int16
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, short>>() != null);
        return null;
      }
    }

    public IParser<byte, ushort> UInt16
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, ushort>>() != null);
        return null;
      }
    }

    public IParser<byte, int> Int32
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, int>>() != null);
        return null;
      }
    }

    public IParser<byte, uint> UInt32
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, uint>>() != null);
        return null;
      }
    }

    public IParser<byte, long> Int64
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, long>>() != null);
        return null;
      }
    }

    public IParser<byte, ulong> UInt64
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, ulong>>() != null);
        return null;
      }
    }

    public IParser<byte, float> Single
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, float>>() != null);
        return null;
      }
    }

    public IParser<byte, double> Double
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, double>>() != null);
        return null;
      }
    }

    public IParser<byte, char> Char
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<byte, char>>() != null);
        return null;
      }
    }

    public IEnumerable<byte> Order(IEnumerable<byte> bytes)
    {
      Contract.Requires(bytes != null);
      Contract.Ensures(Contract.Result<IEnumerable<byte>>() != null);
      Contract.Ensures(Contract.Result<IEnumerable<byte>>().Count() == bytes.Count());
      return null;
    }

    public IParser<byte, string> HexString(int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParser<byte, string>>() != null);
      return null;
    }

    public IParser<byte, string> String(Encoding encoding, int length)
    {
      Contract.Requires(encoding != null);
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IParser<byte, string>>() != null);
      return null;
    }

    public IParser<byte, string> String(Encoding encoding)
    {
      Contract.Requires(encoding != null);
      Contract.Ensures(Contract.Result<IParser<byte, string>>() != null);
      return null;
    }

    public IParser<byte, TEnum> Enum<TEnum>()
      where TEnum : struct
    {
#if !PORT_45 && !UNIVERSAL
      Contract.Requires(typeof(TEnum).IsEnum);
#else
      Contract.Requires(typeof(TEnum).GetTypeInfo().IsEnum);
#endif
      Contract.Ensures(Contract.Result<IParser<byte, TEnum>>() != null);
      return null;
    }

    #region IParser<byte,TResult> Members
    public IParser<byte, byte> Next
    {
      get
      {
        return null;
      }
    }

    public IEnumerable<IParseResult<TResult>> Parse(ICursor<byte> source)
    {
      return null;
    }
    #endregion
  }
}