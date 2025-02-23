﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoqToNSubstitute.Models;
using MoqToNSubstitute.Syntax;
using MoqToNSubstitute.Utilities;

namespace MoqToNSubstitute.Extensions;

internal static class SyntaxNodeExtensions
{
    internal static IEnumerable<T> GetNodes<T>(this SyntaxNode root, params string[] matchStrings)
    {
        return root.DescendantNodes().OfType<T>().Where(node =>
        {
            return (Array.Exists(matchStrings, match => node is not null && node.ToString()!.Contains(match)));
        });
    }

    internal static SyntaxNode ReplaceAssignmentNodes(this SyntaxNode root, CodeSyntax codeSyntax, params string[] matchText)
    {
        var customSyntaxRewriter = new CustomSyntaxRewriter(codeSyntax);
        return root.ReplaceNodes(root.GetNodes<AssignmentExpressionSyntax>(matchText),
            (node, _) =>
            {
                var newNode = customSyntaxRewriter.VisitAssignmentExpression(node);
                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {node}, Transformed: {newNode}");
                return newNode ?? node;
            }
        );
    }

    internal static SyntaxNode ReplaceObjectCreationNodes(this SyntaxNode root, CodeSyntax codeSyntax, params string[] matchText)
    {
        var customSyntaxRewriter = new CustomSyntaxRewriter(codeSyntax);
        return root.ReplaceNodes(root.GetNodes<ObjectCreationExpressionSyntax>(matchText),
            (node, _) =>
            {
                var newNode = customSyntaxRewriter.VisitObjectCreationExpression(node);
                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {node}, Transformed: {newNode}");
                return newNode ?? node;
            }
        );
    }

    internal static SyntaxNode ReplaceExpressionNodes(this SyntaxNode root, CodeSyntax codeSyntax, params string[] matchText)
    {
        var customSyntaxRewriter = new CustomSyntaxRewriter(codeSyntax);
        return root.ReplaceNodes(root.GetNodes<ExpressionStatementSyntax>(matchText),
            (node, _) =>
            {
                var newNode = customSyntaxRewriter.VisitExpressionStatement(node);
                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {node}, Transformed: {newNode}");
                return newNode ?? node;
            }
        );
    }

    internal static SyntaxNode ReplaceVariableNodes(this SyntaxNode root, CodeSyntax codeSyntax, params string[] matchText)
    {
        var customSyntaxRewriter = new CustomSyntaxRewriter(codeSyntax);
        return root.ReplaceNodes(root.GetNodes<VariableDeclarationSyntax>(matchText),
            (node, _) =>
            {
                var newNode = customSyntaxRewriter.VisitVariableDeclaration(node);
                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {node}, Transformed: {newNode}");
                return newNode ?? node;
            }
        );
    }

    internal static SyntaxNode ReplaceArgumentNodes(this SyntaxNode root, CodeSyntax codeSyntax, params string[] matchText)
    {
        var customSyntaxRewriter = new CustomSyntaxRewriter(codeSyntax);
        return root.ReplaceNodes(root.GetNodes<ArgumentSyntax>(matchText),
            (node, _) =>
            {
                var newNode = customSyntaxRewriter.VisitArgument(node);
                Logger.Log($"Line: {node.GetLocation().GetLineSpan().StartLinePosition.Line}, Original: {node}, Transformed: {newNode}");
                return newNode ?? node;
            }
        );
    }
}