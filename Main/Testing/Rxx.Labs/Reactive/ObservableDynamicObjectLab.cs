using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("Dynamic Observable")]
  [Description("Using ObservableDynamicObject to wrap a normal object's "
             + "properties, events and methods into observables.")]
  public sealed class ObservableDynamicObjectLab : BaseConsoleLab
  {
    // Note that Silverlight requires public accessibility for reflection
    public sealed class Poco
    {
      private string message;
      public string Message
      {
        get
        {
          return message;
        }
        set
        {
          message = value;

          System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));

          OnMessageChanged(EventArgs.Empty);
        }
      }

      public event EventHandler MessageChanged;
      private void OnMessageChanged(EventArgs e)
      {
        var messageChanged = MessageChanged;

        if (messageChanged != null)
          messageChanged(this, e);
      }

      public void Ping(int value)
      {
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));

        OnPong(new PongEventArgs(value));
      }

      public event EventHandler<PongEventArgs> Pong;
      private void OnPong(PongEventArgs e)
      {
        var pong = Pong;

        if (pong != null)
          pong(this, e);
      }

      public int Calculate(string value)
      {
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));

        return value.GetHashCode();
      }
    }

    // Note that Silverlight requires public accessibility for reflection
    public sealed class PongEventArgs : EventArgs
    {
      public int Value { get; private set; }

      public PongEventArgs(int value)
      {
        this.Value = value;
      }
    }

    protected override void Main()
    {
      TraceLine(Instructions.PressAnyKeyToCancel);

      // Start with a typical object...
      Poco obj = new Poco();

      // and wrap it with a dynamic proxy.
      dynamic asyncObj = ObservableDynamicObject.Create(obj);

      // Properties, events and methods will now return observables.

      IObservable<string> messageChanged = asyncObj.Message;

      IObservable<EventPattern<PongEventArgs>> pongRaised = asyncObj.Pong;

      IObservable<int> pong = pongRaised.Select(e => e.EventArgs.Value);

      using (messageChanged.Subscribe(ConsoleOutput("Message changed")))
      using (pong.Subscribe(ConsoleOutput("Pong")))
      {
        TraceLine(Text.ObservableDynamicObjectLabAssigningMessage);

        asyncObj.Message = "New Message";

        TraceLine(Text.ObservableDynamicObjectLabCallingPing);

        IObservable<Unit> ping = asyncObj.Ping(12345);

        TraceLine(Text.ObservableDynamicObjectLabCallingCalculate);

        IObservable<int> calculate = asyncObj.Calculate("Hello World");

        using (ping.Subscribe(ConsoleOutput("Ping")))
        using (calculate.Subscribe(ConsoleOutput("Calculate")))
        {
          WaitForKey();
        }
      }
    }
  }
}
