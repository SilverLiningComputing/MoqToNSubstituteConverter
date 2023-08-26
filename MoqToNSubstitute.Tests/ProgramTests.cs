namespace MoqToNSubstitute.Tests;

[TestClass]
public class ProgramTests
{
    private MethodInfo? _entryPoint;

    [TestInitialize]
    public void TestInitialize()
    {
        Program.MoqToNSubstituteConverter = Substitute.For<ICodeConverter>();
        Program.MoqToNSubstituteConverter.Convert(Arg.Any<string>(), Arg.Any<bool>());
        _entryPoint = typeof(Program).Assembly.EntryPoint!;
    }

    [TestMethod]
    public void Test_Main_no_arguments()
    {
        Assert.IsNotNull(_entryPoint);
        _entryPoint.Invoke(null, new object[] { Array.Empty<string>() });
        Program.MoqToNSubstituteConverter.Received(1).Convert();
    }

    [TestMethod]
    [DataRow("false")]
    [DataRow("C:\\folder\\")]
    public void Test_Main_one_argument(string arg)
    {
        Assert.IsNotNull(_entryPoint);
        _entryPoint.Invoke(null, new object[] { new[] { arg } });
        if (bool.TryParse(arg, out var transform))
        {
            Program.MoqToNSubstituteConverter.Received(1).Convert(Arg.Is<string>(x => x == ""), Arg.Is<bool>(x => x == transform));
        }
        else
        {
            Program.MoqToNSubstituteConverter.Received(1).Convert(Arg.Is<string>(x => x == arg));
        }
    }

    [TestMethod]
    [DataRow("true", "c:\\folder\\code\\", "c:\\folder\\code\\", true)]
    [DataRow("false", "c:\\folder\\code\\", "c:\\folder\\code\\", false)]
    [DataRow("c:\\folder\\code\\", "true", "c:\\folder\\code\\", true)]
    [DataRow("c:\\folder\\code\\", "false", "c:\\folder\\code\\", false)]
    [DataRow("false", "false", null, false)]
    [DataRow("true", "false", null, true)]
    [DataRow("false", "true", null, false)]
    [DataRow("true", "", null, true)]
    [DataRow("", "true", null, true)]
    [DataRow("", "", null, null)]
    [DataRow("c:\\folder\\code\\", "c:\\folder\\", "c:\\folder\\code\\", null)]
    public void Text_Main_with_two_arguments(string arg1, string arg2, string? param1, bool? param2)
    {
        Assert.IsNotNull(_entryPoint);
        var args = new[] { arg1, arg2 };
        _entryPoint.Invoke(null, new object[] { args });

        if (param1 != null && param2 != null)
        {
            Program.MoqToNSubstituteConverter.Received(1).Convert(Arg.Is<string>(x => x == param1), Arg.Is<bool>(x => x == param2));
        }

        if (param1 == null && param2 != null)
        {
            Program.MoqToNSubstituteConverter.Received(1).Convert(Arg.Is<string>(x => x == ""), Arg.Is<bool>(x => x == param2));
        }

        if (param1 == null && param2 == null)
        {
            Program.MoqToNSubstituteConverter.Received(1).Convert();
        }

        if (param1 != null && param2 == null)
        {
            Program.MoqToNSubstituteConverter.Received(1).Convert(Arg.Is<string>(x => x == param1));
        }
    }
}