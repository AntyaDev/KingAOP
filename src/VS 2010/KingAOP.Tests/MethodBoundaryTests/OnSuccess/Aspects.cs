using KingAOP.Aspects;

namespace KingAOP.Tests.MethodBoundaryTests.OnSuccess
{
    class ChangeArgumentAspect : OnMethodBoundaryAspect
    {
        public override void OnSuccess(MethodExecutionArgs args)
        {
            for (int i = 0; i < args.Arguments.Count; i++)
            {
                if (args.Arguments[i] is int) args.Arguments[i] = -1;

                else if (args.Arguments[i] is string) args.Arguments[i] = "I changed your value";
            }
        }
    }
}
