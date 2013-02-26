using System.Dynamic;
using System.Linq.Expressions;
using KingAOP.Core;

namespace KingAOP.Tests.MethodBoundaryTests.OnSuccess
{
    internal class MyTestClass : IDynamicMetaObjectProvider
    {
        [IncrementReturnValueAspect]
        public int ReturnArgumentValue(int number)
        {
            return number;
        }

        [IncrementArgumentValueAspect]
        public void InitializeArgumentZeroValue(out int number)
        {
            number = 0;
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this, typeof(MyTestClass));
        }
    }
}
