using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive.Networking
{
  [DisplayName("Sockets (Updated)")]
  [Description("Establishing a socket connection and transmitting data.")]
  public sealed class SocketLab : BaseConsoleLab
  {
    protected override void Main()
    {
      string response = UserInput(Text.PromptFormat, Instructions.EnterTheResponse);

      byte[] responseBytes = Encoding.Unicode.GetBytes(response);

      TraceLine(Instructions.PressAnyKeyToCancel);

      var serverAddress = new IPEndPoint(IPAddress.Loopback, 15007);

      IObservable<int> server =
        from socket in ObservableSocket.Accept(
          AddressFamily.InterNetwork,
          SocketType.Stream,
          ProtocolType.Tcp,
          serverAddress,
          count: 1)
        from _ in Observable.Using(
          () => socket,
          _ => socket.SendUntilCompleted(responseBytes, 0, responseBytes.Length, SocketFlags.None))
        select responseBytes.Length;

      IObservable<string> client =
        from socket in ObservableSocket.Connect(
          AddressFamily.InterNetwork,
          SocketType.Stream,
          ProtocolType.Tcp,
          serverAddress)
        from message in Observable.Using(
          () => socket,
          _ => Observable.Using(
            () => new MemoryStream(),
            stream => socket
              .ReceiveUntilCompleted(SocketFlags.None)
              .Do(buffer => stream.Write(buffer, 0, buffer.Length))
              .TakeLast(1)
              .Select(__ => Encoding.Unicode.GetString(stream.ToArray()))))
        select message;

      // Ensure that the console output is thread-safe
      var gate = new object();
      server = server.Synchronize(gate);
      client = client.Synchronize(gate);

      using (server.Subscribe(ConsoleOutputFormat(Text.Server, Text.SentBytesFormat)))
      using (client.Subscribe(ConsoleOutput(Text.Client)))
      {
        WaitForKey();
      }
    }
  }
}