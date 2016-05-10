using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.Directory
{
    /// <summary>
    /// Saves files to package.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class DirectoryCreateZipPackageAction : SingleFileCreationAction
    {
        /// <summary>
        /// The path of the archive to be created, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.
        /// </summary>
        private readonly string _destinationArchiveFilePath;

        /// <summary>
        /// The parameter determines whether to include subfolders to the archive
        /// </summary>
        private readonly bool _includeBaseDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryCreateZipPackageAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="destinationArchiveFilePath">The path of the archive to be created, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.</param>
        /// <param name="includeBaseDirectory">true to include the directory name from sourceDirectoryName at the root of the archive; false to include only the contents of the directory. False by default</param>
        public DirectoryCreateZipPackageAction(ILogger logger, string filePath, string destinationArchiveFilePath,
            bool includeBaseDirectory = false)
            : base(logger, filePath)
        {
            _destinationArchiveFilePath = destinationArchiveFilePath;
            _includeBaseDirectory = includeBaseDirectory;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _fileManager.PackageDirectory(_filePath, _destinationArchiveFilePath, _includeBaseDirectory);
        }
    }
}
