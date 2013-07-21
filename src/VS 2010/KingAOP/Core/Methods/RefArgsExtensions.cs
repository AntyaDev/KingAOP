using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using KingAOP.Aspects;

namespace KingAOP.Core.Methods
{
    internal static class RefArgsExtensions
    {
        public static Expression UpdateRefParamsByArguments(this Expression method, IEnumerable<DynamicMetaObject> originalArgs, Expression aspectArgsEx)
        {
            var finallyBlock = originalArgs.Select((arg, index) =>
                Expression.Assign(arg.Expression,
                Expression.Convert(Expression.MakeIndex(
                Expression.Property(aspectArgsEx, typeof(MethodArgs).GetProperty("Arguments")),
                typeof(Arguments).GetProperty("Item"), new[] { Expression.Constant(index) }), arg.RuntimeType))).ToList();

            return Expression.TryFinally(method, Expression.Block(finallyBlock));
        }

        public static Expression UpdateArgumentsByRefParams(this Expression method, IEnumerable<DynamicMetaObject> originalArgs, Expression aspectArgsEx)
        {
            var finallyBlock = originalArgs.Select((arg, index) =>
                Expression.Assign(Expression.MakeIndex(Expression.Property(aspectArgsEx,
                typeof (MethodArgs).GetProperty("Arguments")),
                typeof (Arguments).GetProperty("Item"), new[] {Expression.Constant(index)}),
                Expression.Convert(arg.Expression, typeof(object)))).ToList();

            return Expression.TryFinally(method, Expression.Block(finallyBlock));
        }
    }
}
