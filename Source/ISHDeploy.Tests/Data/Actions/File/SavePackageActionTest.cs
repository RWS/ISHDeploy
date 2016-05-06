using ISHDeploy.Data.Actions.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions.File
{
    [TestClass]
    public class SavePackageActionTest : BaseUnitTest
    {
        private const string _filePath = "C:\\DummyFilePath.txt";

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Package_correct_files_to_correct_package_name()
        {
            // Arrange
            var filesToPack = new [] {"file1.txt", "file2.txt"};
            var action = new SavePackageAction(Logger, _filePath, filesToPack, true);

            // Act
            action.Execute();

            // Assert
            FileManager.Received().PackageFiles(_filePath, filesToPack);
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Clear_artifacts_after_making_package()
        {
            // Arrange
            string file1 = "file1.txt";
            string file2 = "file2.txt";
            var action = new SavePackageAction(Logger, _filePath, new [] {file1, file2}, true);

            // Act
            action.Execute();

            // Assert
            FileManager.Received().Delete(file1);
            FileManager.Received().Delete(file2);
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Leave_artifacts_after_making_package()
        {
            // Arrange
            string file1 = "file1.txt";
            string file2 = "file2.txt";
            var action = new SavePackageAction(Logger, _filePath, new[] { file1, file2 }, false);

            // Act
            action.Execute();

            // Assert
            FileManager.DidNotReceive().Delete(file1);
            FileManager.DidNotReceive().Delete(file2);
        }
    }
}
