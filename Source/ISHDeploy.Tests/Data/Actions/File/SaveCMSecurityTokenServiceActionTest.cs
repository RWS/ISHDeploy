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
            _templateManager.GetCMSecurityTokenServiceDoc(hostname, cmWebAppName, wsWebAppName, Path.GetFileName(certificatePath), certificateContent).Returns(documentContent);
            var action = new SaveCMSecurityTokenServiceAction(Logger, outputFilePath, certificatePath, hostname, cmWebAppName, wsWebAppName);
            
            // Act
            action.Execute();

            // Assert
            FileManager.Received().Write(outputFilePath, documentContent);
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Create_folder_if_it_does_not_exist()
        {
            // Arrange
            var outputFilePath = "C:\\Packages\\cert.cer";
            var outputFolder = Path.GetDirectoryName(outputFilePath);
            var action = new SaveCMSecurityTokenServiceAction(Logger, outputFilePath, "c:\\Packages\\cert.cer", "host", "ISHCMSQL2012", "ISHWSSQL2012");

            FileManager.FolderExists(outputFolder).Returns(false);

            // Act
            action.Execute();

            // Assert
            FileManager.Received().CreateDirectory(outputFolder);
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Do_not_create_folder_if_it_exists()
        {
            // Arrange
            var action = new SaveCMSecurityTokenServiceAction(Logger, "C:\\Packages\\file.zip", "c:\\Packages\\cert.cer", "host", "ISHCMSQL2012", "ISHWSSQL2012");
            FileManager.FolderExists(Arg.Any<string>()).Returns(true);

            // Act
            action.Execute();

            // Assert
            FileManager.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }
    }
}
