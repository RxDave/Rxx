using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents an XML parser context over a <see cref="string"/> or an enumerable sequence of <see cref="char"/>
  /// to support in-line grammars.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of XML nodes.</typeparam>
  /// <typeparam name="TQueryValue">The type of the current value in the query context.</typeparam>
  public sealed class XmlParserQueryContext<TResult, TQueryValue> : IXmlParser<TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    internal IXmlParser<TResult> Parser
    {
      get
      {
        Contract.Ensures(Contract.Result<IXmlParser<TResult>>() != null);

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

    private readonly IXmlParser<TResult> parser;
    private readonly TQueryValue queryValue;
    #endregion

    #region Constructors
    internal XmlParserQueryContext(IXmlParser<TResult> parser, TQueryValue value)
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

    #region IParser<char,TResult> Members
    IParser<char, char> IParser<char, TResult>.Next
    {
      get
      {
        return parser.Next;
      }
    }

    IEnumerable<IParseResult<TResult>> IParser<char, TResult>.Parse(ICursor<char> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion

    #region IXmlParser Members
    /// <summary>
    /// Gets a parser with a grammar that matches all characters, including whitespace, up to the start tag of an element.
    /// </summary>
    public IParser<char, XText> Text
    {
      get
      {
        return parser.Text;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches content sequences beginning with &lt;!-- and ending with --&gt;.
    /// </summary>
    public IParser<char, XComment> Comment
    {
      get
      {
        return parser.Comment;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches content sequences beginning with &lt;![CDATA[ and ending with ]]&gt;.
    /// </summary>
    public IParser<char, XCData> CData
    {
      get
      {
        return parser.CData;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches any <see cref="Text"/>, <see cref="Comment"/> or <see cref="CData"/> content.
    /// </summary>
    public IParser<char, XObject> AnyContent
    {
      get
      {
        return parser.AnyContent;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches an XML element with any attributes or content.
    /// </summary>
    public IParser<char, XElement> AnyElement
    {
      get
      {
        return parser.AnyElement;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches an XML attribute.
    /// </summary>
    public IParser<char, XAttribute> AnyAttribute
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
    public IParser<char, XAttribute> Attribute(string name)
    {
      return parser.Attribute(name);
    }

#if !SILVERLIGHT || WINDOWS_PHONE
    /// <summary>
    /// Creates a parser that matches a single XML element containing the specified attributes and children.
    /// </summary>
    /// <param name="content">The parsers that match the element's attributes, child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element containing the specified attributes and children.</returns>
    public IParser<char, XElement> Element(params IParser<char, XObject>[] content)
    {
      return parser.Element(content);
    }
#else
		/// <summary>
		/// Creates a parser that matches a single XML element containing the specified children.
		/// </summary>
		/// <param name="content">The parsers that match the element's child elements, text, cdata and comments.</param>
		/// <returns>A parser that matches a single XML element containing the specified children.</returns>
		public IParser<char, XElement> Element(params IParser<char, XObject>[] content)
		{
			return parser.Element(content);
		}

		/// <summary>
		/// Creates a parser that matches a single XML element containing the specified attributes and children.
		/// </summary>
		/// <remarks>
		/// IParser is not covariant on TResult in Silverlight because <see cref="IEnumerable{T}"/> is not covariant on T.
		/// For this reason, the <paramref name="content"/> parameter cannot accept attributes.
		/// </remarks>
		/// <param name="attributes">The parsers that match the element's attributes.</param>
		/// <param name="content">The parsers that match the element's child elements, text, cdata and comments.</param>
		/// <returns>A parser that matches a single XML element containing the specified attributes and children.</returns>
		public IParser<char, XElement> Element(IEnumerable<IParser<char, XAttribute>> attributes, params IParser<char, XObject>[] content)
		{
			return parser.Element(attributes, content);
		}
#endif

    /// <summary>
    /// Creates a parser that matches a single XML element having no attributes and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element having no attributes and containing the specified <paramref name="content"/>.</returns>
    public IParser<char, XElement> Element(IParser<char, IEnumerable<XObject>> content)
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
    public IParser<char, XElement> Element(IParser<char, IEnumerable<XAttribute>> attributes, IParser<char, IEnumerable<XObject>> content)
    {
      return parser.Element(attributes, content);
    }

#if !SILVERLIGHT || WINDOWS_PHONE
    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/> and containing 
    /// the specified attributes and children.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="content">The parsers that match the element's attributes, child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/> and containing 
    /// the specified attributes and children.</returns>
    public IParser<char, XElement> Element(string name, params IParser<char, XObject>[] content)
    {
      return parser.Element(name, content);
    }
#else
		/// <summary>
		/// Creates a parser that matches a single XML element with the specified <paramref name="name"/> and containing 
		/// the specified children.
		/// </summary>
		/// <param name="name">The name of the element to match.</param>
		/// <param name="content">The parsers that match the element's child elements, text, cdata and comments.</param>
		/// <returns>A parser that matches a single XML element with the specified <paramref name="name"/> and containing 
		/// the specified children.</returns>
		public IParser<char, XElement> Element(string name, params IParser<char, XObject>[] content)
		{
			return parser.Element(name, content);
		}

		/// <summary>
		/// Creates a parser that matches a single XML element with the specified <paramref name="name"/> and containing 
		/// the specified attributes and children.
		/// </summary>
		/// <remarks>
		/// IParser is not covariant on TResult in Silverlight because <see cref="IEnumerable{T}"/> is not covariant on T.
		/// For this reason, the <paramref name="content"/> parameter cannot accept attributes.
		/// </remarks>
		/// <param name="name">The name of the element to match.</param>
		/// <param name="attributes">The parsers that match the element's attributes.</param>
		/// <param name="content">The parsers that match the element's child elements, text, cdata and comments.</param>
		/// <returns>A parser that matches a single XML element with the specified <paramref name="name"/> and containing 
		/// the specified attributes and children.</returns>
		public IParser<char, XElement> Element(string name, IEnumerable<IParser<char, XAttribute>> attributes, params IParser<char, XObject>[] content)
		{
			return parser.Element(name, attributes, content);
		}
#endif

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/>, having no attributes
    /// and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/>, having no attributes 
    /// and containing the specified <paramref name="content"/>.</returns>
    public IParser<char, XElement> Element(string name, IParser<char, IEnumerable<XObject>> content)
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
    public IParser<char, XElement> Element(string name, IParser<char, IEnumerable<XAttribute>> attributes, IParser<char, IEnumerable<XObject>> content)
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
    public IParser<char, XElement> Element(string name, IEnumerable<string> attributes, params IParser<char, XObject>[] content)
    {
      return parser.Element(name, attributes, content);
    }
    #endregion
  }
}
