using Microsoft.CodeAnalysis;

namespace MoqToNSubstitute.Extensions
{
    internal static class SyntaxNodeExtensions
    {
        internal static IEnumerable<T> GetNodes<T>(this SyntaxNode root, params string[] matchStrings)
        {
            return root.DescendantNodes().OfType<T>().Where(node =>
            {
                return !(node == null || !matchStrings.Any(match => node.ToString()!.Contains(match)));
            });
        }
    }
}
