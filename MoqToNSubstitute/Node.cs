using Microsoft.CodeAnalysis;

namespace MoqToNSubstitute;

internal static class Node
{
    internal static IEnumerable<T> GetNodes<T>(this SyntaxNode root, string matchText)
    {
        return root.DescendantNodes().OfType<T>().Where(node => node != null && node.ToString()!.Contains(matchText));
    }
}