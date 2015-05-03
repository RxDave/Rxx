using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Xml.Linq;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.Parsers.Reactive
{
  /// <summary>
  /// Represents an XML parser over an observable sequence of <see cref="char"/>.
  /// </summary>
  /// <typeparam name="TResult">The type of the elements that are generated from parsing the sequence of XML nodes.</typeparam>
  [ContractClass(typeof(XmlObservableParserContract<>))]
  public abstract class XmlObservableParser<TResult> : StringObservableParser<TResult>, IXmlObservableParser<TResult>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    /// <summary>
    /// Gets the <see cref="Schema"/> that is the parser's grammar.
    /// </summary>
    protected sealed override IObservableParser<char, TResult> Start
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
    /// context-sensitive grammars, such as <see cref="Element(IObservableParser{char,XObject}[])"/> and 
    /// <see cref="Attribute(string)"/>
    /// </summary>
    protected abstract IObservableParser<char, TResult> Schema
    {
      get;
    }

    /// <summary>
    /// Gets a parser with a grammar that matches all characters, including whitespace, up to the start tag of an element.
    /// </summary>
    protected IObservableParser<char, XText> Text
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, XText>>() != null);

        return text;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches content sequences beginning with &lt;!-- and ending with --&gt;.
    /// </summary>
    protected IObservableParser<char, XComment> Comment
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, XComment>>() != null);

        return comment;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches content sequences beginning with &lt;![CDATA[ and ending with ]]&gt;.
    /// </summary>
    protected IObservableParser<char, XCData> CData
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, XCData>>() != null);

        return cData;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches any <see cref="Text"/>, <see cref="Comment"/> or <see cref="CData"/> content.
    /// </summary>
    protected IObservableParser<char, XObject> AnyContent
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, XObject>>() != null);

        return Content(text => text);
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches an XML element with any attributes or content.
    /// </summary>
    protected IObservableParser<char, XElement> AnyElement
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);

        var parser = element(_ => true, AnyAttribute.NoneOrMore(), AnyContent.NoneOrMore());

        Contract.Assume(parser != null);

        return parser;
      }
    }

    /// <summary>
    /// Gets a parser with a grammar that matches an XML attribute.
    /// </summary>
    protected IObservableParser<char, XAttribute> AnyAttribute
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, XAttribute>>() != null);

        var parser = attribute(_ => true);

        Contract.Assume(parser != null);

        return parser;
      }
    }

    private readonly StringComparer comparer;
    private readonly IObservableParser<char, string> id;
    private readonly IObservableParser<char, char> tagStart;
    private readonly IObservableParser<char, char> tagEnd;
    private readonly IObservableParser<char, string> tagName;
    private readonly IObservableParser<char, char> attributeDelimiter;
    private readonly IObservableParser<char, string> attributeName;
    private readonly IObservableParser<char, string> attributeValue;

    [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1014:OpeningGenericBracketsMustBeSpacedCorrectly", Justification = "Readability of nested generic types.")]
    private readonly Func<
      Func<string, bool>,
      IObservableParser<char, XAttribute>> attribute;

    [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1014:OpeningGenericBracketsMustBeSpacedCorrectly", Justification = "Readability of nested generic types.")]
    private readonly Func<
      Func<string, bool>,
      IObservableParser<char, IObservable<XAttribute>>,
      IObservableParser<char, Tuple<string, IObservable<XAttribute>, IObservable<bool>>>> openTag;

    private readonly IObservableParser<char, string> closeTag;
    private readonly IObservableParser<char, XText> text;
    private readonly IObservableParser<char, XComment> comment;
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Not Hungarian notation.")]
    private readonly IObservableParser<char, XCData> cData;

    [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1014:OpeningGenericBracketsMustBeSpacedCorrectly", Justification = "Readability of nested generic types.")]
    private readonly Func<
      Func<string, bool>,
      IObservableParser<char, IObservable<XAttribute>>,
      IObservableParser<char, IObservable<XObject>>,
      IObservableParser<char, XElement>> element;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="XmlObservableParser{TResult}" /> class with the specified case-sensitivity
    /// for derived classes.
    /// </summary>
    /// <param name="caseSensitive">Indicates whether the comparison behavior used for matching element and attribute names
    /// must ignore case.</param>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The amount of coupling cannot be reduced without increasing complexity.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Grammar defined in functional programming style is less complicated than alternatives.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grammar defined in functional programming style is less complicated than alternatives.")]
    protected XmlObservableParser(bool caseSensitive)
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
        from isEmpty in tag.Item3
        from children in isEmpty
          ? open.Success(Observable.Empty<XObject>())
          : elementContent.IgnoreTrailing(
            InsignificantWhiteSpace.IgnoreBefore(
            closeTag))
        from array in tag.Item2.Concat(children).ToArray()
        select new XElement(tag.Item1, array);
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="XmlObservableParser{TResult}" /> class with case-sensitive element and 
    /// attribute name comparisons for derived classes.
    /// </summary>
    protected XmlObservableParser()
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
    protected IObservableParser<char, XAttribute> Attribute(string name)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Ensures(Contract.Result<IObservableParser<char, XAttribute>>() != null);

      var parser = attribute(n => comparer.Equals(n, name));

      Contract.Assume(parser != null);

      return parser;
    }

    /// <summary>
    /// Creates a parser that matches a single XML element containing the specified attributes and children.
    /// </summary>
    /// <param name="content">The parsers that match the element's attributes, child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element containing the specified attributes and children.</returns>
    protected IObservableParser<char, XElement> Element(params IObservableParser<char, XObject>[] content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);

      return Element(_ => true, content);
    }

    /// <summary>
    /// Creates a parser that matches a single XML element having no attributes and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element having no attributes and containing the specified <paramref name="content"/>.</returns>
    protected IObservableParser<char, XElement> Element(IObservableParser<char, IObservable<XObject>> content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);

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
    protected IObservableParser<char, XElement> Element(
      IObservableParser<char, IObservable<XAttribute>> attributes,
      IObservableParser<char, IObservable<XObject>> content)
    {
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);

      return Element(_ => true, attributes, content);
    }

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/> and containing 
    /// the specified attributes and children.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="content">The parsers that match the element's attributes, child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/> and containing 
    /// the specified attributes and children.</returns>
    protected IObservableParser<char, XElement> Element(string name, params IObservableParser<char, XObject>[] content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);

      return Element(n => comparer.Equals(n, name), content);
    }

    /// <summary>
    /// Creates a parser that matches a single XML element with the specified <paramref name="name"/>, having no attributes
    /// and containing the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="name">The name of the element to match.</param>
    /// <param name="content">The parser that matches the element's child elements, text, cdata and comments.</param>
    /// <returns>A parser that matches a single XML element with the specified <paramref name="name"/>, having no attributes 
    /// and containing the specified <paramref name="content"/>.</returns>
    protected IObservableParser<char, XElement> Element(string name, IObservableParser<char, IObservable<XObject>> content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);

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
    protected IObservableParser<char, XElement> Element(
      string name,
      IObservableParser<char, IObservable<XAttribute>> attributes,
      IObservableParser<char, IObservable<XObject>> content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);

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
    protected IObservableParser<char, XElement> Element(
      string name,
      IEnumerable<string> attributes,
      params IObservableParser<char, XObject>[] content)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(name));
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);

      var contentList = content.ToList();

#if !SILVERLIGHT || WINDOWS_PHONE
      contentList.AddRange(attributes.Select(a => Attribute(a)));
#else
			contentList.AddRange(attributes.Select(a => Attribute(a)).Cast<IObservableParser<char, XObject>>());
#endif

      return Element(n => comparer.Equals(n, name), contentList);
    }

    private IObservableParser<char, XElement> Element(
      Func<string, bool> name,
      IEnumerable<IObservableParser<char, XObject>> content)
    {
      Contract.Requires(name != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);

      IObservableParser<char, IObservable<XAttribute>> attributes;
      IObservableParser<char, IObservable<XObject>> children = Content(out attributes, content);

      var parser = element(name, attributes, children);

      Contract.Assume(parser != null);

      return parser;
    }

    private IObservableParser<char, XElement> Element(
      Func<string, bool> name,
      IObservableParser<char, IObservable<XAttribute>> attributes,
      IObservableParser<char, IObservable<XObject>> content)
    {
      Contract.Requires(name != null);
      Contract.Requires(attributes != null);
      Contract.Requires(content != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XElement>>() != null);

      var parser = element(name, attributes, Comment.NoneOrMore().And(content));

      Contract.Assume(parser != null);

      return parser;
    }

    private IObservableParser<char, IObservable<XObject>> Content(
      out IObservableParser<char, IObservable<XAttribute>> attributes,
      IEnumerable<IObservableParser<char, XObject>> content)
    {
      Contract.Requires(content != null);
      Contract.Ensures(Contract.ValueAtReturn(out attributes) != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, IObservable<XObject>>>() != null);

      var contentList = content.ToList();

      var attributeList = contentList.OfType<IObservableParser<char, XAttribute>>().ToList();

      var anyAttribute = attribute(_ => true);

      Contract.Assume(anyAttribute != null);

      attributes = (attributeList.Count == 0)
        ? anyAttribute.None()
        : attributeList.AllUnordered();

      foreach (var a in attributeList)
        contentList.Remove(a);

      var contentWithComments = new List<IObservableParser<char, IObservable<XObject>>>(contentList.Count);

      foreach (var item in contentList)
      {
        Contract.Assume(item != null);

        contentWithComments.Add(Comment.Maybe().And(item));
      }

      /* The original code here used a conditional instead of a full if block to return the result.  At runtime, 
       * when targeting Silverlight, it threw an exception: "Operation could destabilize the runtime".  I'm not
       * sure if it was the C# compiler or the Code Contracts rewriter that caused the problem, but expanding it
       * to a full if block has fixed it.
       */
      IObservableParser<char, IObservable<XObject>> result;

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

    private IObservableParser<char, XObject> Content(
      Func<IObservableParser<char, XText>, IObservableParser<char, XText>> textSelector,
      bool includeComments = true)
    {
      Contract.Requires(textSelector != null);
      Contract.Ensures(Contract.Result<IObservableParser<char, XObject>>() != null);

      return ObservableParser.Defer(() =>
      {
        var content = new List<IObservableParser<char, XObject>>(4)
					{
						element(_ => true, AnyAttribute.NoneOrMore(), Content(textSelector, includeComments).NoneOrMore()), 
						textSelector(text), 
						cData
					};

        if (includeComments)
        {
          content.Add(comment);
        }

        return content.Any();
      });
    }
    #endregion

    #region IXmlObservableParser<TResult> Members
    IObservableParser<char, XText> IXmlObservableParser<TResult>.Text
    {
      get
      {
        return Text;
      }
    }

    IObservableParser<char, XComment> IXmlObservableParser<TResult>.Comment
    {
      get
      {
        return Comment;
      }
    }

    IObservableParser<char, XCData> IXmlObservableParser<TResult>.CData
    {
      get
      {
        return CData;
      }
    }

    IObservableParser<char, XObject> IXmlObservableParser<TResult>.AnyContent
    {
      get
      {
        return AnyContent;
      }
    }

    IObservableParser<char, XElement> IXmlObservableParser<TResult>.AnyElement
    {
      get
      {
        return AnyElement;
      }
    }

    IObservableParser<char, XAttribute> IXmlObservableParser<TResult>.AnyAttribute
    {
      get
      {
        return AnyAttribute;
      }
    }

    IObservableParser<char, XAttribute> IXmlObservableParser<TResult>.Attribute(string name)
    {
      return Attribute(name);
    }

    IObservableParser<char, XElement> IXmlObservableParser<TResult>.Element(params IObservableParser<char, XObject>[] content)
    {
      return Element(content);
    }

    IObservableParser<char, XElement> IXmlObservableParser<TResult>.Element(IObservableParser<char, IObservable<XObject>> content)
    {
      return Element(content);
    }

    IObservableParser<char, XElement> IXmlObservableParser<TResult>.Element(IObservableParser<char, IObservable<XAttribute>> attributes, IObservableParser<char, IObservable<XObject>> content)
    {
      return Element(attributes, content);
    }

    IObservableParser<char, XElement> IXmlObservableParser<TResult>.Element(string name, params IObservableParser<char, XObject>[] content)
    {
      return Element(name, content);
    }

    IObservableParser<char, XElement> IXmlObservableParser<TResult>.Element(string name, IObservableParser<char, IObservable<XObject>> content)
    {
      return Element(name, content);
    }

    IObservableParser<char, XElement> IXmlObservableParser<TResult>.Element(string name, IObservableParser<char, IObservable<XAttribute>> attributes, IObservableParser<char, IObservable<XObject>> content)
    {
      return Element(name, attributes, content);
    }

    IObservableParser<char, XElement> IXmlObservableParser<TResult>.Element(string name, IEnumerable<string> attributes, params IObservableParser<char, XObject>[] content)
    {
      return Element(name, attributes, content);
    }
    #endregion
  }

  [ContractClassFor(typeof(XmlObservableParser<>))]
  internal abstract class XmlObservableParserContract<TResult> : XmlObservableParser<TResult>
  {
    protected override IObservableParser<char, TResult> Schema
    {
      get
      {
        Contract.Ensures(Contract.Result<IObservableParser<char, TResult>>() != null);
        return null;
      }
    }
  }
}