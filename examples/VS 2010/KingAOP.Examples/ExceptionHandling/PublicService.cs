using System;
using System.Dynamic;
using System.Linq.Expressions;
using KingAOP.Core;
using KingAOP.Examples.TestData;

namespace KingAOP.Examples.ExceptionHandling
{
    class PublicService : IDynamicMetaObjectProvider
    {   
        public string SessionId { get; set; }

        public string ServiceName { get; set; }

        public uint Port { get; set; }

        [ExceptionHandlingAspect]
        public void Send(TestEntity entity)
        {
            throw new Exception();
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this);
        }
    }
}
