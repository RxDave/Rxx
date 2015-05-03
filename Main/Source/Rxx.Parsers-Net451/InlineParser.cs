using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers
{
  internal sealed class InlineParser<TSource, TResult> : Parser<TSource, TResult>, IParser<TSource, TSource>
  {
    #region Public Properties
    protected override IParser<TSource, TResult> Start
    {
      get
      {
        return start;
      }
    }
    #endregion

    #region Private / Protected
    private IParser<TSource, TResult> start;
    #endregion

    #region Constructors
    public InlineParser()
    {
      start = new AnonymousParser<TSource, TResult>(
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
      Contract.Invariant(start != null);
    }

    public IEnumerable<TResult> Parse(ICursor<TSource> source, IParser<TSource, TResult> grammar)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Requires(grammar != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      this.start = grammar;

      return base.Parse(source);
    }
    #endregion

    #region IParser<TSource,TSource> Members
    IParser<TSource, TSource> IParser<TSource, TSource>.Next
    {
      get
      {
        return Next;
      }
    }

    IEnumerable<IParseResult<TSource>> IParser<TSource, TSource>.Parse(ICursor<TSource> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion
  }
}