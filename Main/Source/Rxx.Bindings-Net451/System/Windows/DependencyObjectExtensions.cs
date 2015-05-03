using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using Rxx.Bindings.Properties;

namespace System.Windows
{
  /// <summary>
  /// Provides <see langword="static" /> methods for creating observables from <see cref="DependencyObject"/> instances.
  /// </summary>
  public static class DependencyObjectExtensions
  {
    /// <summary>
    /// Gets an observable sequence of property change notifications for the specified <see cref="DependencyProperty"/>, 
    /// raised by the specified <see cref="DependencyObject"/>.
    /// </summary>
    /// <typeparam name="TValue">Type of the value represented by the <see cref="DependencyProperty"/>.</typeparam>
    /// <param name="obj">The <see cref="DependencyObject"/> that defines the specified <paramref name="property"/>.</param>
    /// <param name="property">The <see cref="DependencyProperty"/> that raises change notifications for the specified object.</param>
    /// <returns>An observable sequence of property change notifications for the specified <see cref="DependencyProperty"/>, 
    /// raised by the specified <see cref="DependencyObject"/>.</returns>
    public static IObservable<TValue> DependencyPropertyChanged<TValue>(
      this DependencyObject obj,
      DependencyProperty property)
    {
      Contract.Requires(obj != null);
      Contract.Requires(property != null);
      Contract.Ensures(Contract.Result<IObservable<TValue>>() != null);

      var propertyDescriptor = DependencyPropertyDescriptor.FromProperty(property, obj.GetType());

      if (propertyDescriptor == null)
      {
        throw new ArgumentException(Errors.DependencyPropertyNotFound, "property");
      }

      return propertyDescriptor.PropertyChanged(obj).Select(_ => (TValue)obj.GetValue(property));
    }
  }
}