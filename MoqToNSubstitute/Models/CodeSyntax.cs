namespace MoqToNSubstitute.Models
{
    public class CodeSyntax
    {
        public string Identifier { get; set; } = "";
        public string Argument { get; set; } = "";
        public Expression VariableType { get; set; } = new Expression();
        public string AssignmentExpression { get; set; } = "";
        public string ExpressionStatement { get; set; } = "";
    }
}
