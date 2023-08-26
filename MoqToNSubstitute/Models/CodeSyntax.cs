namespace MoqToNSubstitute.Models;

/// <summary>
/// The model that defines all the replacements as string replacements
/// or regular expression replacements
/// </summary>
public class CodeSyntax
{
    /// <summary>
    /// The list of identifier replacements in the type ObjectCreationExpressionSyntax
    /// </summary>
    public IEnumerable<Expression>? Identifier { get; set; }
    /// <summary>
    /// The list of argument replacements in the type ArgumentSyntax
    /// </summary>
    public IEnumerable<Expression>? Argument { get; set; }
    /// <summary>
    /// The list of variable type replacements in the type VariableDeclarationSyntax
    /// </summary>
    public IEnumerable<Expression>? VariableType { get; set; }
    /// <summary>
    /// The list of assignment replacements in the type AssignmentExpressionSyntax
    /// </summary>
    public IEnumerable<Expression>? AssignmentExpression { get; set; }
    /// <summary>
    /// The list of statement replacements in the type ExpressionStatementSyntax
    /// </summary>
    public IEnumerable<Expression>? ExpressionStatement { get; set; }
}