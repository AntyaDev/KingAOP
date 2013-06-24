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

namespace KingAOP.Aspects
{
    /// <summary>
    /// Encapsulation of method arguments.
    /// </summary>
    public class Arguments : IEnumerable<object>
    {
        readonly object[] _objects;

        public Arguments(object[] objects)
        {
            _objects = objects;
        }

        public object this[int index]
        {
            get { return _objects[index]; }
            set { _objects[index] = value; }
        }

        public int Count
        {
            get { return _objects.Length; }
        }

        public object[] ToArray()
        {
            var array = new object[Count];
            _objects.CopyTo(array, 0);
            return array;
        }

        public IEnumerator<object> GetEnumerator()
        {
            for (uint i = 0; i < _objects.Length; i++)
            {
                yield return GetArgument(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        object GetArgument(uint index)
        {
            return _objects[index];
        }
    }
}
