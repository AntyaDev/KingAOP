using KingAOP.Aspects;

namespace KingAOP.Tests.MethodBoundaryTests.OnSuccess
{
    internal class IncrementArgumentValueAspect : OnMethodBoundaryAspect
    {
        public override void OnSuccess(MethodExecutionArgs args)
        {
            args.Arguments[0] = (int)args.Arguments[0] + 1;
        }
    }
}