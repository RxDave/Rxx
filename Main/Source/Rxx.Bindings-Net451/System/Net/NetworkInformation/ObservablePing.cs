using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Net.NetworkInformation
{
  /// <summary>
  /// Provides <see langword="static"/> methods for asynchronously determining whether a remote computer is accessible over the network.
  /// </summary>
  public static class ObservablePing
  {
    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message to the computer that has the specified <see cref="IPAddress"/>, and receive a corresponding ICMP echo reply message from that computer.
    /// </summary>
    /// <param name="address">An <see cref="IPAddress"/> that identifies the computer that is the destination for the ICMP echo message.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Ping is composited by the UsingHot operator.")]
    public static IObservable<PingReply> Send(IPAddress address)
    {
      Contract.Requires(address != null);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return Observable2.UsingHot(new Ping(), ping => ping.SendObservable(address));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message to the specified computer, and receive a corresponding ICMP echo reply message from that computer.
    /// </summary>
    /// <param name="hostNameOrAddress">A <see cref="String"/> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Ping is composited by the UsingHot operator.")]
    public static IObservable<PingReply> Send(string hostNameOrAddress)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(hostNameOrAddress));
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return Observable2.UsingHot(new Ping(), ping => ping.SendObservable(hostNameOrAddress));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message to the computer that has the specified <see cref="IPAddress"/>, and receive a corresponding ICMP echo reply message from that computer.
    /// </summary>
    /// <param name="address">An <see cref="IPAddress"/> that identifies the computer that is the destination for the ICMP echo message.</param>
    /// <param name="timeout">An <see cref="Int32"/> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Ping is composited by the UsingHot operator.")]
    public static IObservable<PingReply> Send(IPAddress address, int timeout)
    {
      Contract.Requires(address != null);
      Contract.Requires(timeout >= 0);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return Observable2.UsingHot(new Ping(), ping => ping.SendObservable(address, timeout));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message to the specified computer, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation.
    /// </summary>
    /// <param name="hostNameOrAddress">A <see cref="String"/> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
    /// <param name="timeout">An <see cref="Int32"/> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Ping is composited by the UsingHot operator.")]
    public static IObservable<PingReply> Send(string hostNameOrAddress, int timeout)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(hostNameOrAddress));
      Contract.Requires(timeout >= 0);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return Observable2.UsingHot(new Ping(), ping => ping.SendObservable(hostNameOrAddress, timeout));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="IPAddress"/>, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation.
    /// </summary>
    /// <param name="address">An <see cref="IPAddress"/> that identifies the computer that is the destination for the ICMP echo message.</param>
    /// <param name="timeout">An <see cref="Int32"/> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
    /// <param name="buffer">A <see cref="Byte"/> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Ping is composited by the UsingHot operator.")]
    public static IObservable<PingReply> Send(IPAddress address, int timeout, byte[] buffer)
    {
      Contract.Requires(address != null);
      Contract.Requires(timeout >= 0);
      Contract.Requires(buffer != null);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return Observable2.UsingHot(new Ping(), ping => ping.SendObservable(address, timeout, buffer));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation.
    /// </summary>
    /// <param name="hostNameOrAddress">A <see cref="String"/> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
    /// <param name="timeout">An <see cref="Int32"/> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
    /// <param name="buffer">A <see cref="Byte"/> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Ping is composited by the UsingHot operator.")]
    public static IObservable<PingReply> Send(string hostNameOrAddress, int timeout, byte[] buffer)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(hostNameOrAddress));
      Contract.Requires(timeout >= 0);
      Contract.Requires(buffer != null);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return Observable2.UsingHot(new Ping(), ping => ping.SendObservable(hostNameOrAddress, timeout, buffer));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="IPAddress"/>, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation and control fragmentation and Time-to-Live values for the ICMP echo message packet.
    /// </summary>
    /// <param name="address">An <see cref="IPAddress"/> that identifies the computer that is the destination for the ICMP echo message.</param>
    /// <param name="timeout">An <see cref="Int32"/> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
    /// <param name="buffer">A <see cref="Byte"/> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
    /// <param name="options">A <see cref="PingOptions"/> object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Ping is composited by the UsingHot operator.")]
    public static IObservable<PingReply> Send(IPAddress address, int timeout, byte[] buffer, PingOptions options)
    {
      Contract.Requires(address != null);
      Contract.Requires(timeout >= 0);
      Contract.Requires(buffer != null);
      Contract.Requires(options != null);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return Observable2.UsingHot(new Ping(), ping => ping.SendObservable(address, timeout, buffer, options));
    }

    /// <summary>
    /// Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation and control fragmentation and Time-to-Live values for the ICMP packet.
    /// </summary>
    /// <param name="hostNameOrAddress">A <see cref="String"/> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
    /// <param name="timeout">An <see cref="Int32"/> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
    /// <param name="buffer">A <see cref="Byte"/> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
    /// <param name="options">A <see cref="PingOptions"/> object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
    /// <returns>A hot observable containing a <see cref="PingReply"/> instance that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
    [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Ping is composited by the UsingHot operator.")]
    public static IObservable<PingReply> Send(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options)
    {
      Contract.Requires(!string.IsNullOrWhiteSpace(hostNameOrAddress));
      Contract.Requires(timeout >= 0);
      Contract.Requires(buffer != null);
      Contract.Requires(options != null);
      Contract.Ensures(Contract.Result<IObservable<PingReply>>() != null);

      return Observable2.UsingHot(new Ping(), ping => ping.SendObservable(hostNameOrAddress, timeout, buffer, options));
    }
  }
}