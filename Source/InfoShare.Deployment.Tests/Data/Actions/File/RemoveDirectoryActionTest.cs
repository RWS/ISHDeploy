using InfoShare.Deployment.Data.Actions.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Actions.File
{
	[TestClass]
	public class RemoveDirectoryActionTest : BaseUnitTest
	{
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

	}
}
