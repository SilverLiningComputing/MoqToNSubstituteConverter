using Calculator.Services;

namespace Calculator.Tests.Services
{
    [TestClass]
    public class TaxServiceTests
    {
        private Mock<ICalculatorService>? _calculatorServiceMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _calculatorServiceMock = new Mock<ICalculatorService>();
            _calculatorServiceMock.Setup(x => x.Divide(It.IsAny<double>(), It.IsAny<double>())).Returns(0.5);
        }

        [TestMethod]
        public void Test_calculate_taxes()
        {
            var taxService = new TaxService(new CalculatorService());
            var taxPayment = taxService.CalculateTaxes(32, 2500);
            Assert.AreEqual(800, taxPayment);
        }

        [TestMethod]
        public void Test_calculate_taxes_unit_test()
        {
            Assert.IsNotNull(_calculatorServiceMock);
            var taxService = new TaxService(_calculatorServiceMock.Object);
            var taxPayment = taxService.CalculateTaxes(50, 2500);
            Assert.AreEqual(1250, taxPayment);
            _calculatorServiceMock.Verify(x => x.Divide(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
        }
    }
}