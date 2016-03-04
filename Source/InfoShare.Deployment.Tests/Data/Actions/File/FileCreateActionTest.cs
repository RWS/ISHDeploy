using InfoShare.Deployment.Data.Actions.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Actions.File
{
	[TestClass]
	public class FileCreateActionTest : BaseUnitTest
	{
		[TestMethod]
		[TestCategory("Actions")]
		public void Execute_Create_file()
		{
			// Arrange
			var testFilePath = GetIshFilePath("SourceFolder");
			string fileName = "test.txt";
			string fileContent = "this is content";

			// Act
			(new FileCreateAction(Logger, testFilePath, fileName, fileContent)).Execute();

			// Assert
			FileManager.Received(1).Write(System.IO.Path.Combine(testFilePath.AbsolutePath, fileName), fileContent);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}

	}
}