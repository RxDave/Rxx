using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using DaveSexton.Labs;
using Rxx.Labs.Properties;

namespace Rxx.Labs
{
#if !SILVERLIGHT
  public abstract class BaseConsoleLab : WindowsConsoleLab
#elif WINDOWS_PHONE
	public abstract class BaseConsoleLab : PhoneConsoleLab
#else
	public abstract class BaseConsoleLab : SilverlightConsoleLab
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
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="BaseConsoleLab" /> class for derived classes.
    /// </summary>
    protected BaseConsoleLab()
    {
      ShowTimeOnNext = true;
    }
    #endregion

    #region Methods
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
  }
}