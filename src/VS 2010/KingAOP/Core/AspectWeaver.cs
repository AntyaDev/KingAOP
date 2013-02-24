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
        private readonly Type _objType;
        private readonly Hashtable _methods;

        public AspectWeaver(Expression expression, object obj, Type objType)
            : base(expression, BindingRestrictions.Empty, obj)
        {
            _objType = objType;
            _methods = MethodsCache.Get(_objType);

            if (_methods == null) // checking on existence in global methods cache
            {
                _methods = new Hashtable();

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
                        _methods.Add(method, aspects.Values);
                    }
                }
                MethodsCache.Put(_objType, _methods); // put found methods to global cache
            }
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            var argsTypes = args.Select(arg => arg.RuntimeType).ToArray();
            var method = _objType.GetMethod(binder.Name, argsTypes);
            var dynamicObj = DynamicMetaObjectsCache.Get(method);
            
            if (dynamicObj == null)
            {
                var aspects = (IEnumerable)_methods[method];
                if (aspects == null)
                {
                    dynamicObj = base.BindInvokeMember(binder, args);
                }
                else
                {
                    dynamicObj = WeaveAspect(base.BindInvokeMember(binder, args), aspects,
                        new MethodExecutionArgs(Value, method, new Arguments(args)));
                }
                DynamicMetaObjectsCache.Put(method, dynamicObj);
            }
            return dynamicObj;
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
