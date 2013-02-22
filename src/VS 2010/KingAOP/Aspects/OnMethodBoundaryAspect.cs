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

namespace KingAOP.Aspects
{
    /// <summary>
    /// Represents a class that can wrap itself around any given method call.
    /// </summary>
    public abstract class OnMethodBoundaryAspect : Aspect
    {
        /// <summary>
        /// Method executed before the body of method to which this aspect is applied.
        /// </summary>
        /// <param name="args">Method arguments including return value and all necessary info.</param>
        public virtual void OnEntry(MethodExecutionArgs args) 
        { }

        /// <summary>
        /// Method executed after the body of method to which this aspect is applied
        /// (this method is invoked from the finally block, it's mean that this method will be invoked in any way)
        /// </summary>
        /// <param name="args">Method arguments including return value and all necessary info.</param>
        public virtual void OnExit(MethodExecutionArgs args) 
        { }

        /// <summary>
        /// Method executed after the body of method to which this aspect is applied, 
        /// but only when the method successfully returns
        /// </summary>
        /// <param name="args">Method arguments including return value and all necessary info.</param>
        public virtual void OnSuccess(MethodExecutionArgs args)
        { }

        /// <summary>
        /// When an exception is happened then this method will be called.
        /// </summary>
        /// <param name="args">Method arguments including return value and all necessary info.</param>
        public virtual void OnException(MethodExecutionArgs args) 
        { }
    }
}
