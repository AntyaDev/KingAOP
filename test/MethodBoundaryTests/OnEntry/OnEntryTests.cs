﻿using KingAOP.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KingAOP.Tests.MethodBoundaryTests.OnEntry
{
    [TestClass]
    public class OnEntryTests
    {
        [TestMethod]
        public void OnEntry_ShouldNotChange_StringArgument()
        {
            dynamic myTest = new MyTestClass();
            string argument = myTest.ResturnStringArgument("argument");
            Assert.AreEqual(argument, "argument");
        }

        [TestMethod]
        public void OnEntry_ShouldChange_StringArgument_PassedAsRef()
        {
            dynamic myTest = new MyTestClass();
            string argument = "argument";
            string ret = myTest.ResturnStringArgumentPassedAsRef(ref argument);
            Assert.AreNotEqual(ret, "argument");
        }

        [TestMethod]
        public void OnEntry_ShouldHaveAbilityToChange_ReferenceArgument()
        {
            var entity = new TestEntity {Name = "Name", Number = 0};

            dynamic myTest = new MyTestClass();
            myTest.ResturnObjectArgument(entity);

            Assert.AreEqual("ChangedName", entity.Name);
            Assert.AreEqual(999, entity.Number);
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
