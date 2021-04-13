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
using KingAOP.Core.Methods;
using KingAOP.Core.Properties;

namespace KingAOP
{
    /// <summary>
    /// Represent weaver for weaving aspects. 
    /// </summary>
    public class AspectWeaver : DynamicMetaObject
    {
        readonly Type _objType;

        public AspectWeaver(Expression expression, object obj)
            : base(expression, BindingRestrictions.Empty, obj)
        {
            _objType = obj.GetType();
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            var metaObj = base.BindInvokeMember(binder, args);

            var argsTypes = GetMethodArgsTypes(metaObj);
            var method = _objType.GetMethod(binder.Name,
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                argsTypes,
                null);

            if (method != null && method.IsDefined(typeof(IMethodAspect), false))
            {
                var aspects = RetrieveAspects(method);

                if (method.IsDefined(typeof(OnMethodBoundaryAspect), false))
                {
                    metaObj = new BoundaryAspectGenerator(Value, metaObj, aspects.Cast<OnMethodBoundaryAspect>(), method, args, argsTypes).Generate();
                }
                else
                {
                    metaObj = new InterceptionAspectGenerator(Value, metaObj.Restrictions, aspects.Cast<MethodInterceptionAspect>(), method, args, argsTypes).Generate();
                }
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
                metaObj = new GetterGenerator(Value, metaObj.Restrictions, aspects, property).Generate();
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
                metaObj = new SetterGenerator(Value, metaObj, aspects, property, value).Generate();
            }
            return metaObj;
        }

        IEnumerable RetrieveAspects(MemberInfo member)
        {
            var aspects = new SortedList<int, object>(new InvertedComparer());
            foreach (Aspect aspect in member.GetCustomAttributes(typeof(Aspect), false))
            {
                var instance = Activator.CreateInstance(aspect.GetType());
                aspects.Add(aspect.AspectPriority, instance);
            }
            return aspects.Values;
        }

        Type[] GetMethodArgsTypes(DynamicMetaObject metaObj)
        {
            MethodCallExpression callExpr = RetrieveMethodCall(metaObj);
            return (callExpr).Arguments.Select(arg =>
            {
                if (arg.NodeType != ExpressionType.Parameter) return arg.Type;

                return ((ParameterExpression)arg).IsByRef ? arg.Type.MakeByRefType() : arg.Type;
            }).ToArray();
        }

        MethodCallExpression RetrieveMethodCall(DynamicMetaObject metaObj)
        {
            Expression callExpr = null;

            while (callExpr == null || callExpr.NodeType != ExpressionType.Call)
            {
                switch (metaObj.Expression.NodeType)
                {
                    case ExpressionType.Call: return (MethodCallExpression)metaObj.Expression;

                    case ExpressionType.Block:
                        var block = (BlockExpression)metaObj.Expression;
                        return (MethodCallExpression)block.Expressions.First(expr => expr.NodeType == ExpressionType.Call);

                    case ExpressionType.Convert: callExpr = ((UnaryExpression)metaObj.Expression).Operand; break;

                    default: throw new NotImplementedException();
                }
            }
            return (MethodCallExpression)callExpr;
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
