﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoqToNSubstitute.Syntax;

namespace MoqToNSubstitute.Tests.Syntax;

[TestClass]
public class CustomSyntaxRewriterTests
{
    private static CustomSyntaxRewriter? _customSyntaxRewriter;
    private static SyntaxNode? _root;
    private static Assembly? _assembly;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        _ = context;
        var substitutions = ReplacementTemplate.ReturnReplacementSyntax();
        _customSyntaxRewriter = new CustomSyntaxRewriter(substitutions);
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
        Assert.IsNotNull(_customSyntaxRewriter);
        var testNode = _root.GetNodes<ObjectCreationExpressionSyntax>("Mock").FirstOrDefault();
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
        Assert.IsNotNull(_customSyntaxRewriter);
        var testNode = _root.GetNodes<ArgumentSyntax>(".Object").FirstOrDefault();
        Assert.IsNotNull(testNode);
        Assert.AreEqual("_calculatorServiceMock.Object", testNode.ToString());
        var replacementNode = _customSyntaxRewriter.VisitArgument(testNode);
        Assert.IsNotNull(replacementNode);
        Assert.AreEqual("_calculatorServiceMock", replacementNode.ToString());
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Test_VisitVariableDeclaration(bool isRegEx)
    {
        Assert.IsNotNull(_customSyntaxRewriter);
        const string node = "Mock<ITestClass> testClass = new Mock<ITestClass>();";
        var tree = CSharpSyntaxTree.ParseText(node);
        var root = tree.GetRoot();
        var testNode = root.GetNodes<VariableDeclarationSyntax>("Mock").FirstOrDefault();
        Assert.IsNotNull(testNode);
        Assert.AreEqual("Mock<ITestClass> testClass = new Mock<ITestClass>()", testNode.ToString());
        if (isRegEx)
        {
            var replacementNode = _customSyntaxRewriter.VisitVariableDeclaration(testNode);
            Assert.IsNotNull(replacementNode);
            Assert.AreEqual("ITestClass testClass = new Mock<ITestClass>()", replacementNode.ToString());
        }
        else
        {
            var substitutions = new CodeSyntax
            {
                VariableType = new List<Expression>
                {
                    new("Mock<", "", false)
                }
            };
            var customSyntaxRewriter = new CustomSyntaxRewriter(substitutions);
            var replacementNode = customSyntaxRewriter.VisitVariableDeclaration(testNode);
            Assert.IsNotNull(replacementNode);
            Assert.AreEqual("ITestClass> testClass = new Mock<ITestClass>()", replacementNode.ToString());
        }
    }

    [TestMethod]
    public void Test_VisitAssignmentExpression()
    {
        Assert.IsNotNull(_customSyntaxRewriter);
        const string node = "_testClass = new Mock<ITextClass>(_mockClass.Object, _realClass);";
        var tree = CSharpSyntaxTree.ParseText(node);
        var root = tree.GetRoot();
        var testNode = root.GetNodes<AssignmentExpressionSyntax>("Mock").FirstOrDefault();
        Assert.IsNotNull(testNode);
        Assert.AreEqual("_testClass = new Mock<ITextClass>(_mockClass.Object, _realClass)", testNode.ToString());
        var replacementNode = _customSyntaxRewriter.VisitAssignmentExpression(testNode);
        Assert.IsNotNull(replacementNode);
        Assert.AreEqual("_testClass = Substitute.For<ITextClass>", replacementNode.ToString());
    }

    [TestMethod]
    public void Test_VisitExpressionStatement()
    {
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