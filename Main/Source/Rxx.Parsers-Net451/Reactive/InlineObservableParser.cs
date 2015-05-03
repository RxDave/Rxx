using System;
using System.Diagnostics.Contracts;
using System.Reactive;

namespace Rxx.Parsers.Reactive
{
  internal sealed class InlineObservableParser<TSource, TResult> : ObservableParser<TSource, TResult>, IObservableParser<TSource, TSource>
  {
    #region Public Properties
    protected override IObservableParser<TSource, TResult> Start
    {
      get
      {
        return start;
      }
    }
    #endregion

    #region Private / Protected
    private IObservableParser<TSource, TResult> start;
    #endregion

    #region Constructors
    public InlineObservableParser()
    {
      start = new AnonymousObservableParser<TSource, TResult>(
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

    public IObservable<TResult> Parse(IObservableCursor<TSource> source, IObservableParser<TSource, TResult> grammar)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Requires(grammar != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      this.start = grammar;

      return base.Parse(source);
    }
    #endregion

    #region IParser<TSource,TSource> Members
    IObservableParser<TSource, TSource> IObservableParser<TSource, TSource>.Next
    {
      get
      {
        return Next;
      }
    }

    IObservable<IParseResult<TSource>> IObservableParser<TSource, TSource>.Parse(IObservableCursor<TSource> source)
    {
      throw new NotSupportedException(Properties.Errors.InlineParseNotSupported);
    }
    #endregion
  }
}