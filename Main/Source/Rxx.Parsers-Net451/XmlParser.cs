using System.Xml.Linq;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents a context-free XML parser over a <see cref="string"/> or an enumerable sequence of <see cref="char"/>.
  /// </summary>
  public class XmlParser : XmlParser<XElement>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    /// <summary>
    /// Gets the parser's grammar.  The default grammar matches an XML element with any attributes or content.
    /// </summary>
    protected override IParser<char, XElement> Schema
    {
      get
      {
        return AnyElement;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="XmlParser" /> class with the specified case-sensitivity.
    /// </summary>
    /// <param name="caseSensitive">Indicates whether the comparison behavior used for matching element and attribute names
    /// must ignore case.</param>
    public XmlParser(bool caseSensitive)
      : base(caseSensitive)
    {
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="XmlParser" /> class.
    /// </summary>
    public XmlParser()
    {
    }
    #endregion

    #region Methods
    #endregion
  }
}
