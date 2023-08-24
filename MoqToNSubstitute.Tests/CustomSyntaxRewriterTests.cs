using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoqToNSubstitute.Models;
using MoqToNSubstitute.Tests.Helpers;
using System.Reflection;

namespace MoqToNSubstitute.Tests
{
    [TestClass]
    public class CustomSyntaxRewriterTests
    {
        private static CodeSyntax? _original;
        private static CodeSyntax? _replacement;
        private static CustomSyntaxRewriter? _customSyntaxRewriter;
        private static SyntaxNode? _root;
        private static Assembly? _assembly;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _ = context;
            _original = new CodeSyntax
            {
                Identifier = "Mock",
                Argument = ".Object",
                VariableType = new Expression
                {
                    Text = "Mock\\<(?<start>.+)\\>",
                    IsRegex = true
                },
                AssignmentExpression = new Expression
                {
                    Text = "new Mock",
                    IsRegex = false
                }
            };
            _replacement = new CodeSyntax
            {
                Identifier = "Substitute.For",
                Argument = "",
                VariableType = new Expression
                {
                    Text = "${start}",
                    IsRegex = true
                },
                AssignmentExpression = new Expression
                {
                    Text = "Substitute.For",
                    IsRegex = false
                }
            };
            _customSyntaxRewriter = new CustomSyntaxRewriter(_original, _replacement);
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
            Assert.IsNotNull(_original);
            Assert.IsNotNull(_customSyntaxRewriter);
            var testNode = _root.GetNodes<ObjectCreationExpressionSyntax>(_original.Identifier).FirstOrDefault();
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
            Assert.IsNotNull(_original);
            Assert.IsNotNull(_customSyntaxRewriter);
            var testNode = _root.GetNodes<ArgumentSyntax>(_original.Argument).FirstOrDefault();
            Assert.IsNotNull(testNode);
            Assert.AreEqual("_calculatorServiceMock.Object", testNode.ToString());
            var replacementNode = _customSyntaxRewriter.VisitArgument(testNode);
            Assert.IsNotNull(replacementNode);
            Assert.AreEqual("_calculatorServiceMock", replacementNode.ToString());
        }

        [TestMethod]
        public void Test_VisitVariableDeclaration()
        {
            Assert.IsNotNull(_original);
            Assert.IsNotNull(_customSyntaxRewriter);
            const string node = "Mock<ITestClass> testClass = new();"; 
            var tree = CSharpSyntaxTree.ParseText(node); 
            var root = tree.GetRoot();
            var testNode = root.GetNodes<VariableDeclarationSyntax>(_original.Identifier).FirstOrDefault();
            Assert.IsNotNull(testNode);
            Assert.AreEqual("Mock<ITestClass> testClass = new()", testNode.ToString());
            var replacementNode = _customSyntaxRewriter.VisitVariableDeclaration(testNode);
            Assert.IsNotNull(replacementNode);
            Assert.AreEqual("ITestClass testClass = new()", replacementNode.ToString());
        }

        [TestMethod]
        public void Test_VisitAssignmentExpression()
        {
            Assert.IsNotNull(_original);
            Assert.IsNotNull(_customSyntaxRewriter);
            const string node = "_testClass = new Mock<ITextClass>(_mockClass.Object, _realClass);";
            var tree = CSharpSyntaxTree.ParseText(node);
            var root = tree.GetRoot();
            var testNode = root.GetNodes<AssignmentExpressionSyntax>(_original.Identifier).FirstOrDefault();
            Assert.IsNotNull(testNode);
            Assert.AreEqual("_testClass = new Mock<ITextClass>(_mockClass.Object, _realClass)", testNode.ToString());
            var replacementNode = _customSyntaxRewriter.VisitAssignmentExpression(testNode);
            Assert.IsNotNull(replacementNode);
            Assert.AreEqual("_testClass = Substitute.For<ITextClass>", replacementNode.ToString());
        }
    }
}
