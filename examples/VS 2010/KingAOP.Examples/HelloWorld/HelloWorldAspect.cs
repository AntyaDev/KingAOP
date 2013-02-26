using System;
using KingAOP.Aspects;

namespace KingAOP.Examples.HelloWorld
{    
    class HelloWorldAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            Console.WriteLine("OnEntry: Hello KingAOP");
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Console.WriteLine("OnSuccess: Hello KingAOP");
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Console.WriteLine("OnExit: Hello KingAOP");
        }
    }
}
