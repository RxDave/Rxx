using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net.NetworkInformation
{
  /// <summary>
  /// Provides <see langword="static"/> extension methods for <see cref="Ping"/> objects that asynchronously determine whether a remote computer is accessible over the network.
  /// </summary>
  public static class PingExtensions
  {
    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message to the computer that has the specified <see cref="IPAddress"/>, and receive a corresponding ICMP echo reply message from that computer.
    /// </summary>
    /// <param name="ping">The object that sends the ICMP echo message.</param>
    /// <param name="address">An <see cref="IPAddress"/> that identifies the computer that is the destination for the ICMP echo message.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    public static IObservable<PingReply> SendObservable(this Ping ping, IPAddress address)
    {
      Contract.Requires(ping != null);
      Contract.Requires(address != null);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return CreatePingObservable(ping, token => ping.SendAsync(address, token));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message to the specified computer, and receive a corresponding ICMP echo reply message from that computer.
    /// </summary>
    /// <param name="ping">The object that sends the ICMP echo message.</param>
    /// <param name="hostNameOrAddress">A <see cref="String"/> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    public static IObservable<PingReply> SendObservable(this Ping ping, string hostNameOrAddress)
    {
      Contract.Requires(ping != null);
      Contract.Requires(!string.IsNullOrWhiteSpace(hostNameOrAddress));
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return CreatePingObservable(ping, token => ping.SendAsync(hostNameOrAddress, token));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message to the computer that has the specified <see cref="IPAddress"/>, and receive a corresponding ICMP echo reply message from that computer.
    /// </summary>
    /// <param name="ping">The object that sends the ICMP echo message.</param>
    /// <param name="address">An <see cref="IPAddress"/> that identifies the computer that is the destination for the ICMP echo message.</param>
    /// <param name="timeout">An <see cref="Int32"/> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    public static IObservable<PingReply> SendObservable(this Ping ping, IPAddress address, int timeout)
    {
      Contract.Requires(ping != null);
      Contract.Requires(address != null);
      Contract.Requires(timeout >= 0);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return CreatePingObservable(ping, token => ping.SendAsync(address, timeout, token));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message to the specified computer, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation.
    /// </summary>
    /// <param name="ping">The object that sends the ICMP echo message.</param>
    /// <param name="hostNameOrAddress">A <see cref="String"/> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
    /// <param name="timeout">An <see cref="Int32"/> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    public static IObservable<PingReply> SendObservable(this Ping ping, string hostNameOrAddress, int timeout)
    {
      Contract.Requires(ping != null);
      Contract.Requires(!string.IsNullOrWhiteSpace(hostNameOrAddress));
      Contract.Requires(timeout >= 0);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return CreatePingObservable(ping, token => ping.SendAsync(hostNameOrAddress, timeout, token));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="IPAddress"/>, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation.
    /// </summary>
    /// <param name="ping">The object that sends the ICMP echo message.</param>
    /// <param name="address">An <see cref="IPAddress"/> that identifies the computer that is the destination for the ICMP echo message.</param>
    /// <param name="timeout">An <see cref="Int32"/> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
    /// <param name="buffer">A <see cref="Byte"/> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    public static IObservable<PingReply> SendObservable(this Ping ping, IPAddress address, int timeout, byte[] buffer)
    {
      Contract.Requires(ping != null);
      Contract.Requires(address != null);
      Contract.Requires(timeout >= 0);
      Contract.Requires(buffer != null);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return CreatePingObservable(ping, token => ping.SendAsync(address, timeout, buffer, token));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation.
    /// </summary>
    /// <param name="ping">The object that sends the ICMP echo message.</param>
    /// <param name="hostNameOrAddress">A <see cref="String"/> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
    /// <param name="timeout">An <see cref="Int32"/> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
    /// <param name="buffer">A <see cref="Byte"/> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    public static IObservable<PingReply> SendObservable(this Ping ping, string hostNameOrAddress, int timeout, byte[] buffer)
    {
      Contract.Requires(ping != null);
      Contract.Requires(!string.IsNullOrWhiteSpace(hostNameOrAddress));
      Contract.Requires(timeout >= 0);
      Contract.Requires(buffer != null);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return CreatePingObservable(ping, token => ping.SendAsync(hostNameOrAddress, timeout, buffer, token));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="IPAddress"/>, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation and control fragmentation and Time-to-Live values for the ICMP echo message packet.
    /// </summary>
    /// <param name="ping">The object that sends the ICMP echo message.</param>
    /// <param name="address">An <see cref="IPAddress"/> that identifies the computer that is the destination for the ICMP echo message.</param>
    /// <param name="timeout">An <see cref="Int32"/> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
    /// <param name="buffer">A <see cref="Byte"/> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
    /// <param name="options">A <see cref="PingOptions"/> object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    public static IObservable<PingReply> SendObservable(this Ping ping, IPAddress address, int timeout, byte[] buffer, PingOptions options)
    {
      Contract.Requires(ping != null);
      Contract.Requires(address != null);
      Contract.Requires(timeout >= 0);
      Contract.Requires(buffer != null);
      Contract.Requires(options != null);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return CreatePingObservable(ping, token => ping.SendAsync(address, timeout, buffer, options, token));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation and control fragmentation and Time-to-Live values for the ICMP packet.
    /// </summary>
    /// <param name="ping">The object that sends the ICMP echo message.</param>
    /// <param name="hostNameOrAddress">A <see cref="String"/> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
    /// <param name="timeout">An <see cref="Int32"/> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
    /// <param name="buffer">A <see cref="Byte"/> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
    /// <param name="options">A <see cref="PingOptions"/> object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    public static IObservable<PingReply> SendObservable(this Ping ping, string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options)
    {
      Contract.Requires(ping != null);
      Contract.Requires(!string.IsNullOrWhiteSpace(hostNameOrAddress));
      Contract.Requires(timeout >= 0);
      Contract.Requires(buffer != null);
      Contract.Requires(options != null);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return CreatePingObservable(ping, token => ping.SendAsync(hostNameOrAddress, timeout, buffer, options, token));
    }

    private static IObservable<PingReply> CreatePingObservable(Ping ping, Action<object> start)
    {
      Contract.Requires(ping != null);
      Contract.Requires(start != null);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return Observable2.FromEventBasedAsyncPattern<PingCompletedEventHandler, PingCompletedEventArgs>(
        h => h.Invoke,
        h => ping.PingCompleted += h,
        h => ping.PingCompleted -= h,
        start,
        ping.SendAsyncCancel)
        .Select(e => e.EventArgs.Reply);
    }
  }
}