using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive.UI
{
  [DisplayName("Properties and Events")]
  [Description("Observing DependencyProperty changes and RoutedEvent notifications.")]
  public partial class PropertiesAndEventsLab : BaseLab
  {
    public PropertiesAndEventsLab()
    {
      InitializeComponent();

#if SILVERLIGHT
			/* Due to the limited capabilities of DependencyProperty in Silverlight, 
			 * the DependencyPropertyChanged extension method is not available to 
			 * Silverlight and Windows Phone applications.
			 */
			EnterTarget.Visibility = System.Windows.Visibility.Collapsed;
			LayoutRoot.RowDefinitions[0].Height = new GridLength(0);
#endif
    }

#if SILVERLIGHT
		protected override IEnumerable<IDisposable> Main()
		{
			yield return ClickTarget
				.RoutedEventRaised<MouseButtonEventHandler, MouseButtonEventArgs>(UIElement.MouseLeftButtonUpEvent)
				.Select(e => e.EventArgs.GetPosition(ClickTarget))
				.Subscribe(ConsoleOutput(Text.Clicked));
		}
#else
    protected override IEnumerable<IDisposable> Main()
    {
      yield return ClickTarget
        .RoutedEventRaised<MouseEventArgs>(UIElement.MouseLeftButtonUpEvent)
        .Select(e => e.EventArgs.GetPosition(ClickTarget))
        .Subscribe(ConsoleOutput(Text.Clicked));

      yield return EnterTarget
        .DependencyPropertyChanged<bool>(UIElement.IsMouseOverProperty)
        .Subscribe(ConsoleOutput(Text.Entered));
    }
#endif
  }
}