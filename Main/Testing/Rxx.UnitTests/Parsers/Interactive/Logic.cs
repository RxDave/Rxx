using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rxx.Parsers.Linq;

namespace Rxx.UnitTests.Parsers.Interactive
{
  [TestClass]
  public class Logic : RxxTests
  {
    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxNot()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
          from next in parser
          let letter = next.Where(char.IsLetter)
          select next.Not(letter)),
        Triples.NotLetter);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxAny()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let space = next.Where(char.IsWhiteSpace)
          select new[]
					{
						number, space, letter 
					}
          .Any()),
        Triples.OrLettersNumbersSpaces);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxAll()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
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

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxAnd()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
          from next in parser
          let letter = next.Where(char.IsLetter)
          let punctuation = next.Where(char.IsPunctuation)
          select letter.And(punctuation).Join()),
        Triples.AndLetterPunctuation);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxAndNestedLeft()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let punctuation = next.Where(char.IsPunctuation)
          select letter.And(punctuation).And(number).Join()),
        Triples.AndLetterPunctuationNumber);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxAndNestedRight()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let punctuation = next.Where(char.IsPunctuation)
          select letter.And(number.And(punctuation)).Join()),
        Triples.AndLetterNumberPunctuation);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxAndNestedAllLeft()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
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

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxAndNestedAllRight()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
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
          .All()).Join()),
        Triples.AndStarPunctuationLetterNumberDollars);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxAndNestedAll()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
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
          .All()
          .And(new[]
					{
						letter, number, dollars
					}
          .All())
          .Join()),
        Triples.AndStarPunctuationLetterNumberDollars);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxOr()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          select letter.Or(number)),
        Triples.OrLettersNumbers);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxOrNestedLeft()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let space = next.Where(char.IsWhiteSpace)
          select space.Or(number).Or(letter)),
        Triples.OrLettersNumbersSpaces);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxOrNestedRight()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
          from next in parser
          let number = next.Where(char.IsNumber)
          let letter = next.Where(char.IsLetter)
          let space = next.Where(char.IsWhiteSpace)
          select space.Or(letter.Or(number))),
        Triples.OrLettersNumbersSpaces);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxOrNestedAnyLeft()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
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

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxOrNestedAnyRight()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
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

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserIxOrNestedAny()
    {
      AssertEqual(
        Triples.Sequence.Parse(parser =>
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