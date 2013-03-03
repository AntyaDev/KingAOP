using System.Dynamic;
using System.Linq.Expressions;
using KingAOP.Core;

namespace KingAOP.Tests.MethodBoundaryTests.OnEntry
{
    internal class MyTestClass : IDynamicMetaObjectProvider
    {
        [ChangeStringArgumentAspect]
        public string ResturnStringArgument(string argument)
        {
            return argument;
        }

        [ChangeStringArgumentAspect]
        public string ResturnStringArgumentPassedAsRef(ref string argument)
        {
            return argument;
        }
        
        [ChangeObjectArgumentAspect]
        public object ResturnObjectArgument(object argument)
        {
            return argument;
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this, typeof(MyTestClass));
        }
    }
}
