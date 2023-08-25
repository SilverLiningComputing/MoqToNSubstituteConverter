using MoqToNSubstitute.Conversion;
using MoqToNSubstitute.Tests.Helpers;
using System.Reflection;

namespace MoqToNSubstitute.Tests.Extensions
{
    [TestClass]
    public class MoqToNSubstituteTransformerTests
    {
        private string? _fileContents;

        [TestInitialize]
        public void TestInitialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(n => n.EndsWith("VariableSample.cs"));
            Assert.IsFalse(string.IsNullOrEmpty(resourceName));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
        }

        [TestMethod]
        public void Test_GetNodes()
        {
            Assert.IsNotNull(_fileContents);
            MoqToNSubstituteTransformer.GetNodeTypesFromString(_fileContents);
        }

        [TestMethod]
        public void Test_GetAssignmentNodes()
        {
            Assert.IsNotNull(_fileContents);
            MoqToNSubstituteTransformer.GetNodeTypesFromString(_fileContents);
        }
    }
}
