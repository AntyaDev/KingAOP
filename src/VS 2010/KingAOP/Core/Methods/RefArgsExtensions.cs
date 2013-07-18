using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using KingAOP.Aspects;

namespace KingAOP.Core.Methods
{
    internal static class RefArgsExtensions
    {
        public static Expression UpdateRefArgs(this Expression method, IEnumerable<DynamicMetaObject> originalArgs,  Expression aspectArgsEx)
        {
            var finallyBlock = originalArgs.Select((arg, index) =>
                Expression.Assign(arg.Expression,
                Expression.Convert(Expression.MakeIndex(
                Expression.Property(aspectArgsEx, typeof(MethodInterceptionArgs).GetProperty("Arguments")),
                typeof(Arguments).GetProperty("Item"), new[] { Expression.Constant(index) }), arg.RuntimeType))).ToList();

            return Expression.TryFinally(method, Expression.Block(finallyBlock));
        }
    }
}
