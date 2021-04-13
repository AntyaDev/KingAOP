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

namespace KingAOP.Aspects
{
    /// <summary>
    /// Arguments of aspect which in executing for a property.
    /// </summary>
    public abstract class LocationInterceptionArgs : AdviceArgs
    {
        internal LocationInterceptionArgs(object instance, PropertyInfo property, object value) : base(instance)
        {
            Location = property;
            
            if (value != null) Value = value;

            else Value = property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null;
        }

        /// <summary>
        /// Gets or sets the location value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets the property location related to the aspect being executed.
        /// </summary>
        public PropertyInfo Location { get; private set; }

        /// <summary>
        /// Retrieves the current value of the location without overwriting the property.
        /// </summary>
        /// <returns>
        /// The current value of the location, as returned by the next node in the chain of invocation.
        /// </returns>
        public abstract object GetCurrentValue();

        /// <summary>
        /// Invokes the <b>Get Location Value</b> semantic on the next node in the chain of invocation and stores the location value in the property.
        /// </summary>
        public abstract void ProceedGetValue();

        /// <summary>
        /// Invokes the <b>Set Location Value</b> semantic on the next node in the chain of invocation and stores the value of the property into the location.
        /// </summary>
        public abstract void ProceedSetValue();

        /// <summary>
        /// Sets the value of the location without overwriting the property.
        /// </summary>
        /// <param name="value">The value to be passed to the next node in the chain of invocation.</param>
        public abstract void SetNewValue(object value);
    }
}