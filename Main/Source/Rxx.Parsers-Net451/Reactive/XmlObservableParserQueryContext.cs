using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Xml.Linq;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Represents an XML parser context over an observable sequence of <see cref="char"/>
  /// to support in-line grammars.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of XML nodes.</typeparam>
  /// <typeparam name="TQueryValue">The type of the current value in the query context.</typeparam>
  public sealed class XmlObservableParserQueryContext<TResult, TQueryValue> : IXmlObservableParser<TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    internal IXmlObservableParser<TResult> Parser
    {
      get
      {
        Contract.Ensures(Contract.Result<IXmlObservableParser<TResult>>() != null);

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

    private readonly IXmlObservableParser<TResult> parser;
    private readonly TQueryValue queryValue;
    #endregion

    #region Constructors
    internal XmlObservableParserQueryContext(IXmlObservableParser<TResult> parser, TQueryValue value)
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

    #region IObservableParser<char,TResult> Members
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

    #region IXmlObservableParser Members
    /// <summary>
    /// Gets a parser with a grammar that matches all characters, including whitespace, up to the start tag of an element.
    /// </summary>
    public IObservableParser<char, XText> Text
    {
      get
      {
        return parser.Text;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches content sequences beginning with &lt;!-- and ending with --&gt;.
    /// </summary>
    public IObservableParser<char, XComment> Comment
    {
      get
      {
        return parser.Comment;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches content sequences beginning with &lt;![CDATA[ and ending with ]]&gt;.
    /// </summary>
    public IObservableParser<char, XCData> CData
    {
      get
      {
        return parser.CData;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches any <see cref="Text"/>, <see cref="Comment"/> or <see cref="CData"/> content.
    /// </summary>
    public IObservableParser<char, XObject> AnyContent
    {
      get
      {
        return parser.AnyContent;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches an XML element with any attributes or content.
    /// </summary>
    public IObservableParser<char, XElement> AnyElement
    {
      get
      {
        return parser.AnyElement;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches an XML attribute.
    /// </summary>
    public IObservableParser<char, XAttribute> AnyAttribute
    {
      get
      {
        return parser.AnyAttribute;
      }
    }

    /// <summary>
    /// Creates a parser that matches a single XML attribute with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the attribute to match.</param>
    /// <returns>A parser that matches a single XML attribute with the specified <paramref name="name"/>.</returns>
    public IObservableParser<char, XAttribute> Attribute(string name)
    {
      return parser.Attribute(name);
    }

    /// <summary>
    /// Creates a parser that matches a single XML element containing the specified attributes and children.
    /// </summary>
    /// <param name="content">The parsers that match the element's attributes, child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element containing the specified attributes and children.</returns>
    public IObservableParser<char, XElement> Element(params IObservableParser<char, XObject>[] content)
    {
      return parser.Element(content);
    }

    /// <summary>
    /// Creates a parser that matches a single XML element having no attributes and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element having no attributes and containing the specified <paramref name="content"/>.</returns>
    public IObservableParser<char, XElement> Element(IObservableParser<char, IObservable<XObject>> content)
    {
      return parser.Element(content);
    }

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="attributes"/> and containing 
    /// the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="attributes">The parser that matches the element's attributes.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="attributes"/> and containing 
    /// the specified <paramref name="content"/>.</returns>
    public IObservableParser<char, XElement> Element(IObservableParser<char, IObservable<XAttribute>> attributes, IObservableParser<char, IObservable<XObject>> content)
    {
      return parser.Element(attributes, content);
    }

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/> and containing 
    /// the specified attributes and children.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="content">The parsers that match the element's attributes, child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/> and containing 
    /// the specified attributes and children.</returns>
    public IObservableParser<char, XElement> Element(string name, params IObservableParser<char, XObject>[] content)
    {
      return parser.Element(name, content);
    }

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/>, having no attributes
    /// and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/>, having no attributes 
    /// and containing the specified <paramref name="content"/>.</returns>
    public IObservableParser<char, XElement> Element(string name, IObservableParser<char, IObservable<XObject>> content)
    {
      return parser.Element(name, content);
    }

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/>, the specified 
    /// <paramref name="attributes"/> and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="attributes">The parser that matches the element's attributes.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/>, the specified 
    /// <paramref name="attributes"/> and containing the specified <paramref name="content"/>.</returns>
    public IObservableParser<char, XElement> Element(string name, IObservableParser<char, IObservable<XAttribute>> attributes, IObservableParser<char, IObservable<XObject>> content)
    {
      return parser.Element(name, attributes, content);
    }

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/>, the specified 
    /// <paramref name="attributes"/> and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="attributes">An enumerable sequence containing the names of the element's attributes.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/>, the specified 
    /// <paramref name="attributes"/> and containing the specified <paramref name="content"/>.</returns>
    public IObservableParser<char, XElement> Element(string name, IEnumerable<string> attributes, params IObservableParser<char, XObject>[] content)
    {
      return parser.Element(name, attributes, content);
    }
    #endregion
  }
}