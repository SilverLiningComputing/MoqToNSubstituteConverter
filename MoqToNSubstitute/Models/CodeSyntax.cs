namespace MoqToNSubstitute.Models
{
    public class CodeSyntax
    {
        public string Identifier { get; set; } = "";
        public string Argument { get; set; } = "";
        public Expression VariableType { get; set; } = new();
        public Expression AssignmentExpression { get; set; } = new();
        public IEnumerable<Expression> ExpressionStatement { get; set; }
    }
}
