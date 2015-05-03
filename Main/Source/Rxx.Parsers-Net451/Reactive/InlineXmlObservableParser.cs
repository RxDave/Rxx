using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Xml.Linq;

namespace Rxx.Parsers.Reactive
{
  internal sealed class InlineXmlObservableParser<TResult> : XmlObservableParser<TResult>, IXmlObservableParser<char>
  {
    #region Public Properties
    protected override IObservableParser<char, TResult> Schema
    {
      get
      {
        return schema;
      }
    }
    #endregion

    #region Private / Protected
    private IObservableParser<char, TResult> schema;
    #endregion

    #region Constructors
    public InlineXmlObservableParser()
    {
      schema = new AnonymousObservableParser<char, TResult>(
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

    public IObservable<TResult> Parse(IObservableCursor<char> source, IObservableParser<char, TResult> grammar)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Requires(grammar != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      this.schema = grammar;

      return base.Parse(source);
    }
    #endregion

    #region IObservableParser<char,char> Members
    IObservableParser<char, char> IObservableParser<char, char>.Next
    {
      get
      {
        return Next;
      }
    }

    IObservable<IParseResult<char>> IObservableParser<char, char>.Parse(IObservableCursor<char> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion

    #region IXmlObservableParser<char> Members
    IObservableParser<char, XText> IXmlObservableParser<char>.Text
    {
      get
      {
        return Text;
      }
    }

    IObservableParser<char, XComment> IXmlObservableParser<char>.Comment
    {
      get
      {
        return Comment;
      }
    }

    IObservableParser<char, XCData> IXmlObservableParser<char>.CData
    {
      get
      {
        return CData;
      }
    }

    IObservableParser<char, XObject> IXmlObservableParser<char>.AnyContent
    {
      get
      {
        return AnyContent;
      }
    }

    IObservableParser<char, XElement> IXmlObservableParser<char>.AnyElement
    {
      get
      {
        return AnyElement;
      }
    }

    IObservableParser<char, XAttribute> IXmlObservableParser<char>.AnyAttribute
    {
      get
      {
        return AnyAttribute;
      }
    }

    IObservableParser<char, XAttribute> IXmlObservableParser<char>.Attribute(string name)
    {
      return Attribute(name);
    }

    IObservableParser<char, XElement> IXmlObservableParser<char>.Element(params IObservableParser<char, XObject>[] content)
    {
      return Element(content);
    }

    IObservableParser<char, XElement> IXmlObservableParser<char>.Element(IObservableParser<char, IObservable<XObject>> content)
    {
      return Element(content);
    }

    IObservableParser<char, XElement> IXmlObservableParser<char>.Element(IObservableParser<char, IObservable<XAttribute>> attributes, IObservableParser<char, IObservable<XObject>> content)
    {
      return Element(attributes, content);
    }

    IObservableParser<char, XElement> IXmlObservableParser<char>.Element(string name, params IObservableParser<char, XObject>[] content)
    {
      return Element(name, content);
    }

    IObservableParser<char, XElement> IXmlObservableParser<char>.Element(string name, IObservableParser<char, IObservable<XObject>> content)
    {
      return Element(name, content);
    }

    IObservableParser<char, XElement> IXmlObservableParser<char>.Element(string name, IObservableParser<char, IObservable<XAttribute>> attributes, IObservableParser<char, IObservable<XObject>> content)
    {
      return Element(name, attributes, content);
    }

    IObservableParser<char, XElement> IXmlObservableParser<char>.Element(string name, IEnumerable<string> attributes, params IObservableParser<char, XObject>[] content)
    {
      return Element(name, attributes, content);
    }
    #endregion
  }
}
