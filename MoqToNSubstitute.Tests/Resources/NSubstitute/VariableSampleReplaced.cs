namespace MoqToNSubstitute.Tests
{
    [TestClass]
    public class VariableSample
    {
        private ITestClass _testClassMock;
        private ITestClass? _testClassMock;
        private ITestClass _testClassMock = new();
        private ITestClass? _testClassMock = new();

        [TestMethod]
        public void Test_Assignments()
        {
ITestClass testClass = new();
ITestClass testClass = Substitute.For<ITestClass>();
var testClass = Substitute.For<ITestClass>();
var testClass = Substitute.For<ITestClass>(_mockClass, _realClass);
        }
    }
}