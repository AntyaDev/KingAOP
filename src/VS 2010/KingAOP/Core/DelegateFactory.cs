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

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KingAOP.Core
{
    internal delegate object LateBoundFunc(object[] arguments);
    internal delegate void LateBoundAction(object[] arguments);
    internal delegate void LateBoundSetter(object value);
    internal delegate object LateBoundGetter();

    /// <summary>
    /// This factory creates delegate to call any method.
    /// </summary>
    internal class DelegateFactory
    {
        /// <summary>
        /// Creates a delegate to wrap a "function" (method which has return value).
        /// </summary>
        public static LateBoundFunc CreateFunction(object instance, MethodInfo method)
        {
            ParameterExpression args = Expression.Parameter(typeof(object[]), "arguments");
            MethodCallExpression call = Expression.Call(
                Expression.Constant(instance),
                method,
                CreateParameterExpressions(method, args));
            return Expression.Lambda<LateBoundFunc>(Expression.Convert(call, typeof(object)), args).Compile();
        }

        /// <summary>
        /// Creates a delegate to wrap a "siple method call" (method which has not return value, just returns void).
        /// </summary>
        public static LateBoundAction CreateMethodCall(object instance, MethodInfo method)
        {
            ParameterExpression args = Expression.Parameter(typeof(object[]), "arguments");
            MethodCallExpression call = Expression.Call(
                Expression.Constant(instance),
                method,
                CreateParameterExpressions(method, args));
            return Expression.Lambda<LateBoundAction>(call, args).Compile();
        }

        public static LateBoundSetter CreateSetter(object instance, MethodInfo method)
        {
            ParameterExpression value = Expression.Parameter(typeof(object), "value");
            MethodCallExpression call = Expression.Call(
                Expression.Constant(instance),
                method,
                Expression.Convert(value, method.GetParameters()[0].ParameterType));
            return Expression.Lambda<LateBoundSetter>(call, value).Compile();
        }

        public static LateBoundGetter CreateGetter(object instance, MethodInfo method)
        {
            MethodCallExpression call = Expression.Call(Expression.Constant(instance), method);
            return Expression.Lambda<LateBoundGetter>(Expression.Convert(call, typeof(object))).Compile();
        }

        public static Delegate CreateDelegate(object instance, MethodInfo method)
        {
            ParameterExpression[] argsExp = method.GetParameters().Select(
                arg => Expression.Parameter(arg.ParameterType, arg.Name)).ToArray();
            MethodCallExpression call = Expression.Call(Expression.Constant(instance), method, argsExp);
            return Expression.Lambda(call, argsExp).Compile();
        }

        private static Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            return method.GetParameters().Select((parameter, index) =>
              Expression.Convert(
                Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)), parameter.ParameterType)).ToArray();
        }
    }
}
