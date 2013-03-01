using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KingAOP.Tests.MethodBoundaryTests.OnEntry
{
    [TestClass]
    public class OnEntryTests
    {
        [TestMethod]
        public void OnEntry_ShouldCanAbility_To_Change_TheStringArgument_Value()
        {
            dynamic myTest = new MyTestClass();
            string argument = myTest.ResturnStringArgument("argument");
            Assert.AreEqual(argument, "I changed your value");
        }
    }
}
