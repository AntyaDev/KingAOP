using KingAOP.Aspects;
using KingAOP.Tests.TestData;

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

    class IncrementArgumentValueAspect : OnMethodBoundaryAspect
    {
        public override void OnSuccess(MethodExecutionArgs args)
        {
            args.Arguments[0] = (int)args.Arguments[0] + 1;
        }
    }

    class IncrementReturnValueAspect : OnMethodBoundaryAspect
    {
        public override void OnSuccess(MethodExecutionArgs args)
        {
            args.ReturnValue = (int)args.ReturnValue + 1;
        }
    }

    class InitTestEntityAspect : OnMethodBoundaryAspect
    {
        public override void OnSuccess(MethodExecutionArgs args)
        {
            var testEntity = (TestEntity)args.Arguments[0];
            testEntity.Name = "KingAOP_OnSuccess";
            testEntity.Number = 100;
        }
    }
}
