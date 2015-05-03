using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers.Linq
{
  public static partial class Parser
  {
    /// <summary>
    /// Matches the <paramref name="content"/> between the specified <paramref name="open"/> and <paramref name="close"/> parsers.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TOpen">The type of the elements that are generated from parsing the <paramref name="open"/> elements.</typeparam>
    /// <typeparam name="TClose">The type of the elements that are generated from parsing the <paramref name="close"/> elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the <paramref name="content"/> elements.</typeparam>
    /// <param name="open">The parser after which the matching of <paramref name="content"/> begins.</param>
    /// <param name="content">The parser that matches values between the <paramref name="open"/> and <paramref name="close"/> parsers.</param>
    /// <param name="close">The parser at which the matching of <paramref name="content"/> ends.</param>
    /// <returns>A parser with a grammar that matches the <paramref name="open"/> parser, followed by the <paramref name="content"/> parser
    /// and finally the <paramref name="close"/> parser, yielding the results of the <paramref name="content"/> parser only.</returns>
    public static IParser<TSource, TResult> Group<TSource, TOpen, TClose, TResult>(
      this IParser<TSource, TOpen> open,
      IParser<TSource, TResult> content,
      IParser<TSource, TClose> close)
    {
      Contract.Requires(open != null);
      Contract.Requires(content != null);
      Contract.Requires(close != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return from _ in open
             from result in content
             from __ in close
             select result;
    }

    /// <summary>
    /// Matches zero or more values in between the specified <paramref name="open"/> and <paramref name="close"/> parsers.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TOpen">The type of the elements that are generated from parsing the <paramref name="open"/> elements.</typeparam>
    /// <typeparam name="TClose">The type of the elements that are generated from parsing the <paramref name="close"/> elements.</typeparam>
    /// <param name="open">The parser after which the group begins.</param>
    /// <param name="close">The parser at which the group ends.</param>
    /// <returns>A parser with a grammar that matches the <paramref name="open"/> parser, followed by everything up to the first 
    /// match of the <paramref name="close"/> parser, yielding the results in between.</returns>
    public static IParser<TSource, IEnumerable<TSource>> Group<TSource, TOpen, TClose>(
      this IParser<TSource, TOpen> open,
      IParser<TSource, TClose> close)
    {
      Contract.Requires(open != null);
      Contract.Requires(close != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TSource>>>() != null);

      return open.Group(open.Next.Not(close).NoneOrMore(), close);
    }

    /// <summary>
    /// Matches everything in between the specified <paramref name="open"/> and <paramref name="close"/> parsers, 
    /// yielding the first unambiguous match as well as everything in between any sub-groups and overlapping groups, 
    /// extending past the unambiguous match of the <paramref name="close"/> parser, that match the same grammar.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <param name="open">The parser after which the group begins.</param>
    /// <param name="close">The parser at which the group ends.</param>
    /// <remarks>
    /// The same <paramref name="open"/> or <paramref name="close"/> parser may produce multiple matches at the same index.
    /// </remarks>
    /// <returns>A parser with a grammar that matches the <paramref name="open"/> parser, followed by everything up to the first 
    /// match of the <paramref name="close"/> parser, yielding the results in between as well as the results of all ambiguous 
    /// matches of the group grammar.</returns>
    public static IParser<TSource, IEnumerable<TSource>> AmbiguousGroup<TSource>(
      this IParser<TSource, TSource> open,
      IParser<TSource, TSource> close)
    {
      Contract.Requires(open != null);
      Contract.Requires(close != null);
      Contract.Ensures(Contract.Result<IParser<TSource, IEnumerable<TSource>>>() != null);

      return open.Yield("AmbiguousGroup", AmbiguousGroupIterator, close);
    }

    private static IEnumerable<IParseResult<IEnumerable<TSource>>> AmbiguousGroupIterator<TSource>(
      ICursor<TSource> source,
      IParser<TSource, TSource> open,
      IParser<TSource, TSource> close)
    {
      Contract.Assert(source != null);
      Contract.Assert(open != null);
      Contract.Assert(close != null);

      foreach (var openResult in open.Parse(source))
      {
        int openCount = 1;

        var openSinks = new List<Action<IParseResult<TSource>>>();
        var closeSinks = new List<Action<IParseResult<TSource>>>();
        var contentSinks = new List<Action<IParseResult<TSource>>>();
        var bufferedResults = new List<IEnumerable<IParseResult<IEnumerable<TSource>>>>();

        Func<List<IParseResult<IEnumerable<TSource>>>> addBuffer = () =>
        {
          var buffer = new List<IParseResult<IEnumerable<TSource>>>();
          bufferedResults.Add(buffer);
          return buffer;
        };

        Action installSinks = () =>
        {
          var buffer = addBuffer();
          var content = new List<TSource>();

          openSinks.Add(result => content.Add(result.Value));
          contentSinks.Add(result => content.Add(result.Value));
          closeSinks.Add(result =>
          {
            // copy the content list to create a new branch
            var branch = result.Yield(new List<TSource>(content).AsReadOnly());

            buffer.Add(branch);

            if (openCount > 0)
              content.Add(result.Value);
          });
        };

        // base sinks must be installed first - openCount must be incremented before other sinks are executed
        openSinks.Add(_ => { openCount++; installSinks(); });
        closeSinks.Add(_ => { openCount--; });

        // now we can install the sinks for the first open (matched in the foreach above)
        installSinks();

        using (var branch = source.Remainder(openResult.Length))
        {
          AmbiguousGroupRun(ref openCount, branch, open, close, openSinks, closeSinks, contentSinks);
        }

        if (openCount == 0)
        {
          foreach (var result in bufferedResults.SelectMany(r => r))
            yield return result;
        }
      }
    }

#if SILVERLIGHT
		[ContractVerification(false)]		// Static checker is timing out
#endif
    private static void AmbiguousGroupRun<TSource>(
      ref int openCount,
      ICursor<TSource> source,
      IParser<TSource, TSource> open,
      IParser<TSource, TSource> close,
      List<Action<IParseResult<TSource>>> openSinks,
      List<Action<IParseResult<TSource>>> closeSinks,
      List<Action<IParseResult<TSource>>> contentSinks)
    {
      Contract.Requires(source != null);
      Contract.Requires(source.IsForwardOnly);
      Contract.Requires(open != null);
      Contract.Requires(close != null);
      Contract.Requires(openSinks != null);
      Contract.Requires(closeSinks != null);
      Contract.Requires(contentSinks != null);

      bool sourceCompleted;
      IParser<TSource, TSource> current = open;

    Loop:
      do
      {
        Contract.Assume(source.IsForwardOnly);

        var results = open.Parse(source);

        foreach (var openResult in results)
        {
          Contract.Assume(openResult != null);

          var clone = openSinks.ToList();

          // the sinks list is modified when open is matched, so we must run a clone
          clone.ForEach(sink => sink(openResult));

          Contract.Assume(openResult.Length == 0 || !source.IsSequenceTerminated);
          Contract.Assert(!source.IsSequenceTerminated || (source.LatestIndex + 1) - source.CurrentIndex >= 0);

          source = source.Remainder(openResult.Length);

          current = open;

          // TODO: Support nested open ambiguity
          goto Loop;
        }

        Contract.Assume(source.IsForwardOnly);

        results = close.Parse(source);

        foreach (var closeResult in results)
        {
          Contract.Assume(closeResult != null);

          closeSinks.ForEach(sink => sink(closeResult));

          Contract.Assume(closeResult.Length == 0 || !source.IsSequenceTerminated);
          Contract.Assert(!source.IsSequenceTerminated || (source.LatestIndex + 1) - source.CurrentIndex >= 0);

          source = source.Remainder(closeResult.Length);

          current = close;

          // TODO: Support nested close ambiguity
          goto Loop;
        }

        Contract.Assume(source.IsForwardOnly);

        sourceCompleted = true;
        results = current.Next.Parse(source);

        foreach (var content in results)
        {
          Contract.Assume(content != null);

          sourceCompleted = false;

          contentSinks.ForEach(sink => sink(content));

          Contract.Assume(content.Length == 0 || !source.IsSequenceTerminated);
          Contract.Assert(!source.IsSequenceTerminated || (source.LatestIndex + 1) - source.CurrentIndex >= 0);

          source = source.Remainder(content.Length);
        }
      }
      while (openCount > 0 && !sourceCompleted);
    }
  }
}