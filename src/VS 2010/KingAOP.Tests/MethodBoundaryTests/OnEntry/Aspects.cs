using KingAOP.Aspects;

namespace KingAOP.Tests.MethodBoundaryTests.OnEntry
{
    class ChangeStringArgumentAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            args.Arguments[0] = "I changed your value";
        }
    }
}
