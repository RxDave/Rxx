using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;

namespace Rxx.Parsers
{
  internal sealed class InlineXmlParser<TResult> : XmlParser<TResult>, IXmlParser<char>
  {
    #region Public Properties
    protected override IParser<char, TResult> Schema
    {
      get
      {
        return schema;
      }
    }
    #endregion

    #region Private / Protected
    private IParser<char, TResult> schema;
    #endregion

    #region Constructors
    public InlineXmlParser()
    {
      schema = new AnonymousParser<char, TResult>(
        "Inline",
        () => Next,
        source =>
        {
          throw new NotSupportedException(Properties.Errors.InlineParserWithoutGrammar);
        });
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(schema != null);
    }

    public IEnumerable<TResult> Parse(ICursor<char> source, IParser<char, TResult> grammar)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Requires(grammar != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      this.schema = grammar;

      return base.Parse(source);
    }
    #endregion

    #region IParser<char,char> Members
    IParser<char, char> IParser<char, char>.Next
    {
      get
      {
        return Next;
      }
    }

    IEnumerable<IParseResult<char>> IParser<char, char>.Parse(ICursor<char> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion

    #region IXmlParser<char> Members
    IParser<char, XText> IXmlParser<char>.Text
    {
      get
      {
        return Text;
      }
    }

    IParser<char, XComment> IXmlParser<char>.Comment
    {
      get
      {
        return Comment;
      }
    }

    IParser<char, XCData> IXmlParser<char>.CData
    {
      get
      {
        return CData;
      }
    }

    IParser<char, XObject> IXmlParser<char>.AnyContent
    {
      get
      {
        return AnyContent;
      }
    }

    IParser<char, XElement> IXmlParser<char>.AnyElement
    {
      get
      {
        return AnyElement;
      }
    }

    IParser<char, XAttribute> IXmlParser<char>.AnyAttribute
    {
      get
      {
        return AnyAttribute;
      }
    }

    IParser<char, XAttribute> IXmlParser<char>.Attribute(string name)
    {
      return Attribute(name);
    }

    IParser<char, XElement> IXmlParser<char>.Element(params IParser<char, XObject>[] content)
    {
      return Element(content);
    }

#if SILVERLIGHT && !WINDOWS_PHONE
		IParser<char, XElement> IXmlParser<char>.Element(IEnumerable<IParser<char, XAttribute>> attributes, params IParser<char, XObject>[] content)
		{
			return Element(attributes, content);
		}
#endif

    IParser<char, XElement> IXmlParser<char>.Element(IParser<char, IEnumerable<XObject>> content)
    {
      return Element(content);
    }

    IParser<char, XElement> IXmlParser<char>.Element(IParser<char, IEnumerable<XAttribute>> attributes, IParser<char, IEnumerable<XObject>> content)
    {
      return Element(attributes, content);
    }

    IParser<char, XElement> IXmlParser<char>.Element(string name, params IParser<char, XObject>[] content)
    {
      return Element(name, content);
    }

#if SILVERLIGHT && !WINDOWS_PHONE
		IParser<char, XElement> IXmlParser<char>.Element(string name, IEnumerable<IParser<char, XAttribute>> attributes, params IParser<char, XObject>[] content)
		{
			return Element(name, attributes, content);
		}
#endif

    IParser<char, XElement> IXmlParser<char>.Element(string name, IParser<char, IEnumerable<XObject>> content)
    {
      return Element(name, content);
    }

    IParser<char, XElement> IXmlParser<char>.Element(string name, IParser<char, IEnumerable<XAttribute>> attributes, IParser<char, IEnumerable<XObject>> content)
    {
      return Element(name, attributes, content);
    }

    IParser<char, XElement> IXmlParser<char>.Element(string name, IEnumerable<string> attributes, params IParser<char, XObject>[] content)
    {
      return Element(name, attributes, content);
    }
    #endregion
  }
}
