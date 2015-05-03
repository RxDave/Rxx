using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.UnitTests.Parsers.Reactive
{
  [TestClass]
  public class Logic : RxxTests
  {
    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxNot()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let letter = next.Where(char.IsLetter)
          select next.Not(letter)),
        Triples.NotLetter);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxAny()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(c => char.IsNumber(c))
          let letter = next.Where(c => char.IsLetter(c))
          let space = next.Where(c => char.IsWhiteSpace(c))
          select new[]
					{ 
						number, space, letter
					}
          .Any()),
        Triples.OrLettersNumbersSpaces);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxAll()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let punctuation = next.Where(char.IsPunctuation)
          select new[]
					{
						number, letter, punctuation
					}
          .All().Join()),
        Triples.AndNumberLetterPunctuation);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxAnd()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let letter = next.Where(char.IsLetter)
          let punctuation = next.Where(char.IsPunctuation)
          select letter.And(punctuation).Join()),
        Triples.AndLetterPunctuation);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxAndNestedLeft()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let punctuation = next.Where(char.IsPunctuation)
          select letter.And(punctuation).And(number).Join()),
        Triples.AndLetterPunctuationNumber);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxAndNestedRight()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let punctuation = next.Where(char.IsPunctuation)
          select letter.And(number.And(punctuation)).Join()),
        Triples.AndLetterNumberPunctuation);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxAndNestedAllLeft()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let punctuation = next.Where(char.IsPunctuation)
          let star = next.Where(c => c == '*')
          let dollars = next.Where(c => c == '$')
          select new[]
					{
						star, punctuation, letter
					}
          .All()
          .And(number.And(dollars)).Join()),
        Triples.AndStarPunctuationLetterNumberDollars);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxAndNestedAllRight()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let punctuation = next.Where(char.IsPunctuation)
          let star = next.Where(c => c == '*')
          let dollars = next.Where(c => c == '$')
          select star.And(punctuation).And(new[]
					{
						letter, number, dollars
					}
          .All())
          .Join()),
        Triples.AndStarPunctuationLetterNumberDollars);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxAndNestedAll()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let punctuation = next.Where(char.IsPunctuation)
          let star = next.Where(c => c == '*')
          let dollars = next.Where(c => c == '$')
          select new[]
					{
						star, punctuation
					}
          .All().And(new[]
					{
						letter, number, dollars
					}
          .All())
          .Join()),
        Triples.AndStarPunctuationLetterNumberDollars);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxOr()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          select letter.Or(number)),
        Triples.OrLettersNumbers);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxOrNestedLeft()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let space = next.Where(char.IsWhiteSpace)
          select space.Or(number).Or(letter)),
        Triples.OrLettersNumbersSpaces);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxOrNestedRight()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let space = next.Where(char.IsWhiteSpace)
          select space.Or(letter.Or(number))),
        Triples.OrLettersNumbersSpaces);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxOrNestedAnyLeft()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let space = next.Where(char.IsWhiteSpace)
          let underscore = next.Where(c => c == '_')
          let plus = next.Where(c => c == '+')
          select new[]
					{
						underscore, letter, space
					}
          .Any().Or(number.Or(plus))),
        Triples.OrLettersNumbersSpacesUnderscorePlus);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxOrNestedAnyRight()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let space = next.Where(char.IsWhiteSpace)
          let underscore = next.Where(c => c == '_')
          let plus = next.Where(c => c == '+')
          select underscore.Or(letter).Or(new[]
					{
						space, number, plus
					}
          .Any())),
        Triples.OrLettersNumbersSpacesUnderscorePlus);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Rx")]
    public void ParserRxOrNestedAny()
    {
      AssertEqual(
        Triples.ObservableSequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let space = next.Where(char.IsWhiteSpace)
          let underscore = next.Where(c => c == '_')
          let plus = next.Where(c => c == '+')
          select new[]
					{
						underscore, letter
					}
          .Any().Or(new[]
					{
						space, number, plus
					}
          .Any())),
        Triples.OrLettersNumbersSpacesUnderscorePlus);
    }
  }
}