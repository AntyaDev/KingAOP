﻿using KingAOP.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KingAOP.Tests.MethodBoundaryTests.OnSuccess
{
    [TestClass]
    public class OnSuccessTests
    {
        [TestMethod]
        public void OnSuccess_AfterCall_ReturnArgumentValue_ShouldReturnValueWhichBiggerInPlusOne()
        {
            int initNumber = 1;
            dynamic myTest = new MyTestClass();
            int initNumber2 = myTest.ReturnArgumentValue(initNumber);
            Assert.AreEqual(initNumber + 1, initNumber2);
        }

        [TestMethod]
        public void OnSuccess_WhenSomeExceptionHappens_Then_OnSuccessShouldNotBeCalled()
        {
            int initNumber = 1, initNumber2 = 0;
            dynamic myTest = new MyTestClass();
            try
            {
                initNumber2 = myTest.ReturnArgumentValueWithException(initNumber);
            }
            catch
            { }
            Assert.AreEqual(0, initNumber2);
        }

        [TestMethod]
        public void OnSuccess_AfterCall_InitTestEntity_ShouldBeApplied_IncrementArgumentValueAspect()
        {
            var entity = new TestEntity();
            dynamic myTest = new MyTestClass();
            myTest.InitTestEntity(entity);
            Assert.AreEqual(entity.Name, "KingAOP_OnSuccess");
            Assert.AreEqual(entity.Number, 100);
        }

        [TestMethod]
        public void IntByRefArgumentShouldBeUpdatedByAspect()
        {
            dynamic test = new MyTestClass();

            int value = 5;
            test.MethodWithRefArgs(ref value);

            Assert.IsTrue(value == -1);
        }
    }
}
