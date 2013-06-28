using KingAOP.Aspects;
using KingAOP.Tests.TestData;

namespace KingAOP.Tests.MethodBoundaryTests.OnEntry
{
    class ChangeArgumentAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            for (int i = 0; i < args.Arguments.Count; i++)
            {
                if (args.Arguments[i] is int) args.Arguments[i] = -1;

                else if (args.Arguments[i] is string) args.Arguments[i] = "I changed your value";
            }
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
