using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Text;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Represents a parser context over an observable sequence of bytes to support in-line grammars.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of bytes.</typeparam>
  /// <typeparam name="TQueryValue">The type of the current value in the query context.</typeparam>
  public sealed class BinaryObservableParserQueryContext<TResult, TQueryValue> : IBinaryObservableParser<TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    internal IBinaryObservableParser<TResult> Parser
    {
      get
      {
        Contract.Ensures(Contract.Result<IBinaryObservableParser<TResult>>() != null);

        return parser;
      }
    }

    internal TQueryValue Value
    {
      get
      {
        return queryValue;
      }
    }

    private readonly IBinaryObservableParser<TResult> parser;
    private readonly TQueryValue queryValue;
    #endregion

    #region Constructors
    internal BinaryObservableParserQueryContext(IBinaryObservableParser<TResult> parser, TQueryValue value)
    {
      Contract.Requires(parser != null);

      this.parser = parser;
      this.queryValue = value;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(parser != null);
    }
    #endregion

    #region IObservableParser<byte,TParseResult> Members
    IObservableParser<byte, byte> IObservableParser<byte, TResult>.Next
    {
      get
      {
        return parser.Next;
      }
    }

    IObservable<IParseResult<TResult>> IObservableParser<byte, TResult>.Parse(IObservableCursor<byte> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion

    #region IBinaryObservableParser<TResult> Members
    /// <summary>
    /// Gets a parser with a grammar that matches a single byte and converts it into a boolean value.
    /// </summary>
    public IObservableParser<byte, bool> Boolean
    {
      get
      {
        return parser.Boolean;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches a single byte.
    /// </summary>
    public IObservableParser<byte, byte> Byte
    {
      get
      {
        return parser.Byte;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches a single byte as a signed byte.
    /// </summary>
    [CLSCompliant(false)]
    public IObservableParser<byte, sbyte> SByte
    {
      get
      {
        return parser.SByte;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into an integer.
    /// </summary>
    public IObservableParser<byte, short> Int16
    {
      get
      {
        return parser.Int16;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into an unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    public IObservableParser<byte, ushort> UInt16
    {
      get
      {
        return parser.UInt16;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into an integer.
    /// </summary>
    public IObservableParser<byte, int> Int32
    {
      get
      {
        return parser.Int32;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into an unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    public IObservableParser<byte, uint> UInt32
    {
      get
      {
        return parser.UInt32;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into an integer.
    /// </summary>
    public IObservableParser<byte, long> Int64
    {
      get
      {
        return parser.Int64;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into an unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    public IObservableParser<byte, ulong> UInt64
    {
      get
      {
        return parser.UInt64;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches four bytes and converts them into a floating point number.
    /// </summary>
    public IObservableParser<byte, float> Single
    {
      get
      {
        return parser.Single;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches eight bytes and converts them into a floating point number.
    /// </summary>
    public IObservableParser<byte, double> Double
    {
      get
      {
        return parser.Double;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches two bytes and converts them into a unicode character.
    /// </summary>
    public IObservableParser<byte, char> Char
    {
      get
      {
        return parser.Char;
      }
    }

    /// <summary>
    /// Orders the specified sequence of bytes according to the endianness of the parser and the 
    /// endianness of the host platform.
    /// </summary>
    /// <param name="bytes">The sequence of bytes to be ordered.</param>
    /// <returns>The specified <paramref name="bytes"/> in reverse order, if the endianness of the 
    /// parser does not match the host platform; otherwise, <paramref name="bytes"/> is returned 
    /// unmodified.</returns>
    public IEnumerable<byte> Order(IEnumerable<byte> bytes)
    {
      return parser.Order(bytes);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string of hyphen-delimited hexadecimal pairs representing each byte.
    /// </summary>
    /// <param name="length">The number of bytes to be converted.</param>
    /// <returns>A parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string of hyphen-delimited hexadecimal pairs representing each byte..</returns>
    public IObservableParser<byte, string> HexString(int length)
    {
      return parser.HexString(length);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string in the specified <paramref name="encoding"/>.
    /// </summary>
    /// <param name="encoding">The character encoding for converting the bytes into a string.</param>
    /// <param name="length">The number of bytes to be converted.</param>
    /// <returns>A parser with a grammar that matches the specified number of bytes and converts them 
    /// into a string in the specified <paramref name="encoding"/>.</returns>
    public IObservableParser<byte, string> String(Encoding encoding, int length)
    {
      return parser.String(encoding, length);
    }

    /// <summary>
    /// Creates a parser with a grammar that returns a string in the specified <paramref name="encoding"/>
    /// and with a length that is specified by a 7-bit encoded prefix, which is written by the 
    /// <see cref="System.IO.BinaryWriter.Write7BitEncodedInt"/> method.
    /// </summary>
    /// <param name="encoding">The character encoding for converting the bytes into a string.</param>
    /// <returns>A parser with a grammar that returns a string in the specified <paramref name="encoding"/>
    /// and with a length that is specified by a 7-bit encoded prefix.</returns>
    public IObservableParser<byte, string> String(Encoding encoding)
    {
      return parser.String(encoding);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches the number of bytes in the specified 
    /// enum type and converts them into an instance of the enum.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum value to be parsed.</typeparam>
    /// <returns>A parser with a grammar that matches the number of bytes in the specified 
    /// enum type and converts them into an instance of the enum.</returns>
    public IObservableParser<byte, TEnum> Enum<TEnum>()
      where TEnum : struct
    {
      return parser.Enum<TEnum>();
    }
    #endregion
  }
}