using System;
using InfoShare.Deployment.Data.Actions.File;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Actions.SingleFile
{
    [TestClass]
    public class FileBackupRollbackAction : BaseUnitTest
    {
        [TestInitialize]
        public void TestInitializer()
        {
            ObjectFactory.SetInstance(FileManager);
			ObjectFactory.SetInstance(Substitute.For<IXmlConfigManager>());
		}

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Backup_File_Creted()
        {
			// Arrange
			var testFilePath = this.GetIshFilePath("Test.xml");
	        var backUpFilePath = String.Concat(testFilePath.AbsolutePath, ".back");

			FileManager.Exists(testFilePath.AbsolutePath).Returns(true);
			FileManager.Exists(testFilePath.VanillaPath).Returns(true);

			// Act
			new XmlSetAttributeValueAction(Logger, testFilePath, "", "", "");

			// Assert
			FileManager.Received(1).Copy(testFilePath.AbsolutePath, backUpFilePath);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}

		[TestMethod]
		[TestCategory("Actions")]
		public void Execute_Backup_File_Disposed()
		{
			// Arrange
			var testFilePath = this.GetIshFilePath("Test.xml");
			var backUpFilePath = String.Concat(testFilePath.AbsolutePath, ".back");

			FileManager.Exists(testFilePath.AbsolutePath).Returns(true);
			FileManager.Exists(backUpFilePath).Returns(true);
			FileManager.Exists(testFilePath.VanillaPath).Returns(true);

			// Act
			(new XmlSetAttributeValueAction(Logger, testFilePath, "", "", "")).Dispose();

			// Assert
			FileManager.Received(1).Delete(backUpFilePath);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}

		[TestMethod]
		[TestCategory("Actions")]
		public void Execute_Vanilla_Backup_Created()
		{
			// Arrange
			var testFilePath = this.GetIshFilePath("Test.xml");
			FileManager.Exists(testFilePath.AbsolutePath).Returns(true);

			// Act
			new XmlSetAttributeValueAction(Logger, testFilePath, "", "", "");

			// Assert
			FileManager.Received(1).Copy(testFilePath.AbsolutePath, testFilePath.VanillaPath);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}

		#region Undo actions

		[TestMethod]
		[TestCategory("Actions")]
		public void Execute_Copy_Files()
		{
			// Arrange
			var souceFolder = "Source";
			var destinationFolder = "Target";

			// Act
			(new FileCopyDirectoryAction(Logger, souceFolder, destinationFolder)).Execute();

			// Assert
			FileManager.Received(1).CopyDirectoryContent(souceFolder, destinationFolder);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}

		[TestMethod]
		[TestCategory("Actions")]
		public void Execute_Clean_Directory()
		{
			// Arrange
			var souceFolder = "Source";

			// Act
			(new FileCleanDirectoryAction(Logger, souceFolder)).Execute();

			// Assert
			FileManager.Received(1).CleanFolder(souceFolder);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}

		[TestMethod]
		[TestCategory("Actions")]
		public void Execute_Remove_Directory()
		{
			// Arrange
			var souceFolder = "Source";

			// Act
			(new RemoveDirectoryAction(Logger, souceFolder)).Execute();

			// Assert
			FileManager.Received(1).DeleteFolder(souceFolder);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}

		#endregion
	}
}
