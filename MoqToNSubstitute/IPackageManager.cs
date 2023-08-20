namespace MoqToNSubstitute;
internal interface IPackageManager
{
    void Uninstall(string projectPath, string packageName);
    void Install(string projectPath, string packageName);
}