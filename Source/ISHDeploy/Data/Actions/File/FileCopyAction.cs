using System.IO;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.File
{
	/// <summary>
	/// Implements file copy action
	/// </summary>
    public class FileCopyAction : BaseAction, IRestorableAction
    {
        /// <summary>
        /// The source file path
        /// </summary>
        private readonly string _sourcePath;

        /// <summary>
        /// The destination file path
        /// </summary>
        private readonly string _destinationPath;

        /// <summary>
        /// The force switch identifies if file needs to be replaced.
        /// </summary>
        private readonly bool _force;

        /// <summary>
        /// The file manager
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCopyAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sourcePath">The source file path.</param>
        /// <param name="destinationPath">The destination file path.</param>
        /// <param name="force">Replaces existing file if true.</param>
        public FileCopyAction(ILogger logger, string sourcePath, ISHFilePath destinationPath, bool force = false) 
			: base(logger)
        {
            _sourcePath = sourcePath;
			_destinationPath = destinationPath.AbsolutePath;
            _force = force;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

		/// <summary>
		/// Executes the action.
		/// </summary>
		public override void Execute()
		{
			_fileManager.CopyToDirectory(_sourcePath, _destinationPath, _force);
		}

        /// <summary>
        /// Reverts an asset to initial state.
        /// </summary>
        public virtual void Rollback()
		{
			var fileName = Path.GetFileName(_sourcePath);

		    if (string.IsNullOrEmpty(fileName))
		    {
		        return;
		    }

			var copiedFileName = Path.Combine(_destinationPath, fileName);

			if (_fileManager.Exists(copiedFileName))
		    {
				_fileManager.Delete(copiedFileName);
			}
		}

		/// <summary>
		/// Used to create a backup of the file, however, as this command is doing no modification 
		/// on existing file we keep this method empty
		/// </summary>
		public void Backup()
		{
			//	Otherwise backup means removing added item
			//	So do nothing here
		}
	}
}
