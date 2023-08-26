namespace MoqToNSubstitute.Tests
{
    [TestClass]
    public class AssignmentSample
    {
        [TestMethod]
        public void Test_Assignments()
        {
            _testClass = new Mock<ITestClass>();
            _testClass = new Mock<ITestClass>(_mockClass.Object, _realClass);
            _testClass = new Mock<ITestClass>(_mockClass.Object, _realClass) { CallBase = true };
            _testClass = new Mock<ITestClass>() { CallBase = true };
        }
    }
}