using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Xml.Linq;
using Rxx.Labs.Properties;
using Rxx.Parsers.Reactive;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.Labs.Parsers.Reactive
{
  [DisplayName("XML")]
  [Description("Parses a basic observable XML string into a LINQ to XML element tree.")]
  public sealed class XmlParserLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForCompletion);

      IObservable<char> xml = Observable.ToObservable(@"
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
");

      IObservable<XElement> parsed = xml.Parse(new XmlObservableParser());

      parsed.ForEach(ConsoleOutput(Environment.NewLine));
    }
  }
}
