using System.IO;

namespace Rxx.Labs.Parsers
{
  internal sealed class VarTerm : Term
  {
    public string Ident
    {
      get;
      private set;
    }

    public VarTerm(string ident)
    {
      Ident = ident;
    }

    public override void Write(TextWriter writer)
    {
      writer.Write(Ident);
    }
  }
}
