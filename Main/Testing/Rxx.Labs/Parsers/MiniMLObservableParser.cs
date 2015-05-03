using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using Rxx.Parsers.Reactive;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.Labs.Parsers
{
  internal sealed class MiniMLObservableParser : StringObservableParser<Term>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    protected override IObservableParser<char, Term> Start
    {
      get
      {
        return term.IgnoreTrailing(WsChr(';'));
      }
    }

    private readonly IObservableParser<char, string> anyId;
    private readonly IObservableParser<char, string> identity;
    private readonly IObservableParser<char, string> keyword;
    private readonly IObservableParser<char, string> letId;
    private readonly IObservableParser<char, string> inId;
    private readonly IObservableParser<char, Term> term;
    private readonly IObservableParser<char, Term> term1;
    #endregion

    #region Constructors
    public MiniMLObservableParser()
    {
      anyId = from _ in InsignificantWhiteSpace
              from first in Character(char.IsLetter)
              from remainder in Character(char.IsLetterOrDigit).NoneOrMore().Join()
              select first + remainder;

      letId = anyId.Where(id => id == "let");
      inId = anyId.Where(id => id == "in");
      keyword = letId.Or(inId);
      identity = anyId.Not(keyword);

      term1 = (from id in identity
               select (Term)new VarTerm(id))
              .Or(
              from _ in WsChr('(')
              from t in term
              from __ in WsChr(')')
              select t);

      term = (from _ in WsChr('\\')
              from id in identity
              from __ in WsChr('.')
              from t in term
              select (Term)new LambdaTerm(id, t))
              .Or(
              from _ in letId
              from id in identity
              from __ in WsChr('=')
              from rhs in term
              from ___ in inId
              from body in term
              select (Term)new LetTerm(id, rhs, body))
              .Or(
              from t in term1
              from remainder in term1.NoneOrMore()
              from list in remainder.ToList()
              select (Term)new AppTerm(t, list));
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(anyId != null);
      Contract.Invariant(identity != null);
      Contract.Invariant(keyword != null);
      Contract.Invariant(letId != null);
      Contract.Invariant(inId != null);
      Contract.Invariant(term != null);
      Contract.Invariant(term1 != null);
    }

    private IObservableParser<char, char> WsChr(char value)
    {
      Contract.Ensures(Contract.Result<IObservableParser<char, char>>() != null);

      return InsignificantWhiteSpace.IgnoreBefore(Character(value));
    }
    #endregion
  }
}
