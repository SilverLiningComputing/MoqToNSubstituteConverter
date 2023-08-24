namespace MoqToNSubstitute.Tests
{
    [TestClass]
    public class VariableSample
    {
        private ITestClass _testClassMock = Substitute.For<ITestClass>();
        private ITestClass? _testClassMock = Substitute.For<ITestClass>();
    }
}