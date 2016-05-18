using ISHDeploy.Data.Actions.Directory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions.File
{
    [TestClass]
    public class SavePackageActionTest : BaseUnitTest
    {
        private const string _filePath = "C:\\DummyFilePath.txt";
        private const string _destinationArchiveFilePath = "C:\\Packages";

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Package_correct_files_to_correct_package_name()
        {
            // Arrange
            var action = new DirectoryCreateZipPackageAction(Logger, _filePath, _destinationArchiveFilePath);

            // Act
            action.Execute();

            // Assert
            FileManager.Received().PackageDirectory(_filePath, _destinationArchiveFilePath);
        }
    }
}
