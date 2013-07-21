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
    internal class BoundaryAspectGenerator
    {
        readonly BindingRestrictions _rule;
        readonly AspectCalls _aspectCalls;

        public BoundaryAspectGenerator(object instance, DynamicMetaObject metaObj, IEnumerable<OnMethodBoundaryAspect> aspects,
            MethodInfo method, IEnumerable<DynamicMetaObject> args, IEnumerable<Type> argsTypes)
        {
            _rule = metaObj.Restrictions;
            
            var methExecArgs = new MethodExecutionArgs(instance, method, new Arguments(args.Select(x => x.Value).ToArray()));

            _aspectCalls = new AspectCalls(metaObj.Expression, aspects, args, methExecArgs, argsTypes.Any(t => t.IsByRef));
        }

        public DynamicMetaObject Generate()
        {
            Expression method = Expression.Block(
            new[]
            {
                AssignMethodArgsByRetValue(_aspectCalls.ArgsExpression, _aspectCalls.RetMethodValue),
                _aspectCalls.EntryCalls.First(),
                Expression.TryCatchFinally(
                _aspectCalls.OriginalCall,
                _aspectCalls.ExitCalls.First(),
                GenerateCatchBlock(_aspectCalls.ArgsExpression, _aspectCalls.ExceptionCalls.First(), _aspectCalls.RetMethodValue))
            });

            for (int i = 1; i < _aspectCalls.EntryCalls.Count; i++)
            {
                method = Expression.Block(
                new[]
                {
                    _aspectCalls.EntryCalls[i],
                    Expression.TryCatchFinally(Expression.Block(method, _aspectCalls.SuccessCalls[i]), _aspectCalls.ExitCalls[i])
                });
            }

            return new DynamicMetaObject(Expression.Block(new[] { _aspectCalls.RetMethodValue }, method, _aspectCalls.RetMethodValue), _rule);
        }

        CatchBlock GenerateCatchBlock(Expression methArgEx, Expression exceptionCall, ParameterExpression retMethodValue)
        {
            var curEx = Expression.Parameter(typeof(Exception));
            return Expression.Catch(curEx,
                Expression.Block(
                Expression.Call(methArgEx, typeof(MethodExecutionArgs).GetProperty("Exception").GetSetMethod(), curEx),
                exceptionCall,
                GenerateSwitchExpession(methArgEx),
                retMethodValue));
        }

        SwitchExpression GenerateSwitchExpession(Expression methArgEx)
        {
            return Expression.Switch(
                Expression.Call(methArgEx, typeof(MethodExecutionArgs).GetProperty("FlowBehavior").GetGetMethod()),
                new[] 
                {
                    Expression.SwitchCase(Expression.Rethrow(),
                    Expression.Constant(FlowBehavior.Default),
                    Expression.Constant(FlowBehavior.RethrowException)),

                    Expression.SwitchCase(
                    Expression.Call(methArgEx, typeof(MethodExecutionArgs).GetProperty("Exception").GetSetMethod(), Expression.Constant(null, typeof(Exception))), 
                    Expression.Constant(FlowBehavior.Continue),
                    Expression.Constant(FlowBehavior.Return)),

                    Expression.SwitchCase(
                    Expression.Throw(Expression.Call(methArgEx, typeof(MethodExecutionArgs).GetProperty("Exception").GetGetMethod())),
                    Expression.Constant(FlowBehavior.ThrowException))
                });
        }

        // [ MethodExecutionArgs.ReturnValue = returnValue; ]
        static Expression AssignMethodArgsByRetValue(Expression methArgEx, ParameterExpression retMethodValue)
        {
            return Expression.Call(methArgEx, typeof(MethodExecutionArgs).GetProperty("ReturnValue").GetSetMethod(), retMethodValue);
        }

        /// <summary>
        /// Represent a container for an all aspects which should be applied for a specific method
        /// </summary>
        internal class AspectCalls
        {
            readonly bool _isByRefArgs;
            readonly IEnumerable<DynamicMetaObject> _args;
            readonly Expression _origMethod;

            public Expression ArgsExpression { get; private set; }
            public ParameterExpression RetMethodValue { get; private set; }
            public MethodExecutionArgs MethodExecutionArgs { get; private set; }

            public List<Expression> EntryCalls { get; private set; }
            public List<Expression> SuccessCalls { get; private set; }
            public List<Expression> ExceptionCalls { get; private set; }
            public List<Expression> ExitCalls { get; private set; }
            public Expression OriginalCall { get; private set; }

            private AspectCalls()
            {
                EntryCalls = new List<Expression>();
                SuccessCalls = new List<Expression>();
                ExceptionCalls = new List<Expression>();
                ExitCalls = new List<Expression>();
            }

            public AspectCalls(Expression origMethod, IEnumerable<OnMethodBoundaryAspect> aspects,
                IEnumerable<DynamicMetaObject> args, MethodExecutionArgs methExecArgs, bool isByRefArgs)
                : this()
            {
                _origMethod = origMethod;
                _args = args;
                MethodExecutionArgs = methExecArgs;
                _isByRefArgs = isByRefArgs;

                ArgsExpression = Expression.Constant(methExecArgs);
                RetMethodValue = Expression.Parameter(typeof(object));

                foreach (var aspect in aspects)
                {
                    EntryCalls.Add(GenerateCall("OnEntry", aspect));
                    SuccessCalls.Add(GenerateCall("OnSuccess", aspect));
                    ExceptionCalls.Add(GenerateCall("OnException", aspect));
                    ExitCalls.Add(GenerateCall("OnExit", aspect));
                }

                OriginalCall = GenerateOriginalCall();
            }

            Expression GenerateCall(string methodName, OnMethodBoundaryAspect aspect)
            {
                var method = Expression.Block(
                    Expression.Call(Expression.Constant(aspect), typeof(OnMethodBoundaryAspect).GetMethod(methodName), ArgsExpression),
                    Expression.Assign(RetMethodValue,
                    Expression.Call(ArgsExpression, typeof(MethodExecutionArgs).GetProperty("ReturnValue").GetGetMethod())));

                return _isByRefArgs ? method.UpdateRefParamsByArguments(_args, ArgsExpression) : method;
            }

            Expression GenerateOriginalCall()
            {
                var invokeCalls = new List<Expression>();

                if (MethodExecutionArgs.Method.ReturnType != typeof(void))
                {
                    invokeCalls.Add(Expression.Assign(RetMethodValue, _origMethod)); // [ returnValue = interceptedMethod.Call(); ]
                    invokeCalls.Add(AssignMethodArgsByRetValue(ArgsExpression, RetMethodValue));  // [ MethodExecutionArgs.ReturnValue = returnValue; ]
                }
                else
                {
                    invokeCalls.Add(_origMethod); // [ interceptedMethod.Call(); ]
                }

                if (_isByRefArgs) invokeCalls[0] = invokeCalls[0].UpdateArgumentsByRefParams(_args, ArgsExpression);

                invokeCalls.Add(SuccessCalls[0]);

                return Expression.Block(invokeCalls);
            }
        }
    }
}
