using System.IO;

namespace Rxx.Labs.Parsers
{
  internal sealed class OpTerm : Term
  {
    public char Operator
    {
      get;
      private set;
    }

    public int Left
    {
      get;
      private set;
    }

    public int Right
    {
      get;
      private set;
    }

    public OpTerm(int left, char op, int right)
    {
      Left = left;
      Operator = op;
      Right = right;
    }

    public override void Write(TextWriter writer)
    {
      writer.Write(Left);
      writer.Write(Operator);
      writer.Write(Right);
    }
  }
}
