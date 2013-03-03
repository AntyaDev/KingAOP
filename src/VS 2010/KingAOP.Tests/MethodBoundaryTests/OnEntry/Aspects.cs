using KingAOP.Aspects;
using KingAOP.Tests.TestData;

namespace KingAOP.Tests.MethodBoundaryTests.OnEntry
{
    class ChangeStringArgumentAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            args.Arguments[0] = "I changed your value";
        }
    }

    class ChangeObjectArgumentAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            var entity = (TestEntity)args.Arguments[0];
            entity.Name = "ChangedName";
            entity.Number = 999;
        }
    }
}
