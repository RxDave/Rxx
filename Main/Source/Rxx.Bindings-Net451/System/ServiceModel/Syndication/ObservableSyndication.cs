using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;

namespace System.ServiceModel.Syndication
{
  /// <summary>
  /// Provides <see langword="static" /> methods for downloading syndication feeds.
  /// </summary>
  public static class ObservableSyndication
  {
    /// <summary>
    /// Downloads the specified RSS 2.0 syndication <paramref name="feeds"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadRss(params Uri[] feeds)
    {
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Rss20FeedFormatter().DownloadObservable(feeds);
    }

    /// <summary>
    /// Downloads the specified RSS 2.0 syndication <paramref name="feeds"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="client">The object used to make web requests.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadRss(WebClient client, params Uri[] feeds)
    {
      Contract.Requires(client != null);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Rss20FeedFormatter().DownloadObservable(client, feeds);
    }

    /// <summary>
    /// Downloads the specified RSS 2.0 syndication <paramref name="feeds"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadRss(IEnumerable<Uri> feeds)
    {
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Rss20FeedFormatter().DownloadObservable(feeds);
    }

    /// <summary>
    /// Downloads the specified RSS 2.0 syndication <paramref name="feeds"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="client">The object used to make web requests.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadRss(WebClient client, IEnumerable<Uri> feeds)
    {
      Contract.Requires(client != null);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Rss20FeedFormatter().DownloadObservable(client, feeds);
    }

    /// <summary>
    /// Downloads all of the specified RSS 2.0 syndication <paramref name="feeds"/> at the specified <paramref name="interval"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="interval">The duration between downloads.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <para>
    /// <see cref="DownloadRss(TimeSpan,Uri[])"/> downloads all of the feeds upon subscription and then repeats the process at each <paramref name="interval"/>.
    /// All of the <paramref name="feeds"/> are downloaded at each interval and merged into the observable sequence.
    /// </para>
    /// <para>
    /// To avoid duplicate items, consider calling the <strong>Distinct</strong> method on the sequence.  Alternatively, use the 
    /// <strong>Collect</strong> method to create an <see cref="System.Reactive.Subjects.IDictionarySubject{TKey,TValue}"/>.
    /// </para>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadRss(TimeSpan interval, params Uri[] feeds)
    {
      Contract.Requires(interval >= TimeSpan.Zero);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Rss20FeedFormatter().DownloadObservable(interval, feeds);
    }

    /// <summary>
    /// Downloads all of the specified RSS 2.0 syndication <paramref name="feeds"/> at the specified <paramref name="interval"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="client">The object used to make web requests.</param> 
    /// <param name="interval">The duration between downloads.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <para>
    /// <see cref="DownloadRss(TimeSpan,Uri[])"/> downloads all of the feeds upon subscription and then repeats the process at each <paramref name="interval"/>.
    /// All of the <paramref name="feeds"/> are downloaded at each interval and merged into the observable sequence.
    /// </para>
    /// <para>
    /// To avoid duplicate items, consider calling the <strong>Distinct</strong> method on the sequence.  Alternatively, use the 
    /// <strong>Collect</strong> method to create an <see cref="System.Reactive.Subjects.IDictionarySubject{TKey,TValue}"/>.
    /// </para>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadRss(WebClient client, TimeSpan interval, params Uri[] feeds)
    {
      Contract.Requires(client != null);
      Contract.Requires(interval >= TimeSpan.Zero);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Rss20FeedFormatter().DownloadObservable(client, interval, feeds);
    }

    /// <summary>
    /// Downloads all of the specified RSS 2.0 syndication <paramref name="feeds"/> at the specified <paramref name="interval"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="interval">The duration between downloads.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <para>
    /// <see cref="DownloadRss(TimeSpan,IEnumerable{Uri})"/> downloads all of the feeds upon subscription and then repeats the process at each <paramref name="interval"/>.
    /// All of the <paramref name="feeds"/> are downloaded at each interval and merged into the observable sequence.
    /// </para>
    /// <para>
    /// To avoid duplicate items, consider calling the <strong>Distinct</strong> method on the sequence.  Alternatively, use the 
    /// <strong>Collect</strong> method to create an <see cref="System.Reactive.Subjects.IDictionarySubject{TKey,TValue}"/>.
    /// </para>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadRss(TimeSpan interval, IEnumerable<Uri> feeds)
    {
      Contract.Requires(interval >= TimeSpan.Zero);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Rss20FeedFormatter().DownloadObservable(interval, feeds);
    }

    /// <summary>
    /// Downloads all of the specified RSS 2.0 syndication <paramref name="feeds"/> at the specified <paramref name="interval"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="client">The object used to make web requests.</param>
    /// <param name="interval">The duration between downloads.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <para>
    /// <see cref="DownloadRss(TimeSpan,IEnumerable{Uri})"/> downloads all of the feeds upon subscription and then repeats the process at each <paramref name="interval"/>.
    /// All of the <paramref name="feeds"/> are downloaded at each interval and merged into the observable sequence.
    /// </para>
    /// <para>
    /// To avoid duplicate items, consider calling the <strong>Distinct</strong> method on the sequence.  Alternatively, use the 
    /// <strong>Collect</strong> method to create an <see cref="System.Reactive.Subjects.IDictionarySubject{TKey,TValue}"/>.
    /// </para>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadRss(WebClient client, TimeSpan interval, IEnumerable<Uri> feeds)
    {
      Contract.Requires(client != null);
      Contract.Requires(interval >= TimeSpan.Zero);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Rss20FeedFormatter().DownloadObservable(client, interval, feeds);
    }

    /// <summary>
    /// Downloads the specified Atom 1.0 syndication <paramref name="feeds"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadAtom(params Uri[] feeds)
    {
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Atom10FeedFormatter().DownloadObservable(feeds);
    }

    /// <summary>
    /// Downloads the specified Atom 1.0 syndication <paramref name="feeds"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="client">The object used to make web requests.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadAtom(WebClient client, params Uri[] feeds)
    {
      Contract.Requires(client != null);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Atom10FeedFormatter().DownloadObservable(client, feeds);
    }

    /// <summary>
    /// Downloads the specified Atom 1.0 syndication <paramref name="feeds"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadAtom(IEnumerable<Uri> feeds)
    {
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Atom10FeedFormatter().DownloadObservable(feeds);
    }

    /// <summary>
    /// Downloads the specified Atom 1.0 syndication <paramref name="feeds"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="client">The object used to make web requests.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadAtom(WebClient client, IEnumerable<Uri> feeds)
    {
      Contract.Requires(client != null);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Atom10FeedFormatter().DownloadObservable(client, feeds);
    }

    /// <summary>
    /// Downloads all of the specified Atom 1.0 syndication <paramref name="feeds"/> at the specified <paramref name="interval"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="interval">The duration between downloads.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <para>
    /// <see cref="DownloadAtom(TimeSpan,Uri[])"/> downloads all of the feeds upon subscription and then repeats the process at each <paramref name="interval"/>.
    /// All of the <paramref name="feeds"/> are downloaded at each interval and merged into the observable sequence.
    /// </para>
    /// <para>
    /// To avoid duplicate items, consider calling the <strong>Distinct</strong> method on the sequence.  Alternatively, use the 
    /// <strong>Collect</strong> method to create an <see cref="System.Reactive.Subjects.IDictionarySubject{TKey,TValue}"/>.
    /// </para>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadAtom(TimeSpan interval, params Uri[] feeds)
    {
      Contract.Requires(interval >= TimeSpan.Zero);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Atom10FeedFormatter().DownloadObservable(interval, feeds);
    }

    /// <summary>
    /// Downloads all of the specified Atom 1.0 syndication <paramref name="feeds"/> at the specified <paramref name="interval"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="client">The object used to make web requests.</param>
    /// <param name="interval">The duration between downloads.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <para>
    /// <see cref="DownloadAtom(TimeSpan,Uri[])"/> downloads all of the feeds upon subscription and then repeats the process at each <paramref name="interval"/>.
    /// All of the <paramref name="feeds"/> are downloaded at each interval and merged into the observable sequence.
    /// </para>
    /// <para>
    /// To avoid duplicate items, consider calling the <strong>Distinct</strong> method on the sequence.  Alternatively, use the 
    /// <strong>Collect</strong> method to create an <see cref="System.Reactive.Subjects.IDictionarySubject{TKey,TValue}"/>.
    /// </para>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadAtom(WebClient client, TimeSpan interval, params Uri[] feeds)
    {
      Contract.Requires(client != null);
      Contract.Requires(interval >= TimeSpan.Zero);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Atom10FeedFormatter().DownloadObservable(client, interval, feeds);
    }

    /// <summary>
    /// Downloads all of the specified Atom 1.0 syndication <paramref name="feeds"/> at the specified <paramref name="interval"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="interval">The duration between downloads.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <para>
    /// <see cref="DownloadAtom(TimeSpan,IEnumerable{Uri})"/> downloads all of the feeds upon subscription and then repeats the process at each <paramref name="interval"/>.
    /// All of the <paramref name="feeds"/> are downloaded at each interval and merged into the observable sequence.
    /// </para>
    /// <para>
    /// To avoid duplicate items, consider calling the <strong>Distinct</strong> method on the sequence.  Alternatively, use the 
    /// <strong>Collect</strong> method to create an <see cref="System.Reactive.Subjects.IDictionarySubject{TKey,TValue}"/>.
    /// </para>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadAtom(TimeSpan interval, IEnumerable<Uri> feeds)
    {
      Contract.Requires(interval >= TimeSpan.Zero);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Atom10FeedFormatter().DownloadObservable(interval, feeds);
    }

    /// <summary>
    /// Downloads all of the specified Atom 1.0 syndication <paramref name="feeds"/> at the specified <paramref name="interval"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="client">The object used to make web requests.</param>
    /// <param name="interval">The duration between downloads.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <para>
    /// <see cref="DownloadAtom(TimeSpan,IEnumerable{Uri})"/> downloads all of the feeds upon subscription and then repeats the process at each <paramref name="interval"/>.
    /// All of the <paramref name="feeds"/> are downloaded at each interval and merged into the observable sequence.
    /// </para>
    /// <para>
    /// To avoid duplicate items, consider calling the <strong>Distinct</strong> method on the sequence.  Alternatively, use the 
    /// <strong>Collect</strong> method to create an <see cref="System.Reactive.Subjects.IDictionarySubject{TKey,TValue}"/>.
    /// </para>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadAtom(WebClient client, TimeSpan interval, IEnumerable<Uri> feeds)
    {
      Contract.Requires(client != null);
      Contract.Requires(interval >= TimeSpan.Zero);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return new Atom10FeedFormatter().DownloadObservable(client, interval, feeds);
    }
  }
}