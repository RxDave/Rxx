using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;

namespace System.Configuration
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for <see cref="ApplicationSettingsBase"/> objects.
  /// </summary>
  public static class ApplicationSettingsBaseExtensions
  {
    /// <summary>
    /// Returns an observable sequence of setting values that have changed.
    /// </summary>
    /// <param name="settings">The object that defines the settings to observe.</param>
    /// <returns>An observable sequence of setting values that have changed.</returns>
    public static IObservable<SettingsPropertyValue> SettingChanges(this ApplicationSettingsBase settings)
    {
      Contract.Requires(settings != null);
      Contract.Ensures(Contract.Result<IObservable<SettingsPropertyValue>>() != null);

      return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          eh => eh.Invoke,
          eh => settings.PropertyChanged += eh,
          eh => settings.PropertyChanged -= eh)
        .Select(e => settings.PropertyValues[e.EventArgs.PropertyName]);
    }

    /// <summary>
    /// Returns an observable sequence of values that have changed for the settings with the specified <paramref name="names"/>.
    /// </summary>
    /// <param name="settings">The object that defines the settings to observe.</param>
    /// <param name="names">The names of one or more settings to be observed.</param>
    /// <returns>An observable sequence of values that have changed for the settings with the specified <paramref name="names"/>.</returns>
    public static IObservable<SettingsPropertyValue> SettingChanges(this ApplicationSettingsBase settings, params string[] names)
    {
      Contract.Requires(settings != null);
      Contract.Requires(names != null);
      Contract.Requires(names.Length > 0);
      Contract.Ensures(Contract.Result<IObservable<SettingsPropertyValue>>() != null);

      return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          eh => eh.Invoke,
          eh => settings.PropertyChanged += eh,
          eh => settings.PropertyChanged -= eh)
        .Where(e => names.Any(name => string.Equals(e.EventArgs.PropertyName, name, StringComparison.Ordinal)))
        .Select(e => settings.PropertyValues[e.EventArgs.PropertyName]);
    }
  }
}