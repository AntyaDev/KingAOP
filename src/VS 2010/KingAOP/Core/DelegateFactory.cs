// Copyright (c) 2013 Antya Dev
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KingAOP.Core
{
    internal delegate object LateBoundFunction(object[] arguments);
    internal delegate void LateBoundCall(object[] arguments);

    internal class DelegateFactory
    {
        public static LateBoundFunction CreateFunction(object instance, MethodInfo method)
        {
            ParameterExpression args = Expression.Parameter(typeof(object[]), "arguments");

            MethodCallExpression call = Expression.Call(
              Expression.Constant(instance),
              method,
              CreateParameterExpressions(method, args));

            return Expression.Lambda<LateBoundFunction>(Expression.Convert(call, typeof(object)), args).Compile();
        }

        public static LateBoundCall CreateMethodCall(object instance, MethodInfo method)
        {
            ParameterExpression args = Expression.Parameter(typeof(object[]), "arguments");

            MethodCallExpression call = Expression.Call(
              Expression.Constant(instance),
              method,
              CreateParameterExpressions(method, args));

            return Expression.Lambda<LateBoundCall>(call, args).Compile();
        }

        private static Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            return method.GetParameters().Select((parameter, index) =>
              Expression.Convert(
                Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)), parameter.ParameterType)).ToArray();
        }
    }
}
