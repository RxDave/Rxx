using System.Diagnostics.Contracts;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Constructs an observable sequence that depends on a resource object, whose lifetime is tied to the lifetime of the hot observable that is returned 
    /// by <paramref name="hotObservableFactory"/>. The resource is disposed immediately upon termination of the observable, regardless of whether the 
    /// observable returned by this operator has any subscriptions.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the produced sequence.</typeparam>
    /// <typeparam name="TResource">The type of the resource used during the generation of the resulting sequence. Needs to implement <see cref="IDisposable"/>.</typeparam>
    /// <param name="resource">The object whose lifetime is tied to the lifetime of the hot observable that is returned by <paramref name="hotObservableFactory"/>.</param>
    /// <param name="hotObservableFactory">Asynchronous factory function to obtain a hot observable sequence that depends on the obtained resource.</param>
    /// <returns>An observable sequence whose lifetime is entirely independent of the lifetime of the specified <paramref name="resource"/> object.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "Returned in the observable.")]
    public static IObservable<TResult> UsingHot<TResult, TResource>(TResource resource, Func<TResource, IObservable<TResult>> hotObservableFactory)
      where TResource : IDisposable
    {
      Contract.Requires(resource != null);
      Contract.Requires(hotObservableFactory != null);
      Contract.Ensures(Contract.Result<IObservable<TResult>>() != null);

      try
      {
        var observable = hotObservableFactory(resource);

        observable.Finally(resource.Dispose).Subscribe(
          _ => { },
          ex => { });		// Safe to ignore because the observable is hot; i.e., all observers receive the same error.

        return observable;
      }
      catch (Exception ex)
      {
        resource.Dispose();

        return Observable.Throw<TResult>(ex);
      }
    }
  }
}