using System.Dynamic;
using System.Linq.Expressions;
using KingAOP.Core;
using KingAOP.Examples.TestData;

namespace KingAOP.Examples.Logging
{
    internal class TestRepository : IDynamicMetaObjectProvider
    {
        [LoggingAspect]
        public void Save(TestEntity entity)
        {
            entity.Name = "Mark";
            entity.Number = 250;
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this, typeof (TestRepository));
        }
    }
}
