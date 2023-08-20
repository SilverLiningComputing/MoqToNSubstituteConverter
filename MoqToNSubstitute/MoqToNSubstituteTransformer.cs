using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using MoqToNSubstitute.Enums;
using MoqToNSubstitute.Utilities;
using System.Text.RegularExpressions;

namespace MoqToNSubstitute;

internal class MoqToNSubstituteTransformer : ICodeTransformer
{
    public void Transform(string sourceFilePath, bool analysisOnly = true)
    {
        var sourceText = File.ReadAllText(sourceFilePath);

        if (!sourceText.Contains("Moq")) return;

        var tree = CSharpSyntaxTree.ParseText(sourceText);
        var root = tree.GetRoot();

        var rootAssignment = root.ReplaceNodes(root.GetNodes<InvocationExpressionSyntax>("new Mock"),
            (node, _) =>
            {
                var originalCode = node.ToString();
                var transformedCode = ReplaceArgument(originalCode, NSubstituteArguments.Assignment);

                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {transformedCode}");

                return node.WithExpression(SyntaxFactory.ParseExpression(transformedCode));
            }
        );
        var rootObject = rootAssignment.ReplaceNodes(rootAssignment.GetNodes<ArgumentSyntax>(".Object"),
            (node, _) =>
            {
                var originalCode = node.ToString();
                var transformedCode = ReplaceArgument(originalCode, NSubstituteArguments.Object);

                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {transformedCode}");

                return node.WithExpression(SyntaxFactory.ParseExpression(transformedCode));
            }
        );
        var rootFields = rootObject.ReplaceNodes(rootObject.GetNodes<VariableDeclarationSyntax>("Mock<"),
            (node, _) =>
            {
                var originalType = node.Type.ToString();
                var transformedCode = ReplaceArgument(originalType, NSubstituteArguments.Fields);

                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original Type: {originalType}, Transformed: {transformedCode}");

                return node.Update(SyntaxFactory.ParseTypeName(transformedCode), node.Variables);
            }
        );
        var rootSetup = rootFields.ReplaceNodes(rootFields.GetNodes<ExpressionStatementSyntax>(".Setup("),
            (node, _) =>
            {
                var originalCode = node.ToString();
                var transformedCode = ReplaceArgument(originalCode, NSubstituteArguments.Setup);

                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {transformedCode}");

                return node.WithExpression(SyntaxFactory.ParseExpression(transformedCode));
            }
        );
        var rootVerify = rootSetup.ReplaceNodes(rootSetup.GetNodes<ExpressionStatementSyntax>(".Verify("),
            (node, _) =>
            {
                var originalCode = node.ToString();
                var transformedCode = ReplaceArgument(originalCode, NSubstituteArguments.Verify);

                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {transformedCode}");

                return node.WithExpression(SyntaxFactory.ParseExpression(transformedCode));
            }
        );

        Logger.Log($"Modified File: \r\n{rootVerify}\r\n");
        if (root.ToFullString() == rootVerify.ToFullString() || analysisOnly) return;
        var modifiedCode = rootVerify.ToFullString();
        File.WriteAllText(sourceFilePath, modifiedCode);
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

    internal static string ReplaceArgument(string originalCode, Enum argumentType)
    {
        string transformedCode;
        switch (argumentType)
        {
            case NSubstituteArguments.Object:
                return originalCode.Replace(".Object", "");
            case NSubstituteArguments.Assignment:
                return originalCode.Replace("new Mock", "Substitute.For");
            case NSubstituteArguments.Fields:
                return Regex.Replace(originalCode, "Mock\\<(?<start>.+)\\>", "${start} ");
            case NSubstituteArguments.Setup:
                // remove the carriage returns from the expression
                transformedCode = Regex.Replace(originalCode, "\r\n *", "")
                    .Replace("It.IsAny", "Arg.Any")
                    .Replace("It.Is", "Arg.Is")
                    .Replace(".Verifiable()", "");
                transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.ReturnsAsync(?<end>.+);", "${start}.${middle}.Returns${end}");
                transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.Returns(?<end>.+);", "${start}.${middle}.Returns${end}");
                transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.ThrowsAsync(?<end>.+);", "${start}.${middle}.Throws${end}");
                transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.Throws(?<end>.+);", "${start}.${middle}.Throws${end}");
                return Regex.Replace(transformedCode, "(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)(?<end>.+);", "${start}.${middle}.Throws${end}");
            case NSubstituteArguments.Verify:
                // remove the carriage returns from the expression
                transformedCode = Regex.Replace(originalCode, "\r\n *", "")
                    .Replace("It.IsAny", "Arg.Any")
                    .Replace("It.Is", "Arg.Is");
                transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Once\\);", "${start}.Received(1).${middle}");
                transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Never\\);", "${start}..DidNotReceive().${middle}");
                transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Exactly((?<times>.+))\\);", "${start}.Received(${times}).${middle}");
                return Regex.Replace(transformedCode, "(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+);", "${start}.Received().${middle}");
            default:
                throw new ArgumentOutOfRangeException(nameof(argumentType));
        }
    }
}