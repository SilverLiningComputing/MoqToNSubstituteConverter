using System;

namespace MoqToNSubstitute.Tests.Resources.Moq
{
    internal class SetupAndVerify
    {
        [TestMethod]
        public void Test_setup()
        {
            _classMock.Setup(x => x.Setup(It.IsAny<string>(), It.IsAny<int>())).Returns(0);
            _classMock.Setup(mock => mock.Setup(It.IsAny<string>(), It.IsAny<int>())).Returns(0);
            _classMock.Setup(x => x.Setup(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(0);
            _classMock.Setup(x => x.Setup(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(0);
            _classMock.Setup(x => x.Setup(
                It.IsAny<string>(), 
                It.IsAny<int>()))
                .ReturnsAsync(0);
            _classMock.Setup(x => x.Setup(It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
            _classMock.Setup(x => x.Setup(It.IsAny<string>(), It.Is<int>(p => p == 4))).ThrowsAsync(new Exception());
            _classMock.Setup(x => x.Setup(It.IsAny<string>(), It.IsAny<int>())).Throws(new Exception);
            _classMock.Setup(x => x.Setup()).Returns(0).Verifiable();
            _classMock.SetupGet(_ => _.Value).Returns(12);
            _classMock.Setup(x => x.Setup(It.IsAny<string>(), It.IsAny<int>()));
            _classMock.Setup(x => x.Setup(It.IsAny<string>(), It.IsAny<int>())).Result.Returns(0);
        }

        [TestMethod]
        public void Test_verify()
        {
            _classMock.Verify(x => x.Setup(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            _classMock.Verify(mock => mock.Setup(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(2));
            _classMock
                .Verify(x => x.Setup(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            _classMock.Verify(x => x.Setup(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _classMock.Verify(x => x.Setup(
                It.IsAny<string>(),
                It.IsAny<int>()), Times.Once);
            _classMock.Verify(x => x.Setup(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(3));
            _classMock.Verify(x => x.Setup(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(4));
            _classMock.Verify(x => x.Setup(), Times.Once);
            _classMock.Verify(x => x.Setup(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _classMock.Verify(x => x.Setup(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }
    }
}