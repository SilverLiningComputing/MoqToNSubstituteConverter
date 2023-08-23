using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MoqToNSubstitute
{

    class MockToSubstituteRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            // Check if the object creation expression is of type "new Mock<T>()"
            if (node.Type is GenericNameSyntax genericName &&
                genericName.Identifier.Text == "Mock" &&
                genericName.TypeArgumentList.Arguments.Count == 1)
            {
                // Create a new "Substitute.For<T>()" expression
                var substituteExpression = SyntaxFactory.ParseExpression($"Substitute.For<{genericName.TypeArgumentList.Arguments[0]}>()");

                // Replace the old node with the new expression
                return substituteExpression;
            }

            return base.VisitObjectCreationExpression(node);
        }
    }
}