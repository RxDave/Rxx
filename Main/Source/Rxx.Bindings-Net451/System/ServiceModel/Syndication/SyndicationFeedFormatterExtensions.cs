using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Xml;

namespace System.ServiceModel.Syndication
{
  /// <summary>
  /// Provides <see langword="static" /> extension methods for <see cref="SyndicationFeedFormatter"/> objects.
  /// </summary>
  public static class SyndicationFeedFormatterExtensions
  {
    /// <summary>
    /// Downloads the specified syndication <paramref name="feeds"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="formatter">The object that reads each feed.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadObservable(this SyndicationFeedFormatter formatter, params Uri[] feeds)
    {
      Contract.Requires(formatter != null);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      if (feeds.Length == 0)
      {
        return Observable.Empty<SyndicationItem>();
      }
      else
      {
        return formatter.DownloadObservable((IEnumerable<Uri>)feeds);
      }
    }

    /// <summary>
    /// Downloads the specified syndication <paramref name="feeds"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="formatter">The object that reads each feed.</param>
    /// <param name="client">The object used to make the web request.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadObservable(this SyndicationFeedFormatter formatter, WebClient client, params Uri[] feeds)
    {
      Contract.Requires(formatter != null);
      Contract.Requires(client != null);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      if (feeds.Length == 0)
      {
        return Observable.Empty<SyndicationItem>();
      }
      else
      {
        return formatter.DownloadObservable(client, (IEnumerable<Uri>)feeds);
      }
    }

    /// <summary>
    /// Downloads the specified syndication <paramref name="feeds"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="formatter">The object that reads each feed.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadObservable(this SyndicationFeedFormatter formatter, IEnumerable<Uri> feeds)
    {
      Contract.Requires(formatter != null);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return (from uri in feeds
              select from stream in ObservableWebClient.OpenRead(uri)
                     select GetFeed(stream, formatter))
             .Merge()
             .SelectMany(feed => feed.Items.Do(item =>
               {
                 if (item.SourceFeed == null)
                 {
                   item.SourceFeed = feed;
                 }
               }));
    }

    /// <summary>
    /// Downloads the specified syndication <paramref name="feeds"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="formatter">The object that reads each feed.</param>
    /// <param name="client">The object used to make the web request.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <alert type="tip">
    /// To get the original <see cref="SyndicationFeed"/> for each <see cref="SyndicationItem"/>, use the <see cref="SyndicationItem.SourceFeed"/> property.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of items from all of the feeds merged together.</returns>
    public static IObservable<SyndicationItem> DownloadObservable(this SyndicationFeedFormatter formatter, WebClient client, IEnumerable<Uri> feeds)
    {
      Contract.Requires(formatter != null);
      Contract.Requires(client != null);
      Contract.Requires(feeds != null);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return (from uri in feeds
              select Observable.Defer(() => from stream in client.OpenReadObservable(uri)
                                            select GetFeed(stream, formatter)))
             .Concat()
             .SelectMany(feed => feed.Items.Do(item =>
             {
               if (item.SourceFeed == null)
               {
                 item.SourceFeed = feed;
               }
             }));
    }

    /// <summary>
    /// Downloads all of the specified syndication <paramref name="feeds"/> at the specified <paramref name="interval"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="formatter">The object that reads each feed.</param>
    /// <param name="interval">The duration between downloads.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <para>
    /// <see cref="DownloadObservable(SyndicationFeedFormatter,TimeSpan,Uri[])"/> downloads all of the feeds upon subscription and then repeats the process at each <paramref name="interval"/>.
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
    public static IObservable<SyndicationItem> DownloadObservable(this SyndicationFeedFormatter formatter, TimeSpan interval, params Uri[] feeds)
    {
      Contract.Requires(formatter != null);
      Contract.Requires(feeds != null);
      Contract.Requires(interval >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      if (feeds.Length == 0)
      {
        return Observable.Empty<SyndicationItem>();
      }
      else
      {
        return formatter.DownloadObservable(interval, (IEnumerable<Uri>)feeds);
      }
    }

    /// <summary>
    /// Downloads all of the specified syndication <paramref name="feeds"/> at the specified <paramref name="interval"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="formatter">The object that reads each feed.</param>
    /// <param name="client">The object used to make the web request.</param>
    /// <param name="interval">The duration between downloads.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <para>
    /// <see cref="DownloadObservable(SyndicationFeedFormatter,TimeSpan,Uri[])"/> downloads all of the feeds upon subscription and then repeats the process at each <paramref name="interval"/>.
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
    public static IObservable<SyndicationItem> DownloadObservable(this SyndicationFeedFormatter formatter, WebClient client, TimeSpan interval, params Uri[] feeds)
    {
      Contract.Requires(formatter != null);
      Contract.Requires(client != null);
      Contract.Requires(feeds != null);
      Contract.Requires(interval >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      if (feeds.Length == 0)
      {
        return Observable.Empty<SyndicationItem>();
      }
      else
      {
        return formatter.DownloadObservable(client, interval, (IEnumerable<Uri>)feeds);
      }
    }

    /// <summary>
    /// Downloads all of the specified syndication <paramref name="feeds"/> at the specified <paramref name="interval"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="formatter">The object that reads each feed.</param>
    /// <param name="interval">The duration between downloads.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <para>
    /// <see cref="DownloadObservable(SyndicationFeedFormatter,TimeSpan,IEnumerable{Uri})"/> downloads all of the feeds upon subscription and then repeats the process at each <paramref name="interval"/>.
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
    public static IObservable<SyndicationItem> DownloadObservable(this SyndicationFeedFormatter formatter, TimeSpan interval, IEnumerable<Uri> feeds)
    {
      Contract.Requires(formatter != null);
      Contract.Requires(feeds != null);
      Contract.Requires(interval >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return (from _ in Observable.Timer(TimeSpan.Zero, interval)
#if !SILVERLIGHT
              let uris = feeds
#else
              let uris = feeds.EnsureNoCache()
#endif
              select formatter.DownloadObservable(uris))
              .Switch();
    }

    /// <summary>
    /// Downloads all of the specified syndication <paramref name="feeds"/> at the specified <paramref name="interval"/> and merges them into an observable sequence.
    /// </summary>
    /// <param name="formatter">The object that reads each feed.</param>
    /// <param name="client">The object used to make the web request.</param>
    /// <param name="interval">The duration between downloads.</param>
    /// <param name="feeds">The <see cref="Uri"/> objects identifying the feeds to be downloaded.</param>
    /// <remarks>
    /// <para>
    /// <see cref="DownloadObservable(SyndicationFeedFormatter,TimeSpan,IEnumerable{Uri})"/> downloads all of the feeds upon subscription and then repeats the process at each <paramref name="interval"/>.
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
    public static IObservable<SyndicationItem> DownloadObservable(this SyndicationFeedFormatter formatter, WebClient client, TimeSpan interval, IEnumerable<Uri> feeds)
    {
      Contract.Requires(formatter != null);
      Contract.Requires(feeds != null);
      Contract.Requires(interval >= TimeSpan.Zero);
      Contract.Ensures(Contract.Result<IObservable<SyndicationItem>>() != null);

      return (from _ in Observable.Timer(TimeSpan.Zero, interval)
#if !SILVERLIGHT
              let uris = feeds
#else
              let uris = feeds.EnsureNoCache()
#endif
              select formatter.DownloadObservable(client, uris))
              .Switch();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
      Justification = "It is safe to ensure that stream is Disposed even if the XmlReader disposes it.")]
    private static SyndicationFeed GetFeed(Stream stream, SyndicationFeedFormatter formatter)
    {
      Contract.Requires(stream != null);
      Contract.Requires(formatter != null);
      Contract.Ensures(Contract.Result<SyndicationFeed>() != null);

      using (stream)
      using (var reader = XmlReader.Create(stream))
      {
        formatter.ReadFrom(reader);
      }

      var feed = formatter.Feed;

      Contract.Assume(feed != null);

      return feed;
    }
  }
}