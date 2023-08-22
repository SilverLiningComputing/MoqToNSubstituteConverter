using Microsoft.CodeAnalysis.CSharp;
using MoqToNSubstitute.Enums;
using MoqToNSubstitute.Utilities;

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
        var rootObject = rootAssignment.ReplaceArgumentNodes(".Object");
        var rootFields = rootObject.ReplaceVariableNodes("Mock<");
        var rootSetup = rootFields.ReplaceExpressionNodes(".Setup(", NSubstituteArguments.Setup);
        var rootVerify = rootSetup.ReplaceExpressionNodes(".Verify(", NSubstituteArguments.Verify);

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
}