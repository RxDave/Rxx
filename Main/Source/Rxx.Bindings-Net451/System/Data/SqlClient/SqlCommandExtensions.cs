using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;
using System.Xml;

namespace System.Data.SqlClient
{
  /// <summary>
  /// Provides <see langword="static" /> extension methods for instances of the <see cref="SqlCommand"/> class.
  /// </summary>
  public static class SqlCommandExtensions
  {
    /// <summary>
    /// Returns an observable sequence of values indicating when the execution of Transact-SQL statements have completed.
    /// </summary>
    /// <param name="command">The <see cref="SqlCommand"/> that provides notifications.</param>
    /// <returns>An observable sequence of values indicating when the execution of Transact-SQL statements have completed.</returns>
    public static IObservable<EventPattern<StatementCompletedEventArgs>> StatementCompletedObservable(this SqlCommand command)
    {
      Contract.Requires(command != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<StatementCompletedEventArgs>>>() != null);

      return Observable.FromEventPattern<StatementCompletedEventHandler, StatementCompletedEventArgs>(
        eh => command.StatementCompleted += eh,
        eh => command.StatementCompleted -= eh);
    }

    /// <summary>
    /// Initiates the asynchronous execution of the Transact-SQL statement or stored procedure.
    /// </summary>
    /// <param name="command">The <see cref="SqlCommand"/> to be executed.</param>
    /// <returns>A singleton observable sequence containing number of rows affected.</returns>
    public static IObservable<int> ExecuteNonQueryObservable(this SqlCommand command)
    {
      Contract.Requires(command != null);
      Contract.Ensures(Contract.Result<IObservable<int>>() != null);

      return Observable.StartAsync(cancel => command.ExecuteNonQueryAsync(cancel));
    }

    /// <summary>
    /// Initiates the asynchronous execution of the Transact-SQL statement or stored procedure and retrieves one or more 
    /// result sets from the server.
    /// </summary>
    /// <param name="command">The <see cref="SqlCommand"/> to be executed.</param>
    /// <returns>A singleton observable sequence containing a <see cref="SqlDataReader"/> object that provides access to
    /// the result sets of the specified <paramref name="command"/>.</returns>
    public static IObservable<SqlDataReader> ExecuteReaderObservable(this SqlCommand command)
    {
      Contract.Requires(command != null);
      Contract.Ensures(Contract.Result<IObservable<SqlDataReader>>() != null);

      return Observable.StartAsync(cancel => command.ExecuteReaderAsync(cancel));
    }

#if NET_45
    /// <summary>
    /// Initiates the asynchronous execution of the Transact-SQL statement or stored procedure using one of the 
    /// <see cref="CommandBehavior"/> values, and retrieving one or more result sets from the server.
    /// </summary>
    /// <param name="command">The <see cref="SqlCommand"/> to be executed.</param>
    /// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
    /// <returns>A singleton observable sequence containing a <see cref="SqlDataReader"/> object that provides access to
    /// the result sets of the specified <paramref name="command"/>.</returns>
    public static IObservable<SqlDataReader> ExecuteReaderObservable(this SqlCommand command, CommandBehavior behavior)
    {
      Contract.Requires(command != null);
      Contract.Ensures(Contract.Result<IObservable<SqlDataReader>>() != null);

      return Observable.StartAsync(cancel => command.ExecuteReaderAsync(behavior, cancel));
    }
#endif

    /// <summary>
    /// Initiates the asynchronous execution of the Transact-SQL statement or stored procedure and returns results as an 
    /// <see cref="XmlReader"/> object contained by an observable sequence.
    /// </summary>
    /// <param name="command">The <see cref="SqlCommand"/> to be executed.</param>
    /// <returns>A singleton observable sequence containing an <see cref="XmlReader"/> object that provides access to
    /// the results of the specified <paramref name="command"/>.</returns>
    public static IObservable<XmlReader> ExecuteXmlReaderObservable(this SqlCommand command)
    {
      Contract.Requires(command != null);
      Contract.Ensures(Contract.Result<IObservable<XmlReader>>() != null);

      return Observable.StartAsync(cancel => command.ExecuteXmlReaderAsync(cancel));
    }

    /// <summary>
    /// Creates a <see cref="SqlDependency"/> for the specified <paramref name="command"/> and returns an observable sequence of change notifications.
    /// </summary>
    /// <param name="command">The <see cref="SqlCommand"/> object to associate with the created <see cref="SqlDependency"/> object.</param>
    /// <remarks>
    /// <para>
    /// <see cref="OnChangeObservable(SqlCommand)"/> creates a new <see cref="SqlDependency"/> object and associates it with the specified 
    /// <paramref name="command"/>.  Then it returns an observable for the <see cref="SqlDependency.OnChange"/> event.  Notifications are not filtered.
    /// </para>
    /// <alert type="tip">
    /// To receive change notifications, call <see href="http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldependency.start.aspx">Start</see>
    /// at least once in the current <see cref="AppDomain"/> for the connection string associated with the specified <paramref name="command"/>.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of change notifications related to the results of the specified <paramref name="command"/>.</returns>
    public static IObservable<EventPattern<SqlNotificationEventArgs>> OnChangeObservable(this SqlCommand command)
    {
      Contract.Requires(command != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<SqlNotificationEventArgs>>>() != null);

      var dependency = new SqlDependency(command);

      return Observable.FromEventPattern<OnChangeEventHandler, SqlNotificationEventArgs>(
        eh => dependency.OnChange += eh,
        eh => dependency.OnChange -= eh);
    }

    /// <summary>
    /// Creates a <see cref="SqlDependency"/> for the specified <paramref name="command"/> and returns an observable sequence of change notifications.
    /// </summary>
    /// <param name="command">The <see cref="SqlCommand"/> object to associate with the created <see cref="SqlDependency"/> object.</param>
    /// <param name="options">The notification request options to be used by this dependency or <see langword="null"/> to use the default service.</param>
    /// <param name="timeout">The time-out for this notification in seconds.  The default is 0, indicating that the server's time-out should be used.</param>
    /// <remarks>
    /// <para>
    /// <see cref="OnChangeObservable(SqlCommand,string,int)"/> creates a new <see cref="SqlDependency"/> object and associates it with the specified 
    /// <paramref name="command"/>.  Then it returns an observable for the <see cref="SqlDependency.OnChange"/> event.  Notifications are not filtered.
    /// </para>
    /// <alert type="tip">
    /// To receive change notifications, call <see href="http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldependency.start.aspx">Start</see>
    /// at least once in the current <see cref="AppDomain"/> for the connection string associated with the specified <paramref name="command"/>.
    /// </alert>
    /// </remarks>
    /// <returns>An observable sequence of change notifications related to the results of the specified <paramref name="command"/>.</returns>
    public static IObservable<EventPattern<SqlNotificationEventArgs>> OnChangeObservable(this SqlCommand command, string options, int timeout)
    {
      Contract.Requires(command != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<SqlNotificationEventArgs>>>() != null);

      var dependency = new SqlDependency(command, options, timeout);

      return Observable.FromEventPattern<OnChangeEventHandler, SqlNotificationEventArgs>(
        eh => dependency.OnChange += eh,
        eh => dependency.OnChange -= eh);
    }
  }
}