using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive.Networking
{
  [DisplayName("Web Socket Hosting")]
  [Description("Serving web requests over web sockets using ObservableWebClient and ObservableHttpListener.")]
  public sealed class WebSocketLab : BaseConsoleLab
  {
    protected override void Main()
    {
      if (Environment.OSVersion.Version < new Version(6, 2))
      {
        TraceError(Text.LabRequiresWindows8OrHigher);
        return;
      }

      const int port = 5494;
      string subProtocol = GetType().Name;

      var userMessages = new BehaviorSubject<string>(null);

      var client = new ClientWebSocket();

      client.Options.AddSubProtocol(subProtocol);

      using (client)
      using (var cancel = new CancellationDisposable())
      using (ObservableHttpListener
        .StartWebSockets(new IPEndPoint(IPAddress.Loopback, port), subProtocol)
        .Subscribe(
          async request =>
          {
            using (var socket = request.WebSocket)
            {
              try
              {
                var message = await ReceiveMessageAsync(socket, cancel.Token);
                await SendMessageAsync("You sent \"" + message + "\"", socket, cancel.Token);
                await ReceiveCloseMessageAsync(socket, cancel.Token);
              }
              catch (OperationCanceledException)
              {
              }
            }
          },
          TraceError))
      using ((from _ in client.ConnectAsync(new Uri("ws://localhost:" + port), cancel.Token).ToObservable()
                .Do(_ => TraceLine(Environment.NewLine + "(Connected to host on sub-protocol \"{0}\")", client.SubProtocol))
              from message in userMessages.Where(m => m != null).Take(1)
              from __ in SendMessageAsync(message, client, cancel.Token).ToObservable()
              from response in ReceiveMessageAsync(client, cancel.Token).ToObservable()
                .Do(___ => client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", cancel.Token))
              select response)
              .Subscribe(
                response => TraceLine("Response: {0}", response),
                TraceError,
                () => TraceLine("{0}: {1}", Text.Client, Text.Done)))
      {
        userMessages.OnNext(UserInput("{0}> ", Instructions.EnterAMessage));

        TraceStatus(Instructions.PressAnyKeyToCancel);

        WaitForKey();
      }
    }

    private static async Task<string> ReceiveMessageAsync(WebSocket socket, CancellationToken cancel)
    {
      using (var stream = new MemoryStream())
      {
        var buffer = ClientWebSocket.CreateServerBuffer(1024);
        WebSocketReceiveResult received;

        do
        {
          received = await socket.ReceiveAsync(buffer, cancel);

          stream.Write(buffer.Array, buffer.Offset, received.Count);
        }
        while (!received.EndOfMessage);

        stream.Position = 0;

        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
          return reader.ReadToEnd();
        }
      }
    }

    private static async Task ReceiveCloseMessageAsync(WebSocket socket, CancellationToken cancel)
    {
      var buffer = ClientWebSocket.CreateServerBuffer(1024);
      var received = await socket.ReceiveAsync(buffer, cancel);

      if (received.MessageType != WebSocketMessageType.Close)
      {
        throw new InvalidOperationException();
      }
    }

    private static Task SendMessageAsync(string message, WebSocket socket, CancellationToken cancel)
    {
      var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));

      return socket.SendAsync(buffer, WebSocketMessageType.Text, true, cancel);
    }
  }
}