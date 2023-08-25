using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoqToNSubstitute.Extensions;
using MoqToNSubstitute.Tests.Helpers;
using System.Reflection;

namespace MoqToNSubstitute.Tests.Extensions
{
    [TestClass]
    public class SyntaxNodeExtensionsTests
    {
        private static Assembly? _assembly;
        private static string? _fileContents;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _ = context;
            _assembly = Assembly.GetExecutingAssembly();
            var resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("TaxServiceTests.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
        }

        [TestMethod]
        public void Test_GetNodes()
        {
            Assert.IsNotNull(_fileContents);
            var tree = CSharpSyntaxTree.ParseText(_fileContents);
            var root = tree.GetRoot();

            var assignmentExpressionSyntaxCollection = root.GetNodes<AssignmentExpressionSyntax>("new Mock");
            Assert.IsNotNull(assignmentExpressionSyntaxCollection);
            var assignmentExpressionSyntaxArray = assignmentExpressionSyntaxCollection.ToArray();
            Assert.IsNotNull(assignmentExpressionSyntaxArray);
            Assert.AreEqual(1, assignmentExpressionSyntaxArray.Length);
        }
    }
}
