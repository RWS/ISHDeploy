using System.Collections.Generic;
using System.IO;
using ISHDeploy.Business.Operations;
using ISHDeploy.Data.Actions.File;
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
            var documentContent = "Generated Document Content";

            var parameters = new Dictionary<string, string>
            {
                {"$ishhostname", "host"},
                {"$ishcmwebappname", "ISHCMSQL2012"},
                {"$ishwswebappname", "ISHWSSQL2012"},
                {"$ishwscertificate", OperationPaths.TemporarySTSConfigurationFileNames.ISHWSCertificateFileName},
                {"$ishwscontent", certificateContent}
            };

            FileManager.ReadAllText(certificatePath).Returns(certificateContent);
            var action = new FileTemplateFillInAndSaveAction(Logger, outputFilePath, OperationPaths.TemporarySTSConfigurationFileNames.CMSecurityTokenServiceTemplateFileName,  parameters);
            
            // Act
            action.Execute();

            // Assert
            FileManager.Received().Write(outputFilePath, "");
        }
    }
}
