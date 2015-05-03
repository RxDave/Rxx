using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Creates a flattened observable sequence of property changed notifications for every property on the specified 
    /// <paramref name="source"/> object.
    /// </summary>
    /// <typeparam name="TSource">Type of the object from which all properties are to be retrieved.</typeparam>
    /// <param name="source">The object from which to listen for all property changed events.</param>
    /// <remarks>
    /// The following property changed notification patterns are supported: 
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <returns>A flattened observable sequence of property changed notifications for the specified 
    /// <paramref name="source"/> object.</returns>
    public static IObservable<EventPattern<PropertyChangedEventArgs>> FromPropertyChangedPattern<TSource>(TSource source)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      return FromPropertyChangedPattern(source, propertyInfo: null);
    }

    /// <summary>
    /// Creates an observable sequence of property changed notifications for the specified 
    /// <paramref name="property"/> on the specified <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="TSource">Type of the object that defines the specified <paramref name="property"/>.</typeparam>
    /// <typeparam name="TValue">Type of the property's value.</typeparam>
    /// <param name="source">The object that defines the specified <paramref name="property"/>.</param>
    /// <param name="property">The property on the specified <paramref name="source"/> from which to generate property changed notifications.</param>
    /// <remarks>
    /// The following property changed notification patterns are supported: 
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <returns>An observable sequence of property changed notifications.</returns>
    public static IObservable<TValue> FromPropertyChangedPattern<TSource, TValue>(
      TSource source,
      Expression<Func<TSource, TValue>> property)
    {
      Contract.Requires(source != null);
      Contract.Requires(property != null);
      Contract.Ensures(Contract.Result<IObservable<TValue>>() != null);

      var compile = new Lazy<Func<TSource, TValue>>(
        property.Compile,
        isThreadSafe: false);		// Rx ensures that it's thread-safe

      var observable =
        from e in FromPropertyChangedPattern(source, property.ToPropertyInfo())
        let getter = compile.Value
        select getter(source);

      return observable;
    }

#if !SILVERLIGHT || WINDOWS_PHONE
    /// <summary>
    /// Creates an observable sequence of property changed notifications for the specified <paramref name="property"/>.
    /// </summary>
    /// <typeparam name="TValue">Type of the property's value.</typeparam>
    /// <param name="property">The property from which to generate property changed notifications.</param>
    /// <remarks>
    /// The following property changed notification patterns are supported: 
    /// <list type="bullet">
    /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
    /// <item>[Property]Changed event pattern.</item>
    /// <item>WPF dependency properties.</item>
    /// </list>
    /// </remarks>
    /// <returns>An observable sequence of property changed notifications.</returns>
    public static IObservable<TValue> FromPropertyChangedPattern<TValue>(
      Expression<Func<TValue>> property)
    {
      Contract.Requires(property != null);
      Contract.Ensures(Contract.Result<IObservable<TValue>>() != null);

      var compile = new Lazy<Func<TValue>>(
        property.Compile,
        isThreadSafe: false);		// Rx ensures that it's thread-safe

      object source;
      PropertyInfo propertyInfo = property.ToPropertyInfo(out source);

      return from e in FromPropertyChangedPattern(source, propertyInfo)
             let getter = compile.Value
             select getter();
    }
#endif

    private static IObservable<EventPattern<PropertyChangedEventArgs>> FromPropertyChangedPattern(
      object source,
      PropertyInfo propertyInfo = null)
    {
      Contract.Requires(source != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      /* TypeDescriptor supports INotifyPropertyChanged; however, it doesn't acknowledge that changed
       * events can also be raised for inherited properties, thus we should use INotifyPropertyChanged
       * directly when it's available.
       * 
       * Original discussion: 
       * http://social.msdn.microsoft.com/Forums/en/rx/thread/2fc8ab3c-28ed-45a9-a96f-59133a3d103c
       */
      var notifies = source as INotifyPropertyChanged;

      if (notifies != null)
      {
        var changes = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          eh => notifies.PropertyChanged += eh,
          eh => notifies.PropertyChanged -= eh);

        return propertyInfo == null
             ? changes
             : changes.Where(e => string.Equals(e.EventArgs.PropertyName, propertyInfo.Name, StringComparison.Ordinal));
      }
      else if (propertyInfo != null)
      {
        var propertyDescriptor = Rxx.ComponentReflection.GetProperty(source, propertyInfo.Name, StringComparison.Ordinal);

        Contract.Assume(propertyDescriptor != null);

        return propertyDescriptor.PropertyChanged(source);
      }
      else
      {
        return from property in Rxx.ComponentReflection.GetProperties(source).ToObservable()
               where property.SupportsChangeEvents
               from change in property.PropertyChanged(source)
               select change;
      }
    }

    private static IObservable<EventPattern<PropertyChangedEventArgs>> FromPropertyChangedPattern(
      object source,
      int maximumDepth,
      PropertyInfo propertyInfo = null)
    {
      Contract.Requires(source != null);
      Contract.Requires(maximumDepth >= 0);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<PropertyChangedEventArgs>>>() != null);

      var changes = FromPropertyChangedPattern(source, propertyInfo);

      if (maximumDepth > 0)
      {
        maximumDepth--;

        changes = changes.Publish(published => published.Merge(
          from change in published
#if NET_40 || PORT_40
          let property = (propertyInfo ?? source.GetType().GetProperty(change.EventArgs.PropertyName))
#else
          let property = (propertyInfo ?? source.GetType().GetRuntimeProperty(change.EventArgs.PropertyName))
#endif
          let value = property.GetValue(source, null)
          where value != null
          from descendantChange in
            FromPropertyChangedPattern(value, maximumDepth).TakeUntil(
              from otherChange in published
              where otherChange.EventArgs.PropertyName == change.EventArgs.PropertyName
              select otherChange)
          select descendantChange));
      }

      return changes;
    }
  }
}