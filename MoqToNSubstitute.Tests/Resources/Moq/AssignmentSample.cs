namespace MoqToNSubstitute.Tests
{
    [TestClass]
    public class AssignmentSample
    {
        [TestMethod]
        public void Test_Assignments()
        {
            _testClass = new Mock<ITestClass>();
        }
    }
}