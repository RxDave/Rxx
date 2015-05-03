using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;

namespace System.Net.Mail
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods on <see cref="SmtpClient"/> objects that asynchronously send data to and observe data from a resource identified by a URI.
  /// </summary>
  public static class SmtpClientExtensions
  {
    /// <summary>
    /// Sends the specified e-mail message to an SMTP server for delivery.
    /// </summary>
    /// <param name="client">The object that sends the e-mail message.</param>
    /// <param name="message">A <see cref="MailMessage"/> that contains the message to send.</param>
    /// <returns>A singleton observable that contains the cached result of sending the specified <paramref name="message"/>.</returns>
    public static IObservable<EventPattern<AsyncCompletedEventArgs>> SendObservable(
      this SmtpClient client,
      MailMessage message)
    {
      Contract.Requires(client != null);
      Contract.Requires(message != null);
      Contract.Ensures(Contract.Result<IObservable<EventPattern<AsyncCompletedEventArgs>>>() != null);

      return Observable2.FromEventBasedAsyncPattern<SendCompletedEventHandler>(
        d => d.Invoke,
        eh => client.SendCompleted += eh,
        eh => client.SendCompleted -= eh,
        token => client.SendAsync(message, token),
        client.SendAsyncCancel);
    }

    /// <summary>
    /// Sends an e-mail message to an SMTP server for delivery.
    /// </summary>
    /// <param name="client">The object that sends the e-mail message.</param>
    /// <param name="from">Contains the address information of the message sender.</param>
    /// <param name="recipients">Contains the addresses that the message is sent to.</param>
    /// <param name="subject">Contains the subject line for the message.</param>
    /// <param name="body">Contains the message body.</param>
    /// <returns>A singleton observable that contains the cached result of sending the message.</returns>
    public static IObservable<EventPattern<AsyncCompletedEventArgs>> SendObservable(
      this SmtpClient client,
      string from,
      string recipients,
      string subject,
      string body)
    {
      Contract.Requires(client != null);
      Contract.Requires(!string.IsNullOrEmpty(from));
      Contract.Requires(!string.IsNullOrEmpty(recipients));
      Contract.Ensures(Contract.Result<IObservable<EventPattern<AsyncCompletedEventArgs>>>() != null);

      return Observable2.FromEventBasedAsyncPattern<SendCompletedEventHandler>(
        d => d.Invoke,
        eh => client.SendCompleted += eh,
        eh => client.SendCompleted -= eh,
        token => client.SendAsync(from, recipients, subject, body, token),
        client.SendAsyncCancel);
    }
  }
}