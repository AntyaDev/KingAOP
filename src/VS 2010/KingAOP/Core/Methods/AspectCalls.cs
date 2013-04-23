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

namespace KingAOP.Core.Methods
{
    /// <summary>
    /// Represent a container for an all aspects which should be applied for a specific method
    /// </summary>
    internal class AspectCalls
    {
        public List<Expression> EntryCalls { get; private set; }
        public List<Expression> SuccessCalls { get; private set; }
        public List<Expression> ExceptionCalls { get; private set; }
        public List<Expression> ExitCalls { get; private set; }

        private AspectCalls()
        {
            EntryCalls = new List<Expression>();
            SuccessCalls = new List<Expression>();
            ExceptionCalls = new List<Expression>();
            ExitCalls = new List<Expression>();
        }

        public AspectCalls(IEnumerable aspects, Expression args, ParameterExpression retValue) : this()
        {
            foreach (var aspect in aspects)
            {
                var methBoundAsp = aspect as OnMethodBoundaryAspect;
                if (methBoundAsp != null)
                {
                    EntryCalls.Add(Expression.Call(Expression.Constant(aspect), typeof(OnMethodBoundaryAspect).GetMethod("OnEntry"), args));
                    SuccessCalls.Add(GenerateCall("OnSuccess", methBoundAsp, args, retValue));
                    ExceptionCalls.Add(Expression.Call(Expression.Constant(aspect), typeof (OnMethodBoundaryAspect).GetMethod("OnException"), args));
                    ExitCalls.Add(GenerateCall("OnExit", methBoundAsp, args, retValue));
                }
            }
        }

        private Expression GenerateCall(string methodName, OnMethodBoundaryAspect aspect, Expression args, ParameterExpression retValue)
        {
            return Expression.Block(
                Expression.Call(Expression.Constant(aspect), typeof(OnMethodBoundaryAspect).GetMethod(methodName), args),
                Expression.Assign(retValue,
                Expression.Convert(
                Expression.Call(args, typeof(MethodExecutionArgs).GetProperty("ReturnValue").GetGetMethod()), retValue.Type)));
        }
    }
}