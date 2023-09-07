using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MoqToNSubstitute.Extensions;
using MoqToNSubstitute.Templates;
using MoqToNSubstitute.Utilities;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MoqToNSubstitute.Tests")]

namespace MoqToNSubstitute.Conversion;

internal class MoqToNSubstituteTransformer : ICodeTransformer
{
    public void Transform(string sourceFilePath, bool transform = false)
    {
        var substitutions = ReplacementTemplate.ReturnReplacementSyntax();
        var sourceText = File.ReadAllText(sourceFilePath);

        if (!sourceText.Contains("Mock")) return;

        var tree = CSharpSyntaxTree.ParseText(sourceText);
        var root = tree.GetRoot();

        var rootObject = root.ReplaceArgumentNodes(substitutions, ".Object", "It.IsAny", "It.Is");
        var rootAssignment = rootObject.ReplaceAssignmentNodes(substitutions, "Mock");
        var rootNewObject = rootAssignment.ReplaceObjectCreationNodes(substitutions, "new Mock");
        var rootFields = rootNewObject.ReplaceVariableNodes(substitutions, "Mock<");
        var rootExpression = rootFields.ReplaceExpressionNodes(substitutions, ".Setup(", ".Verify(");

        Logger.Log($"Modified File: \r\n{rootExpression}\r\n");
        var modifiedCode = rootExpression.NormalizeWhitespace().ToFullString();
        if (root.ToFullString() == rootExpression.ToFullString() || !transform) return;
        File.WriteAllText(sourceFilePath, modifiedCode);
    }

    internal static void GetNodeTypesFromFile(string sourceFilePath)
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