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
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using KingAOP.Aspects;
using System.Linq;

namespace KingAOP.Core.Methods
{
    internal class InterceptionAspectGenerator
    {
        readonly BindingRestrictions _rule;
        readonly IEnumerable<MethodInterceptionAspect> _aspects;
        readonly MethodInterceptionArgs _aspectArgs;
        readonly bool _isRetValue;
        readonly bool _isByRefArgs;
        readonly IEnumerable<DynamicMetaObject> _originalArgs;

        public InterceptionAspectGenerator(object instance, BindingRestrictions rule,
            IEnumerable<MethodInterceptionAspect> aspects, MethodInfo method,
            IEnumerable<DynamicMetaObject> args, IEnumerable<Type> argsTypes)
        {
            _rule = rule;
            _aspects = aspects;
            _originalArgs = args;
            var argsValues = args.Select(x => x.Value).ToArray();
            _isRetValue = method.ReturnType != typeof(void);
            _isByRefArgs = argsTypes.Any(t => t.IsByRef);

            if (_isByRefArgs)
            {
                if (_isRetValue) _aspectArgs = new FuncInterceptionRefArgs(instance, method, argsValues, DelegateFactory.CreateDelegate(instance, method));
                
                else _aspectArgs = new ActionInterceptionRefArgs(instance, method, argsValues, DelegateFactory.CreateDelegate(instance, method));
            }
            else
            {
                if (_isRetValue) _aspectArgs = new FuncInterceptionArgs(instance, method, argsValues, DelegateFactory.CreateFunction(instance, method));
                
                else _aspectArgs = new ActionInterceptionArgs(instance, method, argsValues, DelegateFactory.CreateMethodCall(instance, method));
            }
        }

        public DynamicMetaObject Generate()
        {
            var retType = _isRetValue ? _aspectArgs.Method.ReturnType : typeof(object);

            ParameterExpression retValue = Expression.Parameter(retType);
            Expression aspectArgsEx = Expression.Constant(_aspectArgs);
            var aspectCalls = GenerateAspectCalls(_aspects, aspectArgsEx, retValue);

            Expression method = aspectCalls.First();

            for (int i = 1; i < aspectCalls.Count; i++)
            {
                method = Expression.Block(aspectCalls[i], method);
            }

            if (_isByRefArgs) method = method.UpdateRefParamsByArguments(_originalArgs, aspectArgsEx);
            
            return new DynamicMetaObject(Expression.Block(new[] { retValue }, method, Expression.Convert(retValue, typeof(object))), _rule);
        }

        List<Expression> GenerateAspectCalls(IEnumerable<MethodInterceptionAspect> aspects, Expression aspectArgsEx, ParameterExpression retValue)
        {
            var calls = new List<Expression>();
            foreach (var aspect in aspects)
            {
                MethodCallExpression aspectExpr = Expression.Call(Expression.Constant(aspect),
                    typeof(MethodInterceptionAspect).GetMethod("OnInvoke"), aspectArgsEx);

                if (_isRetValue)
                {
                    calls.Add(
                        Expression.Block(aspectExpr,
                        Expression.Assign(retValue,
                        Expression.Convert(
                        Expression.Call(aspectArgsEx, typeof(MethodInterceptionArgs).GetProperty("ReturnValue").GetGetMethod()), retValue.Type))));
                }
                else
                {
                    calls.Add(aspectExpr);
                }
            }
            return calls;
        }
    }
}
