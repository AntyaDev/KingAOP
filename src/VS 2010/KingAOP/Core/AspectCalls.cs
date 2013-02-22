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
using System.Linq.Expressions;
using KingAOP.Aspects;

namespace KingAOP.Core
{
    /// <summary>
    /// Represent a container for an all aspects which should be applied for a specific method or property
    /// </summary>
    internal class AspectCalls
    {
        public List<Expression> EntryCalls = new List<Expression>();
        public List<Expression> SuccessCalls = new List<Expression>();
        public List<Expression> ExceptionCalls = new List<Expression>();
        public List<Expression> ExitCalls = new List<Expression>();

        public AspectCalls(IEnumerable aspects, Expression methArgEx, ParameterExpression retMethodValue)
        {
            foreach (var aspect in aspects)
            {
                var methBoundAsp = aspect as OnMethodBoundaryAspect;
                if (methBoundAsp != null)
                {
                    EntryCalls.Add(Expression.Call(Expression.Constant(aspect), typeof(OnMethodBoundaryAspect).GetMethod("OnEntry"), methArgEx));
                    SuccessCalls.Add(GenerateCall("OnSuccess", methBoundAsp, methArgEx, retMethodValue));
                    ExceptionCalls.Add(Expression.Call(Expression.Constant(aspect), typeof (OnMethodBoundaryAspect).GetMethod("OnException"), methArgEx));
                    ExitCalls.Add(GenerateCall("OnExit", methBoundAsp, methArgEx, retMethodValue));
                }
            }
        }

        private Expression GenerateCall(string methodName, OnMethodBoundaryAspect aspect, Expression methArgEx, ParameterExpression retMethodValue)
        {
            return Expression.Block(
                Expression.Call(Expression.Constant(aspect), typeof(OnMethodBoundaryAspect).GetMethod(methodName), methArgEx),
                Expression.Assign(retMethodValue,
                Expression.Convert(
                Expression.Call(methArgEx, typeof(MethodExecutionArgs).GetProperty("ReturnValue").GetGetMethod()), retMethodValue.Type)));
        }
    }
}