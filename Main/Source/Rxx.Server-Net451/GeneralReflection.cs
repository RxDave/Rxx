using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Rxx
{
  internal static class GeneralReflection
  {
    public static Type MakeGenericType(Type type, params Type[] typeArguments)
    {
      Contract.Requires(type != null);
      Contract.Requires(typeArguments != null);
      Contract.Ensures(Contract.Result<Type>() != null);

      Contract.Assume(type.IsGenericTypeDefinition);
      Contract.Assume(typeArguments.Length == type.GetGenericArguments().Length);

      return type.MakeGenericType(typeArguments);
    }

    public static MethodInfo GetMethodWithParamsOfGenericArgs(Type type, string methodName, params Type[] parameterTypes)
    {
      Contract.Requires(type != null);
      Contract.Requires(methodName != null);
      Contract.Requires(parameterTypes != null);
      Contract.Ensures(Contract.Result<MethodInfo>() != null);

      if (type.ContainsGenericParameters)
      {
        throw new InvalidOperationException();
      }

      foreach (var method in from MethodInfo method in type.GetMember(methodName, MemberTypes.Method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                             where method.GetParameters().Select(p => Normalize(p.ParameterType)).SequenceEqual(parameterTypes.Select(Normalize))
                             select method)
      {
        Contract.Assume(method != null);

        var failedMatch = false;

        var parameters = method.GetParameters();

        Contract.Assume(parameters.Length == parameterTypes.Length);

        for (int i = 0; i < parameters.Length; i++)
        {
          var param = parameters[i];

          Contract.Assume(param != null);

          var paramType = param.ParameterType;

          if (parameterTypes[i] != paramType)
          {
            Contract.Assume(paramType.IsGenericType);

            var args = paramType.GetGenericArguments();

            for (int a = 0; a < args.Length; a++)
            {
              var arg = args[a];

              Contract.Assume(arg != null);

              if (!arg.IsGenericParameter)
              {
                failedMatch = true;
                break;
              }
            }

            if (failedMatch)
            {
              break;
            }
          }
        }

        if (!failedMatch)
        {
          return method;
        }
      }

      throw new InvalidOperationException();
    }

    private static Type Normalize(Type type)
    {
      Contract.Requires(type != null);
      Contract.Ensures(Contract.Result<Type>() != null);

      return type.IsGenericTypeDefinition || !type.IsGenericType ? type : type.GetGenericTypeDefinition();
    }
  }
}