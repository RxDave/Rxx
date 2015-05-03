using System;
using System.Diagnostics.Contracts;
using System.Reactive;

namespace Rxx.Parsers.Reactive
{
  internal sealed class InlineStringObservableParser<TResult> : StringObservableParser<TResult>, IStringObservableParser<char>
  {
    #region Public Properties
    protected override IObservableParser<char, TResult> Start
    {
      get
      {
        return start;
      }
    }
    #endregion

    #region Private / Protected
    private IObservableParser<char, TResult> start;
    #endregion

    #region Constructors
    public InlineStringObservableParser()
    {
      start = new AnonymousObservableParser<char, TResult>(
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

    public IObservable<TResult> Parse(IObservableCursor<char> source, IObservableParser<char, TResult> grammar)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Requires(grammar != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      this.start = grammar;

      return base.Parse(source);
    }
    #endregion

    #region IObservableParser<char,char> Members
    IObservableParser<char, char> IObservableParser<char, char>.Next
    {
      get
      {
        return Next;
      }
    }

    IObservable<IParseResult<char>> IObservableParser<char, char>.Parse(IObservableCursor<char> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion

    #region IStringObservableParser<char> Members
    IObservableParser<char, char> IStringObservableParser<char>.AnyCharacter
    {
      get
      {
        return AnyCharacter;
      }
    }

    IObservableParser<char, string> IStringObservableParser<char>.WhiteSpace
    {
      get
      {
        return WhiteSpace;
      }
    }

    IObservableParser<char, string> IStringObservableParser<char>.InsignificantWhiteSpace
    {
      get
      {
        return InsignificantWhiteSpace;
      }
    }

    IObservableParser<char, string> IStringObservableParser<char>.AnyCharacterUntil(params char[] stops)
    {
      return AnyCharacterUntil(stops);
    }

    IObservableParser<char, string> IStringObservableParser<char>.AnyCharacterUntil(params string[] stopWords)
    {
      return AnyCharacterUntil(stopWords);
    }

    IObservableParser<char, string> IStringObservableParser<char>.Word(string value)
    {
      return Word(value);
    }

    IObservableParser<char, char> IStringObservableParser<char>.Character(char value)
    {
      return Character(value);
    }

    IObservableParser<char, char> IStringObservableParser<char>.Character(Func<char, bool> predicate)
    {
      return Character(predicate);
    }

    IObservableParser<char, char> IStringObservableParser<char>.Character(System.Globalization.UnicodeCategory category)
    {
      return Character(category);
    }
    #endregion
  }
}
