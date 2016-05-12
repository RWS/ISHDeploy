using ISHDeploy.Data.Actions.Directory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions.File
{
	[TestClass]
	public class DirectoryRemoveActionTest : BaseUnitTest
	{
		[TestMethod]
		[TestCategory("Actions")]
		public void Execute_Remove_directory()
		{
			// Arrange
			var souceFolder = "Source";

			// Act
			(new DirectoryRemoveAction(Logger, souceFolder)).Execute();

			// Assert
			FileManager.Received(1).DeleteFolder(souceFolder);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}

	}
}
