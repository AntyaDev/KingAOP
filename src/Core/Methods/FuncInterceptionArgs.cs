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

using System.Reflection;
using KingAOP.Aspects;

namespace KingAOP.Core.Methods
{
    /// <summary>
    ///  Arguments of aspect which intercept a method with return value.
    /// </summary>
    internal class FuncInterceptionArgs : MethodInterceptionArgs
    {
        readonly LateBoundFunc _func;
        readonly object[] _argsValues;

        public FuncInterceptionArgs(object instance, MethodInfo method, object[] argsValues, LateBoundFunc func)
            : base(instance, method, new Arguments(argsValues))
        {
            _func = func;
            _argsValues = argsValues;
        }

        /// <summary>
        ///  Proceeds with invocation of the method that has been intercepted by calling the next node in the chain of invocation, 
        /// passing the current <see cref="Arguments"/> to that method 
        /// and storing its return value into the property ReturnValue.
        /// </summary>
        public override void Proceed()
        {
            ReturnValue = _func.Invoke(_argsValues);
        }
    }
}
