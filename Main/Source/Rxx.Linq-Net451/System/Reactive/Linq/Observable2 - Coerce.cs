using System.Diagnostics.Contracts;
using System.Linq;
#if PORT_45 || UNIVERSAL
using System.Reflection;
#endif

namespace System.Reactive.Linq
{
  /// <summary>
  /// Provides a set of <see langword="static"/> methods for query operations over observable sequences.
  /// </summary>
  public static partial class Observable2
  {
    [ContractVerification(false)]
    internal static object Coerce(this IObservable<object> source, Type targetElementType)
    {
      Contract.Requires(source != null);
      Contract.Requires(targetElementType != null);
      Contract.Ensures(Contract.Result<object>() != null);

      return Activator.CreateInstance(
        typeof(CoercingObservable<,>).MakeGenericType(typeof(object), targetElementType),
        source);
    }

    [ContractVerification(false)]
    internal static object Coerce(this IObservable<EventPattern<EventArgs>> source, Type targetEventArgsType)
    {
      Contract.Requires(source != null);
      Contract.Requires(targetEventArgsType != null);
      Contract.Ensures(Contract.Result<object>() != null);

      return Activator.CreateInstance(
        typeof(EventCoercingObservable<,>).MakeGenericType(typeof(EventArgs), targetEventArgsType),
        source);
    }

    /// <summary>
    /// Coerces the specified object into an observable sequence if it implements <see cref="IObservable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    /// <remarks>
    /// <para>
    /// When a value type is used as the <strong>T</strong> parameter for <see cref="IObservable{T}"/>, then the interface
    /// cannot be treated as contra-variant on <strong>T</strong> even though any <srong>T</srong> can be treated as an <see cref="object"/>.
    /// Casting to <strong>IObservable&lt;object&gt;</strong> throws an exception at runtime.  This makes it somewhat difficult to 
    /// acquire an untyped reference to the object as an observable sequence of objects, hence the purpose of <see cref="Coerce(object)"/>.
    /// </para>
    /// </remarks>
    /// <param name="source">An object that may or may not implement <see cref="IObservable{T}"/>, or <see langword="null"/>.</param>
    /// <returns>The specified object as an observable sequence if it implements <see cref="IObservable{T}"/>; otherwise, 
    /// <see langword="null"/>.</returns>
    internal static IObservable<T> Coerce<T>(object source)
    {
      var observable = source as IObservable<T>;

      if (observable != null)
      {
        return observable;
      }

      if (source != null)
      {
#if !PORT_45 && !UNIVERSAL
        var type = source.GetType();
        var interfaces = type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IObservable<>));
#else
        var type = source.GetType().GetTypeInfo();
        var interfaces = type.ImplementedInterfaces.Where(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(IObservable<>));
#endif

        foreach (var observableType in interfaces)
        {
          Contract.Assume(observableType != null);

#if !PORT_45 && !UNIVERSAL
          var arguments = observableType.GetGenericArguments();
#else
          var arguments = observableType.GenericTypeArguments;
#endif

          Contract.Assume(arguments.Length > 0);

          Type elementType = arguments[0];

#if !PORT_45 && !UNIVERSAL
          if (typeof(T).IsAssignableFrom(elementType))
#else
          if (typeof(T).GetTypeInfo().IsAssignableFrom(elementType.GetTypeInfo()))
#endif
          {
#if PORT_40
            var map = type.GetInterfaces().First(i => i == observableType);
            var methods = map.GetMethods();

            Contract.Assume(methods != null);
            Contract.Assume(methods.Length > 0);

            var subscribeDefinition = methods[0];
            var subscribe = observableType.GetMethods().First(m => Rxx.ComponentReflection.IsInterfaceMap(m, subscribeDefinition));
#else
#if !PORT_45 && !UNIVERSAL
            var map = type.GetInterfaceMap(observableType);
#else
            var map = type.GetRuntimeInterfaceMap(observableType);
#endif

            Contract.Assume(map.TargetMethods != null);
            Contract.Assume(map.TargetMethods.Length > 0);

            var subscribe = map.TargetMethods[0];
#endif

            return Observable.Create<T>(
              observer =>
              {
                var coerced = Activator.CreateInstance(
                  typeof(CoercingObserver<,>).MakeGenericType(elementType, typeof(T)),
                  observer);

                return (IDisposable)subscribe.Invoke(source, new[] { coerced });
              });
          }
        }
      }

      return null;
    }

    [ContractVerification(false)]
    internal static IObserver<T> CoerceObserver<T>(object target)
    {
      var observer = target as IObserver<T>;

      if (observer != null)
      {
        return observer;
      }

      if (target != null)
      {
#if !PORT_45 && !UNIVERSAL
        var type = target.GetType();
        var interfaces = type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IObserver<>));
#else
        var type = target.GetType().GetTypeInfo();
        var interfaces = type.ImplementedInterfaces.Where(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(IObserver<>));
#endif

        foreach (var observerType in interfaces)
        {
          Contract.Assume(observerType != null);

#if !PORT_45 && !UNIVERSAL
          var arguments = observerType.GetGenericArguments();
#else
          var arguments = observerType.GenericTypeArguments;
#endif

          Contract.Assume(arguments.Length > 0);

          Type elementType = arguments[0];

#if !PORT_45 && !UNIVERSAL
          if (typeof(T).IsAssignableFrom(elementType))
#else
          if (typeof(T).GetTypeInfo().IsAssignableFrom(elementType.GetTypeInfo()))
#endif
          {
            return (IObserver<T>)Activator.CreateInstance(
              typeof(CoercingObserver<,>).MakeGenericType(typeof(T), elementType),
              target);
          }
        }
      }

      return null;
    }
  }
}