using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rxx.Parsers.Linq;

namespace Rxx.UnitTests.Parsers.Interactive
{
  [TestClass]
  public class Projection : RxxTests
  {
    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserSelectManyCollection_MovesToEndOfFirstMatch()
    {
      var source = new[] { 2, 2, 0, 1, 0, 1 };

      var results = source.Parse(parser =>
        from next in parser
        select from elements in next.Exactly(2)
               from value in elements
               where value < 2
               select value);

      var values = new List<int>();

      foreach (var result in results)
      {
        values.Add(result);
      }

      Assert.AreEqual(4, values.Count);
    }

    [TestMethod, TestCategory("Parsers"), TestCategory("Ix")]
    public void ParserSelectManyCollection_MovesToEndOfFirstMatch_Nested()
    {
      var source = new[] { 2, 2, 0, 1, 0, 1 };

      var results = source.Parse(parser =>
        from next in parser
        select from elements in next.Exactly(2)
               from value in
                 elements.Parse(subparser =>
                   from subnext in subparser
                   select subnext.Where(value => value < 2))
               select value);

      var values = new List<int>();

      foreach (var result in results)
      {
        values.Add(result);
      }

      Assert.AreEqual(4, values.Count);
    }
  }
}