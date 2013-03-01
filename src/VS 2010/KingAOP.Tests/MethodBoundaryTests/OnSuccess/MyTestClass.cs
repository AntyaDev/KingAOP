using System;
using System.Dynamic;
using System.Linq.Expressions;
using KingAOP.Core;
using KingAOP.Tests.TestData;

namespace KingAOP.Tests.MethodBoundaryTests.OnSuccess
{
    internal class MyTestClass : IDynamicMetaObjectProvider
    {
        [IncrementReturnValueAspect]
        public int ReturnArgumentValue(int number)
        {
            return number;
        }

        [IncrementReturnValueAspect]
        public int ReturnArgumentValueWithException(int number)
        {
            throw new Exception();
        }

        [IncrementArgumentValueAspect]
        public void InitializeArgumentZeroValue(out int number)
        {
            number = 0;
        }

        [InitTestEntityAspect]
        public void InitTestEntity(TestEntity testEntity)
        {
            testEntity.Name = "test";
            testEntity.Number = 0;
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this, typeof(MyTestClass));
        }
    }
}
