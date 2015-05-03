using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;

namespace System.Net.Mail
{
  /// <summary>
  /// Provides <see langword="static"/> methods for asynchronously sending data to and observing data from a resource identified by a URI.
  /// </summary>
  public static class ObservableSmtpClient
  {
    /// <summary>
    /// Sends an e-mail message to an SMTP server for delivery.
    /// </summary>
    /// <param name="from">Contains the address information of the message sender.</param>
    /// <param name="recipients">Contains the addresses that the message is sent to.</param>
    /// <param name="subject">Contains the subject line for the message.</param>
    /// <param name="body">Contains the message body.</param>
    /// <returns>A singleton observable that contains the cached result of sending the message.</returns>
    public static IObservable<EventPattern<AsyncCompletedEventArgs>> Send(
      string from,
      string recipients,
      string subject,
      string body)
    {
      Contract.Requires(!string.IsNullOrEmpty(from));
      Contract.Requires(!string.IsNullOrEmpty(recipients));
      Contract.Ensures(Contract.Result<IObservable<EventPattern<AsyncCompletedEventArgs>>>() != null);

      return Observable.Using(
        () => new SmtpClient(),
        client => client.SendObservable(from, recipients, subject, body));
    }

    /// <summary>
    /// Sends the specified e-mail message to an SMTP server for delivery.
    /// </summary>
    /// <param name="message">A <see cref="MailMessage"/> that contains the message to send.</param>
    /// <returns>A singleton observable that contains the cached result of sending the specified <paramref name="message"/>.</returns>
    public static IObservable<EventPattern<AsyncCompletedEventArgs>> Send(
      this MailMessage message)
    {
      Contract.Requires(message != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<AsyncCompletedEventArgs>>>() != null);

      return Observable.Using(
        () => new SmtpClient(),
        client => client.SendObservable(message));
    }
  }
}