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
    public static IObservableParser<TSource, TResult> Group<TSource, TOpen, TClose, TResult>(
      this IObservableParser<TSource, TOpen> open,
      IObservableParser<TSource, TResult> content,
      IObservableParser<TSource, TClose> close)
    {
      Contract.Requires(open != null);
      Contract.Requires(content != null);
      Contract.Requires(close != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, TResult>>() != null);

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
    public static IObservableParser<TSource, IObservable<TSource>> Group<TSource, TOpen, TClose>(
      this IObservableParser<TSource, TOpen> open,
      IObservableParser<TSource, TClose> close)
    {
      Contract.Requires(open != null);
      Contract.Requires(close != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TSource>>>() != null);

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
    public static IObservableParser<TSource, IObservable<TSource>> AmbiguousGroup<TSource>(
      this IObservableParser<TSource, TSource> open,
      IObservableParser<TSource, TSource> close)
    {
      Contract.Requires(open != null);
      Contract.Requires(close != null);
      Contract.Ensures(Contract.Result<IObservableParser<TSource, IObservable<TSource>>>() != null);

      return open.Yield("AmbiguousGroup", source => AmbiguousGroupInternal(source, open, close));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
      Justification = "The current amount of coupling seems appropriate for the purpose of this method.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
      Justification = "Although it's certainly complex, I believe the use of closures greatly simplifies the overall computation.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "Passed to the observer.")]
    private static IObservable<IParseResult<IObservable<TSource>>> AmbiguousGroupInternal<TSource>(
      IObservableCursor<TSource> source,
      IObservableParser<TSource, TSource> open,
      IObservableParser<TSource, TSource> close)
    {
      Contract.Requires(source != null);
      Contract.Requires(open != null);
      Contract.Requires(close != null);
      Contract.Ensures(Contract.Result<IObservable<IParseResult<IObservable<TSource>>>>() != null);

      return Observable.Create<IParseResult<IObservable<TSource>>>(
        observer =>
        {
          var branch = source.Branch();

          var disposables = new CompositeDisposable(branch);

          bool hasOpenResult = false;

          disposables.Add(open.Parse(source).SubscribeSafe(
            openResult =>
            {
              hasOpenResult = true;

              int openCount = 1;

              var openSinks = new List<Action<IParseResult<TSource>>>();
              var closeSinks = new List<Action<IParseResult<TSource>>>();
              var contentSinks = new List<Action<IParseResult<TSource>>>();
              var bufferedResults = new List<IEnumerable<IParseResult<IObservable<TSource>>>>();

              Func<List<IParseResult<IObservable<TSource>>>> addBuffer = () =>
              {
                var buffer = new List<IParseResult<IObservable<TSource>>>();
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
                  var branchResult = result.Yield(new List<TSource>(content).ToObservable(Scheduler.Immediate));

                  buffer.Add(branchResult);

                  if (openCount > 0)
                    content.Add(result.Value);
                });
              };

              // base sinks must be installed first - openCount must be incremented before other sinks are executed
              openSinks.Add(_ => { openCount++; installSinks(); });
              closeSinks.Add(_ => { openCount--; });

              // now we can install the sinks for the first open (matched in the foreach above)
              installSinks();

              var innerBranch = branch.Remainder(openResult.Length);

              bool hasInnerResult = false;
              IObservableParser<TSource, TSource> current = open;

              var recursion = new SingleAssignmentDisposable();

              disposables.Add(recursion);

              recursion.Disposable = Scheduler.Immediate.Schedule(
                self =>
                {
                  var capturedBranch = innerBranch;

                  var innerParser =
                    open.OnSuccess(innerOpenResult =>
                    {
                      hasInnerResult = true;

                      var clone = openSinks.ToList();

                      // the sinks list is modified when open is matched, so we must run a clone
                      clone.ForEach(sink => sink(innerOpenResult));

                      innerBranch = capturedBranch.Remainder(innerOpenResult.Length);

                      current = open;
                    })
                  .Or(
                    close.OnSuccess(closeResult =>
                    {
                      hasInnerResult = true;

                      closeSinks.ForEach(sink => sink(closeResult));

                      innerBranch = capturedBranch.Remainder(closeResult.Length);

                      current = close;
                    }))
                  .Or(
                    current.Next.OnSuccess(content =>
                    {
                      hasInnerResult = true;

                      contentSinks.ForEach(sink => sink(content));

                      innerBranch = capturedBranch.Remainder(content.Length);
                    }));

                  var innerSubscription = new SingleAssignmentDisposable();

                  disposables.Add(innerSubscription);

                  innerSubscription.Disposable = innerParser.Parse(capturedBranch).SubscribeSafe(
                    _ => { },
                    observer.OnError,
                    () =>
                    {
                      if (openCount > 0 && hasInnerResult)
                      {
                        self();
                      }
                      else
                      {
                        innerBranch.Dispose();

                        disposables.Remove(innerSubscription);
                        disposables.Remove(recursion);

                        if (hasInnerResult)
                        {
                          try
                          {
                            bufferedResults.Concat().ForEach(observer.OnNext);
                          }
                          catch (Exception ex)
                          {
                            observer.OnError(ex);
                          }

                          observer.OnCompleted();
                        }
                        else
                        {
                          // completing without results is failure
                          observer.OnCompleted();
                        }
                      }
                    });
                });
            },
            observer.OnError,
            () =>
            {
              if (!hasOpenResult)
              {
                observer.OnCompleted();
              }
            }));

          return disposables;
        });
    }
  }
}