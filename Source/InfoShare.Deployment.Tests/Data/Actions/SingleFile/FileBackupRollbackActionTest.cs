using System;
using InfoShare.Deployment.Data.Actions.File;
using InfoShare.Deployment.Data.Actions.XmlFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Actions.SingleFile
{
    [TestClass]
    public class FileBackupRollbackActionTest : BaseUnitTest
    {
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

		#region Undo actions

		[TestMethod]
		[TestCategory("Actions")]
		public void Execute_Copy_files()
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
		public void Execute_Clean_directory()
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
		public void Execute_Remove_directory()
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
