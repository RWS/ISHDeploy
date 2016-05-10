using System.IO;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.Template;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions.File
{
    [TestClass]
    public class SaveCMSecurityTokenServiceActionTest : BaseUnitTest
    {
        private const string _filePath = "C:\\DummyFilePath.txt";
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
            var outputFilePath = "c:\\Packages\\output.zip";
            var certificatePath = "c:\\Packages\\cert.cer";
            var certificateContent = "certificateContent";
            var hostname = "host";
            var cmWebAppName = "ISHCMSQL2012";
            var wsWebAppName = "ISHWSSQL2012";
            var documentContent = "Generated Document Content";

            FileManager.ReadAllText(certificatePath).Returns(certificateContent);
            var action = new SaveCMSecurityTokenServiceAction(Logger, outputFilePath, hostname, cmWebAppName, wsWebAppName, "cert.cer", certificateContent);
            
            // Act
            action.Execute();

            // Assert
            FileManager.Received().Write(outputFilePath, "");
        }
    }
}
