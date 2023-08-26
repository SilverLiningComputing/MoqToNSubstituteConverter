namespace MoqToNSubstitute.Tests.Resources.Moq
{
    [TestClass]
    public class ArgumentSample
    {
        private SpectralService _plotFactoryMock;

        [TestMethod]
        public void Test_argument_replacement()
        {
            _spectralService = new SpectralService(_logger, _plotFactoryMock.Object);
        }
    }
}