using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoqToNSubstitute.Models;
using System.Text.RegularExpressions;

namespace MoqToNSubstitute.Syntax;

public class CustomSyntaxRewriter : CSharpSyntaxRewriter
{
    private readonly CodeSyntax _substitutions;

    public CustomSyntaxRewriter(CodeSyntax substitutions)
    {
        _substitutions = substitutions;
    }

    public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        var originalType = node.Type.ToString();
        var initializer = node.Initializer?.ToString();
        var replacementCode = originalType;
        foreach (var identifier in _substitutions.Identifier)
        {
            // RegEx is not feasible for this syntax so use IsRegEx here to check the initializer
            if (identifier.IsRegex)
            {
                // if the initializer has a value then do a replace
                if (!string.IsNullOrEmpty(initializer))
                {
                    replacementCode = replacementCode.Replace(identifier.Original,
                        identifier.Replacement);
                }
            }
            else
            {
                replacementCode = replacementCode.Replace(identifier.Original,
                    identifier.Replacement);
            }
        }

        // Create a new "Replacement<T>(arguments)" expression
        var substituteExpression = SyntaxFactory.ParseExpression($"{replacementCode}{node.ArgumentList}");

        // Return the node with the new expression
        return substituteExpression;
    }

    public override SyntaxNode? VisitArgument(ArgumentSyntax node)
    {
        var originalCode = node.ToString();
        var replacementCode = _substitutions.Argument.Aggregate(originalCode, (current, argument) => current.Replace(argument.Original, argument.Replacement));
        return originalCode != replacementCode ? node.WithExpression(SyntaxFactory.ParseExpression(replacementCode)) : base.VisitArgument(node);
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
        var replacementCode = originalAssignment;
        foreach (var assignment in _substitutions.AssignmentExpression)
        {
            if (assignment.IsRegex)
            {
                replacementCode = Regex.Replace(replacementCode, assignment.Original, assignment.Replacement);
            }
            else
            {
                replacementCode = replacementCode.Replace(assignment.Original,
                    assignment.Replacement);
            }
        }
        return node.Update(node.Left, node.OperatorToken, SyntaxFactory.ParseTypeName(replacementCode));
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
        return node.WithExpression(SyntaxFactory.ParseExpression(replacementCode));
    }
}