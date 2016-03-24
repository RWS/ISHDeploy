using InfoShare.Deployment.Data.Actions.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Actions.File
{
    [TestClass]
    public class FileCopyDirectoryActionTest : BaseUnitTest
    {
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
	}
}
