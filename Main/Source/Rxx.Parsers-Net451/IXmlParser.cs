using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents an XML parser over a <see cref="string"/> or an enumerable sequence of <see cref="char"/>.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of XML nodes.</typeparam>
  [ContractClass(typeof(IXmlParserContract<>))]
  public interface IXmlParser<TResult> : IParser<char, TResult>
  {
    /// <summary>
    /// Gets a parser with a grammar that matches all characters, including whitespace, up to the start tag of an element.
    /// </summary>
    IParser<char, XText> Text
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches content sequences beginning with &lt;!-- and ending with --&gt;.
    /// </summary>
    IParser<char, XComment> Comment
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches content sequences beginning with &lt;![CDATA[ and ending with ]]&gt;.
    /// </summary>
    IParser<char, XCData> CData
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches any <see cref="Text"/>, <see cref="Comment"/> or <see cref="CData"/> content.
    /// </summary>
    IParser<char, XObject> AnyContent
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches an XML element with any attributes or content.
    /// </summary>
    IParser<char, XElement> AnyElement
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches an XML attribute.
    /// </summary>
    IParser<char, XAttribute> AnyAttribute
    {
      get;
    }

    /// <summary>
    /// Creates a parser that matches a single XML attribute with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the attribute to match.</param>
    /// <returns>A parser that matches a single XML attribute with the specified <paramref name="name"/>.</returns>
    IParser<char, XAttribute> Attribute(string name);

#if !SILVERLIGHT || WINDOWS_PHONE
    /// <summary>
    /// Creates a parser that matches a single XML element containing the specified attributes and children.
    /// </summary>
    /// <param name="content">The parsers that match the element's attributes, child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element containing the specified attributes and children.</returns>
    IParser<char, XElement> Element(params IParser<char, XObject>[] content);
#else
		/// <summary>
		/// Creates a parser that matches a single XML element containing the specified children.
		/// </summary>
		/// <param name="content">The parsers that match the element's child elements, text, cdata and comments.</param>
		/// <returns>A parser that matches a single XML element containing the specified children.</returns>
		IParser<char, XElement> Element(params IParser<char, XObject>[] content);

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
		IParser<char, XElement> Element(IEnumerable<IParser<char, XAttribute>> attributes, params IParser<char, XObject>[] content);
#endif

    /// <summary>
    /// Creates a parser that matches a single XML element having no attributes and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element having no attributes and containing the specified <paramref name="content"/>.</returns>
    IParser<char, XElement> Element(IParser<char, IEnumerable<XObject>> content);

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="attributes"/> and containing 
    /// the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="attributes">The parser that matches the element's attributes.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="attributes"/> and containing 
    /// the specified <paramref name="content"/>.</returns>
    IParser<char, XElement> Element(
      IParser<char, IEnumerable<XAttribute>> attributes,
      IParser<char, IEnumerable<XObject>> content);

#if !SILVERLIGHT || WINDOWS_PHONE
    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/> and containing 
    /// the specified attributes and children.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="content">The parsers that match the element's attributes, child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/> and containing 
    /// the specified attributes and children.</returns>
    IParser<char, XElement> Element(string name, params IParser<char, XObject>[] content);
#else
		/// <summary>
		/// Creates a parser that matches a single XML element with the specified <paramref name="name"/> and containing 
		/// the specified children.
		/// </summary>
		/// <param name="name">The name of the element to match.</param>
		/// <param name="content">The parsers that match the element's child elements, text, cdata and comments.</param>
		/// <returns>A parser that matches a single XML element with the specified <paramref name="name"/> and containing 
		/// the specified children.</returns>
		IParser<char, XElement> Element(string name, params IParser<char, XObject>[] content);

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
		IParser<char, XElement> Element(string name, IEnumerable<IParser<char, XAttribute>> attributes, params IParser<char, XObject>[] content);
#endif

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/>, having no attributes
    /// and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/>, having no attributes 
    /// and containing the specified <paramref name="content"/>.</returns>
    IParser<char, XElement> Element(string name, IParser<char, IEnumerable<XObject>> content);

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/>, the specified 
    /// <paramref name="attributes"/> and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="attributes">The parser that matches the element's attributes.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/>, the specified 
    /// <paramref name="attributes"/> and containing the specified <paramref name="content"/>.</returns>
    IParser<char, XElement> Element(
      string name,
      IParser<char, IEnumerable<XAttribute>> attributes,
      IParser<char, IEnumerable<XObject>> content);

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/>, the specified 
    /// <paramref name="attributes"/> and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="attributes">An enumerable sequence containing the names of the element's attributes.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/>, the specified 
    /// <paramref name="attributes"/> and containing the specified <paramref name="content"/>.</returns>
    IParser<char, XElement> Element(
      string name,
      IEnumerable<string> attributes,
      params IParser<char, XObject>[] content);
  }

  [ContractClassFor(typeof(IXmlParser<>))]
  internal abstract class IXmlParserContract<TResult> : IXmlParser<TResult>
  {
    public IParser<char, XText> Text
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, XText>>() != null);
        return null;
      }
    }

    public IParser<char, XComment> Comment
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, XComment>>() != null);
        return null;
      }
    }

    public IParser<char, XCData> CData
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, XCData>>() != null);
        return null;
      }
    }

    public IParser<char, XObject> AnyContent
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, XObject>>() != null);
        return null;
      }
    }

    public IParser<char, XElement> AnyElement
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);
        return null;
      }
    }

    public IParser<char, XAttribute> AnyAttribute
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, XAttribute>>() != null);
        return null;
      }
    }

    public IParser<char, XAttribute> Attribute(string name)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Ensures(Contract.Result<IParser<char, XAttribute>>() != null);
      return null;
    }

    public IParser<char, XElement> Element(params IParser<char, XObject>[] content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);
      return null;
    }

#if SILVERLIGHT && !WINDOWS_PHONE
		public IParser<char, XElement> Element(IEnumerable<IParser<char, XAttribute>> attributes, params IParser<char, XObject>[] content)
		{
			Contract.Requires(content != null);
			Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);
			return null;
		}
#endif

    public IParser<char, XElement> Element(IParser<char, IEnumerable<XObject>> content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);
      return null;
    }

    public IParser<char, XElement> Element(IParser<char, IEnumerable<XAttribute>> attributes, IParser<char, IEnumerable<XObject>> content)
    {
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);
      return null;
    }

    public IParser<char, XElement> Element(string name, params IParser<char, XObject>[] content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);
      return null;
    }

#if SILVERLIGHT && !WINDOWS_PHONE
		public IParser<char, XElement> Element(string name, IEnumerable<IParser<char, XAttribute>> attributes, params IParser<char, XObject>[] content)
		{
			Contract.Requires(!string.IsNullOrWhiteSpace(name));
			Contract.Requires(content != null);
			Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);
			return null;
		}
#endif

    public IParser<char, XElement> Element(string name, IParser<char, IEnumerable<XObject>> content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);
      return null;
    }

    public IParser<char, XElement> Element(string name, IParser<char, IEnumerable<XAttribute>> attributes, IParser<char, IEnumerable<XObject>> content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);
      return null;
    }

    public IParser<char, XElement> Element(string name, IEnumerable<string> attributes, params IParser<char, XObject>[] content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);
      return null;
    }

    #region IParser<char,TResult> Members
    public IParser<char, char> Next
    {
      get
      {
        return null;
      }
    }

    public IEnumerable<IParseResult<TResult>> Parse(ICursor<char> source)
    {
      return null;
    }
    #endregion
  }
}
