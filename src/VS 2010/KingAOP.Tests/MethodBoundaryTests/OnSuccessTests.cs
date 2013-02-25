using System;
using System.Dynamic;
using System.Linq.Expressions;
using KingAOP.Aspects;
using KingAOP.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KingAOP.Tests.MethodBoundaryTests
{
    [TestClass]
    public class OnSuccessTests
    {
        #region IncrementAspect

        class MyTestClass : IDynamicMetaObjectProvider
        {
            [IncrementAspect]
            public int TestCall(int number)
            {
                return number;
            }

            public DynamicMetaObject GetMetaObject(Expression parameter)
            {
                return new AspectWeaver(parameter, this, typeof(MyTestClass));
            }
        }

        [Serializable]
        class IncrementAspect : OnMethodBoundaryAspect
        {
            public override void OnSuccess(MethodExecutionArgs args)
            {
                args.ReturnValue = (int)args.ReturnValue + 1;
            }
        }

        [TestMethod]
        public void AfterOnSuccess_TheReturnValue_ShouldBeChanged_InPlusOne()
        {
            int initNumber = 1;
            dynamic myTest = new MyTestClass();
            int initNumber2 = myTest.TestCall(initNumber);
            Assert.AreEqual(initNumber + 1, initNumber2);
        }

        #endregion
    }
}
