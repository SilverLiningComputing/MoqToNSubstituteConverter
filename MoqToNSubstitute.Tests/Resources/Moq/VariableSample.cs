namespace MoqToNSubstitute.Tests
{
    [TestClass]
    public class VariableSample
    {
        private Mock<ITestClass> _testClassMock;
        private Mock<ITestClass>? _testClassMock;
        private Mock<ITestClass> _testClassMock = new();
        private Mock<ITestClass>? _testClassMock = new();

        [TestMethod]
        public void Test_Assignments()
        {
            Mock<ITestClass> testClass = new();
            Mock<ITestClass> testClass = new Mock<ITestClass>();
            Mock<ITestClass> testClass = new Mock<ITestClass>(_mockClass.Object, _realClass);
            Mock<ITestClass> testClass = new Mock<ITestClass>(_mockClass.Object, _realClass) { CallBase = true };
            var testClass = new Mock<ITestClass>();
            var testClass = new Mock<ITestClass>(_mockClass.Object, _realClass);
            var testClass = new Mock<ITestClass>(_mockClass.Object, _realClass) { CallBase = true };
        }
    }
}