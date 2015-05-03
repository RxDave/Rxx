using System.Diagnostics.Contracts;
using System.IO;

namespace Rxx.Labs.Parsers
{
  internal sealed class LambdaTerm : Term
  {
    public string Ident
    {
      get;
      private set;
    }

    public Term Term
    {
      get;
      private set;
    }

    public LambdaTerm(string i, Term term)
    {
      Contract.Requires(term != null);

      Ident = i;
      Term = term;
    }

    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(Term != null);
    }

    public override void Write(TextWriter writer)
    {
      writer.Write('\\');
      writer.Write(Ident);
      writer.Write('.');

      Term.Write(writer);
    }
  }
}
