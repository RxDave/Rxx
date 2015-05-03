using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace System.Windows.Input
{
  /// <summary>
  /// Provides <see langword="static" /> extension methods for <see cref="ICommand"/> objects.
  /// </summary>
  public static partial class CommandExtensions
  {
    /// <summary>
    /// Returns an observable sequence that indicates when the <paramref name="command"/> can be executed, 
    /// starting with the current state of the <paramref name="command"/>.
    /// </summary>
    /// <remarks>
    /// The observable is a sequence of values returned by <see cref="ICommand.CanExecute"/> 
    /// whenever the <paramref name="command"/> raises its <see cref="ICommand.CanExecuteChanged"/> event.
    /// </remarks>
    /// <param name="command">The <see cref="ICommand"/> to be observed.</param>
    /// <returns>An observable sequence that indicates when the <paramref name="command"/> can be executed, 
    /// starting with the current state of the <paramref name="command"/>.</returns>
    public static IObservable<bool> CanExecuteObservable(
      this ICommand command)
    {
      Contract.Requires(command != null);
      Contract.Ensures(Contract.Result<IObservable<bool>>() != null);

      return command.CanExecuteObservable(parameter: null);
    }

    /// <summary>
    /// Returns an observable sequence that indicates when the <paramref name="command"/> can be executed, 
    /// starting with the current state of the <paramref name="command"/>.
    /// </summary>
    /// <remarks>
    /// The observable is a sequence of values returned by <see cref="ICommand.CanExecute"/> 
    /// whenever the <paramref name="command"/> raises its <see cref="ICommand.CanExecuteChanged"/> event.
    /// </remarks>
    /// <param name="command">The <see cref="ICommand"/> to be observed.</param>
    /// <param name="parameter">The object that is passed to the <see cref="ICommand.CanExecute"/> method.
    /// This value can be <see langword="null"/> if the <paramref name="command"/> supports it.</param>
    /// <returns>An observable sequence that indicates when the <paramref name="command"/> can be executed, 
    /// starting with the current state of the <paramref name="command"/>.</returns>
    public static IObservable<bool> CanExecuteObservable(
      this ICommand command,
      object parameter)
    {
      Contract.Requires(command != null);
      Contract.Ensures(Contract.Result<IObservable<bool>>() != null);

      return Observable.FromEventPattern(
        handler => command.CanExecuteChanged += handler,
        handler => command.CanExecuteChanged -= handler)
        .Select(_ => command.CanExecute(parameter))
        .Publish(command.CanExecute(parameter))
        .RefCount()
        .DistinctUntilChanged();
    }
  }
}