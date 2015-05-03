using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace Rxx.Labs.Parsers
{
  internal sealed class AppTerm : Term
  {
    public Term Func
    {
      get;
      private set;
    }

    public IList<Term> Args
    {
      get;
      private set;
    }

    public AppTerm(Term func, IEnumerable<Term> args)
    {
      Contract.Requires(func != null);
      Contract.Requires(args != null);

      Func = func;
      Args = args.ToList().AsReadOnly();
    }

    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(Func != null);
      Contract.Invariant(Args != null);
    }

    public override void Write(TextWriter writer)
    {
      writer.Write('(');

      Func.Write(writer);

      var args = Args.ToList();

      if (args.Count > 0)
      {
        writer.Write(' ');

        for (int t = 0; t < args.Count; t++)
        {
          var arg = args[t];

          if (arg != null)
          {
            arg.Write(writer);

            if (t + 1 < args.Count)
              writer.Write(' ');
          }
        }
      }

      writer.Write(')');
    }
  }
}
