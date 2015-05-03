using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Xml.Linq;
using Rxx.Labs.Properties;
using Rxx.Parsers.Linq;
using Rxx.Parsers.Reactive.Linq;

namespace Rxx.Labs.Parsers.Reactive
{
  [DisplayName("XML Schema")]
  [Description("Parses an XML string against a basic XML schema defined as a context-sensitive grammar (CSG).")]
  public sealed class XmlSchemaLab : BaseConsoleLab
  {
    protected override void Main()
    {
      TraceLine(Instructions.WaitForCompletion);

      IObservable<char> xml = Observable.ToObservable(@"
<root>
	<vehicles>
		<car make=""We Get U There"" model=""Toy"" />
		<plane />
		<boat>
			<!-- Typically found on water -->
		</boat>
	</vehicles>
	<equipment>
		<tire>
			Round things on a car.
		</tire>
		<wing>
			Long things on a plane.
		</wing>
		<sail>
			Tall things on a boat.
		</sail>
	</equipment>
</root>");

      IObservable<XElement> parsed = xml.ParseXml(parser =>
        from next in parser
        let elementsWithText = parser.Element(parser.Text).OneOrMore()
        select parser.Element("root",
                parser.Element("vehicles",
                  parser.Element("car", parser.Attribute("model"), parser.Attribute("make")),
                  parser.Element("plane"),
                  parser.Element("boat")),
                parser.Element("equipment",
                  elementsWithText)));

      parsed.ForEach(ConsoleOutput(Environment.NewLine));
    }
  }
}