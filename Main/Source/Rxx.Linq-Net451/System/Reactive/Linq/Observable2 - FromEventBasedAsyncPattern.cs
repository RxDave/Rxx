using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Creates an observable sequence that contains the cached result from an 
    /// Event-based Asynchronous Pattern (EBAP) implementation.
    /// </summary>
    /// <typeparam name="TDelegate">Type of the event's delegate.</typeparam>
    /// <param name="conversion">A function used to convert the given event handler to a delegate compatible
    /// with the underlying .NET event. The resulting delegate is used in calls to the 
    /// <paramref name="addHandler"/> and <paramref name="removeHandler"/> action parameters.</param>
    /// <param name="addHandler">Action that attaches the given event handler to the underlying .NET event.</param>
    /// <param name="removeHandler">Action that detaches the given event handler from the underlying .NET event.</param>
    /// <param name="start">An action that receives the user token object and begins the asynchronous process.</param>
    /// <param name="cancel">An action that cancels the asynchronous process when any subscription to the returned 
    /// observable is disposed.</param>
    /// <returns>An observable sequence that contains the cached result of the asynchronous operation.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/wewwczdw.aspx">
    /// Event-based Asynchronous Pattern Overview
    /// </seealso>
    public static IObservable<EventPattern<AsyncCompletedEventArgs>> FromEventBasedAsyncPattern<TDelegate>(
      Func<EventHandler<AsyncCompletedEventArgs>, TDelegate> conversion,
      Action<TDelegate> addHandler,
      Action<TDelegate> removeHandler,
      Action<object> start,
      Action cancel)
    {
      Contract.Requires(conversion != null);
      Contract.Requires(addHandler != null);
      Contract.Requires(removeHandler != null);
      Contract.Requires(start != null);
      Contract.Requires(cancel != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<AsyncCompletedEventArgs>>>() != null);

      return FromEventBasedAsyncPattern(
        conversion,
        addHandler,
        removeHandler,
        start,
        cancel,
        () => { });
    }

    /// <summary>
    /// Creates an observable sequence that contains the cached result from an 
    /// Event-based Asynchronous Pattern (EBAP) implementation.
    /// </summary>
    /// <typeparam name="TDelegate">Type of the event's delegate.</typeparam>
    /// <typeparam name="TEventArgs">Type of the event's arguments.</typeparam>
    /// <param name="conversion">A function used to convert the given event handler to a delegate compatible
    /// with the underlying .NET event. The resulting delegate is used in calls to the 
    /// <paramref name="addHandler"/> and <paramref name="removeHandler"/> action parameters.</param>
    /// <param name="addHandler">Action that attaches the given event handler to the underlying .NET event.</param>
    /// <param name="removeHandler">Action that detaches the given event handler from the underlying .NET event.</param>
    /// <param name="start">An action that receives the user token object and begins the asynchronous process.</param>
    /// <param name="cancel">An action that cancels the asynchronous process when any subscription to the returned 
    /// observable is disposed.</param>
    /// <returns>An observable sequence that contains the cached result of the asynchronous operation.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/wewwczdw.aspx">
    /// Event-based Asynchronous Pattern Overview
    /// </seealso>
    public static IObservable<EventPattern<TEventArgs>> FromEventBasedAsyncPattern<TDelegate, TEventArgs>(
      Func<EventHandler<TEventArgs>, TDelegate> conversion,
      Action<TDelegate> addHandler,
      Action<TDelegate> removeHandler,
      Action<object> start,
      Action cancel)
      where TEventArgs : AsyncCompletedEventArgs
    {
      Contract.Requires(conversion != null);
      Contract.Requires(addHandler != null);
      Contract.Requires(removeHandler != null);
      Contract.Requires(start != null);
      Contract.Requires(cancel != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<TEventArgs>>>() != null);

      return FromEventBasedAsyncPattern(
        conversion,
        addHandler,
        removeHandler,
        start,
        cancel,
        () => { });
    }

    /// <summary>
    /// Creates an observable sequence with two notifications channels from an Event-based Asynchronous Pattern (EBAP) 
    /// implementation, containing progress notifications in the left channel and the cached result in the right.
    /// </summary>
    /// <typeparam name="TDelegate">Type of the event's delegate.</typeparam>
    /// <typeparam name="TProgressDelegate">Type of the progress event's delegate.</typeparam>
    /// <typeparam name="TProgressEventArgs">Type of the progress event's arguments.</typeparam>
    /// <param name="conversion">A function used to convert the given event handler to a delegate compatible
    /// with the underlying .NET event. The resulting delegate is used in calls to the 
    /// <paramref name="addHandler"/> and <paramref name="removeHandler"/> action parameters.</param>
    /// <param name="addHandler">Action that attaches the given event handler to the underlying .NET event.</param>
    /// <param name="removeHandler">Action that detaches the given event handler from the underlying .NET event.</param>
    /// <param name="progressConversion">A function used to convert the given progress event handler to a delegate compatible
    /// with the underlying .NET progress event. The resulting delegate is used in calls to the 
    /// <paramref name="addProgressHandler"/> and <paramref name="removeProgressHandler"/>  action parameters.</param>
    /// <param name="addProgressHandler">Action that attaches the given progress event handler to the underlying .NET progress event.</param>
    /// <param name="removeProgressHandler">Action that detaches the given progress event handler from the underlying .NET progress event.</param>
    /// <param name="start">An action that receives the user token object and begins the asynchronous process.</param>
    /// <param name="cancel">An action that cancels the asynchronous process when any subscription to the returned 
    /// observable is disposed.</param>
    /// <returns>An observable sequence with two notifications channels containing progress notifications in the left 
    /// channel and the cached result in the right.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/wewwczdw.aspx">
    /// Event-based Asynchronous Pattern Overview
    /// </seealso>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Subject disposal is unnecessary here because subscribers can dispose of individual subscriptions themselves.")]
    public static IObservable<Either<EventPattern<TProgressEventArgs>, EventPattern<AsyncCompletedEventArgs>>> FromEventBasedAsyncPattern<TDelegate, TProgressDelegate, TProgressEventArgs>(
      Func<EventHandler<AsyncCompletedEventArgs>, TDelegate> conversion,
      Action<TDelegate> addHandler,
      Action<TDelegate> removeHandler,
      Func<EventHandler<TProgressEventArgs>, TProgressDelegate> progressConversion,
      Action<TProgressDelegate> addProgressHandler,
      Action<TProgressDelegate> removeProgressHandler,
      Action<object> start,
      Action cancel)
      where TProgressEventArgs : ProgressChangedEventArgs
    {
      Contract.Requires(conversion != null);
      Contract.Requires(addHandler != null);
      Contract.Requires(removeHandler != null);
      Contract.Requires(progressConversion != null);
      Contract.Requires(addProgressHandler != null);
      Contract.Requires(removeProgressHandler != null);
      Contract.Requires(start != null);
      Contract.Requires(cancel != null);
      Contract.Ensures(Contract.Result<IObservable<Either<EventPattern<TProgressEventArgs>, EventPattern<AsyncCompletedEventArgs>>>>() != null);

      return FromEventBasedAsyncPattern<TDelegate, AsyncCompletedEventArgs, TProgressDelegate, TProgressEventArgs>(
        conversion,
        addHandler,
        removeHandler,
        progressConversion,
        addProgressHandler,
        removeProgressHandler,
        start,
        cancel);
    }

    /// <summary>
    /// Creates an observable sequence with two notifications channels from an Event-based Asynchronous Pattern (EBAP) 
    /// implementation, containing progress notifications in the left channel and the cached result in the right.
    /// </summary>
    /// <typeparam name="TDelegate">Type of the event's delegate.</typeparam>
    /// <typeparam name="TEventArgs">Type of the event's arguments.</typeparam>
    /// <typeparam name="TProgressDelegate">Type of the progress event's delegate.</typeparam>
    /// <typeparam name="TProgressEventArgs">Type of the progress event's arguments.</typeparam>
    /// <param name="conversion">A function used to convert the given event handler to a delegate compatible
    /// with the underlying .NET event. The resulting delegate is used in calls to the 
    /// <paramref name="addHandler"/> and <paramref name="removeHandler"/> action parameters.</param>
    /// <param name="addHandler">Action that attaches the given event handler to the underlying .NET event.</param>
    /// <param name="removeHandler">Action that detaches the given event handler from the underlying .NET event.</param>
    /// <param name="progressConversion">A function used to convert the given progress event handler to a delegate compatible
    /// with the underlying .NET progress event. The resulting delegate is used in calls to the 
    /// <paramref name="addProgressHandler"/> and <paramref name="removeProgressHandler"/>  action parameters.</param>
    /// <param name="addProgressHandler">Action that attaches the given progress event handler to the underlying .NET progress event.</param>
    /// <param name="removeProgressHandler">Action that detaches the given progress event handler from the underlying .NET progress event.</param>
    /// <param name="start">An action that receives the user token object and begins the asynchronous process.</param>
    /// <param name="cancel">An action that cancels the asynchronous process when any subscription to the returned 
    /// observable is disposed.</param>
    /// <returns>An observable sequence with two notifications channels containing progress notifications in the left 
    /// channel and the cached result in the right.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/wewwczdw.aspx">
    /// Event-based Asynchronous Pattern Overview
    /// </seealso>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Subject disposal is unnecessary here because subscribers can dispose of individual subscriptions themselves.")]
    public static IObservable<Either<EventPattern<TProgressEventArgs>, EventPattern<TEventArgs>>> FromEventBasedAsyncPattern<TDelegate, TEventArgs, TProgressDelegate, TProgressEventArgs>(
      Func<EventHandler<TEventArgs>, TDelegate> conversion,
      Action<TDelegate> addHandler,
      Action<TDelegate> removeHandler,
      Func<EventHandler<TProgressEventArgs>, TProgressDelegate> progressConversion,
      Action<TProgressDelegate> addProgressHandler,
      Action<TProgressDelegate> removeProgressHandler,
      Action<object> start,
      Action cancel)
      where TEventArgs : AsyncCompletedEventArgs
      where TProgressEventArgs : ProgressChangedEventArgs
    {
      Contract.Requires(conversion != null);
      Contract.Requires(addHandler != null);
      Contract.Requires(removeHandler != null);
      Contract.Requires(progressConversion != null);
      Contract.Requires(addProgressHandler != null);
      Contract.Requires(removeProgressHandler != null);
      Contract.Requires(start != null);
      Contract.Requires(cancel != null);
      Contract.Ensures(Contract.Result<IObservable<Either<EventPattern<TProgressEventArgs>, EventPattern<TEventArgs>>>>() != null);

      TProgressDelegate progressHandler = default(TProgressDelegate);
      int isProgressHandlerRemoved = 0;

      Action tryRemoveProgressHandler = () =>
      {
        if (Interlocked.Exchange(ref isProgressHandlerRemoved, 1) == 0)
        {
          removeProgressHandler(progressHandler);
        }
      };

      var progressSubject = Subject.Synchronize(new Subject<EventPattern<TProgressEventArgs>>());

      var response =
        FromEventBasedAsyncPattern<TDelegate, TEventArgs>(
          conversion,
          addHandler,
          removeHandler,
          token =>
          {
            progressHandler = progressConversion((sender, e) =>
            {
              if (object.ReferenceEquals(e.UserState, token))
              {
                progressSubject.OnNext(new EventPattern<TProgressEventArgs>(sender, e));
              }
            });

            addProgressHandler(progressHandler);

            start(token);
          },
          cancel,
          tryRemoveProgressHandler);

      return progressSubject
        .TakeUntil(response.DefaultIfEmpty())
        .Pair(response);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "Exception is passed to observer.")]
    private static IObservable<EventPattern<TEventArgs>> FromEventBasedAsyncPattern<TDelegate, TEventArgs>(
      Func<EventHandler<TEventArgs>, TDelegate> conversion,
      Action<TDelegate> addHandler,
      Action<TDelegate> removeHandler,
      Action<object> start,
      Action cancel,
      Action canceledOrCompleted)
      where TEventArgs : AsyncCompletedEventArgs
    {
      Contract.Requires(conversion != null);
      Contract.Requires(addHandler != null);
      Contract.Requires(removeHandler != null);
      Contract.Requires(start != null);
      Contract.Requires(cancel != null);
      Contract.Requires(canceledOrCompleted != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<TEventArgs>>>() != null);

      var subject = new AsyncSubject<EventPattern<TEventArgs>>();
      TDelegate handler = default(TDelegate);

      object token = new object();
      int isHandlerRemoved = 0, wasCanceled = 0, completed = 0;

      Action tryRemoveHandler = () =>
      {
        if (Interlocked.Exchange(ref isHandlerRemoved, 1) == 0)
        {
          try
          {
            removeHandler(handler);
          }
          finally
          {
            canceledOrCompleted();
          }
        }
      };

      try
      {
        handler = conversion((sender, e) =>
        {
          if (object.ReferenceEquals(e.UserState, token))
          {
            if (Interlocked.Exchange(ref completed, 1) == 0)
            {
              try
              {
                // ensure the handler is removed in case there are no subscribers
                tryRemoveHandler();
              }
              catch (Exception ex)
              {
                subject.OnError(ex);
                return;
              }

              if (e.Error != null)
              {
                subject.OnError(e.Error);
              }
              else
              {
                if (!e.Cancelled)
                {
                  subject.OnNext(new EventPattern<TEventArgs>(sender, e));
                }

                subject.OnCompleted();
              }
            }
          }
        });

        addHandler(handler);

        start(token);
      }
      catch (Exception ex)
      {
        subject.OnError(ex);

        return subject.AsObservable();
      }

      return Observable.Create<EventPattern<TEventArgs>>(
        observer => new CompositeDisposable(
          Disposable.Create(() =>
          {
            if (Interlocked.Exchange(ref completed, 1) == 0)
            {
              try
              {
                try
                {
                  if (Interlocked.Exchange(ref wasCanceled, 1) == 0)
                  {
                    cancel();
                  }
                }
                finally
                {
                  tryRemoveHandler();
                }
              }
              catch (Exception ex)
              {
                subject.OnError(ex);
              }
            }
          }),
          subject.SubscribeSafe(observer)));
    }
  }
}