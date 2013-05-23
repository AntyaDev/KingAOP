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
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using KingAOP.Aspects;

namespace KingAOP.Core.Properties
{
    internal class SetterGenerator
    {
        private readonly Expression _origSetter;
        private readonly BindingRestrictions _rule;
        private readonly List<Expression> _aspects;

        public SetterGenerator(object instance, DynamicMetaObject metaObj, IEnumerable aspects, PropertyInfo property, DynamicMetaObject value)
        {
            _origSetter = metaObj.Expression;
            _rule = metaObj.Restrictions;
            _aspects = GenerateAspectCalls(aspects, new PropertyInterceptionArgs(instance, property, value.Value));
        }

        public DynamicMetaObject Generate()
        {
            Expression setter = Expression.Block(_aspects.First(), Expression.Default(typeof(object)));
            for (int i = 1; i < _aspects.Count; i++)
            {
                setter = Expression.Block(
                new[]
                {
                    _aspects[i],
                    setter
                });
            }
            return new DynamicMetaObject(setter, _rule);
        }

        private List<Expression> GenerateAspectCalls(IEnumerable aspects, LocationInterceptionArgs args)
        {
            var aspectCalls = new List<Expression>();
            foreach (var aspect in aspects)
            {
                aspectCalls.Add(
                    Expression.Call(Expression.Constant(aspect), typeof(LocationInterceptionAspect).GetMethod("OnSetValue"),
                    Expression.Constant(args)));
            }
            return aspectCalls;
        }
    }
}
