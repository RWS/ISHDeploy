using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.File
{
	/// <summary>
	/// Action responsible for rolling back changes to vanilla state
	/// </summary>
	public class RemoveDirectoryAction : BaseAction
	{
        private readonly IFileManager _fileManager;
        private readonly string _folder;

		/// <summary>
		/// Deletes the folder by folderPath
		/// </summary>
		/// <param name="logger">Logger Object</param>
		/// <param name="folderPath">Folder at deployment, where to put the reverted changes</param>
		public RemoveDirectoryAction(ILogger logger, string folderPath)
			: base(logger)
		{
			_fileManager = ObjectFactory.GetInstance<IFileManager>();
			_folder = folderPath;
		}

		/// <summary>
		/// Deletes the folder
		/// </summary>
		public override void Execute()
		{
			_fileManager.DeleteFolder(_folder);
		}
	}
}
