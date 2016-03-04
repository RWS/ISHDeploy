using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.File
{
	/// <summary>
	/// Action responsible for rolling back changes to vanilla state
	/// </summary>
	public class FileCleanDirectoryAction : BaseAction
	{
        private readonly IFileManager _fileManager;
        private readonly string _folder;

		/// <summary>
		/// Reverts changes to vanilla state. If backup folder is not defined, then we do remove all from deployment folder
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
		/// And cleans up the folder
		/// </summary>
		public override void Execute()
		{
			_fileManager.CleanFolder(_folder);
		}
	}
}
