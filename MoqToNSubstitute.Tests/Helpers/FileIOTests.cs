namespace MoqToNSubstitute.Tests.Helpers;

[TestClass]
public class FileIOTests
{
    [TestMethod]
    public void Test_ReadFileFromEmbeddedResources()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(n => n.EndsWith("TaxServiceTests.cs"));
        var fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
        Assert.IsFalse(string.IsNullOrEmpty(fileContents));
    }
}