using System.Diagnostics.Contracts;
using System.IO;

namespace Rxx.Labs.Parsers
{
  internal sealed class LetTerm : Term
  {
    public string Ident
    {
      get;
      private set;
    }

    public Term Rhs
    {
      get;
      private set;
    }

    public Term Body
    {
      get;
      private set;
    }

    public LetTerm(string i, Term rhs, Term body)
    {
      Contract.Requires(rhs != null);
      Contract.Requires(body != null);

      Ident = i;
      Rhs = rhs;
      Body = body;
    }

    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(Rhs != null);
      Contract.Invariant(Body != null);
    }

    public override void Write(TextWriter writer)
    {
      writer.Write("let ");
      writer.Write(Ident);
      writer.Write(" = ");

      Rhs.Write(writer);

      writer.WriteLine(" in ");

      Body.Write(writer);
    }
  }
}
