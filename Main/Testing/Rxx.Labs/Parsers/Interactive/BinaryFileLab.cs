using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Rxx.Labs.Properties;
using Rxx.Parsers.Linq;

namespace Rxx.Labs.Parsers.Interactive
{
  [DisplayName("Binary File (New)")]
  [Description("Writing and parsing a binary file.")]
  public sealed class BinaryFileLab : BaseConsoleLab
  {
    private const string path = @"BinaryFileLab.bin";

    protected override void Main()
    {
      TraceLine(Instructions.WaitForCompletion);

      try
      {
        CreateFile();

        using (Stream stream = Storage.OpenFile(path, FileMode.Open, FileAccess.Read))
        {
          IEnumerable<byte> bytes = stream.ToEnumerable();

          var parsed = bytes.ParseBinary(parser =>
            from next in parser
            let magicNumber = parser.String(Encoding.UTF8, 3).Where(value => value == "RXX")
            let header = from headerLength in parser.Int32
                         from header in next.Exactly(headerLength)
                         select header
            let message = parser.String(Encoding.UTF8)
            let entry = from length in parser.Int32
                        from data in next.Exactly(length)
                        select data.Aggregate(string.Empty, (s, b) => s + " " + b)
            let entries = from count in parser.Int32
                          from entries in entry.Exactly(count).ToList()
                          select entries
            select from _ in magicNumber.Required("The file's magic number is invalid.")
                   from h in header.Required("The file's header is invalid.")
                   from m in message.Required("The file's message is invalid.")
                   from e in entries.Required("The file's data is invalid.")
                   select new
                   {
                     Header = h.Aggregate(string.Empty, (s, b) => s + " " + b),
                     Message = m,
                     Entries = e.Aggregate(string.Empty, (acc, cur) => acc + cur + Environment.NewLine)
                   });

          parsed.Take(1).ForEach(ConsoleOutput);
        }
      }
      finally
      {
        Storage.DeleteFile(path);
      }
    }

    private static void CreateFile()
    {
      using (var writer = new BinaryWriter(Storage.CreateFile(path), Encoding.UTF8))
      {
        writer.Write(Encoding.UTF8.GetBytes("RXX"));

        // write header
        writer.Write(10);		// length of header
        writer.Write(Enumerable.Range(1, 10).Select(i => (byte)i).ToArray());

        // write message
        writer.Write("Hello");

        // write data
        writer.Write(5);		// number of entries

        for (int i = 1; i <= 5; i++)
        {
          writer.Write(i);	// length of entry

          // write entry
          for (byte b = 0; b < i; b++)
          {
            writer.Write(b);
          }
        }
      }
    }
  }
}