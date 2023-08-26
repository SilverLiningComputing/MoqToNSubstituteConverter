namespace MoqToNSubstitute.Tests.Conversion;

[TestClass]
public class MoqToNSubstituteConverterTests
{
    [TestMethod]
    [DataRow("", false)]
    [DataRow("", true)]
    public void Test_Convert(string path, bool transform)
    {
        var packageManager = Substitute.For<IPackageManager>();
        var codeTransformer = Substitute.For<ICodeTransformer>();
        var moqToNSubstituteConverter = new MoqToNSubstituteConverter
        {
            PackageManager = packageManager,
            CodeTransformer = codeTransformer
        };
        moqToNSubstituteConverter.Convert(path, transform);
        if (transform)
        {
            packageManager.DidNotReceive().Install(Arg.Any<string>(), Arg.Is<string>(x => x == "NSubstitute"));
            packageManager.DidNotReceive().Uninstall(Arg.Any<string>(), Arg.Any<string>());
        }
        else
        {
            packageManager.DidNotReceive().Install(Arg.Any<string>(), Arg.Any<string>());
            packageManager.DidNotReceive().Uninstall(Arg.Any<string>(), Arg.Any<string>());
        }
        codeTransformer.DidNotReceive().Transform(Arg.Any<string>(), transform);

    }
}