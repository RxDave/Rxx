using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Rxx.UnitTests.Parsers
{
  internal static class Triples
  {
    public static IObservable<char> ObservableSequence
    {
      get
      {
        return Sequence.ToObservable();
      }
    }

    public const string Sequence =

      // 6 possible combinations: 
      "1.a` " +   // number punctuation letter
      "b2?_ " +   // letter number punctuation (underscore)
      "*!c3$" +   // punctuation letter number (star & dollars)
      "(d;4 " +   // letter punctuation number
      "|5e: " +   // number letter punctuation
      "%,6f " +   // punctuation number letter

      // 6 possible combinations: 
      "<g#7 " +   // letter punctuation number
      "8&h^ " +   // number punctuation letter
      "[9i= " +   // punctuation number letter
      "0j)+ " +   // number letter punctuation (plus)
      "*]k1$" +   // punctuation letter number (star & dollars)
      "/l2@/ ";   // letter number punctuation


    /* Result Patterns */

    public const string NotLetter = "1.` 2?_ *!3$(;4 |5: %,6 <#7 8&^ [9= 0)+ *]1$/2@/ ";

    public const string OrLettersNumbers = "1ab2c3d45e6fg78h9i0jk1l2";
    public const string OrLettersNumbersSpaces = "1a b2 c3d4 5e 6f g7 8h 9i 0j k1l2 ";
    public const string OrLettersNumbersSpacesUnderscorePlus = "1a b2_ c3d4 5e 6f g7 8h 9i 0j+ k1l2 ";

    public static readonly IList<string> AndLetterPunctuation = ReadOnly("d;", "e:", "g#", "j)");
    public static readonly IList<string> AndLetterPunctuationNumber = ReadOnly("d;4", "g#7");
    public static readonly IList<string> AndLetterNumberPunctuation = ReadOnly("b2?", "l2@");
    public static readonly IList<string> AndNumberLetterPunctuation = ReadOnly("5e:", "0j)");
    public static readonly IList<string> AndStarPunctuationLetterNumberDollars = ReadOnly("*!c3$", "*]k1$");

    internal static ReadOnlyCollection<T> ReadOnly<T>(params T[] objects)
    {
      return objects.ToList().AsReadOnly();
    }
  }
}