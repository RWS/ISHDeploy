using System.IO;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Actions;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.File
{
	/// <summary>
	/// Implements file copy action
	/// </summary>
    public class FileCopyAction : BaseAction, IRestorableAction
	{
		private readonly string _sourcePath;
		private readonly string _destinationPath;
		private readonly bool _force;

		private readonly IFileManager _fileManager;

		/// <summary>
		/// Does copy of file to directory
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="sourcePath"></param>
		/// <param name="destinationPath"></param>
		/// <param name="force"></param>
		public FileCopyAction(ILogger logger, string sourcePath, ISHFilePath destinationPath, bool force = false) 
			: base(logger)
        {
            _sourcePath = sourcePath;
			_destinationPath = destinationPath.AbsolutePath;
            _force = force;

			_fileManager = ObjectFactory.GetInstance<IFileManager>();
		}

		/// <summary>
		/// Executes the action
		/// </summary>
		public override void Execute()
		{
			_fileManager.CopyToDirectory(_sourcePath, _destinationPath, _force);
		}

		/// <summary>
		/// Rollback to initial state
		/// </summary>
		public virtual void Rollback()
		{
			string fileName = Path.GetFileName(_sourcePath);
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
