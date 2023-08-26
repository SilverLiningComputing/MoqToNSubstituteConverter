namespace MoqToNSubstitute.Utilities;
internal interface IPackageManager
{
    /// <summary>
    /// Uninstall a package from csproj
    /// </summary>
    /// <param name="projectPath">The project path</param>
    /// <param name="packageName">The package name</param>

    void Uninstall(string projectPath, string packageName);

    /// <summary>
    /// Install a package into csproj
    /// </summary>
    /// <param name="projectPath">The project path</param>
    /// <param name="packageName">The package name</param>
    void Install(string projectPath, string packageName);
}