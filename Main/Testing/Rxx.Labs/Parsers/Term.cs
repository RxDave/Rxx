using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;

namespace Rxx.Labs.Parsers
{
  [ContractClass(typeof(TermContract))]
  internal abstract class Term
  {
    public abstract void Write(TextWriter writer);

    public override string ToString()
    {
      using (var writer = new StringWriter(CultureInfo.CurrentCulture))
      {
        Write(writer);

        writer.Write(';');

        return writer.ToString();
      }
    }
  }

  [ContractClassFor(typeof(Term))]
  internal abstract class TermContract : Term
  {
    public override void Write(TextWriter writer)
    {
      Contract.Requires(writer != null);
    }
  }
}
