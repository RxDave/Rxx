using System;
using System.Diagnostics.Contracts;
using System.Reactive;

namespace Rxx.Parsers.Reactive
{
  internal sealed class AnonymousObservableParser<TSource, TResult> : IObservableParser<TSource, TResult>
  {
    #region Public Properties
    public IObservableParser<TSource, TSource> Next
    {
      get
      {
        IObservableParser<TSource, TSource> next = getNext();

        Contract.Assume(next != null);

        return next;
      }
    }
    #endregion

    #region Private / Protected
    private readonly Lazy<IObservableParser<TSource, TResult>> parserFactory;
    private readonly Func<IObservableCursor<TSource>, IObservable<IParseResult<TResult>>> parse;
    private readonly Func<IObservableParser<TSource, TSource>> getNext;
    private readonly string name;
    #endregion

    #region Constructors
    public AnonymousObservableParser(
      string name,
      Func<IObservableParser<TSource, TSource>> getNext,
      Func<IObservableCursor<TSource>, IObservable<IParseResult<TResult>>> parse)
    {
      Contract.Requires(name == null || name.Length > 0);
      Contract.Requires(getNext != null);
      Contract.Requires(parse != null);

      this.name = name;
      this.getNext = getNext;
      this.parse = parse;
    }

    public AnonymousObservableParser(string name, Func<IObservableParser<TSource, TResult>> parserFactory)
    {
      Contract.Requires(name == null || name.Length > 0);
      Contract.Requires(parserFactory != null);

      this.name = name;
      this.parserFactory = new Lazy<IObservableParser<TSource, TResult>>(parserFactory);
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

    public IObservable<IParseResult<TResult>> Parse(IObservableCursor<TSource> source)
    {
#if !SILVERLIGHT && !PORT_45 && !PORT_40
      using (ParserTraceSources.TraceQueryCompilation(name))
#endif
      {
        IObservable<IParseResult<TResult>> results;

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