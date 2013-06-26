using KingAOP.Aspects;

namespace KingAOP.Tests.MethodInterceptionTests.OnInvoke
{
    class ChangeArgumentsWithInvokationAspect : MethodInterceptionAspect
    {
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            args.Proceed();
            for (int i = 0; i < args.Arguments.Count; i++)
            {
                if (args.Arguments[i] is int) args.Arguments[i] = -1;
            }
        }
    }

    class NotInvokeOriginalMethodAspect : MethodInterceptionAspect
    {
        public override void OnInvoke(MethodInterceptionArgs args)
        { }
    }
}
