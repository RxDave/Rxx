using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reactive.Disposables;
using TraceSource = System.Diagnostics.TraceSource;

namespace System.Reactive.Linq
{
  public static partial class TraceObservable
  {
    /// <summary>
    /// Returns an observable that traces calls to Subscribe for the specified observable and calls to Dispose of the resulting subscription.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which subscriptions and cancelations will be traced.</param>
    /// <returns>An observable that traces subscriptions and cancelations.</returns>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Subscription is returned to observer.")]
    public static IObservable<T> TraceSubscriptions<T>(this IObservable<T> source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return Observable.Create<T>(observer =>
      {
        System.Diagnostics.Trace.TraceInformation(Rxx.Diagnostics.Properties.Text.DefaultSubscribingMessage);

        var subscription = new CompositeDisposable(
          Disposable.Create(() => System.Diagnostics.Trace.TraceInformation(Rxx.Diagnostics.Properties.Text.DefaultDisposingSubscriptionMessage)),
          source.SubscribeSafe(observer),
          Disposable.Create(() => System.Diagnostics.Trace.TraceInformation(Rxx.Diagnostics.Properties.Text.DefaultDisposedSubscriptionMessage)));

        System.Diagnostics.Trace.TraceInformation(Rxx.Diagnostics.Properties.Text.DefaultSubscribedMessage);

        return subscription;
      });
    }

    /// <summary>
    /// Returns an observable that traces calls to Subscribe for the specified observable and calls to Dispose of the resulting subscription
    /// and includes the specified <paramref name="identity"/> in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which subscriptions and cancelations will be traced.</param>
    /// <param name="identity">Identifies the observer in the trace output.</param>
    /// <returns>An observable that traces subscriptions and cancelations.</returns>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Subscription is returned to observer.")]
    public static IObservable<T> TraceSubscriptions<T>(this IObservable<T> source, string identity)
    {
      Contract.Requires(source != null);
      Contract.Requires(identity != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.TraceSubscriptions(
        string.Format(CultureInfo.CurrentCulture, Rxx.Diagnostics.Properties.Text.SubscribingFormat, identity),
        string.Format(CultureInfo.CurrentCulture, Rxx.Diagnostics.Properties.Text.SubscribedFormat, identity),
        string.Format(CultureInfo.CurrentCulture, Rxx.Diagnostics.Properties.Text.DisposingSubscriptionFormat, identity),
        string.Format(CultureInfo.CurrentCulture, Rxx.Diagnostics.Properties.Text.DisposedSubscriptionFormat, identity));
    }

    /// <summary>
    /// Returns an observable that traces calls to Subscribe for the specified observable and calls to Dispose of the resulting subscription.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which subscriptions and cancelations will be traced.</param>
    /// <param name="subscribingMessage">The message to trace when Subscribe is called.</param>
    /// <param name="subscribedMessage">The message to trace when Subscribe has returned.</param>
    /// <returns>An observable that traces subscriptions and cancelations.</returns>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Subscription is returned to observer.")]
    public static IObservable<T> TraceSubscriptions<T>(this IObservable<T> source, string subscribingMessage, string subscribedMessage)
    {
      Contract.Requires(source != null);
      Contract.Requires(subscribingMessage != null);
      Contract.Requires(subscribedMessage != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return Observable.Create<T>(observer =>
      {
        System.Diagnostics.Trace.TraceInformation(subscribingMessage);

        var subscription = new CompositeDisposable(
          Disposable.Create(() => System.Diagnostics.Trace.TraceInformation(Rxx.Diagnostics.Properties.Text.DefaultDisposingSubscriptionMessage)),
          source.SubscribeSafe(observer),
          Disposable.Create(() => System.Diagnostics.Trace.TraceInformation(Rxx.Diagnostics.Properties.Text.DefaultDisposedSubscriptionMessage)));

        System.Diagnostics.Trace.TraceInformation(subscribedMessage);

        return subscription;
      });
    }

    /// <summary>
    /// Returns an observable that traces calls to Subscribe for the specified observable and calls to Dispose of the resulting subscription.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which subscriptions and cancelations will be traced.</param>
    /// <param name="subscribingMessage">The message to trace when Subscribe is called.</param>
    /// <param name="subscribedMessage">The message to trace when Subscribe has returned.</param>
    /// <param name="disposingMessage">The message to trace when Dispose is called.</param>
    /// <param name="disposedMessage">The message to trace when Dispose has returned.</param>
    /// <returns>An observable that traces subscriptions and cancelations.</returns>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Subscription is returned to observer.")]
    public static IObservable<T> TraceSubscriptions<T>(this IObservable<T> source, string subscribingMessage, string subscribedMessage, string disposingMessage, string disposedMessage)
    {
      Contract.Requires(source != null);
      Contract.Requires(subscribingMessage != null);
      Contract.Requires(subscribedMessage != null);
      Contract.Requires(disposingMessage != null);
      Contract.Requires(disposedMessage != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return Observable.Create<T>(observer =>
      {
        System.Diagnostics.Trace.TraceInformation(subscribingMessage);

        var subscription = new CompositeDisposable(
          Disposable.Create(() => System.Diagnostics.Trace.TraceInformation(disposingMessage)),
          source.SubscribeSafe(observer),
          Disposable.Create(() => System.Diagnostics.Trace.TraceInformation(disposedMessage)));

        System.Diagnostics.Trace.TraceInformation(subscribedMessage);

        return subscription;
      });
    }

    /// <summary>
    /// Returns an observable that traces calls to Subscribe for the specified observable and calls to Dispose of the resulting subscription.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which subscriptions and cancelations will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <returns>An observable that traces subscriptions and cancelations.</returns>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Subscription is returned to observer.")]
    public static IObservable<T> TraceSubscriptions<T>(this IObservable<T> source, TraceSource trace)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return Observable.Create<T>(observer =>
      {
        trace.TraceInformation(Rxx.Diagnostics.Properties.Text.DefaultSubscribingMessage);

        var subscription = new CompositeDisposable(
          Disposable.Create(() => trace.TraceInformation(Rxx.Diagnostics.Properties.Text.DefaultDisposingSubscriptionMessage)),
          source.SubscribeSafe(observer),
          Disposable.Create(() => trace.TraceInformation(Rxx.Diagnostics.Properties.Text.DefaultDisposedSubscriptionMessage)));

        trace.TraceInformation(Rxx.Diagnostics.Properties.Text.DefaultSubscribedMessage);

        return subscription;
      });
    }

    /// <summary>
    /// Returns an observable that traces calls to Subscribe for the specified observable and calls to Dispose of the resulting subscription
    /// and includes the specified <paramref name="identity"/> in the trace output.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which subscriptions and cancelations will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="identity">Identifies the observer in the trace output.</param>
    /// <returns>An observable that traces subscriptions and cancelations.</returns>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Subscription is returned to observer.")]
    public static IObservable<T> TraceSubscriptions<T>(this IObservable<T> source, TraceSource trace, string identity)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(identity != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return source.TraceSubscriptions(
        trace,
        string.Format(CultureInfo.CurrentCulture, Rxx.Diagnostics.Properties.Text.SubscribingFormat, identity),
        string.Format(CultureInfo.CurrentCulture, Rxx.Diagnostics.Properties.Text.SubscribedFormat, identity),
        string.Format(CultureInfo.CurrentCulture, Rxx.Diagnostics.Properties.Text.DisposingSubscriptionFormat, identity),
        string.Format(CultureInfo.CurrentCulture, Rxx.Diagnostics.Properties.Text.DisposedSubscriptionFormat, identity));
    }

    /// <summary>
    /// Returns an observable that traces calls to Subscribe for the specified observable and calls to Dispose of the resulting subscription.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which subscriptions and cancelations will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="subscribingMessage">The message to trace when Subscribe is called.</param>
    /// <param name="subscribedMessage">The message to trace when Subscribe has returned.</param>
    /// <returns>An observable that traces subscriptions and cancelations.</returns>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Subscription is returned to observer.")]
    public static IObservable<T> TraceSubscriptions<T>(this IObservable<T> source, TraceSource trace, string subscribingMessage, string subscribedMessage)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(subscribingMessage != null);
      Contract.Requires(subscribedMessage != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return Observable.Create<T>(observer =>
      {
        trace.TraceInformation(subscribingMessage);

        var subscription = new CompositeDisposable(
          Disposable.Create(() => trace.TraceInformation(Rxx.Diagnostics.Properties.Text.DefaultDisposingSubscriptionMessage)),
          source.SubscribeSafe(observer),
          Disposable.Create(() => trace.TraceInformation(Rxx.Diagnostics.Properties.Text.DefaultDisposedSubscriptionMessage)));

        trace.TraceInformation(subscribedMessage);

        return subscription;
      });
    }

    /// <summary>
    /// Returns an observable that traces calls to Subscribe for the specified observable and calls to Dispose of the resulting subscription.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <param name="source">The observable for which subscriptions and cancelations will be traced.</param>
    /// <param name="trace">The <see cref="TraceSource"/> to be associated with the trace messages.</param>
    /// <param name="subscribingMessage">The message to trace when Subscribe is called.</param>
    /// <param name="subscribedMessage">The message to trace when Subscribe has returned.</param>
    /// <param name="disposingMessage">The message to trace when Dispose is called.</param>
    /// <param name="disposedMessage">The message to trace when Dispose has returned.</param>
    /// <returns>An observable that traces subscriptions and cancelations.</returns>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Subscription is returned to observer.")]
    public static IObservable<T> TraceSubscriptions<T>(this IObservable<T> source, TraceSource trace, string subscribingMessage, string subscribedMessage, string disposingMessage, string disposedMessage)
    {
      Contract.Requires(source != null);
      Contract.Requires(trace != null);
      Contract.Requires(subscribingMessage != null);
      Contract.Requires(subscribedMessage != null);
      Contract.Requires(disposingMessage != null);
      Contract.Requires(disposedMessage != null);
      Contract.Ensures(Contract.Result<IObservable<T>>() != null);

      return Observable.Create<T>(observer =>
      {
        trace.TraceInformation(subscribingMessage);

        var subscription = new CompositeDisposable(
          Disposable.Create(() => trace.TraceInformation(disposingMessage)),
          source.SubscribeSafe(observer),
          Disposable.Create(() => trace.TraceInformation(disposedMessage)));

        trace.TraceInformation(subscribedMessage);

        return subscription;
      });
    }
  }
}