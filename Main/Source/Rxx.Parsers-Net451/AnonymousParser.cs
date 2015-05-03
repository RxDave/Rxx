using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers
{
  internal sealed class AnonymousParser<TSource, TResult> : IParser<TSource, TResult>
  {
    #region Public Properties
    public IParser<TSource, TSource> Next
    {
      get
      {
        IParser<TSource, TSource> next = getNext();

        Contract.Assume(next != null);

        return next;
      }
    }
    #endregion

    #region Private / Protected
    private readonly Lazy<IParser<TSource, TResult>> parserFactory;
    private readonly Func<ICursor<TSource>, IEnumerable<IParseResult<TResult>>> parse;
    private readonly Func<IParser<TSource, TSource>> getNext;
    private readonly string name;
    #endregion

    #region Constructors
    public AnonymousParser(
      string name,
      Func<IParser<TSource, TSource>> getNext,
      Func<ICursor<TSource>, IEnumerable<IParseResult<TResult>>> parse)
    {
      Contract.Requires(name == null || name.Length > 0);
      Contract.Requires(getNext != null);
      Contract.Requires(parse != null);

      this.name = name;
      this.getNext = getNext;
      this.parse = parse;
    }

    public AnonymousParser(string name, Func<IParser<TSource, TResult>> parserFactory)
    {
      Contract.Requires(name == null || name.Length > 0);
      Contract.Requires(parserFactory != null);

      this.name = name;
      this.parserFactory = new Lazy<IParser<TSource, TResult>>(parserFactory);
      this.getNext = () => this.parserFactory.Value.Next;
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(name == null || name.Length > 0);
      Contract.Invariant(parse != null || parserFactory != null);
      Contract.Invariant(getNext != null);
    }

    public IEnumerable<IParseResult<TResult>> Parse(ICursor<TSource> source)
    {
#if !SILVERLIGHT && !PORT_45 && !PORT_40
      using (ParserTraceSources.TraceQueryCompilation(name))
#endif
      {
        IEnumerable<IParseResult<TResult>> results;

        if (parserFactory == null)
        {
          results = parse(source);

          Contract.Assume(results != null);
        }
        else
        {
          var parser = parserFactory.Value;

          Contract.Assume(parser != null);

          results = parser.Parse(source);
        }

        return results;
      }
    }

    public override string ToString()
    {
      return (name == null ? string.Empty : "(" + name + ") ")
            + Next.ToString();
    }
    #endregion
  }
}