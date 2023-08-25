namespace MoqToNSubstitute.Models
{
    public class CodeSyntax
    {
        public Expression Identifier { get; set; }
        public IEnumerable<Expression> Argument { get; set; }
        public IEnumerable<Expression> VariableType { get; set; }
        public Expression AssignmentExpression { get; set; }
        public IEnumerable<Expression> ExpressionStatement { get; set; }
    }
}
