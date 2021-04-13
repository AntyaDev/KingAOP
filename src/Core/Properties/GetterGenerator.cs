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
    internal class GetterGenerator
    {
        private readonly BindingRestrictions _rule;
        private readonly List<Expression> _aspects;
        private readonly PropertyInterceptionArgs _args;

        public GetterGenerator(object instance, BindingRestrictions rule, IEnumerable aspects, PropertyInfo property) 
        {
            _rule = rule;
            _args = new PropertyInterceptionArgs(instance, property, null);
            _aspects = GenerateAspectCalls(aspects, _args);
        }

        public DynamicMetaObject Generate()
        {
            ParameterExpression retValue = Expression.Parameter(typeof(object));

            var retValueExpr = Expression.Assign(retValue,
                Expression.Call(Expression.Constant(_args),
                typeof (PropertyInterceptionArgs).GetProperty("Value").GetGetMethod()));

            Expression getter = Expression.Block(_aspects.First(), retValueExpr);
            for (int i = 1; i < _aspects.Count; i++)
            {
                getter = Expression.Block(
                new[] { _aspects[i], getter });
            }
            return new DynamicMetaObject(Expression.Block(new[] { retValue }, getter), _rule);
        }

        private List<Expression> GenerateAspectCalls(IEnumerable aspects, LocationInterceptionArgs args)
        {
            var aspectCalls = new List<Expression>();
            foreach (var aspect in aspects)
            {
                aspectCalls.Add(
                    Expression.Call(Expression.Constant(aspect), typeof(LocationInterceptionAspect).GetMethod("OnGetValue"),
                    Expression.Constant(args)));
            }
            return aspectCalls;
        }
    }
}
