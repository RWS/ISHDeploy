using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.File
{
	/// <summary>
	/// Action responsible for rolling back changes to vanilla state
	/// </summary>
	public class FileCopyDirectoryAction : BaseAction
	{
        private readonly IFileManager _fileManager;
        private readonly string _sourceFolder;
        private readonly string _destinationFolder;


		/// <summary>
		/// Reverts changes to vanilla state
		/// </summary>
		/// <param name="logger">Logger object</param>
		/// <param name="sourceFolder">Folder which source content</param>
		/// <param name="destinationFolder">Destination folder</param>
		public FileCopyDirectoryAction(ILogger logger, string sourceFolder, string destinationFolder)
            : base(logger)
        {
			_sourceFolder = sourceFolder;
			_destinationFolder = destinationFolder;

			_fileManager = ObjectFactory.GetInstance<IFileManager>();
		}

		/// <summary>
		/// Copies file from source folder to destination folder.
		/// </summary>
		public override void Execute()
	    {
			_fileManager.CopyDirectoryContent(_sourceFolder, _destinationFolder);
		}
	}
}
