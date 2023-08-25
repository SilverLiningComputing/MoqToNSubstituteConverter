using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using MoqToNSubstitute.Models;
using MoqToNSubstitute.Syntax;
using MoqToNSubstitute.Utilities;
using System.Runtime.CompilerServices;
using MoqToNSubstitute.Templates;

[assembly: InternalsVisibleTo("MoqToNSubstitute.Tests")]

namespace MoqToNSubstitute.Conversion;

internal class MoqToNSubstituteTransformer : ICodeTransformer
{
    public void Transform(string sourceFilePath, bool analysisOnly = true)
    {
        var substitutions = ReplacementTemplate.ReturnReplacementSyntax();
        var sourceText = File.ReadAllText(sourceFilePath);

        if (!sourceText.Contains("Mock")) return;

        var tree = CSharpSyntaxTree.ParseText(sourceText);
        var root = tree.GetRoot();

        var rootAssignment = root.ReplaceAssignmentNodes(substitutions, "Mock");
        var rootNewObject = rootAssignment.ReplaceObjectCreationNodes(substitutions, "new Mock");
        var rootObject = rootNewObject.ReplaceArgumentNodes(substitutions, ".Object", "It.IsAny", "It.Is");
        var rootFields = rootObject.ReplaceVariableNodes(substitutions, "Mock<");
        var rootExpression = rootFields.ReplaceExpressionNodes(substitutions, ".Setup(", ".Verify(");

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