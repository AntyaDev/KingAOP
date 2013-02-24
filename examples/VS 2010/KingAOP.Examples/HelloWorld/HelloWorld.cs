using System;
using System.Dynamic;
using System.Linq.Expressions;
using KingAOP.Core;

namespace KingAOP.Examples.HelloWorld
{
    internal class HelloWorld : IDynamicMetaObjectProvider
    {
        [HelloWorldAspect]
        public void HelloWorldCall()
        {
            Console.WriteLine("Hello World");
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this, typeof(HelloWorld));
        }
    }
}