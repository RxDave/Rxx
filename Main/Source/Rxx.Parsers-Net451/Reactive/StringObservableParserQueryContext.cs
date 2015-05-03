using System;
using System.Diagnostics.Contracts;
using System.Reactive;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Represents a parser context over an enumerable sequence of <see cref="char"/>
  /// to support in-line grammars.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of characters.</typeparam>
  /// <typeparam name="TQueryValue">The type of the current value in the query context.</typeparam>
  public sealed class StringObservableParserQueryContext<TResult, TQueryValue> : IStringObservableParser<TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    internal IStringObservableParser<TResult> Parser
    {
      get
      {
        Contract.Ensures(Contract.Result<IStringObservableParser<TResult>>() != null);

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

    private readonly IStringObservableParser<TResult> parser;
    private readonly TQueryValue queryValue;
    #endregion

    #region Constructors
    internal StringObservableParserQueryContext(IStringObservableParser<TResult> parser, TQueryValue value)
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

    #region IObservableParser<char,TParseResult> Members
    IObservableParser<char, char> IObservableParser<char, TResult>.Next
    {
      get
      {
        return parser.Next;
      }
    }

    IObservable<IParseResult<TResult>> IObservableParser<char, TResult>.Parse(IObservableCursor<char> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion

    #region IStringObservableParser<TResult> Members
    /// <summary>
    /// Gets a parser with a grammar that matches any character.
    /// </summary>
    public IObservableParser<char, char> AnyCharacter
    {
      get
      {
        return parser.AnyCharacter;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches one or more consecutive whitespace characters according to the 
    /// rules of <see cref="char.IsWhiteSpace(char)"/> and joins them into a <see cref="string"/>.
    /// </summary>
    public IObservableParser<char, string> WhiteSpace
    {
      get
      {
        return parser.WhiteSpace;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches zero or more consecutive insignificant whitespace characters and joins
    /// them into a <see cref="string"/>.
    /// </summary>
    public IObservableParser<char, string> InsignificantWhiteSpace
    {
      get
      {
        return parser.InsignificantWhiteSpace;
      }
    }

    /// <summary>
    /// Creates a parser with a grammar that matches one or more consecutive characters until any of the specified 
    /// stop characters are parsed and joins them into a <see cref="string"/>, excluding the stop character.
    /// </summary>
    /// <param name="stops">The characters at which to stop consuming characters.</param>
    /// <returns>A parser with a grammar that consumes consecutive characters until any stop character is parsed
    /// or the sequence ends.</returns>
    public IObservableParser<char, string> AnyCharacterUntil(params char[] stops)
    {
      return parser.AnyCharacterUntil(stops);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches one or more consecutive characters until any of the specified 
    /// stop words are parsed and joins them into a <see cref="string"/>, excluding the stop word.
    /// </summary>
    /// <param name="stopWords">The strings at which to stop consuming characters.</param>
    /// <returns>A parser with a grammar that consumes consecutive characters until any stop word is parsed
    /// or the sequence ends.</returns>
    public IObservableParser<char, string> AnyCharacterUntil(params string[] stopWords)
    {
      return parser.AnyCharacterUntil(stopWords);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches the specified string of characters.
    /// </summary>
    /// <param name="value">The string of characters to match.</param>
    /// <returns>A parser with a grammar that matches the specified string.</returns>
    public IObservableParser<char, string> Word(string value)
    {
      return parser.Word(value);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches the specified character.
    /// </summary>
    /// <param name="value">The <see cref="char"/> to match.</param>
    /// <returns>A parser with a grammar that matches the specified character.</returns>
    public IObservableParser<char, char> Character(char value)
    {
      return parser.Character(value);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches when the specified <paramref name="predicate"/>
    /// returns <see langword="true"/> for any given character.
    /// </summary>
    /// <param name="predicate">A function that receives each character and returns whether it will be consumed.</param>
    /// <returns>A parser with a grammar that matches each character for which the specified 
    /// <paramref name="predicate"/> returns <see langword="true" />.</returns>
    public IObservableParser<char, char> Character(Func<char, bool> predicate)
    {
      return parser.Character(predicate);
    }

    /// <summary>
    /// Creates a parser with a grammar that matches each character within the specified 
    /// <paramref name="category"/>.
    /// </summary>
    /// <param name="category">The unicode character in which to match characters.</param>
    /// <returns>A parser with a grammar that matches each character within the specified 
    /// <paramref name="category"/>.</returns>
    public IObservableParser<char, char> Character(System.Globalization.UnicodeCategory category)
    {
      return parser.Character(category);
    }
    #endregion
  }
}