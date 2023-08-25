using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoqToNSubstitute.Models;
using MoqToNSubstitute.Tests.Helpers;
using System.Reflection;
using MoqToNSubstitute.Extensions;
using MoqToNSubstitute.Syntax;

namespace MoqToNSubstitute.Tests
{
    [TestClass]
    public class CustomSyntaxRewriterTests
    {
        private static CodeSyntax? _substitutions;
        private static CustomSyntaxRewriter? _customSyntaxRewriter;
        private static SyntaxNode? _root;
        private static Assembly? _assembly;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
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
            };
            _customSyntaxRewriter = new CustomSyntaxRewriter(_substitutions);
            _assembly = Assembly.GetExecutingAssembly();
            var resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("TaxServiceTests.cs"));
            var fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            var tree = CSharpSyntaxTree.ParseText(fileContents);
            _root = tree.GetRoot();
        }

        [TestMethod]
        public void Test_VisitObjectCreationExpression()
        {
            Assert.IsNotNull(_root);
            Assert.IsNotNull(_substitutions);
            Assert.IsNotNull(_customSyntaxRewriter);
            var testNode = _root.GetNodes<ObjectCreationExpressionSyntax>(_substitutions.Identifier.Original).FirstOrDefault();
            Assert.IsNotNull(testNode);
            Assert.AreEqual("new Mock<ICalculatorService>()", testNode.ToString());
            var replacementNode = _customSyntaxRewriter.VisitObjectCreationExpression(testNode);
            Assert.IsNotNull(replacementNode);
            Assert.AreEqual("Substitute.For<ICalculatorService>()", replacementNode.ToString());
        }

        [TestMethod]
        public void Test_VisitArgument()
        {
            Assert.IsNotNull(_root);
            Assert.IsNotNull(_substitutions);
            Assert.IsNotNull(_customSyntaxRewriter);
            var testNode = _root.GetNodes<ArgumentSyntax>(".Object").FirstOrDefault();
            Assert.IsNotNull(testNode);
            Assert.AreEqual("_calculatorServiceMock.Object", testNode.ToString());
            var replacementNode = _customSyntaxRewriter.VisitArgument(testNode);
            Assert.IsNotNull(replacementNode);
            Assert.AreEqual("_calculatorServiceMock", replacementNode.ToString());
        }

        [TestMethod]
        public void Test_VisitVariableDeclaration()
        {
            Assert.IsNotNull(_substitutions);
            Assert.IsNotNull(_customSyntaxRewriter);
            const string node = "Mock<ITestClass> testClass = new();"; 
            var tree = CSharpSyntaxTree.ParseText(node); 
            var root = tree.GetRoot();
            var testNode = root.GetNodes<VariableDeclarationSyntax>(_substitutions.Identifier.Original).FirstOrDefault();
            Assert.IsNotNull(testNode);
            Assert.AreEqual("Mock<ITestClass> testClass = new()", testNode.ToString());
            var replacementNode = _customSyntaxRewriter.VisitVariableDeclaration(testNode);
            Assert.IsNotNull(replacementNode);
            Assert.AreEqual("ITestClass testClass = new()", replacementNode.ToString());
        }

        [TestMethod]
        public void Test_VisitAssignmentExpression()
        {
            Assert.IsNotNull(_substitutions);
            Assert.IsNotNull(_customSyntaxRewriter);
            const string node = "_testClass = new Mock<ITextClass>(_mockClass.Object, _realClass);";
            var tree = CSharpSyntaxTree.ParseText(node);
            var root = tree.GetRoot();
            var testNode = root.GetNodes<AssignmentExpressionSyntax>(_substitutions.Identifier.Original).FirstOrDefault();
            Assert.IsNotNull(testNode);
            Assert.AreEqual("_testClass = new Mock<ITextClass>(_mockClass.Object, _realClass)", testNode.ToString());
            var replacementNode = _customSyntaxRewriter.VisitAssignmentExpression(testNode);
            Assert.IsNotNull(replacementNode);
            Assert.AreEqual("_testClass = Substitute.For<ITextClass>", replacementNode.ToString());
        }

        [TestMethod]
        public void Test_VisitExpressionStatement()
        {
            Assert.IsNotNull(_substitutions);
            Assert.IsNotNull(_customSyntaxRewriter);
            Assert.IsNotNull(_assembly);
            var resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("SetupAndVerify.cs"));
            var fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
            var tree = CSharpSyntaxTree.ParseText(fileContents);
            var root = tree.GetRoot();
            var testNode = root.GetNodes<ExpressionStatementSyntax>(".Setup").FirstOrDefault();
            Assert.IsNotNull(testNode);
            Assert.AreEqual("_classMock.Setup(x => x.Setup(It.IsAny<string>(), It.IsAny<int>())).Returns(0);", testNode.ToString());
            var replacementNode = _customSyntaxRewriter.VisitExpressionStatement(testNode);
            Assert.IsNotNull(replacementNode);
            Assert.AreEqual("_classMock.Setup(Arg.Any<string>(), Arg.Any<int>()).Returns(0);", replacementNode.ToString());
        }
    }
}
