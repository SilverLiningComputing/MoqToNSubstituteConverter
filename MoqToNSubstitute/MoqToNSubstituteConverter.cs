using System.Runtime.CompilerServices;
using MoqToNSubstitute.Utilities;

[assembly: InternalsVisibleTo("MoqToNSubstitute.Tests")]

namespace MoqToNSubstitute;

internal static class MoqToNSubstituteConverter
{
    private static readonly IPackageManager PackageManager = new DotNetPackageManager();
    private static readonly ICodeTransformer CodeTransformer = new MoqToNSubstituteTransformer();

    public static void Convert(bool analysisOnly = true)
    {
        var solutionDir = Directory.GetCurrentDirectory();
        var csFiles = Directory.GetFiles(solutionDir, "*.cs", SearchOption.AllDirectories);
        var csprojFiles = Directory.GetFiles(solutionDir, "*.csproj", SearchOption.AllDirectories);

        Logger.Log("Starting process...");

        if (!analysisOnly)
        {
            Logger.Log("Processing project files...");

            foreach (var projectFile in csprojFiles)
            {
                Logger.Log($"Uninstalling Moq from {projectFile}");
                PackageManager.Uninstall(projectFile, "Moq");

                Logger.Log($"Installing NSubstitute to {projectFile}");
                PackageManager.Install(projectFile, "NSubstitute");
            }
        }

        Logger.Log("Processing source files...");
        foreach (var sourceFile in csFiles)
        {
            Logger.Log(analysisOnly ? $"Analyzing {sourceFile}" : $"Transforming {sourceFile}");
            if (analysisOnly) CodeTransformer.Analyze(sourceFile);
            else CodeTransformer.Transform(sourceFile);
        }
        Logger.Log(analysisOnly ? "Completed analysis" : "Completed transformations.");
    }
}