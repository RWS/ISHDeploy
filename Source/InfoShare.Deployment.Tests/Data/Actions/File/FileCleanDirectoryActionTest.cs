using InfoShare.Deployment.Data.Actions.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Actions.File
{
	[TestClass]
	public class FileCleanDirectoryActionTest : BaseUnitTest
	{
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

	}
}