using System.Diagnostics.Contracts;
#if WINDOWS_PHONE
using System.Linq;
#endif
using Rxx.Parsers;
using Rxx.Parsers.Linq;

namespace Rxx.Labs.Parsers
{
  internal sealed class MiniMLParser : StringParser<Term>
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    protected override IParser<char, Term> Start
    {
      get
      {
        return term.IgnoreTrailing(WsChr(';'));
      }
    }

    private readonly IParser<char, string> anyId;
    private readonly IParser<char, string> identity;
    private readonly IParser<char, string> keyword;
    private readonly IParser<char, string> letId;
    private readonly IParser<char, string> inId;
    private readonly IParser<char, Term> term;
    private readonly IParser<char, Term> term1;
    #endregion

    #region Constructors
    public MiniMLParser()
    {
      anyId = from _ in InsignificantWhiteSpace
              from first in Character(char.IsLetter)
              from remainder in Character(char.IsLetterOrDigit).NoneOrMore()
#if !WINDOWS_PHONE
              select first + string.Concat(remainder);
#else
							select first + string.Concat(remainder.Cast<object>().ToArray());
#endif

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
              select (Term)new AppTerm(t, remainder));
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

    private IParser<char, char> WsChr(char value)
    {
      Contract.Ensures(Contract.Result<IParser<char, char>>() != null);

      return InsignificantWhiteSpace.IgnoreBefore(Character(value));
    }
    #endregion
  }
}
