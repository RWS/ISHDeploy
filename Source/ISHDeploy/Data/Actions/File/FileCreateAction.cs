using System.IO;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
	/// Action that creates new file with content inside.
    /// </summary>
    /// <seealso cref="BaseAction" />
    /// <seealso cref="IRestorableAction" />
    public class FileCreateAction : BaseAction, IRestorableAction
    {
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The file name that will be created.
        /// </summary>
        private readonly string _fileName;

        /// <summary>
        /// The file content
        /// </summary>
        private readonly string _fileContent;

        /// <summary>
        /// The destination path for new file.
        /// </summary>
        private readonly string _destinationPath;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCreateAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="fileName">Name of the file that will be created.</param>
        /// <param name="fileContent">Content of the new file.</param>
        public FileCreateAction(ILogger logger, ISHFilePath destinationPath, string fileName, string fileContent) 
			: base(logger)
        {
			_fileName = fileName;
			_fileContent = fileContent;
			_destinationPath = destinationPath.AbsolutePath;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
		{
			_fileManager.Write(GetDestinationFileName(), _fileContent);
		}

        /// <summary>
        /// Reverts an asset to initial state
        /// </summary>
        public virtual void Rollback()
		{
			var createdFileName = GetDestinationFileName();
			if (_fileManager.Exists(createdFileName))
		{
				_fileManager.Delete(createdFileName);
			}
		}

        /// <summary>
        /// Creates backup of the asset
        /// </summary>
        public void Backup()
		{
            // This actions does not require backup
            //	So do nothing here
        }

        /// <summary>
        /// Gets name of the file to be created
        /// </summary>
        private string GetDestinationFileName()
		{
			return Path.Combine(_destinationPath, _fileName);
		}
	}
}
