using MoqToNSubstitute.Conversion;
using NSubstitute;

namespace MoqToNSubstitute.Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        [DataRow("true", "c:\\folder\\code\\", "c:\\folder\\code\\", true)]
        [DataRow("false", "c:\\folder\\code\\", "c:\\folder\\code\\", false)]
        [DataRow("c:\\folder\\code\\", "true", "c:\\folder\\code\\", true)]
        [DataRow("c:\\folder\\code\\", "false", "c:\\folder\\code\\", false)]
        [DataRow("false", "false", null, false)]
        [DataRow("true", "false", null, true)]
        [DataRow("false", "true", null, false)]
        [DataRow("true", "", null, false)]
        [DataRow("", "true", null, true)]
        [DataRow("", "", null, null)]
        [DataRow("c:\\folder\\code\\", "c:\\folder\\code\\", "c:\\folder\\code\\", null)]
        public void Main(string arg1, string arg2, string? param1, bool? param2)
        {
            var args = new[] { arg1, arg2 };
            var entryPoint = typeof(Program).Assembly.EntryPoint!;
            Program.MoqToNSubstituteConverter = Substitute.For<MoqToNSubstituteConverter>();
            //Program.MoqToNSubstituteConverter.Convert(Arg.Any<string>(), Arg.Any<bool>());
            entryPoint.Invoke(null, new object[] { args });
             
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
}
