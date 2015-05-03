using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reactive.Disposables;
using DaveSexton.Labs;

namespace Rxx.Labs
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
    Justification = "Disposables are disposed in the Unloaded event.")]
  [System.ComponentModel.Composition.PartNotDiscoverable]		// Required since this class isn't marked abstract to support the VS designer.
#if !SILVERLIGHT
  public class BaseLab : WindowsLab
#elif WINDOWS_PHONE
	public class BaseLab : PhoneLab
#else
	public class BaseLab : SilverlightLab
#endif
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    protected bool ShowTimeOnNext
    {
      get;
      set;
    }

    private IDisposable subscriptions;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="BaseLab" /> class.
    /// </summary>
    public BaseLab()
    {
      ShowTimeOnNext = true;

      Loaded += BaseLab_Loaded;
      Unloaded += BaseLab_Unloaded;
    }
    #endregion

    #region Methods
    protected virtual IEnumerable<IDisposable> Main()
    {
      Contract.Ensures(Contract.Result<IEnumerable<IDisposable>>() != null);

      // for derived classes
      yield break;
    }

    public sealed override string FormatSourceCode(string source)
    {
      return source.Replace("\t", "  ");
    }

    public sealed override string FormatSourceXaml(string source)
    {
      return source.Replace("\t", "  ");
    }

    protected virtual IObserver<object> ConsoleOutput()
    {
      Contract.Ensures(Contract.Result<IObserver<object>>() != null);

      Contract.Assume(Proxy != null);

      var observer = new LabObserver<object>(Proxy, ShowTimeOnNext);

      observer.StartTimer();

      return observer;
    }

    protected virtual Func<IObserver<object>> ConsoleOutput(string name)
    {
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Ensures(Contract.Result<Func<IObserver<object>>>() != null);

      return () =>
      {
        Contract.Ensures(Contract.Result<IObserver<object>>() != null);

        var observer = new LabObserver<object>(Proxy, name, ShowTimeOnNext);

        observer.StartTimer();

        return observer;
      };
    }

    protected virtual Func<IObserver<object>> ConsoleOutput(string nameFormat, params object[] args)
    {
      Contract.Requires(!string.IsNullOrEmpty(nameFormat));
      Contract.Requires(args != null);
      Contract.Ensures(Contract.Result<Func<IObserver<object>>>() != null);

      string name = string.Format(CultureInfo.CurrentCulture, nameFormat, args);

      Contract.Assume(!string.IsNullOrEmpty(name));

      return ConsoleOutput(name);
    }

    protected virtual Func<IObserver<object>> ConsoleOutputFormat(string valueFormat)
    {
      Contract.Requires(!string.IsNullOrEmpty(valueFormat));
      Contract.Ensures(Contract.Result<Func<IObserver<object>>>() != null);

      return () =>
      {
        Contract.Ensures(Contract.Result<IObserver<object>>() != null);

        var observer = new LabObserver<object>(Proxy, null, valueFormat, ShowTimeOnNext);

        observer.StartTimer();

        return observer;
      };
    }

    protected virtual Func<IObserver<object>> ConsoleOutputFormat(string name, string valueFormat)
    {
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Requires(!string.IsNullOrEmpty(valueFormat));
      Contract.Ensures(Contract.Result<Func<IObserver<object>>>() != null);

      return () =>
      {
        Contract.Ensures(Contract.Result<IObserver<object>>() != null);

        var observer = new LabObserver<object>(Proxy, name, valueFormat, ShowTimeOnNext);

        observer.StartTimer();

        return observer;
      };
    }

    protected virtual Action<T> ConsoleOutputOnNext<T>()
    {
      Contract.Ensures(Contract.Result<Action<T>>() != null);

      Contract.Assume(Proxy != null);

      var observer = new LabObserver<object>(Proxy, ShowTimeOnNext);

      observer.StartTimer();

      return value => observer.OnNext(value);
    }

    protected virtual Action<T> ConsoleOutputOnNext<T>(string name)
    {
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Ensures(Contract.Result<Action<T>>() != null);

      Contract.Assume(Proxy != null);

      var observer = new LabObserver<object>(Proxy, name, ShowTimeOnNext);

      observer.StartTimer();

      return value => observer.OnNext(value);
    }

    protected virtual Action<T> ConsoleOutputOnNext<T>(Func<T, string> format)
    {
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<Action<T>>() != null);

      Contract.Assume(Proxy != null);

      var observer = new LabObserver<object>(Proxy, ShowTimeOnNext);

      observer.StartTimer();

      return value => observer.OnNext(format(value));
    }

    protected virtual Action<T> ConsoleOutputOnNext<T>(string name, Func<T, string> format)
    {
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Requires(format != null);
      Contract.Ensures(Contract.Result<Action<T>>() != null);

      Contract.Assume(Proxy != null);

      var observer = new LabObserver<object>(Proxy, name, ShowTimeOnNext);

      observer.StartTimer();

      return value => observer.OnNext(format(value));
    }

    protected virtual Action<Exception> ConsoleOutputOnError()
    {
      Contract.Ensures(Contract.Result<Action<Exception>>() != null);

      Contract.Assume(Proxy != null);

      var observer = LabObserver<object>.Error(Proxy);

      observer.StartTimer();

      return observer.OnError;
    }

    protected virtual Action<Exception> ConsoleOutputOnError(string name)
    {
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Ensures(Contract.Result<Action<Exception>>() != null);

      Contract.Assume(Proxy != null);

      var observer = LabObserver<object>.Error(Proxy, name);

      observer.StartTimer();

      return observer.OnError;
    }

    protected virtual Action ConsoleOutputOnCompleted()
    {
      Contract.Ensures(Contract.Result<Action>() != null);

      Contract.Assume(Proxy != null);

      var observer = LabObserver<object>.Completed(Proxy);

      observer.StartTimer();

      return observer.OnCompleted;
    }

    protected virtual Action ConsoleOutputOnCompleted(string name)
    {
      Contract.Requires(!string.IsNullOrEmpty(name));
      Contract.Ensures(Contract.Result<Action>() != null);

      Contract.Assume(Proxy != null);

      var observer = LabObserver<object>.Completed(Proxy, name);

      observer.StartTimer();

      return observer.OnCompleted;
    }
    #endregion

    #region Event Handlers
    private void BaseLab_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
#if SILVERLIGHT && !WINDOWS_PHONE
			var outOfBrowser = Content as OutOfBrowserContentControl;

			if (outOfBrowser != null && !outOfBrowser.IsRunningOutOfBrowser)
			{
				return;
			}
#endif

      subscriptions = new CompositeDisposable(Main());
    }

    private void BaseLab_Unloaded(object sender, System.Windows.RoutedEventArgs e)
    {
      if (subscriptions != null)
      {
        subscriptions.Dispose();
      }
    }
    #endregion
  }
}