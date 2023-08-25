using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoqToNSubstitute.Models;
using System.Text.RegularExpressions;

namespace MoqToNSubstitute
{
    public class CustomSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly CodeSyntax _substitutions;

        public CustomSyntaxRewriter(CodeSyntax substitutions)
        {
            _substitutions = substitutions;
        }

        public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
                // Check if the object creation expression is of type "new Identifier<T>()"
                if (node.Type is not GenericNameSyntax genericName || genericName.Identifier.Text != _substitutions.Identifier.Original ||
                    genericName.TypeArgumentList.Arguments.Count != 1) return base.VisitObjectCreationExpression(node);
                // Create a new "Replacement<T>(arguments)" expression
                var substituteExpression = SyntaxFactory.ParseExpression($"{_substitutions.Identifier.Replacement}<{genericName.TypeArgumentList.Arguments[0]}>{node.ArgumentList}");

            // Return the node with the new expression
            return substituteExpression;
        }

        public override SyntaxNode? VisitArgument(ArgumentSyntax node)
        {
            var originalCode = node.ToString();
            var replacementCode = _substitutions.Argument.Aggregate(originalCode, (current, argument) => current.Replace(argument.Original, argument.Replacement));
            return originalCode != replacementCode ? SyntaxFactory.ParseExpression(replacementCode) : base.VisitArgument(node);
        }

        public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            var originalType = node.Type.ToString();
            var replacementCode = originalType;
            foreach (var variableType in _substitutions.VariableType)
            {
                if (variableType.IsRegex)
                {
                    replacementCode = Regex.Replace(replacementCode, variableType.Original, variableType.Replacement);
                }
                else
                {
                    replacementCode = replacementCode.Replace(variableType.Original,
                        variableType.Replacement);
                }
            }
            return node.Update(SyntaxFactory.ParseTypeName($"{replacementCode} "), node.Variables);
        }

        public override SyntaxNode? VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            var originalAssignment = node.Right.ToString();
            return node.Update(node.Left, 
                node.OperatorToken, 
                _substitutions.AssignmentExpression.IsRegex ? 
                    SyntaxFactory.ParseTypeName($"{Regex.Replace(originalAssignment, _substitutions.AssignmentExpression.Original, _substitutions.AssignmentExpression.Replacement)} ") :
                    SyntaxFactory.ParseTypeName($"{originalAssignment.Replace(_substitutions.AssignmentExpression.Original, _substitutions.AssignmentExpression.Replacement)} "));
        }

        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            var originalCode = node.ToString();
            var replacementCode = originalCode;
            foreach (var statement in _substitutions.ExpressionStatement)
            {
                if (statement.IsRegex)
                {
                    replacementCode = Regex.Replace(replacementCode, statement.Original, statement.Replacement);
                }
                else
                {
                    replacementCode = replacementCode.Replace(statement.Original,
                        statement.Replacement);
                }
            }
            return SyntaxFactory.ParseExpression(replacementCode);
        }
    }
}