using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoqToNSubstitute.Extensions;
using MoqToNSubstitute.Models;
using MoqToNSubstitute.Syntax;
using MoqToNSubstitute.Tests.Helpers;
using System.Reflection;

namespace MoqToNSubstitute.Tests
{
    [TestClass]
    public class SyntaxReplacerTests
    {
        private static string? _fileContents;
        private static Assembly? _assembly;
        private static CodeSyntax? _substitutions;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _ = context;
            _substitutions = new CodeSyntax
            {
                Identifier = new Expression("Mock", "Substitute.For", false),
                Argument = new List<Expression>
                {
                    new(".Object", "", false),
                    new("It.IsAny", "Arg.Any", false),
                    new("It.Is", "Arg.Is", false)
                },
                VariableType = new List<Expression>
                {
                    new("Mock\\<(?<start>.+)\\>", "${start}", true)
                },
                AssignmentExpression = new Expression("new Mock", "Substitute.For", false),
                ExpressionStatement = new List<Expression>
                {
                    new("\r\n *", "", true),
                    new("It.IsAny", "Arg.Any", false),
                    new("It.Is", "Arg.Is", false),
                    new(".Verifiable()", "", false),
                    new(".Result", "", false),
                    new("(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.ReturnsAsync(?<end>.+);", "${start}.${middle}.Returns${end}", true),
                    new("(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.Returns(?<end>.+);", "${start}.${middle}.Returns${end}", true),
                    new("(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.ThrowsAsync(?<end>.+);", "${start}.${middle}.Throws${end}", true),
                    new("(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.Throws(?<end>.+);", "${start}.${middle}.Throws${end}", true),
                    new("(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)(?<end>.+);", "${start}.${middle}.Throws${end}", true),
                    new("(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Once\\);", "${start}.Received(1).${middle}", true),
                    new("(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Never\\);", "${start}.DidNotReceive().${middle}", true),
                    new("(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Exactly(?<times>.+)\\);", "${start}.Received${times}.${middle}", true),
                    new("(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+);", "${start}.Received().${middle}", true),
                }
            }; _assembly = Assembly.GetExecutingAssembly();
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
            Assert.IsNotNull(_substitutions);
            var resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("VariableSample.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            var tree = CSharpSyntaxTree.ParseText(_fileContents);
            var root = tree.GetRoot();
            var rootFields = root.ReplaceVariableNodes(_substitutions, "Mock<");
            var rootObject = rootFields.ReplaceObjectCreationNodes(_substitutions, "Mock");
            var rootReplaced = rootObject.ReplaceArgumentNodes(_substitutions, ".Object");
            resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("VariableSampleReplaced.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            Assert.AreEqual(_fileContents, rootReplaced.ToString());
        }

        [TestMethod]
        public void Test_ReplaceAssignmentNodes()
        {
            Assert.IsNotNull(_assembly);
            Assert.IsNotNull(_substitutions);
            var resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("AssignmentSample.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            var tree = CSharpSyntaxTree.ParseText(_fileContents);
            var root = tree.GetRoot();
            var rootFields = root.ReplaceAssignmentNodes(_substitutions, "new Mock");
            resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("AssignmentSampleReplaced.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            Assert.AreEqual(_fileContents, rootFields.ToString());
        }

        [TestMethod]
        public void Test_replace_fields_with_assignment_nodes()
        {
            Assert.IsNotNull(_assembly);
            Assert.IsNotNull(_substitutions);
            var resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("FieldsWithAssignment.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            var tree = CSharpSyntaxTree.ParseText(_fileContents);
            var root = tree.GetRoot();
            var rootFields = root.ReplaceVariableNodes(_substitutions, "Mock<");
            var rootNewObject = rootFields.ReplaceObjectCreationNodes(_substitutions, "Mock");
            resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("FieldsWithAssignmentReplaced.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            Assert.AreEqual(_fileContents, rootNewObject.ToString());
        }

        [TestMethod]
        public void Test_setup_and_verify()
        {
            Assert.IsNotNull(_assembly);
            Assert.IsNotNull(_substitutions);
            var resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("SetupAndVerify.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            var tree = CSharpSyntaxTree.ParseText(_fileContents);
            var root = tree.GetRoot();
            var rootExpression = root.ReplaceExpressionNodes(_substitutions, ".Setup(",  ".Verify(");
            resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("SetupAndVerifyReplaced.cs"));
            _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            Assert.IsFalse(string.IsNullOrEmpty(_fileContents));
            Assert.AreEqual(_fileContents, rootExpression.ToString());
        }
    }
}