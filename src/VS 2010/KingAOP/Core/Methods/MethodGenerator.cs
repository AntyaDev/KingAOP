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
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using KingAOP.Aspects;

namespace KingAOP.Core.Methods
{
    /// <summary>
    /// Represent functional to generate method with aspects.
    /// </summary>
    internal class MethodGenerator
    {
        private readonly Expression _origMethod;
        private readonly BindingRestrictions _rule;
        private readonly IEnumerable _aspects;
        private readonly MethodExecutionArgs _args;

        public MethodGenerator(DynamicMetaObject origObj, IEnumerable aspects, MethodExecutionArgs args)
        {
            _origMethod = origObj.Expression;
            _rule = origObj.Restrictions;
            _aspects = aspects;
            _args = args;
        }

        public DynamicMetaObject Generate()
        {
            var retType = _args.Method.ReturnType != typeof (void)
                              ? _args.Method.ReturnType
                              : typeof (object);

            ParameterExpression retMethodValue = Expression.Parameter(retType);
            Expression argsEx = Expression.Constant(_args);
            var aspctCal = new AspectCalls(_aspects, argsEx, retMethodValue);

            Expression method = null;
            for (int i = 0; i < aspctCal.EntryCalls.Count; i++)
            {
                if (i == 0)
                {
                    method = Expression.Block(
                    new []
                    {
                        AssignMethodArgsByRetValue(argsEx, retMethodValue),
                        aspctCal.EntryCalls[i],
                        Expression.TryCatchFinally(
                        GenerateInvokeCall(aspctCal, argsEx, retMethodValue),
                        aspctCal.ExitCalls[i],
                        GenerateCatchBlock(argsEx, aspctCal.ExceptionCalls[i], retMethodValue))
                    });
                }
                else
                {
                    method = Expression.Block(
                    new []
                    {
                        aspctCal.EntryCalls[i],
                        Expression.TryCatchFinally(Expression.Block(method, aspctCal.SuccessCalls[i]), aspctCal.ExitCalls[i])
                    });
                }
            }
            return new DynamicMetaObject(Expression.Block(new[] { retMethodValue }, method, Expression.Convert(retMethodValue, typeof(object))), _rule);
        }

        private CatchBlock GenerateCatchBlock(Expression methArgEx, Expression exceptionCall, ParameterExpression retMethodValue)
        {
            var curEx = Expression.Parameter(typeof(Exception));
            return Expression.Catch(curEx,
                Expression.Block(
                Expression.Call(methArgEx, typeof(MethodExecutionArgs).GetProperty("Exception").GetSetMethod(), curEx),
                exceptionCall,
                GenerateSwitchExpession(methArgEx),
                retMethodValue));
        }

        private SwitchExpression GenerateSwitchExpession(Expression methArgEx)
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

        private Expression GenerateInvokeCall(AspectCalls aspectCalls, Expression methArgEx, ParameterExpression retMethodValue)
        {
            var invokeCalls = new List<Expression>();

            if (_args.Method.ReturnType != typeof(void))
            {
                invokeCalls.Add(Expression.Assign(retMethodValue, Expression.Convert(_origMethod, retMethodValue.Type))); // [ returnValue = interceptedMethod.Call(); ]
                invokeCalls.Add(AssignMethodArgsByRetValue(methArgEx, retMethodValue));  // [ MethodExecutionArgs.ReturnValue = returnValue; ]
            }
            else
            {
                invokeCalls.Add(_origMethod); // [ interceptedMethod.Call(); ]
            }

            invokeCalls.Add(aspectCalls.SuccessCalls[0]);
            return Expression.Block(invokeCalls);
        }

        // [ MethodExecutionArgs.ReturnValue = returnValue; ]
        private Expression AssignMethodArgsByRetValue(Expression methArgEx, ParameterExpression retMethodValue)
        {
            return Expression.Call(methArgEx, typeof(MethodExecutionArgs).GetProperty("ReturnValue").GetSetMethod(), Expression.Convert(retMethodValue, typeof(object)));
        }
    }
}
