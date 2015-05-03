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
  [DisplayName("TCP Server (New)")]
  [Description("Creating a TCP server with ObservableTcpListener.")]
  public sealed class TcpServerLab : BaseConsoleLab
  {
    protected override void Main()
    {
      var endPoint = new IPEndPoint(IPAddress.Loopback, 15005);

      IObservable<int> clientQuery = Observable.Using(
        () => new TcpClient(),
        client => from _ in client.ConnectObservable(endPoint.Address, endPoint.Port)
                  let socket = client.Client
                  let message = Encoding.UTF8.GetBytes(Text.Rock + Text.Scissors + Text.Paper)
                  from __ in socket.SendUntilCompleted(message, 0, message.Length, SocketFlags.None)
                  select message.Length);

      IObservable<string> serverQuery =
        from client in ObservableTcpListener.Start(endPoint)
          .Synchronize()
          .Take(1)
          .Do(_ =>
          {
            TraceLine(Text.ServerReceivedRequest);

            PressAnyKeyToContinue();
          })
        let stream = new NetworkStream(client.Client, ownsSocket: false)
        from buffer in Observable.Using(
         () => client,
         _ => stream.ReadToEndObservable().SelectMany(b => b).ToArray())
        let message = Encoding.UTF8.GetString(buffer)
        select message;

      using (var responded = new System.Threading.ManualResetEvent(false))
      using (serverQuery.Finally(() => responded.Set()).Subscribe(ConsoleOutput(Text.Server)))
      using (clientQuery.Subscribe(ConsoleOutputFormat(Text.Client, Text.SentBytesFormat)))
      {
        responded.WaitOne();
      }
    }
  }
}