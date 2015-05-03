using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Rxx.Parsers.Reactive.Linq
{
  public static partial class ObservableParser
  {
    /// <summary>
    /// Matches the specified <paramref name="parser"/> or yields the specified default result if there are 
    /// no matches.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that might produce matches.</param>
    /// <param name="defaultResult">The value that is yielded if the specified <paramref name="parser"/> does not match.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> or the specified default result
    /// if the <paramref name="parser"/> does not match.</returns>
    public static IObservableParser<TSource, TResult> WithDefault<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser,
      TResult defaultResult)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

      return parser.Yield(
        "WithDefault",
        (source, observer) =>
        {
          bool hasResult = false;

          return parser.Parse(source).SubscribeSafe(
            result =>
            {
              hasResult = true;

              observer.OnNext(result);
            },
            observer.OnError,
            () =>
            {
              if (!hasResult)
              {
                observer.OnNext(ParseResult.Create(defaultResult, length: 0));
              }

              observer.OnCompleted();
            });
        });
    }

    /// <summary>
    /// Yields success when the specified <paramref name="parser"/> does not match.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser for which any match results in failure.</param>
    /// <returns>A parser that yields failure when the specified <paramref name="parser"/> matches or 
    /// an empty sequence to indicate success when it does not match.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> None<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      if (parser is IObservableParserCursor<TSource>)
      {
        return parser.AtEndOfSequence();
      }
      else
      {
        return parser.Yield<TSource, TResult, IObservable<TResult>>(
          "None",
          (source, observer) =>
          {
            return parser.Parse(source).Any().SubscribeSafe(
              any =>
              {
                if (!any)
                {
                  observer.OnNext(ObservableParseResult.SuccessMany<TResult>(length: 0));
                }
              },
              observer.OnError,
              observer.OnCompleted);
          });
      }
    }

    /// <summary>
    /// Yields success when the specified <paramref name="parser"/> does not match.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="parser">The parser for which any match results in failure.</param>
    /// <param name="successResult">The value that is yielded if the specified <paramref name="parser"/> does not match.</param>
    /// <returns>A parser that yields failure when the specified <paramref name="parser"/> matches or success when 
    /// it does not match.</returns>
    public static IObservableParser<TSource, TSuccess> None<TSource, TResult, TSuccess>(
      this IObservableParser<TSource, TResult> parser,
      TSuccess successResult)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TSuccess>>() != null);

      if (parser is IObservableParserCursor<TSource>)
      {
        return parser.AtEndOfSequence(successResult);
      }
      else
      {
        return parser.Yield<TSource, TResult, TSuccess>(
          "None",
          (source, observer) =>
          {
            return parser.Parse(source).Any().SubscribeSafe(
              any =>
              {
                if (!any)
                {
                  observer.OnNext(ParseResult.Create(successResult, length: 0));
                }
              },
              observer.OnError,
              observer.OnCompleted);
          });
      }
    }

    /// <summary>
    /// Yields success if the specified parser starts at the end of the input sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that provides the context in which to check whether the cursor is at the end of the input sequence.</param>
    /// <returns>A new parser that yields success without parsing if the cursor is positioned at the end of the input sequence; otherwise, yields no results.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> AtEndOfSequence<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.Yield<TSource, TResult, IObservable<TResult>>(
        "AtEndOfSequence",
        (source, observer) =>
        {
          IDisposable disposable;

          if (source.IsSequenceTerminated)
          {
            if (source.AtEndOfSequence)
            {
              observer.OnNext(ObservableParseResult.SuccessMany<TResult>(length: 0));
            }

            observer.OnCompleted();

            disposable = Disposable.Empty;
          }
          else
          {
            bool hasResult = false;

            disposable = source.Subscribe(
              Observer.Create<TSource>(
                result => hasResult = true,
                observer.OnError,
                () =>
                {
                  if (!hasResult)
                  {
                    Contract.Assume(source.AtEndOfSequence);

                    observer.OnNext(ObservableParseResult.SuccessMany<TResult>(length: 0));
                  }

                  observer.OnCompleted();
                }),
              count: 1);
          }

          return disposable;
        });
    }

    /// <summary>
    /// Yields success if the specified parser starts at the end of the input sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="parser">The parser that provides the context in which to check whether the cursor is at the end of the input sequence.</param>
    /// <param name="successResult">The value that is yielded if the specified parser starts at the end of the input sequence.</param>
    /// <returns>A new parser that yields success without parsing if the cursor is positioned at the end of the input sequence; otherwise, yields no results.</returns>
    public static IObservableParser<TSource, TSuccess> AtEndOfSequence<TSource, TResult, TSuccess>(
      this IObservableParser<TSource, TResult> parser,
      TSuccess successResult)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TSuccess>>() != null);

      return parser.Yield<TSource, TResult, TSuccess>(
        "AtEndOfSequence",
        (source, observer) =>
        {
          IDisposable disposable;

          if (source.IsSequenceTerminated)
          {
            if (source.AtEndOfSequence)
            {
              observer.OnNext(ParseResult.Create(successResult, length: 0));
            }

            observer.OnCompleted();

            disposable = Disposable.Empty;
          }
          else
          {
            bool hasResult = false;

            disposable = source.Subscribe(
              Observer.Create<TSource>(
                result => hasResult = true,
                observer.OnError,
                () =>
                {
                  if (!hasResult)
                  {
                    Contract.Assume(source.AtEndOfSequence);

                    observer.OnNext(ParseResult.Create(successResult, length: 0));
                  }

                  observer.OnCompleted();
                }),
              count: 1);
          }

          return disposable;
        });
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> or yields success without a value when it does not match.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that might produce matches.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> or
    /// an empty observable sequence to indicate success when it does not match.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> Maybe<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.Yield<TSource, TResult, IObservable<TResult>>(
        "Maybe",
        (source, observer) =>
        {
          bool hasResult = false;

          return parser.Parse(source).SubscribeSafe(
            result =>
            {
              hasResult = true;

              observer.OnNext(result.YieldMany());
            },
            observer.OnError,
            () =>
            {
              if (!hasResult)
              {
                observer.OnNext(ObservableParseResult.SuccessMany<TResult>(length: 0));
              }

              observer.OnCompleted();
            });
        });
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> or yields success without a value when it does not match.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements of the result sequences that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser that might produce matches.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> or
    /// an empty observable sequence to indicate success when it does not match.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> Maybe<TSource, TResult>(
      this IObservableParser<TSource, IObservable<TResult>> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.Yield(
        "Maybe",
        (source, observer) =>
        {
          bool hasResult = false;

          return parser.Parse(source).SubscribeSafe(
            result =>
            {
              hasResult = true;

              observer.OnNext(result);
            },
            observer.OnError,
            () =>
            {
              if (!hasResult)
              {
                observer.OnNext(ObservableParseResult.SuccessMany<TResult>(length: 0));
              }

              observer.OnCompleted();
            });
        });
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> the specified number of times.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to be matched.</param>
    /// <param name="count">The specified number of times to match the specified <paramref name="parser"/>.</param>
    /// <returns>A parser that matches the specified <paramref name="parser"/> the specified number of times.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> Exactly<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser,
      int count)
    {
      Contract.Requires(parser != null);
      Contract.Requires(count >= 0);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      if (count == 0)
      {
        return parser.Yield(_ => ObservableParseResult.ReturnSuccessMany<TResult>(length: 0));
      }
      else if (count == 1)
      {
        // Profiling has shown this to be about 50% faster than Repeat(parser, 1).All()
        return parser.Amplify();
      }
      else if (parser is IObservableParserCursor<TSource>)
      {
        /* Profiling has shown this to be exponentially faster in next.Exactly(largeN) queries for Ix.
         * It hasn't been profiled in Rx, but I'm assuming that for similar reasons as Ix it would prove
         * to be exponentially faster.  Furthermore, due to the extra plumbing in Rx that's being avoided
         * by this optimization, it may have even greater gains than Ix.
         */
        return parser.Yield<TSource, TResult, IObservable<TResult>>(
          "Exactly",
          source =>
            from list in source.Take(count).ToList()
            where list.Count == count
            select ParseResult.Create(list.Cast<TResult>().ToObservable(Scheduler.Immediate), count));
      }
      else
      {
        return System.Linq.Enumerable.Repeat(parser, count).All();
      }
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> zero or more times consecutively.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match zero or more times consecutively.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> zero or more times consecutively.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> NoneOrMore<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.AtLeast<TSource, TResult, TResult>("NoneOrMore", 0);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> zero or more times consecutively, making the least number of matches possible.
    /// This is the non-greedy variant of <see cref="NoneOrMore{TSource,TResult}(IObservableParser{TSource,TResult})"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match zero or more times consecutively.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> zero or more times consecutively, 
    /// making the least number of matches possible.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> NoneOrMoreNonGreedy<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.AtLeast<TSource, TResult, TResult>("NoneOrMore-NonGreedy", 0, nonGreedy: true);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> zero or more times consecutively, 
    /// matching the specified <paramref name="separator"/> in between.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TSeparator">The type of the separator elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match zero or more times consecutively.</param>
    /// <param name="separator">The parser that matches between consecutive matches of the specified <paramref name="parser"/>.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> zero or more times consecutively, 
    /// matching the specified <paramref name="separator"/> in between.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> NoneOrMore<TSource, TSeparator, TResult>(
      this IObservableParser<TSource, TResult> parser,
      IObservableParser<TSource, TSeparator> separator)
    {
      Contract.Requires(parser != null);
      Contract.Requires(separator != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.AtLeast("NoneOrMore-Separated", 0, separator: separator);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> zero or more times consecutively, matching the specified 
    /// <paramref name="separator"/> in between and making the least number of matches possible.
    /// This is the non-greedy variant of <see cref="NoneOrMore{TSource,TSeparator,TResult}(IObservableParser{TSource,TResult},IObservableParser{TSource,TSeparator})"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match zero or more times consecutively.</param>
    /// <param name="separator">The parser that matches between consecutive matches of the specified <paramref name="parser"/>.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> zero or more times consecutively, 
    /// matching the specified <paramref name="separator"/> in between and making the least number of matches possible.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> NoneOrMoreNonGreedy<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser,
      IObservableParser<TSource, TResult> separator)
    {
      Contract.Requires(parser != null);
      Contract.Requires(separator != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.AtLeast("NoneOrMore-Separated-NonGreedy", 0, separator: separator, nonGreedy: true);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> one or more times consecutively.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match one or more times consecutively.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> one or more times consecutively.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> OneOrMore<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.AtLeast<TSource, TResult, TResult>("OneOrMore", 1);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> one or more times consecutively, making the least number of matches possible.
    /// This is the non-greedy variant of <see cref="OneOrMore{TSource,TResult}(IObservableParser{TSource,TResult})"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match one or more times consecutively.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> one or more times consecutively, 
    /// making the least number of matches possible.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> OneOrMoreNonGreedy<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.AtLeast<TSource, TResult, TResult>("OneOrMore-NonGreedy", 1, nonGreedy: true);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> one or more times consecutively, 
    /// matching the specified <paramref name="separator"/> in between.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TSeparator">The type of the separator elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match one or more times consecutively.</param>
    /// <param name="separator">The parser that matches between consecutive matches of the specified <paramref name="parser"/>.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> one or more times consecutively, 
    /// matching the specified <paramref name="separator"/> in between.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> OneOrMore<TSource, TSeparator, TResult>(
      this IObservableParser<TSource, TResult> parser,
      IObservableParser<TSource, TSeparator> separator)
    {
      Contract.Requires(parser != null);
      Contract.Requires(separator != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.AtLeast("OneOrMore-Separated", 1, separator: separator);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> one or more times consecutively, matching the specified 
    /// <paramref name="separator"/> in between and making the least number of matches possible.
    /// This is the non-greedy variant of <see cref="OneOrMore{TSource,TSeparator,TResult}(IObservableParser{TSource,TResult},IObservableParser{TSource,TSeparator})"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to match one or more times consecutively.</param>
    /// <param name="separator">The parser that matches between consecutive matches of the specified <paramref name="parser"/>.</param>
    /// <returns>A parser that yields matches from the specified <paramref name="parser"/> one or more times consecutively, 
    /// matching the specified <paramref name="separator"/> in between and making the least number of matches possible.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> OneOrMoreNonGreedy<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser,
      IObservableParser<TSource, TResult> separator)
    {
      Contract.Requires(parser != null);
      Contract.Requires(separator != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      return parser.AtLeast("OneOrMore-Separated-NonGreedy", 1, separator: separator, nonGreedy: true);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> consecutively a minimum number of times.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to be matched consecutively a minimum number of times.</param>
    /// <param name="count">The minimum number of times to match the specified <paramref name="parser"/> consecutively.</param>
    /// <returns>A parser that consecutively matches the specified <paramref name="parser"/> the minimum 
    /// number of times specified by <paramref name="count"/>.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> AtLeast<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser,
      int count)
    {
      Contract.Requires(parser != null);
      Contract.Requires(count > 0);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      var name = "AtLeast-" + count;

      Contract.Assume(!string.IsNullOrEmpty(name));

      return parser.AtLeast<TSource, TResult, TResult>(name, count);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> consecutively between the specified number of times, inclusive.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to be matched consecutively between the specified number of times.</param>
    /// <param name="count">The minimum number of times to match the specified <paramref name="parser"/> consecutively.</param>
    /// <param name="maximum">The maximum number of times to match the specified <paramref name="parser"/> consecutively.</param>
    /// <returns>A parser that consecutively matches the specified <paramref name="parser"/> between the specified number of 
    /// times, inclusive.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> AtLeast<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser,
      int count,
      int maximum)
    {
      Contract.Requires(parser != null);
      Contract.Requires(count > 0);
      Contract.Requires(maximum >= count);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      if (maximum == count)
      {
        return parser.Exactly(count);
      }
      else
      {
        var name = "AtLeast-" + count + "-to-" + maximum;

        Contract.Assume(!string.IsNullOrEmpty(name));

        return parser.AtLeast<TSource, TResult, TResult>(name, count, maximum);
      }
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> consecutively a minimum number of times, making the least number of matches possible.
    /// This is the non-greedy variant of <see cref="AtLeast{TSource,TResult}(IObservableParser{TSource,TResult},int)"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to be matched consecutively a minimum number of times.</param>
    /// <param name="count">The minimum number of times to match the specified <paramref name="parser"/> consecutively.</param>
    /// <returns>A parser that consecutively matches the specified <paramref name="parser"/> the minimum 
    /// number of times specified by <paramref name="count"/>, making the least number of matches possible.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> AtLeastNonGreedy<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser,
      int count)
    {
      Contract.Requires(parser != null);
      Contract.Requires(count > 0);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      var name = "AtLeast-NonGreedy-" + count;

      Contract.Assume(!string.IsNullOrEmpty(name));

      return parser.AtLeast<TSource, TResult, TResult>(name, count, nonGreedy: true);
    }

    /// <summary>
    /// Matches the specified <paramref name="parser"/> consecutively between the specified number of times, inclusive, 
    /// making the least number of matches possible.
    /// This is the non-greedy variant of <see cref="AtLeast{TSource,TResult}(IObservableParser{TSource,TResult},int,int)"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser to be matched consecutively a minimum number of times.</param>
    /// <param name="count">The minimum number of times to match the specified <paramref name="parser"/> consecutively.</param>
    /// <param name="maximum">The maximum number of times to match the specified <paramref name="parser"/> consecutively.</param>
    /// <returns>A parser that consecutively matches the specified <paramref name="parser"/> between the specified number of 
    /// times, inclusive, making the least number of matches possible.</returns>
    public static IObservableParser<TSource, IObservable<TResult>> AtLeastNonGreedy<TSource, TResult>(
      this IObservableParser<TSource, TResult> parser,
      int count,
      int maximum)
    {
      Contract.Requires(parser != null);
      Contract.Requires(count > 0);
      Contract.Requires(maximum >= count);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      if (maximum == count)
      {
        return parser.Exactly(count);
      }
      else
      {
        var name = "AtLeast-NonGreedy-" + count + "-to-" + maximum;

        Contract.Assume(!string.IsNullOrEmpty(name));

        return parser.AtLeast<TSource, TResult, TResult>(name, count, maximum, nonGreedy: true);
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
      Justification = "I'm suppressing this warning for now because the method has a comment noting that it must be refactored and possibly simplified.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
      Justification = "I'm suppressing this warning for now because the method has a comment noting that it must be refactored and possibly simplified.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Passed to the observer.")]
    private static IObservableParser<TSource, IObservable<TResult>> AtLeast<TSource, TSeparator, TResult>(
      this IObservableParser<TSource, TResult> parser,
      string name,
      int count,
      int maximum = -1,
      IObservableParser<TSource, TSeparator> separator = null,
      bool nonGreedy = false)
    {
      Contract.Requires(parser != null);
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Requires(count >= 0);
      Contract.Requires(maximum == -1 || maximum >= count);
      Contract.Requires(maximum != 0);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TResult>>>() != null);

      // TODO: Update this method to properly support multi-result parsers.
      /* The current implementation just uses Math.Max for the lengths and aggregates all of the results into a single list.
       * The correct behavior is more like a SelectMany query, so consider using the new SelectMany overload that AllObservableParser uses.
       */

      /* This method is optimized to prevent stack overflows due to two factors: recursion and using Skip to move the source cursor.
       * 
       * The previous implementation used recursive calls to NoneOrMore, in which there was a linear relationship between the number 
       * of stack frames and the number of elements in the input sequence.  As an input sequence grew and the parser continued matching
       * elements, the number of calls to the Skip operator (via the Remainder extension) grew linearly, and so did the number of branches 
       * due to NoneOrMore using the Or operator, which not only added the Or operator to the stack but added all of the calls to the 
       * quantified parser between the stack frames that Or added, for every subsequent element in the sequence that the parser matched.
       */
      return parser.Yield<TSource, TResult, IObservable<TResult>>(
        name,
        (source, observer) =>
        {
          Contract.Requires(source.IsForwardOnly);

          var branch = source.Branch();

          var list = new List<TResult>();

          int total = 0;
          int totalLength = 0;

          Action onCompleted = () =>
            {
              if (total >= count)
              {
                observer.OnNext(new ParseResult<IObservable<TResult>>(list.ToObservable(Scheduler.Immediate), totalLength));
              }

              observer.OnCompleted();
            };

          var subscription = new SerialDisposable();

          Func<IDisposable> start = () =>
            Scheduler.Immediate.Schedule(self =>
            {
              bool hasResult = false;
              bool hasSeparatorResult = false;

              int length = 0;
              int separatorLength = 0;

              Action currentCompleted = () =>
                {
                  if (hasResult)
                  {
                    totalLength += length + separatorLength;

                    if (total < (maximum == -1 ? count : maximum))
                    {
                      total++;
                    }

                    if (total != maximum && (separator == null || hasSeparatorResult))
                    {
                      if (nonGreedy && total >= count)
                      {
                        var lookAhead = new LookAheadParseResult<IObservable<TResult>>(list.ToObservable(Scheduler.Immediate), totalLength);

                        subscription.SetDisposableIndirectly(() => new CompositeDisposable(
                          lookAhead,
                          lookAhead.Subscribe(success =>
                          {
                            if (success)
                            {
                              onCompleted();
                            }
                            else
                            {
                              self();
                            }
                          })));

                        observer.OnNext(lookAhead);
                        return;
                      }
                      else
                      {
                        self();
                        return;
                      }
                    }
                  }

                  onCompleted();
                };

              subscription.SetDisposableIndirectly(() =>
                parser.Parse(branch).SubscribeSafe(
                  result =>
                  {
                    hasResult = true;
                    length = Math.Max(length, result.Length);

                    list.Add(result.Value);
                  },
                  observer.OnError,
                  () =>
                  {
                    branch.Move(length);

                    if (separator == null)
                    {
                      currentCompleted();
                    }
                    else
                    {
                      subscription.SetDisposableIndirectly(() =>
                        separator.Parse(branch).SubscribeSafe(
                          separatorResult =>
                          {
                            hasSeparatorResult = true;
                            separatorLength = Math.Max(separatorLength, separatorResult.Length);
                          },
                          observer.OnError,
                          () =>
                          {
                            branch.Move(separatorLength);

                            currentCompleted();
                          }));
                    }
                  }));
            });

          if (nonGreedy && count == 0)
          {
            var startSubscription = new SingleAssignmentDisposable();

            var lookAhead = new LookAheadParseResult<IObservable<TResult>>(Observable.Empty<TResult>(), length: 0);

            var lookAheadSubscription = lookAhead.Subscribe(success =>
              {
                if (success)
                {
                  onCompleted();
                }
                else
                {
                  startSubscription.Disposable = start();
                }
              });

            observer.OnNext(lookAhead);

            return new CompositeDisposable(branch, subscription, lookAhead, lookAheadSubscription, startSubscription);
          }
          else
          {
            var startSubscription = start();

            return new CompositeDisposable(branch, subscription, startSubscription);
          }
        });
    }
  }
}