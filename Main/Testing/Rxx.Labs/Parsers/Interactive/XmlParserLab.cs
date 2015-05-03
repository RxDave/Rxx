using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using Rxx.Labs.Properties;
using Rxx.Parsers;
using Rxx.Parsers.Linq;

namespace Rxx.Labs.Parsers.Interactive
{
  [DisplayName("XML")]
  [Description("Parses a basic XML string into a LINQ to XML element tree.")]
  public sealed class XmlParserLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForCompletion);

      IEnumerable<char> xml = @"
<foods>
	<food name=""Carrot"" color=""orange"" />
	<food name=""Ice Cream"">
		<flavors>
			<flavor>Vanilla</flavor>
			<flavor>
				Chocolate
			</flavor>
		</flavors>

		<others type=""related"">
			<food name=""Sugar"" />
		</others>
	</food>

	<!-- Last item: -->
	<food name=""Cheese""></food>

	The foods in this group are great.

</foods>
";

      IEnumerable<XElement> parsed = xml.Parse(new XmlParser());

      parsed.ForEach(ConsoleOutput(Environment.NewLine));
    }
  }
}