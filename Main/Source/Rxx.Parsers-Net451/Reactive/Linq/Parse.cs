using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace Rxx.Parsers.Reactive.Linq
{
  public static partial class ObservableParser
  {
    /// <summary>
    /// Applies the specified <paramref name="parser"/> to generate matches.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="source">The observable sequence to parse.</param>
    /// <param name="parser">An object that defines a grammar to be applied to the observable sequence to generate matches.</param>
    /// <returns>An observable sequence of matches.</returns>
    public static IObservable<TResult> Parse<TSource, TResult>(
      this IObservable<TSource> source,
      IObservableParser<TSource, TResult> parser)
    {
      Contract.Requires(source != null);
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      // enableBranchOptimizations must be false: See the comments in the first interactive Rxx.Parsers.Linq.Parser.Parse method for details.
      return parser.Parse(source.ToCursor(forwardOnly: true, enableBranchOptimizations: false)).Select(result => result.Value);
    }

    /// <summary>
    /// Applies the specified unambiguous parser grammar to generate matches.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="source">The observable sequence to parse.</param>
    /// <param name="grammarSelector">A function that receives an object from which a parser cursor can be obtained and returns a grammar
    /// defined as a LINQ <see langword="select"/> query.</param>
    /// <remarks>
    /// The recommended approach to defining an in-line query is to use query comprehension syntax, starting with a <see langword="from"/> 
    /// statement to bind the cursor parser to a query variable named <strong>next</strong>, followed by the definitions of zero or more 
    /// individual grammar rules as <see langword="let"/> statements, and finally the complete grammar as a parser query in terms of the 
    /// previously defined rules projected as a <see langword="select" /> statement.
    /// </remarks>
    /// <example>
    /// The following example illustrates the recommended approach to defining an in-line grammar.
    /// <code><![CDATA[source.Parse(parser =>
    ///		// get the cursor
    /// 	from next in parser
    /// 	// define the grammar rules in terms of the cursor
    /// 	let letter = next.Where(char.IsLetter)
    /// 	let number = next.Where(char.IsNumber)
    /// 	// define the grammar in terms of the rules
    /// 	select from _ in number
    ///					 from twoInARow in letter.And(letter).Join()
    ///					 select twoInARow)]]></code>
    /// </example>
    /// <returns>An observable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IObservable<TResult> Parse<TSource, TResult>(
      this IObservable<TSource> source,
      Func<ObservableParserQueryContext<TSource, TSource, IObservableParser<TSource, TSource>>,
           ObservableParserQueryContext<TSource, TSource, IObservableParser<TSource, TResult>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      var parser = new InlineObservableParser<TSource, TResult>();
      var proxyParser = (IObservableParser<TSource, TSource>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new ObservableParserQueryContext<TSource, TSource, IObservableParser<TSource, TSource>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IObservableParser<TSource, TResult> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first interactive Rxx.Parsers.Linq.Parser.Parse method for details.
      return parser.Parse(source.ToCursor(forwardOnly: true, enableBranchOptimizations: false), start);
    }

    /// <summary>
    /// Applies the specified ambiguous parser grammar to generate matches.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="source">The observable sequence to parse.</param>
    /// <param name="grammarSelector">A function that receives an object from which a parser cursor can be obtained and returns a grammar
    /// defined as a LINQ <see langword="select"/> query.</param>
    /// <remarks>
    /// The recommended approach to defining an in-line query is to use query comprehension syntax, starting with a <see langword="from"/> 
    /// statement to bind the cursor parser to a query variable named <strong>next</strong>, followed by the definitions of zero or more 
    /// individual grammar rules as <see langword="let"/> statements, and finally the complete grammar as a parser query in terms of the 
    /// previously defined rules projected as a <see langword="select" /> statement.
    /// </remarks>
    /// <example>
    /// The following example illustrates the recommended approach to defining an in-line grammar.
    /// <code><![CDATA[source.Parse(parser =>
    ///		// get the cursor
    /// 	from next in parser
    /// 	// define the grammar rules in terms of the cursor
    /// 	let letter = next.Where(char.IsLetter)
    /// 	let number = next.Where(char.IsNumber)
    /// 	// define the grammar in terms of the rules
    /// 	select from _ in number
    ///					 from twoInARow in letter.And(letter).Join()
    ///					 select twoInARow)]]></code>
    /// </example>
    /// <returns>An observable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IObservable<TResult> Parse<TSource, TResult>(
      this IObservable<TSource> source,
      Func<ObservableParserQueryContext<TSource, TSource, IObservableParser<TSource, TSource>>,
           ObservableParserQueryContext<TSource, TSource, IObservableParser<TSource, IObservable<TResult>>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      var parser = new InlineObservableParser<TSource, IObservable<TResult>>();
      var proxyParser = (IObservableParser<TSource, TSource>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new ObservableParserQueryContext<TSource, TSource, IObservableParser<TSource, TSource>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IObservableParser<TSource, IObservable<TResult>> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first interactive Rxx.Parsers.Linq.Parser.Parse method for details.
      return parser.Parse(source.ToCursor(forwardOnly: true, enableBranchOptimizations: false), start).Concat();
    }

    /// <summary>
    /// Applies the specified unambiguous parser grammar to generate matches from a sequence of characters.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source characters.</typeparam>
    /// <param name="source">The observable sequence of <see cref="char"/> to parse.</param>
    /// <param name="grammarSelector">A function that receives an object from which a parser cursor can be obtained and returns a grammar
    /// defined as a LINQ <see langword="select"/> query.</param>
    /// <remarks>
    /// The recommended approach to defining an in-line query is to use query comprehension syntax, starting with a <see langword="from"/> 
    /// statement to bind the cursor parser to a query variable named <strong>next</strong>, followed by the definitions of zero or more 
    /// individual grammar rules as <see langword="let"/> statements, and finally the complete grammar as a parser query in terms of the 
    /// previously defined rules projected as a <see langword="select" /> statement.
    /// </remarks>
    /// <example>
    /// The following example illustrates the recommended approach to defining an in-line grammar.
    /// <code><![CDATA[source.ParseString(parser =>
    ///		// get the cursor
    /// 	from next in parser
    /// 	// define the grammar rules in terms of the cursor or specialized parser functions
    /// 	// that use the cursor
    /// 	let letter = parser.Character(char.IsLetter)
    /// 	let number = parser.Character(char.IsNumber)
    /// 	// define the grammar in terms of the rules
    /// 	select from _ in number
    ///					 from twoInARow in letter.And(letter).Join()
    ///					 select twoInARow)]]></code>
    /// </example>
    /// <returns>An observable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IObservable<TResult> ParseBinary<TResult>(
      this IObservable<byte> source,
      Func<BinaryObservableParserQueryContext<byte, IObservableParser<byte, byte>>,
           BinaryObservableParserQueryContext<byte, IObservableParser<byte, TResult>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      var parser = new InlineBinaryObservableParser<TResult>();
      var proxyParser = (IBinaryObservableParser<byte>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new BinaryObservableParserQueryContext<byte, IObservableParser<byte, byte>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IObservableParser<byte, TResult> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first interactive Rxx.Parsers.Linq.Parser.Parse method for details.
      return parser.Parse(source.ToCursor(forwardOnly: true, enableBranchOptimizations: false), start);
    }

    /// <summary>
    /// Applies the specified ambiguous parser grammar to generate matches from a sequence of characters.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source characters.</typeparam>
    /// <param name="source">The sequence of <see cref="char"/> to parse.</param>
    /// <param name="grammarSelector">A function that receives an object from which a parser cursor can be obtained and returns a grammar
    /// defined as a LINQ <see langword="select"/> query.</param>
    /// <remarks>
    /// The recommended approach to defining an in-line query is to use query comprehension syntax, starting with a <see langword="from"/> 
    /// statement to bind the cursor parser to a query variable named <strong>next</strong>, followed by the definitions of zero or more 
    /// individual grammar rules as <see langword="let"/> statements, and finally the complete grammar as a parser query in terms of the 
    /// previously defined rules projected as a <see langword="select" /> statement.
    /// </remarks>
    /// <example>
    /// The following example illustrates the recommended approach to defining an in-line grammar.
    /// <code><![CDATA[source.ParseString(parser =>
    ///		// get the cursor
    /// 	from next in parser
    /// 	// define the grammar rules in terms of the cursor or specialized parser functions
    /// 	// that use the cursor
    /// 	let letter = parser.Character(char.IsLetter)
    /// 	let number = parser.Character(char.IsNumber)
    /// 	// define the grammar in terms of the rules
    /// 	select from _ in number
    ///					 from twoInARow in letter.And(letter).Join()
    ///					 select twoInARow)]]></code>
    /// </example>
    /// <returns>An observable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IObservable<TResult> ParseBinary<TResult>(
      this IObservable<byte> source,
      Func<BinaryObservableParserQueryContext<byte, IObservableParser<byte, byte>>,
           BinaryObservableParserQueryContext<byte, IObservableParser<byte, IObservable<TResult>>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      var parser = new InlineBinaryObservableParser<IObservable<TResult>>();
      var proxyParser = (IBinaryObservableParser<byte>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new BinaryObservableParserQueryContext<byte, IObservableParser<byte, byte>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IObservableParser<byte, IObservable<TResult>> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first Rxx.Parsers.Linq.Parser.Parse method for details.
      return parser.Parse(source.ToCursor(forwardOnly: true, enableBranchOptimizations: false), start).Concat();
    }

    /// <summary>
    /// Applies the specified unambiguous parser grammar to generate matches from a sequence of characters.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source characters.</typeparam>
    /// <param name="source">The observable sequence of <see cref="char"/> to parse.</param>
    /// <param name="grammarSelector">A function that receives an object from which a parser cursor can be obtained and returns a grammar
    /// defined as a LINQ <see langword="select"/> query.</param>
    /// <remarks>
    /// The recommended approach to defining an in-line query is to use query comprehension syntax, starting with a <see langword="from"/> 
    /// statement to bind the cursor parser to a query variable named <strong>next</strong>, followed by the definitions of zero or more 
    /// individual grammar rules as <see langword="let"/> statements, and finally the complete grammar as a parser query in terms of the 
    /// previously defined rules projected as a <see langword="select" /> statement.
    /// </remarks>
    /// <example>
    /// The following example illustrates the recommended approach to defining an in-line grammar.
    /// <code><![CDATA[source.ParseString(parser =>
    ///		// get the cursor
    /// 	from next in parser
    /// 	// define the grammar rules in terms of the cursor or specialized parser functions
    /// 	// that use the cursor
    /// 	let letter = parser.Character(char.IsLetter)
    /// 	let number = parser.Character(char.IsNumber)
    /// 	// define the grammar in terms of the rules
    /// 	select from _ in number
    ///					 from twoInARow in letter.And(letter).Join()
    ///					 select twoInARow)]]></code>
    /// </example>
    /// <returns>An observable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IObservable<TResult> ParseString<TResult>(
      this IObservable<char> source,
      Func<StringObservableParserQueryContext<char, IObservableParser<char, char>>,
           StringObservableParserQueryContext<char, IObservableParser<char, TResult>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      var parser = new InlineStringObservableParser<TResult>();
      var proxyParser = (IStringObservableParser<char>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new StringObservableParserQueryContext<char, IObservableParser<char, char>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IObservableParser<char, TResult> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first interactive Rxx.Parsers.Linq.Parser.Parse method for details.
      return parser.Parse(source.ToCursor(forwardOnly: true, enableBranchOptimizations: false), start);
    }

    /// <summary>
    /// Applies the specified ambiguous parser grammar to generate matches from a sequence of characters.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source characters.</typeparam>
    /// <param name="source">The sequence of <see cref="char"/> to parse.</param>
    /// <param name="grammarSelector">A function that receives an object from which a parser cursor can be obtained and returns a grammar
    /// defined as a LINQ <see langword="select"/> query.</param>
    /// <remarks>
    /// The recommended approach to defining an in-line query is to use query comprehension syntax, starting with a <see langword="from"/> 
    /// statement to bind the cursor parser to a query variable named <strong>next</strong>, followed by the definitions of zero or more 
    /// individual grammar rules as <see langword="let"/> statements, and finally the complete grammar as a parser query in terms of the 
    /// previously defined rules projected as a <see langword="select" /> statement.
    /// </remarks>
    /// <example>
    /// The following example illustrates the recommended approach to defining an in-line grammar.
    /// <code><![CDATA[source.ParseString(parser =>
    ///		// get the cursor
    /// 	from next in parser
    /// 	// define the grammar rules in terms of the cursor or specialized parser functions
    /// 	// that use the cursor
    /// 	let letter = parser.Character(char.IsLetter)
    /// 	let number = parser.Character(char.IsNumber)
    /// 	// define the grammar in terms of the rules
    /// 	select from _ in number
    ///					 from twoInARow in letter.And(letter).Join()
    ///					 select twoInARow)]]></code>
    /// </example>
    /// <returns>An observable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IObservable<TResult> ParseString<TResult>(
      this IObservable<char> source,
      Func<StringObservableParserQueryContext<char, IObservableParser<char, char>>,
           StringObservableParserQueryContext<char, IObservableParser<char, IObservable<TResult>>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      var parser = new InlineStringObservableParser<IObservable<TResult>>();
      var proxyParser = (IStringObservableParser<char>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new StringObservableParserQueryContext<char, IObservableParser<char, char>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IObservableParser<char, IObservable<TResult>> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first Rxx.Parsers.Linq.Parser.Parse method for details.
      return parser.Parse(source.ToCursor(forwardOnly: true, enableBranchOptimizations: false), start).Concat();
    }

    /// <summary>
    /// Applies the specified unambiguous XML parser grammar to generate matches.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source XML nodes.</typeparam>
    /// <param name="source">The sequence of <see cref="char"/> to parse.</param>
    /// <param name="grammarSelector">A function that receives an object from which a parser cursor can be obtained and returns a grammar
    /// defined as a LINQ <see langword="select"/> query.</param>
    /// <remarks>
    /// The recommended approach to defining an in-line query is to use query comprehension syntax, starting with a <see langword="from"/> 
    /// statement to bind the cursor parser to a query variable named <strong>next</strong>, followed by the definitions of zero or more 
    /// individual grammar rules as <see langword="let"/> statements, and finally the complete grammar as a parser query in terms of the 
    /// previously defined rules projected as a <see langword="select" /> statement.
    /// </remarks>
    /// <example>
    /// The following example illustrates the recommended approach to defining an in-line grammar.
    /// <code><![CDATA[source.ParseXml(parser =>
    ///		// get the cursor
    /// 	from next in parser
    /// 	// define the grammar rules in terms of the cursor or specialized parser functions
    /// 	// that use the cursor
    /// 	let widget = parser.Element("widget")
    /// 	// define the grammar in terms of the rules
    /// 	select parser.Element("products", widget.OneOrMore()))]]></code>
    /// </example>
    /// <returns>An observable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IObservable<TResult> ParseXml<TResult>(
      this IObservable<char> source,
      Func<XmlObservableParserQueryContext<char, IObservableParser<char, char>>,
           XmlObservableParserQueryContext<char, IObservableParser<char, TResult>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      var parser = new InlineXmlObservableParser<TResult>();
      var proxyParser = (IXmlObservableParser<char>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new XmlObservableParserQueryContext<char, IObservableParser<char, char>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IObservableParser<char, TResult> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first interactive Rxx.Parsers.Linq.Parser.Parse method for details.
      return parser.Parse(source.ToCursor(forwardOnly: true, enableBranchOptimizations: false), start);
    }

    /// <summary>
    /// Applies the specified ambiguous XML parser grammar to generate matches.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source XML nodes.</typeparam>
    /// <param name="source">The sequence of <see cref="char"/> to parse.</param>
    /// <param name="grammarSelector">A function that receives an object from which a parser cursor can be obtained and returns a grammar
    /// defined as a LINQ <see langword="select"/> query.</param>
    /// <remarks>
    /// The recommended approach to defining an in-line query is to use query comprehension syntax, starting with a <see langword="from"/> 
    /// statement to bind the cursor parser to a query variable named <strong>next</strong>, followed by the definitions of zero or more 
    /// individual grammar rules as <see langword="let"/> statements, and finally the complete grammar as a parser query in terms of the 
    /// previously defined rules projected as a <see langword="select" /> statement.
    /// </remarks>
    /// <example>
    /// The following example illustrates the recommended approach to defining an in-line grammar.
    /// <code><![CDATA[source.ParseXml(parser =>
    ///		// get the cursor
    /// 	from next in parser
    /// 	// define the grammar rules in terms of the cursor or specialized parser functions
    /// 	// that use the cursor
    /// 	let widget = parser.Element("widget")
    /// 	// define the grammar in terms of the rules
    /// 	select parser.Element("products", widget.OneOrMore()))]]></code>
    /// </example>
    /// <returns>An observable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IObservable<TResult> ParseXml<TResult>(
      this IObservable<char> source,
      Func<XmlObservableParserQueryContext<char, IObservableParser<char, char>>,
           XmlObservableParserQueryContext<char, IObservableParser<char, IObservable<TResult>>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      var parser = new InlineXmlObservableParser<IObservable<TResult>>();
      var proxyParser = (IXmlObservableParser<char>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new XmlObservableParserQueryContext<char, IObservableParser<char, char>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IObservableParser<char, IObservable<TResult>> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first interactive Rxx.Parsers.Linq.Parser.Parse method for details.
      return parser.Parse(source.ToCursor(forwardOnly: true, enableBranchOptimizations: false), start).Concat();
    }

    /// <summary>
    /// Enables defining in-line parser grammars using LINQ.
    /// </summary>
    /// <typeparam name="TParseSource">The type of the original source elements.</typeparam>
    /// <typeparam name="TParseResult">The type of the elements that are originally generated from parsing the source elements.</typeparam>
    /// <typeparam name="TSource">The type of the source elements; typically, this will be an anonymous compiler-generated type.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="source">The parser query context to be projected.</param>
    /// <param name="selector">A function that projects the current result of the query context.</param>
    /// <returns>A new query context that is the projection of the specified query context using the specified <paramref name="selector"/>.</returns>
    public static ObservableParserQueryContext<TParseSource, TParseResult, TResult> Select<TParseSource, TParseResult, TSource, TResult>(
      this ObservableParserQueryContext<TParseSource, TParseResult, TSource> source,
      Func<TSource, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ObservableParserQueryContext<TParseSource, TParseResult, TResult>>() != null);

      return new ObservableParserQueryContext<TParseSource, TParseResult, TResult>(
        source.Parser,
        selector(source.Value));
    }

    /// <summary>
    /// Enables defining in-line parser grammars using LINQ.
    /// </summary>
    /// <typeparam name="TParseResult">The type of the elements that are originally generated from parsing the source elements.</typeparam>
    /// <typeparam name="TSource">The type of the source elements; typically, this will be an anonymous compiler-generated type.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="source">The parser query context to be projected.</param>
    /// <param name="selector">A function that projects the current result of the query context.</param>
    /// <returns>A new query context that is the projection of the specified query context using the specified <paramref name="selector"/>.</returns>
    public static BinaryObservableParserQueryContext<TParseResult, TResult> Select<TParseResult, TSource, TResult>(
      this BinaryObservableParserQueryContext<TParseResult, TSource> source,
      Func<TSource, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<BinaryObservableParserQueryContext<TParseResult, TResult>>() != null);

      return new BinaryObservableParserQueryContext<TParseResult, TResult>(
        source.Parser,
        selector(source.Value));
    }

    /// <summary>
    /// Enables defining in-line parser grammars using LINQ.
    /// </summary>
    /// <typeparam name="TParseResult">The type of the elements that are originally generated from parsing the source elements.</typeparam>
    /// <typeparam name="TSource">The type of the source elements; typically, this will be an anonymous compiler-generated type.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="source">The parser query context to be projected.</param>
    /// <param name="selector">A function that projects the current result of the query context.</param>
    /// <returns>A new query context that is the projection of the specified query context using the specified <paramref name="selector"/>.</returns>
    public static StringObservableParserQueryContext<TParseResult, TResult> Select<TParseResult, TSource, TResult>(
      this StringObservableParserQueryContext<TParseResult, TSource> source,
      Func<TSource, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<StringObservableParserQueryContext<TParseResult, TResult>>() != null);

      return new StringObservableParserQueryContext<TParseResult, TResult>(
        source.Parser,
        selector(source.Value));
    }

    /// <summary>
    /// Enables defining in-line parser grammars using LINQ.
    /// </summary>
    /// <typeparam name="TParseResult">The type of the elements that are originally generated from parsing the source elements.</typeparam>
    /// <typeparam name="TSource">The type of the source elements; typically, this will be an anonymous compiler-generated type.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="source">The parser query context to be projected.</param>
    /// <param name="selector">A function that projects the current result of the query context.</param>
    /// <returns>A new query context that is the projection of the specified query context using the specified <paramref name="selector"/>.</returns>
    public static XmlObservableParserQueryContext<TParseResult, TResult> Select<TParseResult, TSource, TResult>(
      this XmlObservableParserQueryContext<TParseResult, TSource> source,
      Func<TSource, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<XmlObservableParserQueryContext<TParseResult, TResult>>() != null);

      return new XmlObservableParserQueryContext<TParseResult, TResult>(
        source.Parser,
        selector(source.Value));
    }
  }
}