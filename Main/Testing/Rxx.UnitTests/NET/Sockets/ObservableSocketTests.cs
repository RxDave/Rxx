using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rxx.UnitTests.Net.Sockets
{
  [TestClass]
  public class ObservableSocketTests : RxxTests
  {
    /// <summary>
    /// Origin of unit test: 
    /// http://rxx.codeplex.com/workitem/23871
    /// </summary>
    [TestMethod, TestCategory("Rx"), TestCategory("Sockets")]
    public void AcceptAutomatic()
    {
      var address = new IPEndPoint(IPAddress.Loopback, 20154);
      var sockets = ObservableSocket.Accept(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp, address, 5);
      var connect = Observable.Defer(() => ObservableSocket.Connect(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp, address));

      var results = new ConcurrentBag<Socket>();
      var errors = new ConcurrentBag<Exception>();
      var completed = false;
      var completions = 0;
      Action addCompletion = () => Interlocked.Increment(ref completions);

      using (var wait = new ManualResetEventSlim())
      using (sockets.Subscribe(results.Add, errors.Add, wait.Set))
      using (connect.Subscribe(results.Add, errors.Add, addCompletion))
      using (connect.Subscribe(results.Add, errors.Add, addCompletion))
      using (connect.Subscribe(results.Add, errors.Add, addCompletion))
      using (connect.Subscribe(results.Add, errors.Add, addCompletion))
      using (connect.Subscribe(results.Add, errors.Add, addCompletion))
      {
        completed = wait.Wait(TimeSpan.FromSeconds(10));
      }

      Assert.AreEqual(0, errors.Count, string.Join(Environment.NewLine, errors));
      Assert.IsTrue(completed, "Timed out before completion.");
      Assert.AreEqual(10, results.Count);
      Assert.AreEqual(5, completions);
    }

    [TestMethod, TestCategory("Rx"), TestCategory("Sockets")]
    public void AcceptNew()
    {
      var address = new IPEndPoint(IPAddress.Loopback, 20155);
      var acceptSockets = new List<Socket>()
      {
        new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp),
        new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp),
        new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp),
        new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp),
        new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
      };
      var currentAcceptSocket = 0;
      var sockets = ObservableSocket.Accept(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp, address, 5, () => acceptSockets[currentAcceptSocket++]);
      var connect = Observable.Defer(() => ObservableSocket.Connect(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp, address));

      var results = new ConcurrentBag<Socket>();
      var errors = new ConcurrentBag<Exception>();
      var completed = false;
      var completions = 0;
      Action addCompletion = () => Interlocked.Increment(ref completions);

      using (var wait = new ManualResetEventSlim())
      using (sockets.Subscribe(results.Add, errors.Add, wait.Set))
      using (connect.Subscribe(results.Add, errors.Add, addCompletion))
      using (connect.Subscribe(results.Add, errors.Add, addCompletion))
      using (connect.Subscribe(results.Add, errors.Add, addCompletion))
      using (connect.Subscribe(results.Add, errors.Add, addCompletion))
      using (connect.Subscribe(results.Add, errors.Add, addCompletion))
      {
        completed = wait.Wait(TimeSpan.FromSeconds(10));
      }

      Assert.AreEqual(0, errors.Count, string.Join(Environment.NewLine, errors));
      Assert.IsTrue(completed, "Timed out before completion.");
      Assert.AreEqual(10, results.Count);
      Assert.AreEqual(5, currentAcceptSocket);
      Assert.AreEqual(5, completions);

      foreach (var socket in acceptSockets)
      {
        CollectionAssert.Contains(results, socket);
      }
    }
  }
}