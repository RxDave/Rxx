using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents a parser over a <see cref="string"/> or an enumerable sequence of <see cref="char"/>.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of characters.</typeparam>
  [ContractClass(typeof(IStringParserContract<>))]
#if !SILVERLIGHT || WINDOWS_PHONE
  public interface IStringParser<out TResult> : IParser<char, TResult>
#else
	public interface IStringParser<TResult> : IParser<char, TResult>
#endif
  {
    /// <summary>
    /// Gets a parser with a grammar that matches any character.
    /// </summary>
    IParser<char, char> AnyCharacter { get; }

    /// <summary>
    /// Gets a parser with a grammar that matches one or more consecutive whitespace characters according to the 
    /// rules of <see cref="char.IsWhiteSpace(char)"/> and joins them into a <see cref="string"/>.
    /// </summary>
    IParser<char, string> WhiteSpace { get; }

    /// <summary>
    /// Gets a parser with a grammar that matches zero or more consecutive insignificant whitespace characters and joins
    /// them into a <see cref="string"/>.
    /// </summary>
    IParser<char, string> InsignificantWhiteSpace { get; }

    /// <summary>
    /// Creates a parser with a grammar that matches one or more consecutive characters until any of the specified 
    /// stop characters are parsed and joins them into a <see cref="string"/>, excluding the stop character.
    /// </summary>
    /// <param name="stops">The characters at which to stop consuming characters.</param>
    /// <returns>A parser with a grammar that consumes consecutive characters until any stop character is parsed
    /// or the sequence ends.</returns>
    IParser<char, string> AnyCharacterUntil(params char[] stops);

    /// <summary>
    /// Creates a parser with a grammar that matches one or more consecutive characters until any of the specified 
    /// stop words are parsed and joins them into a <see cref="string"/>, excluding the stop word.
    /// </summary>
    /// <param name="stopWords">The strings at which to stop consuming characters.</param>
    /// <returns>A parser with a grammar that consumes consecutive characters until any stop word is parsed
    /// or the sequence ends.</returns>
    IParser<char, string> AnyCharacterUntil(params string[] stopWords);

    /// <summary>
    /// Creates a parser with a grammar that matches the specified string of characters.
    /// </summary>
    /// <param name="value">The string of characters to match.</param>
    /// <returns>A parser with a grammar that matches the specified string.</returns>
    IParser<char, string> Word(string value);

    /// <summary>
    /// Creates a parser with a grammar that matches the specified character.
    /// </summary>
    /// <param name="value">The <see cref="char"/> to match.</param>
    /// <returns>A parser with a grammar that matches the specified character.</returns>
    IParser<char, char> Character(char value);

    /// <summary>
    /// Creates a parser with a grammar that matches when the specified <paramref name="predicate"/>
    /// returns <see langword="true"/> for any given character.
    /// </summary>
    /// <param name="predicate">A function that receives each character and returns whether it will be consumed.</param>
    /// <returns>A parser with a grammar that matches each character for which the specified 
    /// <paramref name="predicate"/> returns <see langword="true" />.</returns>
    IParser<char, char> Character(Func<char, bool> predicate);

    /// <summary>
    /// Creates a parser with a grammar that matches each character within the specified 
    /// <paramref name="category"/>.
    /// </summary>
    /// <param name="category">The unicode character in which to match characters.</param>
    /// <returns>A parser with a grammar that matches each character within the specified 
    /// <paramref name="category"/>.</returns>
    IParser<char, char> Character(UnicodeCategory category);
  }

  [ContractClassFor(typeof(IStringParser<>))]
  internal abstract class IStringParserContract<TResult> : IStringParser<TResult>
  {
    public IParser<char, char> AnyCharacter
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, char>>() != null);
        return null;
      }
    }

    public IParser<char, string> WhiteSpace
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, string>>() != null);
        return null;
      }
    }

    public IParser<char, string> InsignificantWhiteSpace
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, string>>() != null);
        return null;
      }
    }

    public IParser<char, string> AnyCharacterUntil(params char[] stops)
    {
      Contract.Requires(stops != null);
      Contract.Requires(stops.Length > 0);
      Contract.Ensures(Contract.Result<IParser<char, string>>() != null);
      return null;
    }

    public IParser<char, string> AnyCharacterUntil(params string[] stopWords)
    {
      Contract.Requires(stopWords != null);
      Contract.Requires(stopWords.Length > 0);
      ////Contract.Requires(Contract.ForAll(stopWords, word => !string.IsNullOrEmpty(word)));
      Contract.Ensures(Contract.Result<IParser<char, string>>() != null);
      return null;
    }

    public IParser<char, string> Word(string value)
    {
      Contract.Requires(!string.IsNullOrEmpty(value));
      Contract.Ensures(Contract.Result<IParser<char, string>>() != null);
      return null;
    }

    public IParser<char, char> Character(char value)
    {
      Contract.Ensures(Contract.Result<IParser<char, char>>() != null);
      return null;
    }

    public IParser<char, char> Character(Func<char, bool> predicate)
    {
      Contract.Requires(predicate != null);
      Contract.Ensures(Contract.Result<IParser<char, char>>() != null);
      return null;
    }

    public IParser<char, char> Character(UnicodeCategory category)
    {
      Contract.Ensures(Contract.Result<IParser<char, char>>() != null);
      return null;
    }

    #region IParser<char,TResult> Members
    public IParser<char, char> Next
    {
      get
      {
        return null;
      }
    }

    public IEnumerable<IParseResult<TResult>> Parse(ICursor<char> source)
    {
      return null;
    }
    #endregion
  }
}
