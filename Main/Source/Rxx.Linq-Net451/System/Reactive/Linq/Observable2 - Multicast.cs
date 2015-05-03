using System.Diagnostics.Contracts;
using System.Reactive.Subjects;

namespace System.Reactive.Linq
{
  public static partial class Observable2
  {
    /// <summary>
    /// Returns a connectable observable sequence that upon connection causes the <paramref name="source"/> sequence to 
    /// push results into a new fresh subject, which is created by invoking the specified <paramref name="factory"/> function 
    /// each time that a connection is disposed and a new connection is made.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="source">The source sequence whose elements will be pushed into the specified subject.</param>
    /// <param name="factory">The factory function used to create the subject that notifications will be pushed into.</param>
    /// <returns>A connectable observable sequence that upon connection causes the source sequence to push results into a new subject.</returns>
    public static IConnectableObservable<TResult> Multicast<TSource, TResult>(this IObservable<TSource> source, Func<ISubject<TSource, TResult>> factory)
    {
      Contract.Requires(source != null);
      Contract.Requires(factory != null);
      Contract.Ensures(Contract.Result<IConnectableObservable<TResult>>() != null);

      return new ReconnectableObservable<TSource, TResult>(source, factory);
    }
  }
}
