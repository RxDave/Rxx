using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Moves to the next observable sequence when the current sequence throws the specified type of exception 
    /// until one of the observables successfully terminates.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TException">The type of exception to catch.</typeparam>
    /// <param name="sources">The observables to be enumerated.</param>
    /// <returns>An observable sequence with an error channel.</returns>
    public static IObservable<Either<TSource, TException>> Catch<TSource, TException>(
      this IEnumerable<IObservable<TSource>> sources)
      where TException : Exception
    {
      Contract.Requires(sources != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TSource, TException>>>() != null);

      return Observable.Defer<Either<TSource, TException>>(() =>
        {
          var cursor = sources.GetEnumerator();

          return Catch<TSource, TException>(
            cursor,
            ex => cursor,
            ex => TimeSpan.Zero);
        });
    }

    /// <summary>
    /// Moves to the next observable sequence when the current sequence throws the specified type of exception 
    /// using the specified back-off algorithm until one of the observables successfully terminates.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TException">The type of exception to catch.</typeparam>
    /// <param name="sources">The observables to be enumerated.</param>
    /// <param name="backOffSelector">Selects the amount of time to delay before moving to the next observable 
    /// when the current sequence has faulted.</param>
    /// <returns>An observable sequence with an error channel.</returns>
    public static IObservable<Either<TSource, TException>> Catch<TSource, TException>(
      this IEnumerable<IObservable<TSource>> sources,
      Func<TException, TimeSpan> backOffSelector)
      where TException : Exception
    {
      Contract.Requires(sources != null);
      Contract.Requires(backOffSelector != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TSource, TException>>>() != null);

      return Observable.Defer<Either<TSource, TException>>(() =>
        {
          var cursor = sources.GetEnumerator();

          return Catch<TSource, TException>(
            cursor,
            ex => cursor,
            backOffSelector);
        });
    }

    /// <summary>
    /// Moves to the next observable sequence when the current sequence throws the specified type of exception 
    /// until one of the observables successfully terminates.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TException">The type of exception to catch.</typeparam>
    /// <param name="sources">The observables to be enumerated.</param>
    /// <returns>An observable sequence with an error channel.</returns>
    public static IObservable<Either<TSource, TException>> Catch<TSource, TException>(
      this IEnumerator<IObservable<TSource>> sources)
      where TException : Exception
    {
      Contract.Requires(sources != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TSource, TException>>>() != null);

      return Catch<TSource, TException>(sources, ex => sources, ex => TimeSpan.Zero);
    }

    /// <summary>
    /// Moves to the next observable sequence provided by the specified <paramref name="handler"/> when the current 
    /// sequence throws the specified type of exception until one of the observables successfully terminates.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TException">The type of exception to catch.</typeparam>
    /// <param name="sources">The observables to be enumerated.</param>
    /// <param name="handler">Selects the next enumerator when an observable from the current enumerator has faulted.</param>
    /// <returns>An observable sequence with an error channel.</returns>
    public static IObservable<Either<TSource, TException>> Catch<TSource, TException>(
      this IEnumerator<IObservable<TSource>> sources,
      Func<TException, IEnumerator<IObservable<TSource>>> handler)
      where TException : Exception
    {
      Contract.Requires(sources != null);
      Contract.Requires(handler != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TSource, TException>>>() != null);

      return Catch<TSource, TException>(sources, handler, ex => TimeSpan.Zero);
    }

    /// <summary>
    /// Moves to the next observable sequence provided by the specified <paramref name="handler"/> when the current 
    /// sequence throws the specified type of exception using the specified back-off algorithm until one of the observables 
    /// successfully terminates.
    /// </summary>
    /// <typeparam name="TSource">The object that provides notification information.</typeparam>
    /// <typeparam name="TException">The type of exception to catch.</typeparam>
    /// <param name="sources">The observables to be enumerated.</param>
    /// <param name="handler">Selects the next enumerator when an observable from the current enumerator has faulted.</param>
    /// <param name="backOffSelector">Selects the amount of time to delay before moving to the next observable 
    /// when the current sequence has faulted.</param>
    /// <returns>An observable sequence with an error channel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "All exceptions are sent to observers.")]
    public static IObservable<Either<TSource, TException>> Catch<TSource, TException>(
      this IEnumerator<IObservable<TSource>> sources,
      Func<TException, IEnumerator<IObservable<TSource>>> handler,
      Func<TException, TimeSpan> backOffSelector)
      where TException : Exception
    {
      Contract.Requires(sources != null);
      Contract.Requires(handler != null);
      Contract.Requires(backOffSelector != null);
      Contract.Ensures(Contract.Result<IObservable<Either<TSource, TException>>>() != null);

      return EitherObservable.Create<TSource, TException>(
        observer =>
        {
          bool movedNext;
          IObservable<TSource> current = null;

          Func<bool> moveNext = () =>
            {
              try
              {
                movedNext = sources.MoveNext();

                if (movedNext)
                {
                  current = sources.Current;

                  Contract.Assume(current != null);

                  return true;
                }
              }
              catch (Exception ex)
              {
                observer.OnError(ex);
              }

              return false;
            };

          if (!moveNext())
          {
            observer.OnCompleted();

            return sources;
          }

          var subscription = new SerialDisposable();
          var sourcesDisposable = new SerialDisposable();

          sourcesDisposable.Disposable = sources;

          var disposable = Scheduler.CurrentThread.Schedule(
            TimeSpan.Zero,
            self =>
            {
              subscription.SetDisposableIndirectly(() =>
                current.SubscribeSafe(
                  observer.OnNextLeft,
                  ex =>
                  {
                    var typedError = ex as TException;

                    if (typedError == null)
                    {
                      observer.OnError(ex);
                    }
                    else
                    {
                      observer.OnNextRight(typedError);

                      IEnumerator<IObservable<TSource>> next;

                      try
                      {
                        next = handler(typedError);
                      }
                      catch (Exception ex2)
                      {
                        observer.OnError(ex2);
                        return;
                      }

                      Contract.Assume(next != null);

                      if (sources != next)
                      {
                        sources = next;

                        sourcesDisposable.Disposable = sources;
                      }

                      if (moveNext())
                      {
                        TimeSpan delay;

                        try
                        {
                          delay = backOffSelector(typedError);
                        }
                        catch (Exception ex2)
                        {
                          observer.OnError(ex2);
                          return;
                        }

                        if (delay < TimeSpan.Zero)
                        /* Feature that allows callers to indicate when an exception is fatal based on its type */
                        {
                          observer.OnError(ex);
                        }
                        else
                        {
                          self(delay);
                        }
                      }
                      else
                      {
                        observer.OnError(ex);
                      }
                    }
                  },
                  observer.OnCompleted));
            });

          return new CompositeDisposable(subscription, disposable, sourcesDisposable);
        });
    }
  }
}