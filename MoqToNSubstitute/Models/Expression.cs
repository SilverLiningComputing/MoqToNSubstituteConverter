namespace MoqToNSubstitute.Models;

/// <summary>
/// The replacement expression
/// </summary>
public class Expression
{
    /// <summary>
    /// The constructor that passes in the replacement values
    /// </summary>
    /// <param name="original">The original text to replace</param>
    /// <param name="replacement">The replacement text</param>
    /// <param name="isRegex">If this is true, it will use RegEx for the
    /// replacement, otherwise it will use a string replace</param>
    public Expression(string original, string replacement, bool isRegex)
    {
        Original = original;
        Replacement = replacement;
        IsRegex = isRegex;
    }

    /// <summary>
    /// The original text to replace
    /// </summary>
    public string Original { get; set; }

    /// <summary>
    /// The replacement text
    /// </summary>
    public string Replacement { get; set; }

    /// <summary>
    /// If this is true, it will use RegEx for the
    /// replacement, otherwise it will use a string replace
    /// </summary>
    public bool IsRegex { get; set; }
}