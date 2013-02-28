using KingAOP.Aspects;

namespace KingAOP.Tests.MethodBoundaryTests.OnSuccess
{
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
