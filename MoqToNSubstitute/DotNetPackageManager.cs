using System.Diagnostics;
using MoqToNSubstitute.Utilities;

namespace MoqToNSubstitute;

internal class DotNetPackageManager : IPackageManager
{ 
    public void Uninstall(string projectPath, string packageName)
    {
        RunCommand($"dotnet remove {projectPath} package {packageName}");
    }

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
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            }
        };

        process.Start();
        process.StandardInput.WriteLine(command);
        process.StandardInput.Close();
        Logger.Log(process.StandardOutput.ReadToEnd());
        process.WaitForExit();
    }
}