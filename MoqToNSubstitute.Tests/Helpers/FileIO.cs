namespace MoqToNSubstitute.Tests.Helpers;

internal static class FileIO
{
    public static string ReadFileFromEmbeddedResources(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null) return "";
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}