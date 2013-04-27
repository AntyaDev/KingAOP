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
using System.Linq.Expressions;
using System.Reflection;
using KingAOP.Aspects;
using KingAOP.Core.Methods;
using KingAOP.Core.Properties;

namespace KingAOP
{
    /// <summary>
    /// Represent weaver for weaving aspects. 
    /// </summary>
    public class AspectWeaver : DynamicMetaObject
    {
        private readonly Type _objType;

        public AspectWeaver(Expression expression, object obj) : base(expression, BindingRestrictions.Empty, obj)
        {
            _objType = obj.GetType();
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            var metaObj = base.BindInvokeMember(binder, args);

            var argsTypes = GetArgumentsTypes(args);
            var method = _objType.GetMethod(binder.Name,
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                argsTypes,
                null);
            
            if (method != null && method.IsDefined(typeof(IAspect), false))
            {
                var aspects = RetrieveAspects(method);
                var methodArgs = new MethodExecutionArgs(Value, method, new Arguments(args));
                metaObj = new MethodGenerator(metaObj, aspects, methodArgs).Generate();
            }
            return metaObj;
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var metaObj = base.BindGetMember(binder);

            var property = _objType.GetProperty(binder.Name,
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (property != null && property.IsDefined(typeof(IAspect), false))
            {
                var aspects = RetrieveAspects(property);
                var args = new LocationInterceptionArgs(Value, property, null);
                metaObj = new GetterGenerator(metaObj, aspects, args).Generate();
            }
            return metaObj;
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            var metaObj = base.BindSetMember(binder, value);

            var property = _objType.GetProperty(binder.Name,
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (property != null && property.IsDefined(typeof(IAspect), false))
            {
                var aspects = RetrieveAspects(property);
                var args = new LocationInterceptionArgs(Value, property, value);
                metaObj = new SetterGenerator(metaObj, aspects, args).Generate();
            }
            return metaObj;
        }

        private IEnumerable RetrieveAspects(MemberInfo member)
        {
            var aspects = new SortedList<int, object>(new InvertedComparer());
            foreach (Aspect aspect in member.GetCustomAttributes(typeof(Aspect), false))
            {
                var instance = Activator.CreateInstance(aspect.GetType());
                aspects.Add(aspect.AspectPriority, instance);
            }
            return aspects.Values;
        }

        private Type[] GetArgumentsTypes(DynamicMetaObject[] args)
        {
            var argsTypes = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                argsTypes[i] = ((ParameterExpression)args[i].Expression).IsByRef
                    ? args[i].RuntimeType.MakeByRefType()
                    : args[i].RuntimeType;
            }
            return argsTypes;
        }

        internal class InvertedComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return y.CompareTo(x);
            }
        }
    }
}
