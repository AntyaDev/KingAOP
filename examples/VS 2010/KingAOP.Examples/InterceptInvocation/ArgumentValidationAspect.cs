using System;
using KingAOP.Aspects;

namespace KingAOP.Examples.InterceptInvocation
{
    class ArgumentValidationAspect : MethodInterceptionAspect
    {
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            if ((int)args.Arguments[0] < 0) Console.WriteLine("The first argument is less then 0 and we will not invoke " + args.Method.Name);

            else args.Proceed();
        }
    }
}
