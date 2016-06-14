using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.Directory
{
    /// <summary>
	/// Action that creates new folder.
    /// </summary>
    /// <seealso cref="BaseAction" />
    /// <seealso cref="IRestorableAction" />
    public class DirectoryEnsureExistsAction : BaseAction
    {
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The path to folder that will be created.
        /// </summary>
        private readonly string _folderPath;


        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEnsureExistsAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="folderPath">The path to folder that will be created.</param>
        public DirectoryEnsureExistsAction(ILogger logger, string folderPath) 
			: base(logger)
        {
            _folderPath = folderPath;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
		{
			_fileManager.EnsureDirectoryExists(_folderPath);
		}
	}
}
