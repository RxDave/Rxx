using System.Xml.Linq;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Represents a context-free XML parser over an observable sequence of <see cref="char"/>.
  /// </summary>
  public class XmlObservableParser : XmlObservableParser<XElement>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    /// <summary>
    /// Gets the parser's grammar.  The default grammar matches an XML element with any attributes or content.
    /// </summary>
    protected override IObservableParser<char, XElement> Schema
    {
      get
      {
        return AnyElement;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="XmlObservableParser" /> class with the specified case-sensitivity.
    /// </summary>
    /// <param name="caseSensitive">Indicates whether the comparison behavior used for matching element and attribute names
    /// must ignore case.</param>
    public XmlObservableParser(bool caseSensitive)
      : base(caseSensitive)
    {
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="XmlObservableParser" /> class.
    /// </summary>
    public XmlObservableParser()
    {
    }
    #endregion

    #region Methods
    #endregion
  }
}
