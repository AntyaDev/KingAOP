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
    /// Represent functional to generate aspects.
    /// </summary>
    internal class AspectGenerator
    {
        private readonly DynamicMetaObject _origObj;
        private readonly IEnumerable _aspects;
        private readonly MethodExecutionArgs _methodExecutionArgs;

        public AspectGenerator(DynamicMetaObject origObj, IEnumerable aspects, MethodExecutionArgs methodExecutionArgs)
        {
            _origObj = origObj;
            _aspects = aspects;
            _methodExecutionArgs = methodExecutionArgs;
        }

        public Expression GenerateMethod()
        {
            var retType = _methodExecutionArgs.Method.ReturnType != typeof (void)
                              ? _methodExecutionArgs.Method.ReturnType
                              : typeof (object);

            ParameterExpression retMethodValue = Expression.Parameter(retType);
            Expression methArgEx = Expression.Constant(_methodExecutionArgs);
            var aspctCal = new AspectCalls(_aspects, methArgEx, retMethodValue);

            Expression methExpr = null;
            for (int i = 0; i < aspctCal.EntryCalls.Count; i++)
            {
                if (i == 0)
                {
                    methExpr = Expression.Block(
                    new []
                    {
                        AssignMethodArgsByRetValue(methArgEx, retMethodValue),
                        aspctCal.EntryCalls[i],
                        Expression.TryCatchFinally(
                        GenerateInvokeCall(aspctCal, methArgEx, retMethodValue),
                        aspctCal.ExitCalls[i],
                        GenerateCatchBlock(methArgEx, aspctCal.ExceptionCalls[i], retMethodValue))
                    });
                }
                else
                {
                    methExpr = Expression.Block(
                    new []
                    {
                        aspctCal.EntryCalls[i],
                        Expression.TryCatchFinally(Expression.Block(methExpr, aspctCal.SuccessCalls[i]), aspctCal.ExitCalls[i])
                    });
                }
            }
            return Expression.Block(new[] { retMethodValue }, methExpr, Expression.Convert(retMethodValue, typeof(object)));
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

            if (_methodExecutionArgs.Method.ReturnType != typeof(void))
            {
                invokeCalls.Add(Expression.Assign(retMethodValue, Expression.Convert(_origObj.Expression, retMethodValue.Type))); // [ returnValue = interceptedMethod.Call(); ]
                invokeCalls.Add(AssignMethodArgsByRetValue(methArgEx, retMethodValue));  // [ MethodExecutionArgs.ReturnValue = returnValue; ]
            }
            else
            {
                invokeCalls.Add(_origObj.Expression); // [ interceptedMethod.Call(); ]
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
