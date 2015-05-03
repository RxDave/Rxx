using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive
{
  [DisplayName("Multicast")]
  [Description("Resetting a replayable sequence when there are no subscribers.")]
  public sealed class MulticastLab : BaseConsoleLab
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Disposal of the subject is unnecessary because all subscriptions are disposed when the lab ends.")]
    protected override void Main()
    {
      TraceDescription(Instructions.MulticastLab);

      int current = 0;

      IObservable<int> source = Observable
        .Interval(TimeSpan.FromSeconds(1))
        .Select(_ => current++)
        .Multicast(() => new ReplaySubject<int>())
        .RefCount();

      using (var disposables = new CompositeDisposable())
      {
        do
        {
          var key = WaitForKey();

          switch (key.KeyChar)
          {
            case '+':
            case '=':
              var id = (char)('A' + disposables.Count);

              var subscription = source.Subscribe(ConsoleOutput(id.ToString()));

              disposables.Add(subscription);
              break;
            case '-':
            case '_':
              if (disposables.Count > 0)
              {
                disposables.Remove(disposables.Last());
              }
              break;
            default:
              return;
          }
        }
        while (true);
      }
    }
  }
}
