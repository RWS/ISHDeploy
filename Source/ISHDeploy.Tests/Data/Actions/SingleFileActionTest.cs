using System;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions
{
    [TestClass]
    public class SingleFileActionTest : BaseUnitTest
    {
		[TestInitialize]
		public void TestInitializer()
		{
			ObjectFactory.SetInstance(Substitute.For<IXmlConfigManager>());
		}

		[TestMethod]
        [TestCategory("Actions")]
        public void Create_Backup_file_created()
        {
			// Arrange
			var testFilePath = this.GetIshFilePath("Test.xml");
	        var backUpFilePath = String.Concat(testFilePath.AbsolutePath, ".bak");

			FileManager.FileExists(testFilePath.AbsolutePath).Returns(true);
			FileManager.FileExists(testFilePath.VanillaPath).Returns(true);

			// Act
			new SetAttributeValueAction(Logger, testFilePath, "", "", "");

			// Assert
			FileManager.Received(1).Copy(testFilePath.AbsolutePath, backUpFilePath);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}

		[TestMethod]
		[TestCategory("Actions")]
		public void Dispose_Backup_file_disposed()
		{
			// Arrange
			var testFilePath = this.GetIshFilePath("Test.xml");
			var backUpFilePath = String.Concat(testFilePath.AbsolutePath, ".bak");

			FileManager.FileExists(testFilePath.AbsolutePath).Returns(true);
			FileManager.FileExists(backUpFilePath).Returns(true);
			FileManager.FileExists(testFilePath.VanillaPath).Returns(true);

			// Act
			(new SetAttributeValueAction(Logger, testFilePath, "", "", "")).Dispose();

			// Assert
			FileManager.Received(1).Delete(backUpFilePath);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}

		[TestMethod]
		[TestCategory("Actions")]
		public void Create_Vanilla_backup_created()
		{
			// Arrange
			var testFilePath = this.GetIshFilePath("Test.xml");
			FileManager.FileExists(testFilePath.AbsolutePath).Returns(true);

			// Act
			new SetAttributeValueAction(Logger, testFilePath, "", "", "");

			// Assert
			FileManager.Received(1).Copy(testFilePath.AbsolutePath, testFilePath.VanillaPath);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}
	}
}
