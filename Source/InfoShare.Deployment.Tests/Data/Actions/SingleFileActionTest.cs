using System;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Actions
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
	        var backUpFilePath = String.Concat(testFilePath.AbsolutePath, ".back");

			FileManager.Exists(testFilePath.AbsolutePath).Returns(true);
			FileManager.Exists(testFilePath.VanillaPath).Returns(true);

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
			var backUpFilePath = String.Concat(testFilePath.AbsolutePath, ".back");

			FileManager.Exists(testFilePath.AbsolutePath).Returns(true);
			FileManager.Exists(backUpFilePath).Returns(true);
			FileManager.Exists(testFilePath.VanillaPath).Returns(true);

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
			FileManager.Exists(testFilePath.AbsolutePath).Returns(true);

			// Act
			new SetAttributeValueAction(Logger, testFilePath, "", "", "");

			// Assert
			FileManager.Received(1).Copy(testFilePath.AbsolutePath, testFilePath.VanillaPath);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}
	}
}
