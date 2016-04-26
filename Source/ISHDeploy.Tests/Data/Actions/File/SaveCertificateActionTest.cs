using System;
using System.IO;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Managers;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions.File
{
    [TestClass]
    public class SaveCertificateActionTest : BaseUnitTest
    {
        private const string _filePath = "C:\\DummyFilePath.txt";
        private ICertificateManager _certificateManager;
        private IXmlConfigManager _xmlConfigManager;

        [TestInitialize]
        public void TestInitialize()
        {
            _certificateManager = Substitute.For<ICertificateManager>();
            _xmlConfigManager = Substitute.For<IXmlConfigManager>();

            ObjectFactory.SetInstance(_certificateManager);
            ObjectFactory.SetInstance(_xmlConfigManager);
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Certificate_information_retrieved_from_xml_file()
        {
            // Arrange
            var certFilePath = "C:\\Packages\\cert.cer";
            var certContent = "someDummyCertificateContent";
            var thumbprintFile = "C:\\Instance\\config.xml";
            var thumbprintXPath = "someXPath";
            var thumbprint = "someDummyThumbprint";

            var action = new SaveCertificateAction(Logger, certFilePath, thumbprintFile, thumbprintXPath);
            _xmlConfigManager.GetValue(thumbprintFile, thumbprintXPath).Returns(thumbprint);
            _certificateManager.GetCertificatePublicKey(thumbprint).Returns(certContent);

            // Act
            action.Execute();

            // Assert
            FileManager.Received().Write(certFilePath, certContent);
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Create_folder_if_it_does_not_exist()
        {
            // Arrange
            var certFilePath = "C:\\Packages\\cert.cer";
            var certFolder = Path.GetDirectoryName(certFilePath);
            var action = new SaveCertificateAction(Logger, certFilePath, "C:\\Instance\\config.xml", "someXPath");

            FileManager.FolderExists(certFilePath).Returns(false);

            // Act
            action.Execute();

            // Assert
            FileManager.Received().CreateDirectory(certFolder);
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Do_not_create_folder_if_it_exists()
        {
            // Arrange
            var action = new SaveCertificateAction(Logger, "C:\\Packages\\cert.cer", "C:\\Instance\\config.xml", "someXPath");
            FileManager.FolderExists(Arg.Any<string>()).Returns(true);

            // Act
            action.Execute();

            // Assert
            FileManager.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }
    }
}
