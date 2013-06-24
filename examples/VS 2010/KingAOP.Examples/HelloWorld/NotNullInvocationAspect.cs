using System;
using System.Linq;
using KingAOP.Aspects;

namespace KingAOP.Examples.HelloWorld
{
    class NotNullInvocationAspect : MethodInterceptionAspect
    {
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            if (args.Arguments.Any(arg => arg == null)) Console.WriteLine("\n" + args.Method + " can't be called because some of args is null");

            else args.Proceed();
        }
    }
}
