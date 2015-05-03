using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers
{
  internal sealed class InlineStringParser<TResult> : StringParser<TResult>, IStringParser<char>
  {
    #region Public Properties
    protected override IParser<char, TResult> Start
    {
      get
      {
        return start;
      }
    }
    #endregion

    #region Private / Protected
    private IParser<char, TResult> start;
    #endregion

    #region Constructors
    public InlineStringParser()
    {
      start = new AnonymousParser<char, TResult>(
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

    public IEnumerable<TResult> Parse(ICursor<char> source, IParser<char, TResult> grammar)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Requires(grammar != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      this.start = grammar;

      return base.Parse(source);
    }
    #endregion

    #region IParser<char,char> Members
    IParser<char, char> IParser<char, char>.Next
    {
      get
      {
        return Next;
      }
    }

    IEnumerable<IParseResult<char>> IParser<char, char>.Parse(ICursor<char> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion

    #region IStringParser<char> Members
    IParser<char, char> IStringParser<char>.AnyCharacter
    {
      get
      {
        return AnyCharacter;
      }
    }

    IParser<char, string> IStringParser<char>.WhiteSpace
    {
      get
      {
        return WhiteSpace;
      }
    }

    IParser<char, string> IStringParser<char>.InsignificantWhiteSpace
    {
      get
      {
        return InsignificantWhiteSpace;
      }
    }

    IParser<char, string> IStringParser<char>.AnyCharacterUntil(params char[] stops)
    {
      return AnyCharacterUntil(stops);
    }

    IParser<char, string> IStringParser<char>.AnyCharacterUntil(params string[] stopWords)
    {
      return AnyCharacterUntil(stopWords);
    }

    IParser<char, string> IStringParser<char>.Word(string value)
    {
      return Word(value);
    }

    IParser<char, char> IStringParser<char>.Character(char value)
    {
      return Character(value);
    }

    IParser<char, char> IStringParser<char>.Character(Func<char, bool> predicate)
    {
      return Character(predicate);
    }

    IParser<char, char> IStringParser<char>.Character(System.Globalization.UnicodeCategory category)
    {
      return Character(category);
    }
    #endregion
  }
}
