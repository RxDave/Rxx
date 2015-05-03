using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace System
{
  /// <summary>
  /// Provides <see langword="static" /> methods that format trace strings for sequences.
  /// </summary>
  public static class TraceDefaults
  {
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
      Justification = "It's used in a thread-safe manner by only a single consumer, and also it must be exposed for unit testing.")]
    internal static int IdentityCounter;

    /// <summary>
    /// Returns a textual representation of the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">Type of <paramref name="value"/>.</typeparam>
    /// <param name="value">The object for which a string representation is returned.</param>
    /// <returns>String that represents the specified <paramref name="value"/>.</returns>
    public static string DefaultOnNext<T>(T value)
    {
      Contract.Ensures(Contract.Result<string>() != null);

      return value == null ? string.Empty : value.ToString();
    }

    /// <summary>
    /// Returns a textual representation of the specified <paramref name="exception"/>.
    /// </summary>
    /// <param name="exception">The object for which a string representation is returned.</param>
    /// <returns>String that represents the specified <paramref name="exception"/>.</returns>
    public static string DefaultOnError(Exception exception)
    {
      Contract.Ensures(Contract.Result<string>() != null);

      return exception == null ? string.Empty : exception.ToString();
    }

    /// <summary>
    /// Returns a textual representation of an OnCompleted notification.
    /// </summary>
    /// <returns>String that represents OnCompleted.</returns>
    public static string DefaultOnCompleted()
    {
      Contract.Ensures(Contract.Result<string>() != null);

      return Rxx.Diagnostics.Properties.Text.DefaultOnCompletedMessage;
    }

    /// <summary>
    /// Returns a textual representation of the specified <paramref name="value"/> for the specified <paramref name="observerId"/>.
    /// </summary>
    /// <typeparam name="T">Type of <paramref name="value"/>.</typeparam>
    /// <param name="observerId">The identity of the observer that received this notification.</param>
    /// <param name="value">The object for which a string representation is returned.</param>
    /// <returns>String that represents the specified <paramref name="value"/> for the specified  <paramref name="observerId"/>.</returns>
    public static string DefaultOnNext<T>(string observerId, T value)
    {
      Contract.Ensures(Contract.Result<string>() != null);

      return FormatMessage(observerId, DefaultOnNext(value));
    }

    /// <summary>
    /// Returns a textual representation of the specified <paramref name="exception"/> for the specified <paramref name="observerId"/>.
    /// </summary>
    /// <param name="observerId">The identity of the observer that received this notification.</param>
    /// <param name="exception">The object for which a string representation is returned.</param>
    /// <returns>String that represents the specified <paramref name="exception"/> for the specified <paramref name="observerId"/>.</returns>
    public static string DefaultOnError(string observerId, Exception exception)
    {
      Contract.Ensures(Contract.Result<string>() != null);

      return FormatMessage(observerId, DefaultOnError(exception));
    }

    /// <summary>
    /// Returns a textual representation of an OnCompleted notification for the specified <paramref name="observerId"/>.
    /// </summary>
    /// <param name="observerId">The identity of the observer that received this notification.</param>
    /// <returns>String that represents OnCompleted for the specified <paramref name="observerId"/>.</returns>
    public static string DefaultOnCompleted(string observerId)
    {
      Contract.Ensures(Contract.Result<string>() != null);

      return FormatMessage(observerId, DefaultOnCompleted());
    }

    /// <summary>
    /// Formats the specified <paramref name="message"/> for the specified <paramref name="observerId"/>.
    /// </summary>
    /// <param name="observerId">The identity of the observer to which the <paramref name="message"/> belongs.</param>
    /// <param name="message">The message to be formatted.</param>
    /// <returns>The formatted <paramref name="message"/> for the specified <paramref name="observerId"/>.</returns>
    public static string FormatMessage(string observerId, string message)
    {
      Contract.Ensures(Contract.Result<string>() != null);

      return observerId + ": " + message;
    }

    internal static Func<T, string> GetFormatOnNext<T>(string nextFormat)
    {
      Contract.Requires(nextFormat != null);
      Contract.Ensures(Contract.Result<Func<T, string>>() != null);

      return value => string.Format(CultureInfo.CurrentCulture, nextFormat, value);
    }

    internal static Func<Exception, string> GetFormatOnError(string errorFormat)
    {
      Contract.Requires(errorFormat != null);
      Contract.Ensures(Contract.Result<Func<Exception, string>>() != null);

      return error => string.Format(CultureInfo.CurrentCulture, errorFormat, error);
    }

    internal static Func<string> GetMessageOnCompleted(string completedMessage)
    {
      Contract.Requires(completedMessage != null);
      Contract.Ensures(Contract.Result<Func<string>>() != null);

      return () => completedMessage;
    }

    internal static Func<string, T, string> GetIdentityFormatOnNext<T>(string nextFormat)
    {
      Contract.Requires(nextFormat != null);
      Contract.Ensures(Contract.Result<Func<string, T, string>>() != null);

      return (id, value) => string.Format(CultureInfo.CurrentCulture, nextFormat, id, value);
    }

    internal static Func<string, Exception, string> GetIdentityFormatOnError(string errorFormat)
    {
      Contract.Requires(errorFormat != null);
      Contract.Ensures(Contract.Result<Func<string, Exception, string>>() != null);

      return (id, error) => string.Format(CultureInfo.CurrentCulture, errorFormat, id, error);
    }

    internal static Func<string, string> GetIdentityMessageOnCompleted(string completedMessage)
    {
      Contract.Requires(completedMessage != null);
      Contract.Ensures(Contract.Result<Func<string, string>>() != null);

      return id => string.Format(CultureInfo.CurrentCulture, completedMessage, id);
    }
  }
}
