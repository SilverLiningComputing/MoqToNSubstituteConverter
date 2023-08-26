using Microsoft.CodeAnalysis.CSharp;

namespace MoqToNSubstitute.Tests.Conversion;

[TestClass]
public class MoqToNSubstituteTransformerTests
{
    private string? _fileContents;
    private Assembly? _assembly;
    private CodeSyntax? _substitutions;

    [TestInitialize]
    public void TestInitialize()
    {
        _assembly = Assembly.GetExecutingAssembly();
        _substitutions = ReplacementTemplate.ReturnReplacementSyntax();

        var resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("VariableSample.cs"));
        Assert.IsFalse(string.IsNullOrEmpty(resourceName));
        _fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
    }

    [TestMethod]
    public void Test_GetNodes()
    {
        Assert.IsNotNull(_fileContents);
        MoqToNSubstituteTransformer.GetNodeTypesFromString(_fileContents);
    }

    [TestMethod]
    public void Test_ReplaceArgumentNodes()
    {
        Assert.IsNotNull(_assembly);
        Assert.IsNotNull(_substitutions);
        var resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("ArgumentSample.cs"));
        var fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
        var tree = CSharpSyntaxTree.ParseText(fileContents);
        var root = tree.GetRoot();
        var testNode = root.ReplaceArgumentNodes(_substitutions, ".Object", "It.IsAny", "It.Is");
        Assert.IsNotNull(testNode);
        resourceName = _assembly.GetManifestResourceNames().Single(n => n.EndsWith("ArgumentSampleReplaced.cs"));
        fileContents = FileIO.ReadFileFromEmbeddedResources(resourceName);
        Assert.IsNotNull(fileContents);
        Assert.AreEqual(fileContents, testNode.ToString());
    }
}