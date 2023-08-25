namespace MoqToNSubstitute.Models
{
    public class Expression
    {
        public Expression(string original, string replacement, bool isRegex)
        {
            Original = original;
            Replacement = replacement;
            IsRegex = isRegex;
        }

        public string Original { get; set; }
        public string Replacement { get; set; }
        public bool IsRegex { get; set; }
    }
}
