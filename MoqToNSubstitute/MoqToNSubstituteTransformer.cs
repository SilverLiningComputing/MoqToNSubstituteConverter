using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using MoqToNSubstitute.Conversion;
using MoqToNSubstitute.Models;
using MoqToNSubstitute.Syntax;
using MoqToNSubstitute.Utilities;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MoqToNSubstitute.Tests")]

namespace MoqToNSubstitute;

internal class MoqToNSubstituteTransformer : ICodeTransformer
{
    public void Transform(string sourceFilePath, bool analysisOnly = true)
    {
        var substitutions = new CodeSyntax
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
        var sourceText = File.ReadAllText(sourceFilePath);

        if (!sourceText.Contains("Mock")) return;

        var tree = CSharpSyntaxTree.ParseText(sourceText);
        var root = tree.GetRoot();

        var rootAssignment = root.ReplaceAssignmentNodes(substitutions, "Mock");
        var rootNewObject = rootAssignment.ReplaceObjectCreationNodes(substitutions, "new Mock");
        var rootObject = rootNewObject.ReplaceArgumentNodes(substitutions, ".Object", "It.IsAny", "It.Is");
        var rootFields = rootObject.ReplaceVariableNodes(substitutions, "Mock<");
        var rootExpression = rootFields.ReplaceExpressionNodes(substitutions, ".Setup(",".Verify(");

        Logger.Log($"Modified File: \r\n{rootExpression}\r\n");
        if (root.ToFullString() == rootExpression.ToFullString() || analysisOnly) return;
        var modifiedCode = Formatter.Format(rootExpression, new AdhocWorkspace()).ToFullString();
        File.WriteAllText(sourceFilePath, modifiedCode);
    }

    public void GetNodeTypesFromFile(string sourceFilePath)
    {
        var sourceText = File.ReadAllText(sourceFilePath);
        if (!sourceText.Contains("Mock")) return;
        GetNodeTypesFromString(sourceText);
    }

    internal static void GetNodeTypesFromString(string sourceText)
    {
        var tree = CSharpSyntaxTree.ParseText(sourceText);
        var root = tree.GetRoot();

        var descendantNodes = root.DescendantNodes();
        var descendantNodeArray = descendantNodes.ToArray();

        foreach (var node in descendantNodeArray)
        {
            var originalCode = node.ToString();
            var nodeType = node.GetType().ToString();

            Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {originalCode}, Node Type: {nodeType}");
        }
    }
}