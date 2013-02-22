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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using KingAOP.Aspects;

namespace KingAOP.Core
{   
    /// <summary>
    /// Represent weaver for weaving aspects. 
    /// </summary>
    public class AspectWeaver : DynamicMetaObject
    {
        private readonly Type _orignType;

        public AspectWeaver(Expression expression, object obj) : base(expression, BindingRestrictions.Empty, obj)
        {
            _orignType = obj.GetType();
            if (MethodsCache.Get(_orignType) != null) // checking on existence in global methods cache
            {
                return;
            }

            var methodCache = new Hashtable();

            foreach (var method in obj.GetType().GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance 
                | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                if (method.IsDefined(typeof(IAspect), false))
                {
                    var aspects = new SortedList<int, object>(new InvertedComparer());
                    foreach (Aspect attribute in method.GetCustomAttributes(typeof(Aspect), false))
                    {
                        var aspect = AspectCacheFactory.GetAspect(attribute.GetType());
                        aspects.Add(attribute.AspectPriority, aspect);
                    }
                    methodCache.Add(method, aspects.Values);
                }
            }

            // put found methods to global cache
            MethodsCache.Put(_orignType, methodCache);
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            var argsTypes = args.Select(arg => arg.RuntimeType).ToArray();
            var method = _orignType.GetMethod(binder.Name, argsTypes);
            var methods = MethodsCache.Get(_orignType);
            var aspects = (IEnumerable)methods[method];
            return aspects == null 
                ? base.BindInvokeMember(binder, args) 
                : WeaveAspect(base.BindInvokeMember(binder, args), aspects, new MethodExecutionArgs(Value, method, new Arguments(args)));
        }

        private DynamicMetaObject WeaveAspect(DynamicMetaObject origObj, IEnumerable aspects, MethodExecutionArgs executionArgs)
        {
            return new DynamicMetaObject(WeaweMethodBoundaryAspect(origObj, aspects, executionArgs), origObj.Restrictions);
        }

        private Expression WeaweMethodBoundaryAspect(DynamicMetaObject origObj, IEnumerable aspects, MethodExecutionArgs executionArgs)
        {
            var aspectGenerator = new AspectGenerator(origObj, aspects, executionArgs);
            return aspectGenerator.GenerateMethod();
        }
    }
}
