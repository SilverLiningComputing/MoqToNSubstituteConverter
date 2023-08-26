using MoqToNSubstitute.Utilities;

namespace MoqToNSubstitute.Conversion;

internal class MoqToNSubstituteConverter : ICodeConverter
{
    private readonly IPackageManager _packageManager = new DotNetPackageManager();
    private readonly ICodeTransformer _codeTransformer = new MoqToNSubstituteTransformer();

    public void Convert(string path = "", bool transform = false)
    {
        var solutionDir = string.IsNullOrEmpty(path)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(path);
        if (!Directory.Exists(solutionDir))
        {
            Logger.Log("The directory does not exist");
            return;
        }
        var csFiles = Directory.GetFiles(solutionDir, "*.cs", SearchOption.AllDirectories);
        var csprojFiles = Directory.GetFiles(solutionDir, "*.csproj", SearchOption.AllDirectories);

        if (transform)
        {
            Logger.Log("Processing project files...");

            foreach (var projectFile in csprojFiles)
            {
                Logger.Log($"Installing NSubstitute to {projectFile}");
                _packageManager.Install(projectFile, "NSubstitute");
            }
        }

        Logger.Log("Processing source files...");
        foreach (var sourceFile in csFiles)
        {
            Logger.Log(transform ? $"Transforming {sourceFile}" : $"Analyzing {sourceFile}");
            _codeTransformer.Transform(sourceFile, transform);
        }
        Logger.Log(transform ? "Completed transformations." : "Completed analysis");
    }
}