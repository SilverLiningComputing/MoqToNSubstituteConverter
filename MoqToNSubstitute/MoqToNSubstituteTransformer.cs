using Microsoft.CodeAnalysis.CSharp;
using MoqToNSubstitute.Enums;
using MoqToNSubstitute.Utilities;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MoqToNSubstitute.Tests")]

namespace MoqToNSubstitute;

internal class MoqToNSubstituteTransformer : ICodeTransformer
{
    public void Transform(string sourceFilePath, bool analysisOnly = true)
    {
        var sourceText = File.ReadAllText(sourceFilePath);

        if (!sourceText.Contains("Mock")) return;

        var tree = CSharpSyntaxTree.ParseText(sourceText);
        var root = tree.GetRoot();

        var rootAssignment = root.ReplaceAssignmentNodes("Mock");
        var rootNewObject = rootAssignment.ReplaceObjectCreationNodes("new Mock");
        var rootObject = rootNewObject.ReplaceArgumentNodes(".Object");
        var rootFields = rootObject.ReplaceVariableNodes("Mock<");
        var rootSetup = rootFields.ReplaceExpressionNodes(".Setup(", NSubstituteArguments.Setup);
        var rootVerify = rootSetup.ReplaceExpressionNodes(".Verify(", NSubstituteArguments.Verify);

        Logger.Log($"Modified File: \r\n{rootVerify}\r\n");
        if (root.ToFullString() == rootVerify.ToFullString() || analysisOnly) return;
        var modifiedCode = rootVerify.ToFullString();
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