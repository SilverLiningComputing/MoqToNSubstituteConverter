using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using MoqToNsubstitute.Utilities;

namespace MoqToNSubstitute;

internal class MoqToNSubstituteTransformer : ICodeTransformer
{
    public void Transform(string sourceFilePath)
    {
        var sourceText = File.ReadAllText(sourceFilePath);

        if (!sourceText.Contains("Moq")) return;

        var tree = CSharpSyntaxTree.ParseText(sourceText);
        var root = tree.GetRoot();

        var newRoot = root.ReplaceNodes(
            root.DescendantNodes().OfType<InvocationExpressionSyntax>().Where(node => node.ToString().StartsWith("mock.Setup")),
            (node, _) =>
            {
                var originalCode = node.ToString();
                var transformedCode = originalCode.Replace("mock.Setup(m => m.", "mock.");

                Logger.Log($"File: {sourceFilePath}, Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {transformedCode}");

                return node.WithExpression(SyntaxFactory.ParseExpression(transformedCode));
            }
        );

        if (root.ToFullString() != newRoot.ToFullString())
        {
            var formattedCode = Formatter.Format(newRoot, new AdhocWorkspace()).ToFullString();
            File.WriteAllText(sourceFilePath, formattedCode);
        }
    }

    public void Analyze(string sourceFilePath)
    {
        var sourceText = File.ReadAllText(sourceFilePath);

        if (!sourceText.Contains("Moq")) return;

        var tree = CSharpSyntaxTree.ParseText(sourceText);
        var root = tree.GetRoot();

        var assignmentExpressionSyntaxCollection = root.GetNodes<AssignmentExpressionSyntax>("new Mock");
            
        var assignmentExpressionSyntaxArray = assignmentExpressionSyntaxCollection.ToArray();
        foreach (var node in assignmentExpressionSyntaxArray)
        {
            var originalCode = node.ToString();
            var transformedCode = originalCode.Replace("new Mock", "Substitute.For");

            Logger.Log($"File: {sourceFilePath}, Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {transformedCode}");
        }

        var argumentSyntaxCollection = root.GetNodes<ArgumentSyntax>(".Object");

        var argumentSyntaxArray = argumentSyntaxCollection.ToArray();
        foreach (var node in argumentSyntaxArray)
        {
            var originalCode = node.ToString();
            var transformedCode = originalCode.Replace(".Object", "");

            Logger.Log($"File: {sourceFilePath}, Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {transformedCode}");
        }

        var fieldDeclarationSyntaxCollection = root.GetNodes<FieldDeclarationSyntax>("Mock<");
        var fieldDeclarationSyntaxArray = fieldDeclarationSyntaxCollection.ToArray();
        foreach (var node in fieldDeclarationSyntaxArray)
        {
            var originalCode = node.ToString();
            var transformedCode = Regex.Replace(originalCode, "Mock\\<(?<start>.+)\\>", "${start}");
                
            Logger.Log($"File: {sourceFilePath}, Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {transformedCode}");
        }

        var expressionStatementSyntaxCollection = root.GetNodes<ExpressionStatementSyntax>(".Setup(");
        var expressionStatementSyntaxArray = expressionStatementSyntaxCollection.ToArray();

        foreach (var node in expressionStatementSyntaxArray)
        {
            var originalCode = node.ToString();
            // remove the carriage returns from the expression
            var transformedCode = Regex.Replace(originalCode, "\r\n *", "");
            transformedCode = transformedCode.Replace("It.IsAny", "Arg.Any");
            transformedCode = transformedCode.Replace("It.Is", "Arg.Is");
            transformedCode = transformedCode.Replace(".Verifiable()", "");
            transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.ReturnsAsync(?<end>.+)", "${start}.${middle}.Returns${end}");
            transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.Returns(?<end>.+)", "${start}.${middle}.Returns${end}");

            transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.ThrowsAsync(?<end>.+)", "${start}.${middle}.Throws${end}");
            transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.Throws(?<end>.+)", "${start}.${middle}.Throws${end}");
            transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)(?<end>.+)", "${start}.${middle}.Throws${end}");

            Logger.Log($"File: {sourceFilePath}, Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {transformedCode}");
        }

        expressionStatementSyntaxCollection = root.GetNodes<ExpressionStatementSyntax>(".Verify(");
        expressionStatementSyntaxArray = expressionStatementSyntaxCollection.ToArray();

        foreach (var node in expressionStatementSyntaxArray)
        {
            var originalCode = node.ToString();
            // remove the carriage returns from the expression
            var transformedCode = Regex.Replace(originalCode, "\r\n *", "");
            transformedCode = transformedCode.Replace("It.IsAny", "Arg.Any");
            transformedCode = transformedCode.Replace("It.Is", "Arg.Is");
            transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Once\\)(?<end>.+)", "${start}.Received(1).${middle}${end}");
            transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Never\\)(?<end>.+)", "${start}..DidNotReceive().${middle}${end}");
            transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Exactly((?<times>.+))\\)(?<end>.+)", "${start}.Received(${times}).${middle}${end}");
            transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)(?<end>.+)", "${start}.Received().${middle}${end}");

            Logger.Log($"File: {sourceFilePath}, Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {transformedCode}");
        }
    }

    public void ShowNodes(string sourceFilePath)
    {
        var sourceText = File.ReadAllText(sourceFilePath);

        if (!sourceText.Contains("Moq")) return;

        var tree = CSharpSyntaxTree.ParseText(sourceText);
        var root = tree.GetRoot();

        var descendantNodes = root.DescendantNodes();
        var descendantNodeArray = descendantNodes.ToArray();

        Logger.Log($"File: {sourceFilePath}");

        foreach (var node in descendantNodeArray)
        {
            var originalCode = node.ToString();
            var nodeType = node.GetType().ToString();

            Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Node Type: {nodeType}");
        }
    }
}