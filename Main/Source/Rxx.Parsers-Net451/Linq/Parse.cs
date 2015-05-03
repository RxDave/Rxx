using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers.Linq
{
  public static partial class Parser
  {
    /// <summary>
    /// Applies the specified <paramref name="parser"/> to generate matches.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="source">The enumerable sequence to parse.</param>
    /// <param name="parser">An object that defines a grammar to be applied to the enumerable sequence to generate matches.</param>
    /// <returns>An enumerable sequence of matches.</returns>
    public static IEnumerable<TResult> Parse<TSource, TResult>(
      this IEnumerable<TSource> source,
      IParser<TSource, TResult> parser)
    {
      Contract.Requires(source != null);
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      /* "Branch optimization" is disabled in parsers because it decreases overall performance without freeing memory any earlier.
       * The entire grammar must complete parsing before the buffer can be truncated.  Memory is only freed between parsing of the
       * grammar on the same sequence, repeatedly, regardless of whether the optimization is enabled or not.  The optimization checks
       * whether truncation is possible whenever any branch is moved or disposed, which is entirely wasteful.  It will never happen 
       * while the grammar is parsing since the base cursor is not moved until the grammar has completed.  Well-behaved parsers ensure 
       * that when they have completed, all branches they've created have already been disposed, and so the buffer can be truncated
       * whenever the base cursor is moved.  Note that a forward-only cursor is required for automatic truncation.
       */
      using (var cursor = source.ToCursor(forwardOnly: true, enableBranchOptimizations: false))
      {
        foreach (var result in parser.Parse(cursor).Select(result => result.Value))
        {
          yield return result;
        }
      }
    }

    /// <summary>
    /// Applies the specified unambiguous parser grammar to generate matches.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="source">The enumerable sequence to parse.</param>
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
    /// <returns>An enumerable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IEnumerable<TResult> Parse<TSource, TResult>(
      this IEnumerable<TSource> source,
      Func<ParserQueryContext<TSource, TSource, IParser<TSource, TSource>>,
           ParserQueryContext<TSource, TSource, IParser<TSource, TResult>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      var parser = new InlineParser<TSource, TResult>();
      var proxyParser = (IParser<TSource, TSource>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new ParserQueryContext<TSource, TSource, IParser<TSource, TSource>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IParser<TSource, TResult> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first Parse extension for details.
      using (var cursor = source.ToCursor(forwardOnly: true, enableBranchOptimizations: false))
      {
        foreach (var result in parser.Parse(cursor, start))
        {
          yield return result;
        }
      }
    }

    /// <summary>
    /// Applies the specified ambiguous parser grammar to generate matches.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="source">The enumerable sequence to parse.</param>
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
    /// <returns>An enumerable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IEnumerable<TResult> Parse<TSource, TResult>(
      this IEnumerable<TSource> source,
      Func<ParserQueryContext<TSource, TSource, IParser<TSource, TSource>>,
           ParserQueryContext<TSource, TSource, IParser<TSource, IEnumerable<TResult>>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      var parser = new InlineParser<TSource, IEnumerable<TResult>>();
      var proxyParser = (IParser<TSource, TSource>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new ParserQueryContext<TSource, TSource, IParser<TSource, TSource>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IParser<TSource, IEnumerable<TResult>> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first Parse extension for details.
      using (var cursor = source.ToCursor(forwardOnly: true, enableBranchOptimizations: false))
      {
        foreach (var result in parser.Parse(cursor, start).Concat())
        {
          yield return result;
        }
      }
    }

    /// <summary>
    /// Applies the specified unambiguous binary parser grammar to generate matches.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source bytes.</typeparam>
    /// <param name="source">The enumerable sequence of bytes to parse.</param>
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
    /// <code><![CDATA[source.ParseBinary(parser =>
    ///		// get the cursor
    /// 	from next in parser
    /// 	// define the grammar rules in terms of the cursor or specialized parser functions
    /// 	// that use the cursor
    /// 	let magicNumber = parser.String(Encoding.ASCII, 8)
    /// 	let headerSize = parser.Int32
    /// 	// define the grammar in terms of the rules
    /// 	select from _ in magicNumber
    ///					 from length in headerSize
    ///					 from header in next.Exactly(length)
    ///					 from body in next.AtLeast(0)
    ///					 select new { Header = header.ToArray(), Body = body.ToArray() })]]></code>
    /// </example>
    /// <returns>An enumerable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IEnumerable<TResult> ParseBinary<TResult>(
      this IEnumerable<byte> source,
      Func<BinaryParserQueryContext<byte, IParser<byte, byte>>,
           BinaryParserQueryContext<byte, IParser<byte, TResult>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      var parser = new InlineBinaryParser<TResult>();
      var proxyParser = (IBinaryParser<byte>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new BinaryParserQueryContext<byte, IParser<byte, byte>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IParser<byte, TResult> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first Parse extension for details.
      using (var cursor = source.ToCursor(forwardOnly: true, enableBranchOptimizations: false))
      {
        foreach (var result in parser.Parse(cursor, start))
        {
          yield return result;
        }
      }
    }

    /// <summary>
    /// Applies the specified ambiguous binary parser grammar to generate matches.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source bytes.</typeparam>
    /// <param name="source">The enumerable sequence of bytes to parse.</param>
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
    /// <code><![CDATA[source.ParseBinary(parser =>
    ///		// get the cursor
    /// 	from next in parser
    /// 	// define the grammar rules in terms of the cursor or specialized parser functions
    /// 	// that use the cursor
    /// 	let magicNumber = parser.String(Encoding.ASCII, 8)
    /// 	let headerSize = parser.Int32
    /// 	// define the grammar in terms of the rules
    /// 	select from _ in magicNumber
    ///					 from length in headerSize
    ///					 from header in next.Exactly(length)
    ///					 from body in next.AtLeast(0)
    ///					 select new { Header = header.ToArray(), Body = body.ToArray() })]]></code>
    /// </example>
    /// <returns>An enumerable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IEnumerable<TResult> ParseBinary<TResult>(
      this IEnumerable<byte> source,
      Func<BinaryParserQueryContext<byte, IParser<byte, byte>>,
           BinaryParserQueryContext<byte, IParser<byte, IEnumerable<TResult>>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      var parser = new InlineBinaryParser<IEnumerable<TResult>>();
      var proxyParser = (IBinaryParser<byte>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new BinaryParserQueryContext<byte, IParser<byte, byte>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IParser<byte, IEnumerable<TResult>> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first Parse extension for details.
      using (var cursor = source.ToCursor(forwardOnly: true, enableBranchOptimizations: false))
      {
        foreach (var result in parser.Parse(cursor, start).Concat())
        {
          yield return result;
        }
      }
    }

    /// <summary>
    /// Applies the specified unambiguous <see cref="string"/> parser grammar to generate matches.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source characters.</typeparam>
    /// <param name="source">The <see cref="string"/> or enumerable sequence of <see cref="char"/> to parse.</param>
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
    /// <returns>An enumerable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IEnumerable<TResult> ParseString<TResult>(
      this IEnumerable<char> source,
      Func<StringParserQueryContext<char, IParser<char, char>>,
           StringParserQueryContext<char, IParser<char, TResult>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      var parser = new InlineStringParser<TResult>();
      var proxyParser = (IStringParser<char>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new StringParserQueryContext<char, IParser<char, char>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IParser<char, TResult> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first Parse extension for details.
      using (var cursor = source.ToCursor(forwardOnly: true, enableBranchOptimizations: false))
      {
        foreach (var result in parser.Parse(cursor, start))
        {
          yield return result;
        }
      }
    }

    /// <summary>
    /// Applies the specified ambiguous <see cref="string"/> parser grammar to generate matches.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source characters.</typeparam>
    /// <param name="source">The <see cref="string"/> or enumerable sequence of <see cref="char"/> to parse.</param>
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
    /// <returns>An enumerable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IEnumerable<TResult> ParseString<TResult>(
      this IEnumerable<char> source,
      Func<StringParserQueryContext<char, IParser<char, char>>,
           StringParserQueryContext<char, IParser<char, IEnumerable<TResult>>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      var parser = new InlineStringParser<IEnumerable<TResult>>();
      var proxyParser = (IStringParser<char>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new StringParserQueryContext<char, IParser<char, char>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IParser<char, IEnumerable<TResult>> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first Parse extension for details.
      using (var cursor = source.ToCursor(forwardOnly: true, enableBranchOptimizations: false))
      {
        foreach (var result in parser.Parse(cursor, start).Concat())
        {
          yield return result;
        }
      }
    }

    /// <summary>
    /// Applies the specified unambiguous XML parser grammar to generate matches.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source XML nodes.</typeparam>
    /// <param name="source">The <see cref="string"/> or enumerable sequence of <see cref="char"/> to parse.</param>
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
    /// <returns>An enumerable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IEnumerable<TResult> ParseXml<TResult>(
      this IEnumerable<char> source,
      Func<XmlParserQueryContext<char, IParser<char, char>>,
           XmlParserQueryContext<char, IParser<char, TResult>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      var parser = new InlineXmlParser<TResult>();
      var proxyParser = (IXmlParser<char>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new XmlParserQueryContext<char, IParser<char, char>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IParser<char, TResult> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first Parse extension for details.
      using (var cursor = source.ToCursor(forwardOnly: true, enableBranchOptimizations: false))
      {
        foreach (var result in parser.Parse(cursor, start))
        {
          yield return result;
        }
      }
    }

    /// <summary>
    /// Applies the specified ambiguous XML parser grammar to generate matches.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source XML nodes.</typeparam>
    /// <param name="source">The <see cref="string"/> or enumerable sequence of <see cref="char"/> to parse.</param>
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
    /// <returns>An enumerable sequence of matches.</returns>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Readability of nested generic types.")]
    public static IEnumerable<TResult> ParseXml<TResult>(
      this IEnumerable<char> source,
      Func<XmlParserQueryContext<char, IParser<char, char>>,
           XmlParserQueryContext<char, IParser<char, IEnumerable<TResult>>>> grammarSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(grammarSelector != null);
      Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);

      var parser = new InlineXmlParser<IEnumerable<TResult>>();
      var proxyParser = (IXmlParser<char>)parser;

      // The proxy allows the grammar author to use base methods such as Success and Failure
      // while still allowing type inference to work correctly; i.e., the Parse method is unavailable
      // and thus the type of the proxy is based solely on TSource, without requiring TResult to  
      // be in an input position in grammarSelector.
      var context = new XmlParserQueryContext<char, IParser<char, char>>(
        proxyParser,
        parser.Next);

      var grammar = grammarSelector(context);

      Contract.Assume(grammar != null);

      IParser<char, IEnumerable<TResult>> start = grammar.Value;

      Contract.Assume(start != null);

      // enableBranchOptimizations must be false: See the comments in the first Parse extension for details.
      using (var cursor = source.ToCursor(forwardOnly: true, enableBranchOptimizations: false))
      {
        foreach (var result in parser.Parse(cursor, start).Concat())
        {
          yield return result;
        }
      }
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
    public static ParserQueryContext<TParseSource, TParseResult, TResult> Select<TParseSource, TParseResult, TSource, TResult>(
      this ParserQueryContext<TParseSource, TParseResult, TSource> source,
      Func<TSource, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<ParserQueryContext<TParseSource, TParseResult, TResult>>() != null);

      return new ParserQueryContext<TParseSource, TParseResult, TResult>(
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
    public static BinaryParserQueryContext<TParseResult, TResult> Select<TParseResult, TSource, TResult>(
      this BinaryParserQueryContext<TParseResult, TSource> source,
      Func<TSource, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<BinaryParserQueryContext<TParseResult, TResult>>() != null);

      return new BinaryParserQueryContext<TParseResult, TResult>(
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
    public static StringParserQueryContext<TParseResult, TResult> Select<TParseResult, TSource, TResult>(
      this StringParserQueryContext<TParseResult, TSource> source,
      Func<TSource, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<StringParserQueryContext<TParseResult, TResult>>() != null);

      return new StringParserQueryContext<TParseResult, TResult>(
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
    public static XmlParserQueryContext<TParseResult, TResult> Select<TParseResult, TSource, TResult>(
      this XmlParserQueryContext<TParseResult, TSource> source,
      Func<TSource, TResult> selector)
    {
      Contract.Requires(source != null);
      Contract.Requires(selector != null);
      Contract.Ensures(Contract.Result<XmlParserQueryContext<TParseResult, TResult>>() != null);

      return new XmlParserQueryContext<TParseResult, TResult>(
        source.Parser,
        selector(source.Value));
    }
  }
}