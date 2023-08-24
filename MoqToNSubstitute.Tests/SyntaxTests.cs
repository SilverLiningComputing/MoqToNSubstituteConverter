using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoqToNSubstitute.Tests.Helpers;
using System.Reflection;

namespace MoqToNSubstitute.Tests
{
    [TestClass]
    public class SyntaxTests
    {
        private static string? _fileContents;
        private static Assembly? _assembly;

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

        [TestMethod]
        public void Test_ReplaceVariableNodes()
        {
            Assert.IsNotNull(_assembly);
            var resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("VariableSample.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            var tree = CSharpSyntaxTree.ParseText(_fileContents);
            var root = tree.GetRoot();
            var rootFields = root.ReplaceVariableNodes("Mock<");
            var rootObject = rootFields.ReplaceObjectCreationNodes("Mock");
            var rootReplaced = rootObject.ReplaceArgumentNodes(".Object");
            resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("VariableSampleReplaced.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            Assert.AreEqual(_fileContents, rootReplaced.ToString());
        }

        [TestMethod]
        public void Test_ReplaceAssignmentNodes()
        {
            Assert.IsNotNull(_assembly);
            var resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("AssignmentSample.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            var tree = CSharpSyntaxTree.ParseText(_fileContents);
            var root = tree.GetRoot();
            var rootFields = root.ReplaceAssignmentNodes("new Mock");
            resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("AssignmentSampleReplaced.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            Assert.AreEqual(_fileContents, rootFields.ToString());
        }

        [TestMethod]
        public void Test_replace_fields_with_assignment_nodes()
        {
            Assert.IsNotNull(_assembly);
            var resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("FieldsWithAssignment.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            var tree = CSharpSyntaxTree.ParseText(_fileContents);
            var root = tree.GetRoot();
            var rootFields = root.ReplaceVariableNodes("Mock<");
            var rootNewObject = rootFields.ReplaceObjectCreationNodes("Mock");
            resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("FieldsWithAssignmentReplaced.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            Assert.AreEqual(_fileContents, rootNewObject.ToString());
        }
    }
}