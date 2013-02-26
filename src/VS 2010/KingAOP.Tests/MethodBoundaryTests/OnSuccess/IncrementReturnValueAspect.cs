using KingAOP.Aspects;

namespace KingAOP.Tests.MethodBoundaryTests.OnSuccess
{
    internal class IncrementReturnValueAspect : OnMethodBoundaryAspect
    {
        public override void OnSuccess(MethodExecutionArgs args)
        {
            args.ReturnValue = (int)args.ReturnValue + 1;
        }
    }
}
