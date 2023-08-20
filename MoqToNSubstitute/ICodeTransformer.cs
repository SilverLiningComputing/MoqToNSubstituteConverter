namespace MoqToNSubstitute;
internal interface ICodeTransformer
{
    void Transform(string sourceFilePath);
    void Analyze(string sourceFilePath);
    void ShowNodes(string sourceFilePath);
}