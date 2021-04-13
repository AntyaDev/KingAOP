using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KingAOP.Tests.MethodInterceptionTests.OnInvoke
{
    [TestClass]
    public class OnInvokeTests
    {
        [TestMethod]
        public void IntByRefArgumentShouldBeUpdatedByAspect()
        {
            dynamic test = new MyTestClass();

            int value = 5;
            test.MethodWithRefArgs(ref value);
            
            Assert.IsTrue(value == -1);
        }

        [TestMethod]
        public void IntArgumentShouldNotBeUpdatedByAspect()
        {
            dynamic test = new MyTestClass();

            int value = 5;
            test.SimpleMethod(value);

            Assert.IsTrue(value == 5);
        }

        [TestMethod]
        public void OriginalMethodCanBeNotInvoked()
        {
            dynamic test = new MyTestClass();
            
            test.SimpleMethod();

            Assert.IsFalse(test.OriginalMethodCalled);
        }

        [TestMethod]
        public void OriginalMethodCanBeInvokedWithInChaninOfExpressions()
        {
            dynamic test = new MyTestClass();

            string value1 = "9";
            string value2 = "9";
            test.SimpleMethod(Convert.ToInt32(value1) * Convert.ToInt32(value2) - 81);

            Assert.IsTrue(test.OriginalMethodCalled);
        }
    }
}
