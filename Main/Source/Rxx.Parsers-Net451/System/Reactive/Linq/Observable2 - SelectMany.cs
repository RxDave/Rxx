using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;

namespace System.Reactive.Linq
{
  internal static partial class Observable2
  {
    /// <summary>
    /// Avoids StackOverflowException when combining a large number of sequential <strong>SelectMany</strong> operations.
    /// The <strong>Tuple</strong> provides an indicator to selectors whether each element is the last element (<see langword="true"/>)
    /// in the sequence or not (<see langword="false"/>).  This extra info is used by the <em>All</em> parser combinator to eliminate 
    /// extra branching on the last element of each sequence.  See the AllParser.Parse method for details.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1618:GenericTypeParametersMustBeDocumented", Justification = "Internal method.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "Internal method.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1615:ElementReturnValueMustBeDocumented", Justification = "Internal method.")]
    internal static IObservable<TResult> SelectMany<TSource, TContext, TResult>(
      this IObservable<TSource> source,
      TContext context,
      IEnumerable<Func<TContext, Tuple<TSource, bool>, Tuple<TContext, IObservable<TSource>>>> collectionSelectors,
      Func<TSource, IList<TSource>, TResult> resultSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(context != null);
      Contract.Requires(collectionSelectors != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      var enumerator = collectionSelectors.GetEnumerator();

      try
      {
        if (enumerator.MoveNext())
        {
          var first = enumerator.Current;

          Contract.Assume(first != null);

          return source
            .SelectMany(context, first, enumerator.AsEnumerable(), resultSelector)
            .Finally(enumerator.Dispose);
        }
        else
        {
          var results = Observable.Empty<TResult>();

          enumerator.Dispose();

          return results;
        }
      }
      catch
      {
        enumerator.Dispose();
        throw;
      }
    }

    /// <summary>
    /// Avoids StackOverflowException when combining a large number of sequential <strong>SelectMany</strong> operations.
    /// The <strong>Tuple</strong> provides an indicator to selectors whether each element is the last element (<see langword="true"/>)
    /// in the sequence or not (<see langword="false"/>).  This extra info is used by the <em>All</em> parser combinator to eliminate 
    /// extra branching on the last element of each sequence.  See the AllParser.Parse method for details.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
      Justification = "This method is designed to trade internal maintainability for the simplification of consumption.  Furthermore, it only relies on highly cohesive types in the FCL that cannot be changed or decoupled.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
      Justification = "Although it's certainly complex, I believe the use of closures greatly simplifies the overall computation.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "All exceptions are sent to observers.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1618:GenericTypeParametersMustBeDocumented", Justification = "Internal method.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "Internal method.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1615:ElementReturnValueMustBeDocumented", Justification = "Internal method.")]
    internal static IObservable<TResult> SelectMany<TSource, TContext, TCollection, TResult>(
      this IObservable<TSource> source,
      TContext context,
      Func<TContext, Tuple<TSource, bool>, Tuple<TContext, IObservable<TCollection>>> firstCollectionSelector,
      IEnumerable<Func<TContext, Tuple<TCollection, bool>, Tuple<TContext, IObservable<TCollection>>>> otherCollectionSelectors,
      Func<TSource, IList<TCollection>, TResult> resultSelector)
    {
      Contract.Requires(source != null);
      Contract.Requires(context != null);
      Contract.Requires(firstCollectionSelector != null);
      Contract.Requires(otherCollectionSelectors != null);
      Contract.Requires(resultSelector != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      return Observable.Create<TResult>(
        observer =>
        {
          object gate = new object();

          var disposables = new CompositeDisposable();

          int running = 0;
          bool sourceCompleted = false;

          Action<Action> checkCompleted =
            change =>
            {
              bool completeNow = false;

              lock (gate)
              {
                change();

                completeNow = running == 0 && sourceCompleted;
              }

              if (completeNow)
              {
                observer.OnCompleted();
              }
            };

          Action<Func<IObservable<TCollection>>, Action<Tuple<TCollection, bool>>> subscribe =
            (selector, onNext) =>
            {
              IObservable<TCollection> sequence;

              try
              {
                sequence = selector();
              }
              catch (Exception ex)
              {
                observer.OnError(ex);
                return;
              }

              var subscription = new SingleAssignmentDisposable();

              disposables.Add(subscription);

              Interlocked.Increment(ref running);

              subscription.Disposable = sequence.WithLastElementIndicator().SubscribeSafe(
                onNext,
                observer.OnError,
                () =>
                {
                  checkCompleted(() => Interlocked.Decrement(ref running));

                  disposables.Remove(subscription);
                });
            };

          object selectorCacheGate = new object();

          var selectorCache = new List<Func<TContext, Tuple<TCollection, bool>, Tuple<TContext, IObservable<TCollection>>>>();

          var enumerator = otherCollectionSelectors.GetEnumerator();

          disposables.Add(Disposable.Create(() =>
            {
              lock (selectorCacheGate)
              {
                enumerator.Dispose();
                enumerator = null;
              }
            }));

          Func<int, Func<TContext, Tuple<TCollection, bool>, Tuple<TContext, IObservable<TCollection>>>> getSelector =
            depth =>
            {
              Func<TContext, Tuple<TCollection, bool>, Tuple<TContext, IObservable<TCollection>>> selector = null;

              lock (selectorCacheGate)
              {
                if (enumerator == null)
                {
                  return null;
                }
                else if (selectorCache.Count > depth)
                {
                  selector = selectorCache[depth];
                }
                else if (enumerator.MoveNext())
                {
                  selector = enumerator.Current;

                  selectorCache.Add(selector);
                }
                else
                {
                  selectorCache.Add(null);
                }
              }

              return selector;
            };

          Func<int, TContext, TSource, IEnumerable<TCollection>, Action<Tuple<TCollection, bool>>> onNextSequential = null;
          onNextSequential = (depth, currentContext, first, others) =>
            value =>
            {
              var list = others.ToList();

              list.Add(value.Item1);

              var selector = getSelector(depth);

              if (selector != null)
              {
                var sequence = selector(currentContext, value);

                subscribe(
                  () => sequence.Item2,
                  onNextSequential(depth + 1, sequence.Item1, first, list));
              }
              else
              {
                TResult result;

                try
                {
                  result = resultSelector(first, list.AsReadOnly());
                }
                catch (Exception ex)
                {
                  observer.OnError(ex);
                  return;
                }

                observer.OnNext(result);
              }
            };

          disposables.Add(source.WithLastElementIndicator().SubscribeSafe(
            first =>
            {
              var sequence = firstCollectionSelector(context, first);

              subscribe(
                () => sequence.Item2,
                onNextSequential(0, sequence.Item1, first.Item1, Enumerable.Empty<TCollection>()));
            },
            observer.OnError,
            () => checkCompleted(() => sourceCompleted = true)));

          return disposables;
        });
    }
  }
}