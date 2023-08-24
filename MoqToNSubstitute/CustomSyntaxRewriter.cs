using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoqToNSubstitute.Models;
using System.Text.RegularExpressions;

namespace MoqToNSubstitute
{
    public class CustomSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly CodeSyntax _original;
        private readonly CodeSyntax _replacement;

        public CustomSyntaxRewriter(CodeSyntax original, CodeSyntax replacement)
        {
            _original = original;
            _replacement = replacement;
        }

        public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            // Check if the object creation expression is of type "new Identifier<T>()"
            if (node.Type is not GenericNameSyntax genericName || genericName.Identifier.Text != _original.Identifier ||
                genericName.TypeArgumentList.Arguments.Count != 1) return base.VisitObjectCreationExpression(node);
            // Create a new "Replacement<T>()" expression
            var substituteExpression = SyntaxFactory.ParseExpression($"{_replacement.Identifier}<{genericName.TypeArgumentList.Arguments[0]}>()");

            // Return the node with the new expression
            return substituteExpression;
        }

        public override SyntaxNode? VisitArgument(ArgumentSyntax node)
        {
            var originalCode = node.ToString();
            return originalCode.Contains(_original.Argument) ? SyntaxFactory.ParseExpression(originalCode.Replace(_original.Argument, _replacement.Argument)) : base.VisitArgument(node);
        }

        public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            var originalType = node.Type.ToString();
            if (_original.VariableType.IsRegex) return node.Update(SyntaxFactory.ParseTypeName($"{Regex.Replace(originalType, _original.VariableType.Text, _replacement.VariableType.Text)} "), node.Variables);

            return node.Update(SyntaxFactory.ParseTypeName(originalType.Replace(_original.VariableType.Text,
                    _replacement.VariableType.Text)), node.Variables);
        }
    }
}