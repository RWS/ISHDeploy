using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.File
{
    /// <summary>
	/// Action responsible for copying directory content from source to destination directory.
	/// </summary>
    /// <seealso cref="BaseAction" />
    public class FileCopyDirectoryAction : BaseAction
	{
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The source folder path.
        /// </summary>
        private readonly string _sourceFolder;

        /// <summary>
        /// The destination folder path.
        /// </summary>
        private readonly string _destinationFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCopyDirectoryAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sourceFolder">Source folder.</param>
        /// <param name="destinationFolder">Destination folder.</param>
        public FileCopyDirectoryAction(ILogger logger, string sourceFolder, string destinationFolder)
            : base(logger)
        {
			_sourceFolder = sourceFolder;
			_destinationFolder = destinationFolder;

			_fileManager = ObjectFactory.GetInstance<IFileManager>();
		}

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
	    {
			_fileManager.CopyDirectoryContent(_sourceFolder, _destinationFolder);
		}
	}
}
