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

using System.Collections;
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
        private readonly BindingRestrictions _rule;
        private readonly IEnumerable<MethodInterceptionAspect> _aspects;
        private readonly MethodInterceptionArgs _args;

        public InterceptionAspectGenerator(object instance, DynamicMetaObject origObj, IEnumerable aspects, MethodInfo method, IEnumerable<DynamicMetaObject> args)
        {   
            _rule = origObj.Restrictions;
            _aspects = aspects.Cast<MethodInterceptionAspect>();

            if (method.ReturnType == typeof(void))
            {
                _args = new MethodCallInterceptionArgs(instance, method, new Arguments(args.Select(x => x.Value)),
                    DelegateFactory.CreateMethodCall(instance, method));
            }
            else
            {
                _args = new FunctionInterceptionArgs(instance, method, new Arguments(args.Select(x => x.Value)),
                    DelegateFactory.CreateFunction(instance, method));
            }
        }

        public DynamicMetaObject Generate()
        {
            bool isRetValue = _args.Method.ReturnType != typeof(void);

            var retType = isRetValue ? _args.Method.ReturnType : typeof(object);

            ParameterExpression retValue = Expression.Parameter(retType);
            Expression argsEx = Expression.Constant(_args);
            var aspectCalls = GenerateAspectCalls(_aspects, argsEx, retValue, isRetValue);

            Expression method = null;

            for (int i = 0; i < aspectCalls.Count; i++)
            {
                if (i == 0)
                {
                    method = aspectCalls[i];
                }
                else
                {
                    method = Expression.Block(aspectCalls[i], method);
                }
            }
            return new DynamicMetaObject(Expression.Block(new[] { retValue }, method, Expression.Convert(retValue, typeof(object))), _rule);
        }

        private List<Expression> GenerateAspectCalls(IEnumerable<MethodInterceptionAspect> aspects, Expression args, ParameterExpression retValue, bool isRetValue)
        {
            var calls = new List<Expression>();
            foreach (var aspect in aspects)
            {
                MethodCallExpression aspectExpr = Expression.Call(Expression.Constant(aspect),
                    typeof (MethodInterceptionAspect).GetMethod("OnInvoke"), args);

                if (isRetValue)
                {
                    calls.Add(
                        Expression.Block(aspectExpr,
                        Expression.Assign(retValue,
                        Expression.Convert(
                        Expression.Call(args, typeof(MethodInterceptionArgs).GetProperty("ReturnValue").GetGetMethod()), retValue.Type))));
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
