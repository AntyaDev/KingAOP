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
using System.Reflection;
using KingAOP.Aspects;

namespace KingAOP.Core.Properties
{
    /// <summary>
    ///  Arguments of aspect which intercepts invocations of property.
    /// </summary>
    internal class PropertyInterceptionArgs : LocationInterceptionArgs
    {
        private readonly LateBoundGetter _getter;
        private readonly LateBoundSetter _setter;

        public PropertyInterceptionArgs(object instance, PropertyInfo property, object value) 
            : base(instance, property, value)
        {
            var getter = property.GetGetMethod(nonPublic: true);
            
            if (getter != null) _getter = DelegateFactory.CreateGetter(instance, getter);

            var setter = property.GetSetMethod(nonPublic: true);
            
            if (setter != null) _setter = DelegateFactory.CreateSetter(instance, setter);
        }

        public override object GetCurrentValue()
        {
            throw new NotImplementedException();
        }

        public override void ProceedGetValue()
        {
            Value = _getter();
        }

        public override void ProceedSetValue()
        {
            _setter(Value);
        }

        public override void SetNewValue(object value)
        {
            throw new NotImplementedException();
        }
    }
}
