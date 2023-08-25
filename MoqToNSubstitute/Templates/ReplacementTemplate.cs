using MoqToNSubstitute.Models;

namespace MoqToNSubstitute.Templates
{
    internal static class ReplacementTemplate
    {
        internal static CodeSyntax ReturnReplacementSyntax()
        {
            return new CodeSyntax
            {
                Identifier = new List<Expression>
                {
                    new("Mock", "Substitute.For", false),
                },
                Argument = new List<Expression>
                {
                    new(".Object", "", false),
                    new("It.IsAny", "Arg.Any", false),
                    new("It.Is", "Arg.Is", false)
                },
                VariableType = new List<Expression>
                {
                    new("Mock\\<(?<start>.+)\\>", "${start}", true)
                },
                AssignmentExpression = new List<Expression>
                {
                    new("new Mock(?<start>.+).*\\{.*CallBase = true.*\\}", "Substitute.ForPartsOf${start}", true),
                    new("new Mock", "Substitute.For", false),
                },
                ExpressionStatement = new List<Expression>
                {
                    new("\r\n *", "", true),
                    new("It.IsAny", "Arg.Any", false),
                    new("It.Is", "Arg.Is", false),
                    new(".Verifiable()", "", false),
                    new(".Result", "", false),
                    new("(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.ReturnsAsync(?<end>.+);", "${start}.${middle}.Returns${end}", true),
                    new("(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.Returns(?<end>.+);", "${start}.${middle}.Returns${end}", true),
                    new("(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.ThrowsAsync(?<end>.+);", "${start}.${middle}.Throws${end}", true),
                    new("(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)\\.Throws(?<end>.+);", "${start}.${middle}.Throws${end}", true),
                    new("(?<start>.+)\\.Setup\\(.+ => [^.]+\\.(?<middle>.+)\\)(?<end>.+);", "${start}.${middle}.Throws${end}", true),
                    new("(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Once\\);", "${start}.Received(1).${middle}", true),
                    new("(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Never\\);", "${start}.DidNotReceive().${middle}", true),
                    new("(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+)\\, Times.Exactly(?<times>.+)\\);", "${start}.Received${times}.${middle}", true),
                    new("(?<start>.+)\\.Verify\\(.+ => [^.]+\\.(?<middle>.+);", "${start}.Received().${middle}", true),
                }
            };
        }
    }
}
