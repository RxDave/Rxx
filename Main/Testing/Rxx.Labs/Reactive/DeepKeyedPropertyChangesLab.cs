using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Rxx.Labs.Reactive
{
  // http://social.msdn.microsoft.com/Forums/en/rx/thread/0fbf8cdb-0d91-41ae-bca6-ef28cad39896
  [DisplayName("Deep Keyed Property Changes")]
  [Description("Observing deep property changes from an object heirarchy in a DictionarySubject.")]
  public sealed class DeepKeyedPropertyChangesLab : BaseConsoleLab
  {
    protected override void Main()
    {
      var source = new DictionarySubject<string, CustomValue>();

      var monitorKeys = new[] { "A", "C", "D", "E", "F", "G" };

      // PropertyChanges doesn't work on KeyValuePair because it doesn't have any property change events.
      // So we must strip off the key and create a sequence of collection notifications for the values only.
      using (source
        .Where(
          exists: _ => false,
          onAdded: added => monitorKeys.Contains(added.Key),
          onReplaced: (replaced, added) => monitorKeys.Contains(replaced.Key) && monitorKeys.Contains(added.Key),
          onRemoved: removed => monitorKeys.Contains(removed.Key),
          onCleared: () => false)
        .Select(
          _ => null,
          added => CollectionNotification.CreateOnAdded(added.Value),
          (replaced, added) => CollectionNotification.CreateOnReplaced(replaced.Value, added.Value),
          removed => CollectionNotification.CreateOnRemoved(removed.Value),
          () => null)
        .PropertyChanges()
        .Select(e => new { Object = (CustomValue)e.Sender, Property = e.EventArgs.PropertyName })
        .Subscribe(value => TraceLine("Changed {0}: {1}", value.Property, value.Object)))
      {
        var a = new CustomValue("A");
        var b = new CustomValue("B");
        var c = new CustomValue("C");

        source.Add(a.Name, a);
        source.Add(b.Name, b);
        source.Add(c.Name, c);

        a.Child = new CustomValue("D");
        b.Child = new CustomValue("E");
        c.Child = new CustomValue("F");

        a.Child.Child = new CustomValue("G");

        a.Number = 1;
        b.Number = 1;
        c.Number = 1;

        a.Child.Number = 100;
        b.Child.Number = 101;
        c.Child.Number = 102;

        var child = a.Child;
        var grandchild = child.Child;

        grandchild.Number = 103;

        child.Child = null;

        grandchild.Number = 104;

        child.Number = 105;

        a.Child = null;

        child.Number = 106;

        source.Remove(a.Name);

        a.Number = 2;
        b.Number = 2;
        c.Number = 2;

        child.Number = 200;
        b.Child.Number = 201;
        c.Child.Number = 202;

        grandchild.Number = 203;

        source.Remove(b.Name);
        source.Remove(c.Name);

        a.Number = 4;
        b.Number = 4;
        c.Number = 4;

        child.Number = 300;
        b.Child.Number = 301;
        c.Child.Number = 302;

        grandchild.Number = 303;
      }
    }

    private sealed class CustomValue : NotifyPropertyChanged
    {
      public string Name
      {
        get;
        private set;
      }

      public int Number
      {
        get
        {
          return number;
        }
        set
        {
          SetProperty(ref number, value);
        }
      }

      public CustomValue Child
      {
        get
        {
          return child;
        }
        set
        {
          SetProperty(ref child, value);
        }
      }

      private int number;
      private CustomValue child;

      public CustomValue(string name)
      {
        Name = name;
      }

      public override string ToString()
      {
        return Name + "=" + number + " [" + (child == null ? null : child.ToString()) + "]";
      }
    }

    private abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
      public event PropertyChangedEventHandler PropertyChanged;

      protected void SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
      {
        if (!object.Equals(backingField, value))
        {
          backingField = value;
          OnPropertyChanged(propertyName);
        }
      }

      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
      }

      protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
      {
        var handler = PropertyChanged;

        if (handler != null)
        {
          handler(this, e);
        }
      }
    }
  }
}