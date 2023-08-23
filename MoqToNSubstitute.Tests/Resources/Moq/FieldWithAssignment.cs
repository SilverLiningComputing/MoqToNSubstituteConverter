namespace MoqToNSubstitute.Tests
{
    [TestClass]
    public class VariableSample
    {
        private Mock<ITestClass> _testClassMock = new Mock<ITestClass>();
        private Mock<ITestClass>? _testClassMock = new Mock<ITestClass>();
    }
}