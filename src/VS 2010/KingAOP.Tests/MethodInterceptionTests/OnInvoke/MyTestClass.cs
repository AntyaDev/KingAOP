using System.Dynamic;
using System.Linq.Expressions;

namespace KingAOP.Tests.MethodInterceptionTests.OnInvoke
{
    class MyTestClass : IDynamicMetaObjectProvider
    {
        [ChangeArgumentsWithInvokationAspect]
        public void MethodWithRefArgs(ref int value)
        {
            value = 90;
        }

        [ChangeArgumentsWithInvokationAspect]
        public void SimpleMethod(int value)
        {
            value = 90;
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this);
        }
    }
}
