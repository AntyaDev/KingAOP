using System.Dynamic;
using System.Linq.Expressions;

namespace KingAOP.Tests.MethodInterceptionTests.OnInvoke
{
    class MyTestClass : IDynamicMetaObjectProvider
    {
        public bool OriginalMethodCalled { get; private set; }

        [ChangeArgumentsWithInvokationAspect]
        public void MethodWithRefArgs(ref int value)
        {
            OriginalMethodCalled = true;
            value = 90;
        }

        [ChangeArgumentsWithInvokationAspect]
        public void SimpleMethod(int value)
        {
            OriginalMethodCalled = true;
            value = 90;
        }

        [NotInvokeOriginalMethodAspect]
        public void SimpleMethod()
        {
            OriginalMethodCalled = true;
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this);
        }
    }
}
