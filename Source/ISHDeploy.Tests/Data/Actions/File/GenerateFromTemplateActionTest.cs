using System.Collections.Generic;
using ISHDeploy.Data.Actions.Template;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions.File
{
    [TestClass]
    public class GenerateFromTemplateActionTest : BaseUnitTest
    {
        private ITemplateManager _templateManager;

        [TestInitialize]
        public void TestInitialize()
        {
            _templateManager = Substitute.For<ITemplateManager>();
            ObjectFactory.SetInstance(_templateManager);
        }
        
        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Document_with_correct_parameteres_is_created()
        {
            // Arrange
            var templateFilePath = "input.template";
            var outputFilePath = "out.txt";
            var contentValue = "Generated Document Content";
            var inputParam = new Dictionary<string, string>() {};

            _templateManager.GenerateDocument(templateFilePath, inputParam).Returns(contentValue);

            var action = new GenerateFromTemplateAction(Logger, templateFilePath, outputFilePath, inputParam);

            // Act
            action.Execute();

            // Assert
            FileManager.Received().Write(outputFilePath, contentValue);
        }
    }
}
