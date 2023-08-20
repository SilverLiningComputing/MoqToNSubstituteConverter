using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MoqToNSubstitute.Tests
{
    [TestClass]
    public class NodeTests
    {
        [TestMethod]
        public void Test_GetNodes()
        {
            const string sourceText = "{\r\n    internal class SpectralControllerTests\r\n    {\r\n        private Mock<ISpectralService> _spectralServiceMock;\r\n\r\n        [OneTimeSetUp]\r\n        public void Setup()\r\n        {\r\n            // set up the mock service\r\n            _spectralServiceMock = new Mock<ISpectralService>();\r\n        }\r\n\r\n        [Test]\r\n        public void Test_controller_get()\r\n        {\r\n            var spectralController = new SpectralController(_spectralServiceMock.Object);\r\n            var response = spectralController.Get();\r\n            string[] array = { \"Controller\", \"Spectral\" };\r\n            Assert.AreEqual(array, response);\r\n        }\r\n\r\n        [Test]\r\n        public void Test_controller_post()\r\n        {\r\n            const string results = \"{\\\"id\\\":\\\"SpectralMusp\\\",\\\"plotList\\\":[{\\\"label\\\":\\\"Skin μs'\\\",\\\"data\\\":[[650,2.2123013258570863],[700,1.991324464076181],[750,1.8054865273011322],[800,1.6473787686682118],[850,1.5114938050444233],[900,1.3936601517386393],[950,1.290665574863872],[1000,1.2]]}]}\";\r\n            var expected = JsonConvert.DeserializeObject<Plots>(results);\r\n            _spectralServiceMock.Setup(x => x.GetPlotData(It.IsAny<SpectralPlotParameters>()))\r\n                .Returns(expected);\r\n            var spectralController = new SpectralController(_spectralServiceMock.Object);\r\n            var response = spectralController.Post(new SpectralPlotParameters());\r\n            Assert.IsInstanceOf<Plots>(response);\r\n        }\r\n    }\r\n}";
            var tree = CSharpSyntaxTree.ParseText(sourceText);
            var root = tree.GetRoot();

            var assignmentExpressionSyntaxCollection = root.GetNodes<AssignmentExpressionSyntax>("new Mock");
            Assert.IsNotNull(assignmentExpressionSyntaxCollection);
            var assignmentExpressionSyntaxArray = assignmentExpressionSyntaxCollection.ToArray();
            Assert.IsNotNull(assignmentExpressionSyntaxArray);
            Assert.AreEqual(1, assignmentExpressionSyntaxArray.Length);
        }
    }
}