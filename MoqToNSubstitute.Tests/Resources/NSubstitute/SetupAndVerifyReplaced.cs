using System;

namespace MoqToNSubstitute.Tests.Resources.Moq
{
    internal class SetupAndVerify
    {
        [TestMethod]
        public void Test_setup()
        {
_classMock.Setup(Arg.Any<string>(), Arg.Any<int>()).Returns(0);
_classMock.Setup(Arg.Any<string>(), Arg.Any<int>()).Returns(0);
_classMock.Setup(Arg.Any<string>(), Arg.Any<int>()).Returns(0);
_classMock.Setup(Arg.Any<string>(), Arg.Any<int>()).Returns(0);
_classMock.Setup(Arg.Any<string>(), Arg.Any<int>()).Returns(0);
_classMock.Setup(Arg.Any<string>(), Arg.Any<int>()).Throws(new Exception());
_classMock.Setup(Arg.Any<string>(), Arg.Is<int>(p => p == 4)).Throws(new Exception());
_classMock.Setup(Arg.Any<string>(), Arg.Any<int>()).Throws(new Exception);
_classMock.Setup().Returns(0);
            _classMock.SetupGet(_ => _.Value).Returns(12);
_classMock.Setup(Arg.Any<string>(), Arg.Any<int>().Throws);
_classMock.Setup(Arg.Any<string>(), Arg.Any<int>()).Returns(0);
        }

        [TestMethod]
        public void Test_verify()
        {
_classMock.Received(1).Setup(Arg.Any<string>(), Arg.Any<int>());;
_classMock.Received(2).Setup(Arg.Any<string>(), Arg.Any<int>());;
_classMock.Received(1).Setup(Arg.Any<string>(), Arg.Any<int>());;
_classMock.DidNotReceive().Setup(Arg.Any<string>(), Arg.Any<int>());;
_classMock.Received(1).Setup(Arg.Any<string>(),Arg.Any<int>());;
_classMock.Received(3).Setup(Arg.Any<string>(), Arg.Any<int>());;
_classMock.Received(4).Setup(Arg.Any<string>(), Arg.Any<int>());;
_classMock.Received(1).Setup();;
_classMock.DidNotReceive().Setup(Arg.Any<string>(), Arg.Any<int>());;
_classMock.Received(1).Setup(Arg.Any<string>(), Arg.Any<int>());;
        }
    }
}