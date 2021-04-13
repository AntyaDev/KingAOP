using System;
using System.Dynamic;
using System.Linq.Expressions;

namespace KingAOP.Examples.HelloWorld
{
    internal class HelloWorld : IDynamicMetaObjectProvider
    {
        [HelloWorldAspect]
        public void HelloWorldCall()
        {
            Console.WriteLine("Hello World");
        }
        
        [NotNullInvocationAspect]
        public void PrintIfArgNotNull(string text)
        {
            Console.WriteLine("\n" + "PrintIfArgNotNull");
            Console.WriteLine(text);
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new AspectWeaver(parameter, this);
        }
    }
}