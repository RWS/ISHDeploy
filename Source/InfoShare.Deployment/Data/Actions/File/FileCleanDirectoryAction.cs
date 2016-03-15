using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.File
{
	/// <summary>
	/// Action responsible for cleaning folder content.
	/// </summary>
	public class FileCleanDirectoryAction : BaseAction
	{
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The folder which content will be cleaned.
        /// </summary>
        private readonly string _folder;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCleanDirectoryAction"/> class.
        /// </summary>
        /// <param name="logger">Logger Object</param>
		/// <param name="folderPath">Folder at deployment, where to put the reverted changes</param>
        public FileCleanDirectoryAction(ILogger logger, string folderPath)
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
			_fileManager.CleanFolder(_folder);
		}
	}
}
