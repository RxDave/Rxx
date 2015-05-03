using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
#if !SILVERLIGHT
using System.Media;
#endif
using Rxx.Labs.Properties;
using Rxx.Parsers.Linq;

namespace Rxx.Labs.Parsers.Interactive
{
  [DisplayName("Key Combo")]
  [Description("Parses key input by matching from a list of predefined key combinations and "
             + "executes an associated action to indicate when a combo is recognized.")]
  public sealed class KeyComboLab : BaseConsoleLab
  {
    [SuppressMessage("Microsoft.StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly",
      Justification = "Lambda casts.")]
    protected override void Main()
    {
      TraceDescription(Instructions.ParserKeyComboLab);

      bool stopped = false;

      IEnumerable<char> keys =
        from key in UserInputKeys(Text.PromptFormat, Text.Keys).TakeWhile(_ => !stopped)
        select char.ToUpperInvariant(key.KeyChar);

#if SILVERLIGHT
			var hiResponse = "Hello!";
#else
      var hiResponse = "Hello " + Environment.UserName;
#endif

      IEnumerable<Action> keyComboActions = keys.ParseString(parser =>
        from next in parser
        let hi = parser.Word("HI").Select(_ => (Action)(() => Respond(hiResponse)))
#if SILVERLIGHT
				let beep = parser.Word("BEEP").Select(_ => (Action) (() => Respond("*BEEP!*")))
#else
        let beep = parser.Word("BEEP").Select(_ => (Action)SystemSounds.Beep.Play)
#endif
        let numbers = parser.Word("123").Select(_ => (Action)(() => Respond("456")))
        let html = parser.Word("<>").Select(_ => (Action)(() => Respond("<HTML>")))
        let stop = parser.Word("STOP").Select(_ => (Action)(() => stopped = true))
        let escape = parser.Character((char)0x1B).Select(_ => (Action)(() => stopped = true))
        select new[] 
				{
					hi, 
					beep, 
					numbers, 
					html, 
					stop,
					escape
				}
        .Any());

      foreach (var action in keyComboActions)
      {
        action();
      }

      TraceReplaceableLine(string.Empty);
    }

    private void Respond(string response)
    {
      TraceReplaceableLine(Environment.NewLine + response);
    }
  }
}