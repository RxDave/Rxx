using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.Syndication;
using System.Windows;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive.UI
{
  [DisplayName("Observable Syndication")]
  [Description("Binding a ListBox to an RSS or Atom feed.")]
  public partial class ObservableSyndicationLab : BaseLab
  {
#if !SILVERLIGHT
    public static readonly DependencyPropertyKey FeedProperty = DependencyProperty.RegisterReadOnly(
      "Feed",
      typeof(ReadOnlyDictionarySubject<string, SyndicationItem>),
      typeof(ObservableSyndicationLab),
      new PropertyMetadata(null));
#endif

    public ReadOnlyDictionarySubject<string, SyndicationItem> Feed
    {
      get
      {
#if SILVERLIGHT
        var view = (System.Windows.Data.CollectionViewSource) Resources["Feed"];

        return (ReadOnlyDictionarySubject<string, SyndicationItem>) view.Source;
#else
        return (ReadOnlyDictionarySubject<string, SyndicationItem>)GetValue(FeedProperty.DependencyProperty);
#endif
      }
      private set
      {
#if SILVERLIGHT
        var view = (System.Windows.Data.CollectionViewSource) Resources["Feed"];

        view.Source = value;
#else
        SetValue(FeedProperty, value);
#endif
      }
    }

    public Uri FeedUrl
    {
      get;
      set;
    }

    public TimeSpan AutoRefreshInterval
    {
      get;
      set;
    }

    public bool IsFormatRss
    {
      get;
      set;
    }

    private SerialDisposable disposable;

    public ObservableSyndicationLab()
    {
      AutoRefreshInterval = TimeSpan.FromSeconds(30);
      FeedUrl = new Uri("http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/threads?outputAs=atom");
      IsFormatRss = false;

      InitializeComponent();
    }

    protected override IEnumerable<IDisposable> Main()
    {
      disposable = new SerialDisposable();

      UpdateFeed();

      yield return disposable;
    }

    private void UpdateFeed()
    {
      DisconnectFeed();

      if (FeedUrl == null)
      {
        TraceError(Text.InvalidUrl);
      }
      else if (AutoRefreshInterval < TimeSpan.Zero)
      {
        TraceError(Text.InvalidInterval);
      }
      else
      {
        IObservable<SyndicationItem> feedQuery;

        if (IsFormatRss)
        {
          feedQuery = ObservableSyndication.DownloadRss(AutoRefreshInterval, FeedUrl);
        }
        else
        {
          feedQuery = ObservableSyndication.DownloadAtom(AutoRefreshInterval, FeedUrl);
        }

#if SILVERLIGHT
        var tracedFeedQuery = Observable.Defer(() =>
          {
            TraceLine(Text.Subscribing);

            return feedQuery;
          })
          .Finally(() => TraceLine(Text.Disposing));
#else
        var tracedFeedQuery = feedQuery.TraceSubscriptions(Proxy);
#endif

        tracedFeedQuery = tracedFeedQuery
          .ObserveOnDispatcher()
          .Do(
            _ => { },
            ex =>
            {
              TraceError(ex.Message);

              DisconnectFeed();
            });

        var feed = tracedFeedQuery
          .Finally(DisconnectFeed)
          .ToDictionaryObservable(item => item.Id);

        disposable.Disposable = feed;

        this.Feed = feed;
      }
    }

    private void DisconnectFeed()
    {
      var feed = this.Feed;

      this.Feed = null;

      if (feed != null)
      {
        feed.Dispose();
      }
    }

    protected void ApplySettingsButton_Click(object sender, RoutedEventArgs e)
    {
      UpdateFeed();
    }
  }
}