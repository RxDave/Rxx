using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using Rxx.Parsers.Linq;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents an XML parser over a <see cref="string"/> or an enumerable sequence of <see cref="char"/>.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of XML nodes.</typeparam>
  [ContractClass(typeof(XmlParserContract<>))]
  public abstract class XmlParser<TResult> : StringParser<TResult>, IXmlParser<TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    /// <summary>
    /// Gets the <see cref="Schema"/> that is the parser's grammar.
    /// </summary>
    protected sealed override IParser<char, TResult> Start
    {
      get
      {
        return Schema;
      }
    }

    /// <summary>
    /// Gets the parser's grammar as an XML schema that is defined in terms of the parsers created by
    /// <see cref="AnyElement"/>, <see cref="AnyAttribute"/>, <see cref="AnyContent"/>, <see cref="Text"/>, 
    /// <see cref="Comment"/>, <see cref="CData"/> or any of the methods that create parsers with 
    /// context-sensitive grammars, such as <see cref="Element(IParser{char,XObject}[])"/> and 
    /// <see cref="Attribute(string)"/>
    /// </summary>
    protected abstract IParser<char, TResult> Schema
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches all characters, including whitespace, up to the start tag of an element.
    /// </summary>
    protected IParser<char, XText> Text
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, XText>>() != null);

        return text;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches content sequences beginning with &lt;!-- and ending with --&gt;.
    /// </summary>
    protected IParser<char, XComment> Comment
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, XComment>>() != null);

        return comment;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches content sequences beginning with &lt;![CDATA[ and ending with ]]&gt;.
    /// </summary>
    protected IParser<char, XCData> CData
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, XCData>>() != null);

        return cData;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches any <see cref="Text"/>, <see cref="Comment"/> or <see cref="CData"/> content.
    /// </summary>
    protected IParser<char, XObject> AnyContent
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, XObject>>() != null);

        return Content(text => text);
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches an XML element with any attributes or content.
    /// </summary>
    protected IParser<char, XElement> AnyElement
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

        var parser = element(_ => true, AnyAttribute.NoneOrMore(), AnyContent.NoneOrMore());

        Contract.Assume(parser != null);

        return parser;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches an XML attribute.
    /// </summary>
    protected IParser<char, XAttribute> AnyAttribute
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, XAttribute>>() != null);

        var parser = attribute(_ => true);

        Contract.Assume(parser != null);

        return parser;
      }
    }

    private readonly StringComparer comparer;
    private readonly IParser<char, string> id;
    private readonly IParser<char, char> tagStart;
    private readonly IParser<char, char> tagEnd;
    private readonly IParser<char, string> tagName;
    private readonly IParser<char, char> attributeDelimiter;
    private readonly IParser<char, string> attributeName;
    private readonly IParser<char, string> attributeValue;

    [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1014:OpeningGenericBracketsMustBeSpacedCorrectly", Justification = "Readability of nested generic types.")]
    private readonly Func<
      Func<string, bool>,
      IParser<char, XAttribute>> attribute;

    [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1014:OpeningGenericBracketsMustBeSpacedCorrectly", Justification = "Readability of nested generic types.")]
    private readonly Func<
      Func<string, bool>,
      IParser<char, IEnumerable<XAttribute>>,
      IParser<char, Tuple<string, IEnumerable<XAttribute>, bool>>> openTag;

    private readonly IParser<char, string> closeTag;
    private readonly IParser<char, XText> text;
    private readonly IParser<char, XComment> comment;
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Not Hungarian notation.")]
    private readonly IParser<char, XCData> cData;

    [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1014:OpeningGenericBracketsMustBeSpacedCorrectly", Justification = "Readability of nested generic types.")]
    private readonly Func<
      Func<string, bool>,
      IParser<char, IEnumerable<XAttribute>>,
      IParser<char, IEnumerable<XObject>>,
      IParser<char, XElement>> element;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="XmlParser{TResult}" /> class with the specified case-sensitivity
    /// for derived classes.
    /// </summary>
    /// <param name="caseSensitive">Indicates whether the comparison behavior used for matching element and attribute names
    /// must ignore case.</param>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The amount of coupling cannot be reduced without increasing complexity.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Grammar defined in functional programming style is less complicated than alternatives.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grammar defined in functional programming style is less complicated than alternatives.")]
    protected XmlParser(bool caseSensitive)
    {
      comparer = caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

      id = Character(char.IsLetter)
           .And(
           Character(char.IsLetterOrDigit).NoneOrMore())
           .Join();

      const char tagStartChar = '<';
      const char tagEndChar = '>';

      tagStart = Character(tagStartChar);
      tagEnd = Character(tagEndChar);

      tagName = id;

      openTag = (namePredicate, attributes) =>
        from _ in tagStart
        from name in tagName.Where(namePredicate)
        from attrs in attributes
        from __ in InsignificantWhiteSpace
        from empty in Character('/').Maybe()
        from ___ in tagEnd
        select Tuple.Create(name, attrs, empty.Any());

      attributeDelimiter = Character('\'').Or(Character('"'));

      attributeName = id;

      attributeValue = from delimeter in attributeDelimiter
                       from value in AnyCharacterUntil(delimeter)
                       from _ in Character(delimeter)
                       select value;

      attribute = predicate => from _ in InsignificantWhiteSpace
                               from name in attributeName.Where(predicate)
                               from __ in InsignificantWhiteSpace
                               from ___ in Character('=')
                               from ____ in InsignificantWhiteSpace
                               from value in attributeValue
                               select new XAttribute(name, value);

      closeTag = from _ in tagStart.And(Character('/'))
                 from name in tagName
                 from __ in InsignificantWhiteSpace
                 from ___ in tagEnd
                 select name;

      text = AnyCharacterUntil(tagStartChar).Select(t => new XText(t));

      comment = from _ in Word(tagStartChar + "!--")
                from value in AnyCharacterUntil("--")
                from __ in Word("--" + tagEndChar)
                select new XComment(value);

      string startCData = tagStartChar + "![CDATA[";
      string endCData = "]]" + tagEndChar;

      cData = from _ in Word(startCData)
              from value in AnyCharacterUntil(endCData)
              from __ in Word(endCData)
              select new XCData(value);

      element = (namePredicate, attributes, elementContent) =>
        from _ in InsignificantWhiteSpace
        let open = openTag(namePredicate, attributes)
        from tag in open
        from children in tag.Item3
          ? open.Success(Enumerable.Empty<XObject>())  // tag.Item3 indicates an empty XML element
          : elementContent.IgnoreTrailing(
            InsignificantWhiteSpace.IgnoreBefore(
            closeTag))
#if !SILVERLIGHT || WINDOWS_PHONE
        select new XElement(tag.Item1, tag.Item2.Concat(children).ToArray());
#else
        select new XElement(tag.Item1, tag.Item2.Cast<XObject>().Concat(children).ToArray());
#endif
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="XmlParser{TResult}" /> class with case-sensitive element and 
    /// attribute name comparisons for derived classes.
    /// </summary>
    protected XmlParser()
      : this(caseSensitive: true)
    {
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(comparer != null);
      Contract.Invariant(id != null);
      Contract.Invariant(tagStart != null);
      Contract.Invariant(tagEnd != null);
      Contract.Invariant(tagName != null);
      Contract.Invariant(attributeDelimiter != null);
      Contract.Invariant(attributeName != null);
      Contract.Invariant(attributeValue != null);
      Contract.Invariant(attribute != null);
      Contract.Invariant(openTag != null);
      Contract.Invariant(closeTag != null);
      Contract.Invariant(text != null);
      Contract.Invariant(comment != null);
      Contract.Invariant(cData != null);
      Contract.Invariant(element != null);
    }

    /// <summary>
    /// Creates a parser that matches a single XML attribute with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the attribute to match.</param>
    /// <returns>A parser that matches a single XML attribute with the specified <paramref name="name"/>.</returns>
    protected IParser<char, XAttribute> Attribute(string name)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Ensures(Contract.Result<IParser<char, XAttribute>>() != null);

      var parser = attribute(n => comparer.Equals(n, name));

      Contract.Assume(parser != null);

      return parser;
    }

#if !SILVERLIGHT || WINDOWS_PHONE
    /// <summary>
    /// Creates a parser that matches a single XML element containing the specified attributes and children.
    /// </summary>
    /// <param name="content">The parsers that match the element's attributes, child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element containing the specified attributes and children.</returns>
    protected IParser<char, XElement> Element(params IParser<char, XObject>[] content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      return Element(_ => true, content);
    }
#else
    /// <summary>
    /// Creates a parser that matches a single XML element containing the specified children.
    /// </summary>
    /// <param name="content">The parsers that match the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element containing the specified attributes and children.</returns>
    protected IParser<char, XElement> Element(params IParser<char, XObject>[] content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      return Element(_ => true, Enumerable.Empty<IParser<char, XAttribute>>(), content);
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
    protected IParser<char, XElement> Element(IEnumerable<IParser<char, XAttribute>> attributes, params IParser<char, XObject>[] content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      return Element(_ => true, attributes ?? Enumerable.Empty<IParser<char, XAttribute>>(), content);
    }
#endif

    /// <summary>
    /// Creates a parser that matches a single XML element having no attributes and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element having no attributes and containing the specified <paramref name="content"/>.</returns>
    protected IParser<char, XElement> Element(IParser<char, IEnumerable<XObject>> content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      var anyAttribute = attribute(_ => true);

      Contract.Assume(anyAttribute != null);

      return Element(_ => true, anyAttribute.None(), content);
    }

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="attributes"/> and containing 
    /// the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="attributes">The parser that matches the element's attributes.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="attributes"/> and containing 
    /// the specified <paramref name="content"/>.</returns>
    protected IParser<char, XElement> Element(
      IParser<char, IEnumerable<XAttribute>> attributes,
      IParser<char, IEnumerable<XObject>> content)
    {
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      return Element(_ => true, attributes, content);
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
    protected IParser<char, XElement> Element(string name, params IParser<char, XObject>[] content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      return Element(n => comparer.Equals(n, name), content);
    }
#else
    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/> and containing 
    /// the specified children.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="content">The parsers that match the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/> and containing 
    /// the specified attributes and children.</returns>
    protected IParser<char, XElement> Element(string name, params IParser<char, XObject>[] content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      return Element(n => comparer.Equals(n, name), Enumerable.Empty<IParser<char, XAttribute>>(), content);
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
    protected IParser<char, XElement> Element(string name, IEnumerable<IParser<char, XAttribute>> attributes, params IParser<char, XObject>[] content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      return Element(n => comparer.Equals(n, name), attributes ?? Enumerable.Empty<IParser<char, XAttribute>>(), content);
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
    protected IParser<char, XElement> Element(string name, IParser<char, IEnumerable<XObject>> content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      var anyAttribute = attribute(_ => true);

      Contract.Assume(anyAttribute != null);

      return Element(
        n => comparer.Equals(n, name),
        anyAttribute.None(),
        content);
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
    protected IParser<char, XElement> Element(
      string name,
      IParser<char, IEnumerable<XAttribute>> attributes,
      IParser<char, IEnumerable<XObject>> content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      return Element(n => comparer.Equals(n, name), attributes, content);
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
    protected IParser<char, XElement> Element(
      string name,
      IEnumerable<string> attributes,
      params IParser<char, XObject>[] content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      var contentList = content.ToList();

#if !SILVERLIGHT || WINDOWS_PHONE
      contentList.AddRange(attributes.Select(a => Attribute(a)));

      return Element(n => comparer.Equals(n, name), contentList);
#else
      return Element(n => comparer.Equals(n, name), attributes.Select(a => Attribute(a)), contentList);
#endif
    }

#if !SILVERLIGHT || WINDOWS_PHONE
    private IParser<char, XElement> Element(
      Func<string, bool> name,
      IEnumerable<IParser<char, XObject>> content)
    {
      Contract.Requires(name != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      IParser<char, IEnumerable<XAttribute>> attributes;
      IParser<char, IEnumerable<XObject>> children = Content(out attributes, content);

      var parser = element(name, attributes, children);

      Contract.Assume(parser != null);

      return parser;
    }
#else
    // IParser is not covariant on TResult in Silverlight because IEnumerable<T> is not covariant on T.
    private IParser<char, XElement> Element(
      Func<string, bool> name,
      IEnumerable<IParser<char, XAttribute>> attributes,
      IEnumerable<IParser<char, XObject>> content)
    {
      Contract.Requires(name != null);
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

      IParser<char, IEnumerable<XObject>> children = Content(content);

      var attributeList = attributes.ToList();

      var anyAttribute = attribute(_ => true);

      Contract.Assume(anyAttribute != null);

      var attributeParser = (attributeList.Count == 0)
        ? anyAttribute.None()
        : attributeList.AllUnordered();

      var parser = element(name, attributeParser, children);

      Contract.Assume(parser != null);

      return parser;
    }
#endif

    private IParser<char, XElement> Element(
      Func<string, bool> name,
      IParser<char, IEnumerable<XAttribute>> attributes,
      IParser<char, IEnumerable<XObject>> content)
    {
      Contract.Requires(name != null);
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, XElement>>() != null);

#if !SILVERLIGHT || WINDOWS_PHONE
      var parser = element(name, attributes, Comment.NoneOrMore().And(content));
#else
      var parser = element(name, attributes, Comment.Cast<char, XComment, XObject>().NoneOrMore().And(content));
#endif

      Contract.Assume(parser != null);

      return parser;
    }

#if !SILVERLIGHT || WINDOWS_PHONE
    private IParser<char, IEnumerable<XObject>> Content(
      out IParser<char, IEnumerable<XAttribute>> attributes,
      IEnumerable<IParser<char, XObject>> content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.ValueAtReturn(out attributes) != null);
      Contract.Ensures(Contract.Result<IParser<char, IEnumerable<XObject>>>() != null);

      var contentList = content.ToList();

      var attributeList = contentList.OfType<IParser<char, XAttribute>>().ToList();

      var anyAttribute = attribute(_ => true);

      Contract.Assume(anyAttribute != null);

      attributes = (attributeList.Count == 0)
        ? anyAttribute.None()
        : attributeList.AllUnordered();

      foreach (var a in attributeList)
      {
        contentList.Remove(a);
      }

      var contentWithComments = new List<IParser<char, IEnumerable<XObject>>>(contentList.Count);

      foreach (var item in contentList)
      {
        Contract.Assume(item != null);

        contentWithComments.Add(Comment.Maybe().And(item));
      }

      IParser<char, IEnumerable<XObject>> result;

      if (contentWithComments.Count == 0)
      {
        result =
          from noContent in Content(
            text => text.Where(node => node.Value.Trim().Length > 0),
            includeComments: false)   // excluding comments actually allows them because of .None()
            .None()
          from _ in InsignificantWhiteSpace
          from comments in comment.NoneOrMore()
          from __ in InsignificantWhiteSpace
          select comments;
      }
      else
      {
        result = contentWithComments.All();
      }

      return result;
    }
#else
    // IParser is not covariant on TResult in Silverlight because IEnumerable<T> is not covariant on T.
    private IParser<char, IEnumerable<XObject>> Content(
      IEnumerable<IParser<char, XObject>> content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IParser<char, IEnumerable<XObject>>>() != null);

      var contentList = content.ToList();

      var contentWithComments = new List<IParser<char, IEnumerable<XObject>>>(contentList.Count);

      foreach (var item in contentList)
      {
        Contract.Assume(item != null);

        contentWithComments.Add(Comment.Cast<char, XComment, XObject>().Maybe().And(item));
      }

      IParser<char, IEnumerable<XObject>> result;

      if (contentWithComments.Count == 0)
      {
        result =
          from noContent in Content(
            text => text.Where(node => node.Value.Trim().Length > 0),
            includeComments: false)   // excluding comments actually allows them because of .None()
            .None()
          from _ in InsignificantWhiteSpace
          from comments in comment.NoneOrMore()
          from __ in InsignificantWhiteSpace
          select comments.Cast<XObject>();
      }
      else
      {
        result = contentWithComments.All();
      }

      return result;
    }
#endif

    private IParser<char, XObject> Content(
      Func<IParser<char, XText>, IParser<char, XText>> textSelector,
      bool includeComments = true)
    {
      Contract.Requires(textSelector != null);
      Contract.Ensures(Contract.Result<IParser<char, XObject>>() != null);

      return Parser.Defer(() =>
      {
#if !SILVERLIGHT || WINDOWS_PHONE
        var content = new List<IParser<char, XObject>>(4)
          {
            element(_ => true, AnyAttribute.NoneOrMore(), Content(textSelector, includeComments).NoneOrMore()), 
            textSelector(text), 
            cData
          };

        if (includeComments)
        {
          content.Add(comment);
        }
#else
        var content = new List<IParser<char, XObject>>(4)
          {
            element(_ => true, AnyAttribute.NoneOrMore(), Content(textSelector, includeComments).NoneOrMore()).Cast<char, XElement, XObject>(), 
            textSelector(text).Cast<char, XText, XObject>(), 
            cData.Cast<char, XCData, XObject>()
          };

        if (includeComments)
          content.Add(comment.Cast<char, XComment, XObject>());
#endif

        return content.Any();
      });
    }
    #endregion

    #region IXmlParser<TResult> Members
    IParser<char, XText> IXmlParser<TResult>.Text
    {
      get
      {
        return Text;
      }
    }

    IParser<char, XComment> IXmlParser<TResult>.Comment
    {
      get
      {
        return Comment;
      }
    }

    IParser<char, XCData> IXmlParser<TResult>.CData
    {
      get
      {
        return CData;
      }
    }

    IParser<char, XObject> IXmlParser<TResult>.AnyContent
    {
      get
      {
        return AnyContent;
      }
    }

    IParser<char, XElement> IXmlParser<TResult>.AnyElement
    {
      get
      {
        return AnyElement;
      }
    }

    IParser<char, XAttribute> IXmlParser<TResult>.AnyAttribute
    {
      get
      {
        return AnyAttribute;
      }
    }

    IParser<char, XAttribute> IXmlParser<TResult>.Attribute(string name)
    {
      return Attribute(name);
    }

    IParser<char, XElement> IXmlParser<TResult>.Element(params IParser<char, XObject>[] content)
    {
      return Element(content);
    }

#if SILVERLIGHT && !WINDOWS_PHONE
    // IParser is not covariant on TResult in Silverlight because IEnumerable<T> is not covariant on T.
    IParser<char, XElement> IXmlParser<TResult>.Element(IEnumerable<IParser<char, XAttribute>> attributes, params IParser<char, XObject>[] content)
    {
      return Element(attributes, content);
    }
#endif

    IParser<char, XElement> IXmlParser<TResult>.Element(IParser<char, IEnumerable<XObject>> content)
    {
      return Element(content);
    }

    IParser<char, XElement> IXmlParser<TResult>.Element(IParser<char, IEnumerable<XAttribute>> attributes, IParser<char, IEnumerable<XObject>> content)
    {
      return Element(attributes, content);
    }

    IParser<char, XElement> IXmlParser<TResult>.Element(string name, params IParser<char, XObject>[] content)
    {
      return Element(name, content);
    }

#if SILVERLIGHT && !WINDOWS_PHONE
    // IParser is not covariant on TResult in Silverlight because IEnumerable<T> is not covariant on T.
    IParser<char, XElement> IXmlParser<TResult>.Element(string name, IEnumerable<IParser<char, XAttribute>> attributes, params IParser<char, XObject>[] content)
    {
      return Element(name, attributes, content);
    }
#endif

    IParser<char, XElement> IXmlParser<TResult>.Element(string name, IParser<char, IEnumerable<XObject>> content)
    {
      return Element(name, content);
    }

    IParser<char, XElement> IXmlParser<TResult>.Element(string name, IParser<char, IEnumerable<XAttribute>> attributes, IParser<char, IEnumerable<XObject>> content)
    {
      return Element(name, attributes, content);
    }

    IParser<char, XElement> IXmlParser<TResult>.Element(string name, IEnumerable<string> attributes, params IParser<char, XObject>[] content)
    {
      return Element(name, attributes, content);
    }
    #endregion
  }

  [ContractClassFor(typeof(XmlParser<>))]
  internal abstract class XmlParserContract<TResult> : XmlParser<TResult>
  {
    protected override IParser<char, TResult> Schema
    {
      get
      {
        Contract.Ensures(Contract.Result<IParser<char, TResult>>() != null);
        return null;
      }
    }
  }
}
