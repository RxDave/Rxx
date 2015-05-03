using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using Rxx.Parsers.Linq;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents a parser over a <see cref="string"/> or an enumerable sequence of <see cref="char"/>.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of characters.</typeparam>
  public abstract class StringParser<TResult> : Parser<char, TResult>, IStringParser<TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    /// <summary>
    /// Gets a parser with a grammar that matches any character.
    /// </summary>
    protected IParser<char, char> AnyCharacter
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, char>>() != null);

        return Next;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches one or more consecutive whitespace characters according to the 
    /// rules of <see cref="char.IsWhiteSpace(char)"/> and joins them into a <see cref="string"/>.
    /// </summary>
    protected IParser<char, string> WhiteSpace
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, string>>() != null);

        return whiteSpace;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches zero or more consecutive whitespace characters determined by the 
    /// <see cref="InsignificantWhiteSpaceCharacters"/> collection and joins them into a <see cref="string"/>.
    /// </summary>
    protected IParser<char, string> InsignificantWhiteSpace
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, string>>() != null);

        return insignificantWhiteSpace;
      }
    }

    private ICollection<char> InsignificantWhiteSpaceCached
    {
      get
      {
        Contract.Ensures(Contract.Result<ICollection<char>>() != null);

        return insignificantWhiteSpaceCached ?? (insignificantWhiteSpaceCached = new List<char>(InsignificantWhiteSpaceCharacters));
      }
    }

    /// <summary>
    /// Gets a collection of characters that are considered insignificant whitespace for the  
    /// <see cref="InsignificantWhiteSpace"/> parser.  
    /// </summary>
    protected virtual ICollection<char> InsignificantWhiteSpaceCharacters
    {
      get
      {
        Contract.Ensures(Contract.Result<ICollection<char>>() != null);

        return StringParserDefaults.InsignificantWhiteSpaceCharacters;
      }
    }

    private ICollection<char> insignificantWhiteSpaceCached;
    private readonly IParser<char, string> whiteSpace, insignificantWhiteSpace;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="StringParser{TResult}" /> class for derived classes.
    /// </summary>
    protected StringParser()
    {
      whiteSpace = Character(char.IsWhiteSpace).OneOrMore().Join();
      insignificantWhiteSpace = Character(c => InsignificantWhiteSpaceCached.Contains(c)).NoneOrMore().Join();
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(whiteSpace != null);
      Contract.Invariant(insignificantWhiteSpace != null);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches one or more consecutive characters until any of the specified 
    /// stop characters are parsed and joins them into a <see cref="string"/>, excluding the stop character.
    /// </summary>
    /// <param name="stops">The characters at which to stop consuming characters.</param>
    /// <returns>A parser with a grammar that consumes consecutive characters until any stop character is parsed
    /// or the sequence ends.</returns>
    protected IParser<char, string> AnyCharacterUntil(params char[] stops)
    {
      Contract.Requires(stops != null);
      Contract.Requires(stops.Length > 0);
      Contract.Ensures(Contract.Result<IParser<char, string>>() != null);

      return AnyCharacter.Not(Character(c => stops.Contains(c))).OneOrMore().Join();
    }

    /// <summary>
    /// Creates a parser with a grammar that matches one or more consecutive characters until any of the specified 
    /// stop words are parsed and joins them into a <see cref="string"/>, excluding the stop word.
    /// </summary>
    /// <param name="stopWords">The strings at which to stop consuming characters.</param>
    /// <returns>A parser with a grammar that consumes consecutive characters until any stop word is parsed
    /// or the sequence ends.</returns>
    protected IParser<char, string> AnyCharacterUntil(params string[] stopWords)
    {
      Contract.Requires(stopWords != null);
      Contract.Requires(stopWords.Length > 0);
      Contract.Ensures(Contract.Result<IParser<char, string>>() != null);

      var stops = new List<IParser<char, string>>();

      foreach (var word in stopWords)
      {
        Contract.Assume(!string.IsNullOrEmpty(word));

        stops.Add(Word(word));
      }

      return AnyCharacter.Not(stops.Any()).OneOrMore().Join();
    }

    /// <summary>
    /// Creates a parser with a grammar that matches the specified string of characters.
    /// </summary>
    /// <param name="value">The string of characters to match.</param>
    /// <returns>A parser with a grammar that matches the specified string.</returns>
    protected IParser<char, string> Word(string value)
    {
      Contract.Requires(!string.IsNullOrEmpty(value));
      Contract.Ensures(Contract.Result<IParser<char, string>>() != null);

      var parsers = new List<IParser<char, char>>();

      foreach (var wordChar in value)
      {
        parsers.Add(Character(wordChar));
      }

      return parsers.All().Join();
    }

    /// <summary>
    /// Creates a parser with a grammar that matches the specified character.
    /// </summary>
    /// <param name="value">The <see cref="char"/> to match.</param>
    /// <returns>A parser with a grammar that matches the specified character.</returns>
    protected IParser<char, char> Character(char value)
    {
      Contract.Ensures(Contract.Result<IParser<char, char>>() != null);

      return Next.Where(c => c == value);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches when the specified <paramref name="predicate"/>
    /// returns <see langword="true"/> for any given character.
    /// </summary>
    /// <param name="predicate">A function that receives each character and returns whether it will be consumed.</param>
    /// <returns>A parser with a grammar that matches each character for which the specified 
    /// <paramref name="predicate"/> returns <see langword="true" />.</returns>
    protected IParser<char, char> Character(Func<char, bool> predicate)
    {
      Contract.Requires(predicate != null);
      Contract.Ensures(Contract.Result<IParser<char, char>>() != null);

      return Next.Where(predicate);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches each character within the specified 
    /// <paramref name="category"/>.
    /// </summary>
    /// <param name="category">The unicode character in which to match characters.</param>
    /// <returns>A parser with a grammar that matches each character within the specified 
    /// <paramref name="category"/>.</returns>
    protected IParser<char, char> Character(UnicodeCategory category)
    {
      Contract.Ensures(Contract.Result<IParser<char, char>>() != null);

      return Next.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) == category);
    }
    #endregion

    #region IStringParser<TResult> Members
    IParser<char, char> IStringParser<TResult>.AnyCharacter
    {
      get
      {
        return AnyCharacter;
      }
    }

    IParser<char, string> IStringParser<TResult>.WhiteSpace
    {
      get
      {
        return WhiteSpace;
      }
    }

    IParser<char, string> IStringParser<TResult>.InsignificantWhiteSpace
    {
      get
      {
        return InsignificantWhiteSpace;
      }
    }

    IParser<char, string> IStringParser<TResult>.AnyCharacterUntil(params char[] stops)
    {
      return AnyCharacterUntil(stops);
    }

    IParser<char, string> IStringParser<TResult>.AnyCharacterUntil(params string[] stopWords)
    {
      return AnyCharacterUntil(stopWords);
    }

    IParser<char, string> IStringParser<TResult>.Word(string value)
    {
      return Word(value);
    }

    IParser<char, char> IStringParser<TResult>.Character(char value)
    {
      return Character(value);
    }

    IParser<char, char> IStringParser<TResult>.Character(Func<char, bool> predicate)
    {
      return Character(predicate);
    }

    IParser<char, char> IStringParser<TResult>.Character(UnicodeCategory category)
    {
      return Character(category);
    }
    #endregion
  }
}