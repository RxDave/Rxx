using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
#if PORT_45 || UNIVERSAL
using System.Reflection;
#endif
using System.Text;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Represents a parser over an observable sequence of bytes.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of bytes.</typeparam>
  [CLSCompliant(false)]
  [ContractClass(typeof(IBinaryObservableParserContract<>))]
#if !SILVERLIGHT || WINDOWS_PHONE
  public interface IBinaryObservableParser<out TResult> : IObservableParser<byte, TResult>
#else
	public interface IBinaryObservableParser<TResult> : IObservableParser<byte, TResult>
#endif
  {
    /// <summary>
    /// Gets a parser with a grammar that matches a single byte and converts it into a boolean value.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Boolean",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IObservableParser<byte, bool> Boolean
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches a single byte.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Byte",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IObservableParser<byte, byte> Byte
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches a single byte as a signed byte.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "SByte",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IObservableParser<byte, sbyte> SByte
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into an integer.
    /// </summary>
    IObservableParser<byte, short> Int16
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into an unsigned integer.
    /// </summary>
    IObservableParser<byte, ushort> UInt16
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into an integer.
    /// </summary>
    IObservableParser<byte, int> Int32
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into an unsigned integer.
    /// </summary>
    IObservableParser<byte, uint> UInt32
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into an integer.
    /// </summary>
    IObservableParser<byte, long> Int64
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into an unsigned integer.
    /// </summary>
    IObservableParser<byte, ulong> UInt64
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into a floating point number.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Single",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IObservableParser<byte, float> Single
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into a floating point number.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Double",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IObservableParser<byte, double> Double
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into a unicode character.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Char",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IObservableParser<byte, char> Char
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
    IObservableParser<byte, string> HexString(int length);

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
    IObservableParser<byte, string> String(Encoding encoding, int length);

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
    IObservableParser<byte, string> String(Encoding encoding);

    /// <summary>
    /// Creates a parser with a grammar that matches the number of bytes in the specified 
    /// enum type and converts them into an instance of the enum.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum value to be parsed.</typeparam>
    /// <returns>A parser with a grammar that matches the number of bytes in the specified 
    /// enum type and converts them into an instance of the enum.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Enum",
      Justification = "The scope of accessibility is only intended for use within a parser grammar.")]
    IObservableParser<byte, TEnum> Enum<TEnum>()
      where TEnum : struct;
  }

  [ContractClassFor(typeof(IBinaryObservableParser<>))]
  internal abstract class IBinaryObservableParserContract<TResult> : IBinaryObservableParser<TResult>
  {
    public IObservableParser<byte, bool> Boolean
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, bool>>() != null);
        return null;
      }
    }

    public IObservableParser<byte, byte> Byte
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, byte>>() != null);
        return null;
      }
    }

    public IObservableParser<byte, sbyte> SByte
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, sbyte>>() != null);
        return null;
      }
    }

    public IObservableParser<byte, short> Int16
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, short>>() != null);
        return null;
      }
    }

    public IObservableParser<byte, ushort> UInt16
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, ushort>>() != null);
        return null;
      }
    }

    public IObservableParser<byte, int> Int32
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, int>>() != null);
        return null;
      }
    }

    public IObservableParser<byte, uint> UInt32
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, uint>>() != null);
        return null;
      }
    }

    public IObservableParser<byte, long> Int64
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, long>>() != null);
        return null;
      }
    }

    public IObservableParser<byte, ulong> UInt64
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, ulong>>() != null);
        return null;
      }
    }

    public IObservableParser<byte, float> Single
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, float>>() != null);
        return null;
      }
    }

    public IObservableParser<byte, double> Double
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, double>>() != null);
        return null;
      }
    }

    public IObservableParser<byte, char> Char
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<byte, char>>() != null);
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

    public IObservableParser<byte, string> HexString(int length)
    {
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IObservableParser<byte, string>>() != null);
      return null;
    }

    public IObservableParser<byte, string> String(Encoding encoding, int length)
    {
      Contract.Requires(encoding != null);
      Contract.Requires(length >= 0);
      Contract.Ensures(Contract.Result<IObservableParser<byte, string>>() != null);
      return null;
    }

    public IObservableParser<byte, string> String(Encoding encoding)
    {
      Contract.Requires(encoding != null);
      Contract.Ensures(Contract.Result<IObservableParser<byte, string>>() != null);
      return null;
    }

    public IObservableParser<byte, TEnum> Enum<TEnum>()
      where TEnum : struct
    {
#if !PORT_45 && !UNIVERSAL
      Contract.Requires(typeof(TEnum).IsEnum);
#else
      Contract.Requires(typeof(TEnum).GetTypeInfo().IsEnum);
#endif
      Contract.Ensures(Contract.Result<IObservableParser<byte, TEnum>>() != null);
      return null;
    }

    #region IObservableParser<byte,TResult> Members
    public IObservableParser<byte, byte> Next
    {
      get
      {
        return null;
      }
    }

    public IObservable<IParseResult<TResult>> Parse(IObservableCursor<byte> source)
    {
      return null;
    }
    #endregion
  }
}