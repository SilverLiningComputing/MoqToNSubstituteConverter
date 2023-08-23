using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoqToNSubstitute.Enums;
using MoqToNSubstitute.Utilities;
using System.Text.RegularExpressions;

namespace MoqToNSubstitute;

internal static class Node
{
    internal static IEnumerable<T> GetNodes<T>(this SyntaxNode root, string matchText)
    {
        return root.DescendantNodes().OfType<T>().Where(node => node != null && node.ToString()!.Contains(matchText));
    }

    internal static SyntaxNode ReplaceAssignmentNodes(this SyntaxNode root, string matchText)
    {
        return root.ReplaceNodes(root.GetNodes<AssignmentExpressionSyntax>(matchText),
            (node, _) =>
            {
                var originalCode = node.ToString();
                var nodeLeft = node.Left.ToString();
                var nodeLeftTransformed = ReplaceArgument(nodeLeft, NSubstituteArguments.Fields);

                var nodeRight = node.Right.ToString();
                var nodeRightTransformed = ReplaceArgument(nodeRight, NSubstituteArguments.Assignment);

                var updatedNode = node.Update(SyntaxFactory.ParseExpression($"{nodeLeftTransformed} "), node.OperatorToken, SyntaxFactory.ParseExpression($"{nodeRightTransformed}"));
                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {updatedNode}");

                return updatedNode;
            }
        );
    }

    internal static SyntaxNode ReplaceObjectCreationNodes(this SyntaxNode root, string matchText)
    {
        var rewriter = new MockToSubstituteRewriter();
        return root.ReplaceNodes(root.GetNodes<ObjectCreationExpressionSyntax>(matchText),
            (node, _) =>
            {
                var newNode = rewriter.VisitObjectCreationExpression(node);
                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {node}, Transformed: {newNode}");
                return newNode;
            }
        );
    }

    internal static SyntaxNode ReplaceExpressionNodes(this SyntaxNode root, string matchText, Enum nSubstituteArguments)
    {
        return root.ReplaceNodes(root.GetNodes<ExpressionStatementSyntax>(matchText),
            (node, _) =>
            {
                var originalCode = node.ToString();
                var transformedCode = ReplaceArgument(originalCode, nSubstituteArguments);

                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {transformedCode}");

                return node.WithExpression(SyntaxFactory.ParseExpression(transformedCode));
            }
        );
    }

    internal static SyntaxNode ReplaceVariableNodes(this SyntaxNode root, string matchText)
    {
        return root.ReplaceNodes(root.GetNodes<VariableDeclarationSyntax>(matchText),
            (node, _) =>
            {
                var originalType = node.Type.ToString();
                var transformedCode = ReplaceArgument(originalType, NSubstituteArguments.Fields);

                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original Type: {originalType}, Transformed: {transformedCode}");

                return node.Update(SyntaxFactory.ParseTypeName($"{transformedCode} "), node.Variables);
            } 
        );
    }

    internal static SyntaxNode ReplaceArgumentNodes(this SyntaxNode root, string matchText)
    {
        return root.ReplaceNodes(root.GetNodes<ArgumentSyntax>(matchText),
            (node, _) =>
            {
                var originalCode = node.ToString();
                var transformedCode = ReplaceArgument(originalCode, NSubstituteArguments.Object);

                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Transformed: {transformedCode}");

                return node.WithExpression(SyntaxFactory.ParseExpression(transformedCode));
            }
        );

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
                return Regex.Replace(originalCode, "Mock\\<(?<start>.+)\\>", "${start}");
            case NSubstituteArguments.Setup:
                // remove the carriage returns from the expression
                transformedCode = Regex.Replace(originalCode, "\r\n *", "")
                    .Replace("It.IsAny", "Arg.Any")
                    .Replace("It.Is", "Arg.Is")
                    .Replace(".Verifiable()", "")
                    .Replace(".Result", "");
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
                transformedCode = Regex.Replace(transformedCode, "(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Exactly(?<times>.+)\\);", "${start}.Received${times}.${middle}");
                return Regex.Replace(transformedCode, "(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+);", "${start}.Received().${middle}");
            default:
                throw new ArgumentOutOfRangeException(nameof(argumentType));
        }
    }
}