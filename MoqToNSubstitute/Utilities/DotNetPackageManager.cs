using System.Diagnostics;

namespace MoqToNSubstitute.Utilities;

internal class DotNetPackageManager : IPackageManager
{

    /// <summary>
    /// Uninstall a package from csproj
    /// </summary>
    /// <param name="projectPath">The project path</param>
    /// <param name="packageName">The package name</param>
    public void Uninstall(string projectPath, string packageName)
    {
        RunCommand($"dotnet remove {projectPath} package {packageName}");
    }

    /// <summary>
    /// Install a package into csproj
    /// </summary>
    /// <param name="projectPath">The project path</param>
    /// <param name="packageName">The package name</param>
    public void Install(string projectPath, string packageName)
    {
        RunCommand($"dotnet add {projectPath} package {packageName}");
    }

    private static void RunCommand(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = true
            }
        };

        process.Start();
        process.StandardInput.WriteLine(command);
        process.StandardInput.Close();
        Logger.Log(process.StandardOutput.ReadToEnd());
        process.WaitForExit();
    }
}