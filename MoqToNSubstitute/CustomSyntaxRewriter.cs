using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MoqToNSubstitute
{
    internal class CustomSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly string? _identifier;
        private readonly string? _replacement;

        public CustomSyntaxRewriter(string identifier, string replacement)
        {
            _identifier = identifier;
            _replacement = replacement;
        }

        public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            // Check if the object creation expression is of type "new Identifier<T>()"
            if (node.Type is not GenericNameSyntax genericName || genericName.Identifier.Text != _identifier ||
                genericName.TypeArgumentList.Arguments.Count != 1) return base.VisitObjectCreationExpression(node);
            // Create a new "Replacement<T>()" expression
            var substituteExpression = SyntaxFactory.ParseExpression($"{_replacement}<{genericName.TypeArgumentList.Arguments[0]}>()");

            // Return the node with the new expression
            return substituteExpression;
        }

        public override SyntaxNode? VisitArgument(ArgumentSyntax node)
        {
            return base.VisitArgument(node);
        }
    }
}