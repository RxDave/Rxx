using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Xml.Linq;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Represents an XML parser over an observable sequence of <see cref="char"/>.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of XML nodes.</typeparam>
  [ContractClass(typeof(IXmlObservableParserContract<>))]
  public interface IXmlObservableParser<TResult> : IObservableParser<char, TResult>
  {
    /// <summary>
    /// Gets a parser with a grammar that matches all characters, including whitespace, up to the start tag of an element.
    /// </summary>
    IObservableParser<char, XText> Text
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches content sequences beginning with &lt;!-- and ending with --&gt;.
    /// </summary>
    IObservableParser<char, XComment> Comment
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches content sequences beginning with &lt;![CDATA[ and ending with ]]&gt;.
    /// </summary>
    IObservableParser<char, XCData> CData
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches any <see cref="Text"/>, <see cref="Comment"/> or <see cref="CData"/> content.
    /// </summary>
    IObservableParser<char, XObject> AnyContent
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches an XML element with any attributes or content.
    /// </summary>
    IObservableParser<char, XElement> AnyElement
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches an XML attribute.
    /// </summary>
    IObservableParser<char, XAttribute> AnyAttribute
    {
      get;
    }

    /// <summary>
    /// Creates a parser that matches a single XML attribute with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the attribute to match.</param>
    /// <returns>A parser that matches a single XML attribute with the specified <paramref name="name"/>.</returns>
    IObservableParser<char, XAttribute> Attribute(string name);

    /// <summary>
    /// Creates a parser that matches a single XML element containing the specified attributes and children.
    /// </summary>
    /// <param name="content">The parsers that match the element's attributes, child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element containing the specified attributes and children.</returns>
    IObservableParser<char, XElement> Element(params IObservableParser<char, XObject>[] content);

    /// <summary>
    /// Creates a parser that matches a single XML element having no attributes and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element having no attributes and containing the specified <paramref name="content"/>.</returns>
    IObservableParser<char, XElement> Element(IObservableParser<char, IObservable<XObject>> content);

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="attributes"/> and containing 
    /// the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="attributes">The parser that matches the element's attributes.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="attributes"/> and containing 
    /// the specified <paramref name="content"/>.</returns>
    IObservableParser<char, XElement> Element(
      IObservableParser<char, IObservable<XAttribute>> attributes,
      IObservableParser<char, IObservable<XObject>> content);

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/> and containing 
    /// the specified attributes and children.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="content">The parsers that match the element's attributes, child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/> and containing 
    /// the specified attributes and children.</returns>
    IObservableParser<char, XElement> Element(string name, params IObservableParser<char, XObject>[] content);

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/>, having no attributes
    /// and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/>, having no attributes 
    /// and containing the specified <paramref name="content"/>.</returns>
    IObservableParser<char, XElement> Element(string name, IObservableParser<char, IObservable<XObject>> content);

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/>, the specified 
    /// <paramref name="attributes"/> and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="attributes">The parser that matches the element's attributes.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/>, the specified 
    /// <paramref name="attributes"/> and containing the specified <paramref name="content"/>.</returns>
    IObservableParser<char, XElement> Element(
      string name,
      IObservableParser<char, IObservable<XAttribute>> attributes,
      IObservableParser<char, IObservable<XObject>> content);

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/>, the specified 
    /// <paramref name="attributes"/> and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="attributes">An enumerable sequence containing the names of the element's attributes.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/>, the specified 
    /// <paramref name="attributes"/> and containing the specified <paramref name="content"/>.</returns>
    IObservableParser<char, XElement> Element(
      string name,
      IEnumerable<string> attributes,
      params IObservableParser<char, XObject>[] content);
  }

  [ContractClassFor(typeof(IXmlObservableParser<>))]
  internal abstract class IXmlObservableParserContract<TResult> : IXmlObservableParser<TResult>
  {
    public IObservableParser<char, XText> Text
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, XText>>() != null);
        return null;
      }
    }

    public IObservableParser<char, XComment> Comment
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, XComment>>() != null);
        return null;
      }
    }

    public IObservableParser<char, XCData> CData
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, XCData>>() != null);
        return null;
      }
    }

    public IObservableParser<char, XObject> AnyContent
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, XObject>>() != null);
        return null;
      }
    }

    public IObservableParser<char, XElement> AnyElement
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);
        return null;
      }
    }

    public IObservableParser<char, XAttribute> AnyAttribute
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, XAttribute>>() != null);
        return null;
      }
    }

    public IObservableParser<char, XAttribute> Attribute(string name)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Ensures(Contract.Result<IObservableParser<char, XAttribute>>() != null);
      return null;
    }

    public IObservableParser<char, XElement> Element(params IObservableParser<char, XObject>[] content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);
      return null;
    }

    public IObservableParser<char, XElement> Element(IObservableParser<char, IObservable<XObject>> content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);
      return null;
    }

    public IObservableParser<char, XElement> Element(IObservableParser<char, IObservable<XAttribute>> attributes, IObservableParser<char, IObservable<XObject>> content)
    {
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);
      return null;
    }

    public IObservableParser<char, XElement> Element(string name, params IObservableParser<char, XObject>[] content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);
      return null;
    }

    public IObservableParser<char, XElement> Element(string name, IObservableParser<char, IObservable<XObject>> content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);
      return null;
    }

    public IObservableParser<char, XElement> Element(string name, IObservableParser<char, IObservable<XAttribute>> attributes, IObservableParser<char, IObservable<XObject>> content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);
      return null;
    }

    public IObservableParser<char, XElement> Element(string name, IEnumerable<string> attributes, params IObservableParser<char, XObject>[] content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);
      return null;
    }

    #region IObservableParser<char,TResult> Members
    public IObservableParser<char, char> Next
    {
      get
      {
        return null;
      }
    }

    public IObservable<IParseResult<TResult>> Parse(IObservableCursor<char> source)
    {
      return null;
    }
    #endregion
  }
}