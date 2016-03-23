using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
	/// Action responsible for removing directory.
    /// </summary>
    /// <seealso cref="BaseAction" />
    public class RemoveDirectoryAction : BaseAction
	{
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The folder to be removed
        /// </summary>
        private readonly string _folder;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveDirectoryAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
		/// <param name="folderPath">The folder that will be removed.</param>
        public RemoveDirectoryAction(ILogger logger, string folderPath)
			: base(logger)
		{
			_fileManager = ObjectFactory.GetInstance<IFileManager>();
			_folder = folderPath;
		}

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
		{
			_fileManager.DeleteFolder(_folder);
		}
	}
}
