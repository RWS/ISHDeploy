using System.IO;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Actions;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.File
{
	/// <summary>
	/// Implements file create action
	/// </summary>
    public class FileCreateAction : BaseAction, IRestorableAction
    {
		private readonly string _fileName;
		private readonly string _fileContent;
		private readonly string _destinationPath;

        private readonly IFileManager _fileManager;

		/// <summary>
		/// Creates file in directory
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="destinationPath"></param>
		/// <param name="fileName"></param>
		/// <param name="fileContent"></param>
		/// <param name="force"></param>
		public FileCreateAction(ILogger logger, ISHFilePath destinationPath, string fileName, string fileContent) 
			: base(logger)
        {
			_fileName = fileName;
			_fileContent = fileContent;
			_destinationPath = destinationPath.AbsolutePath;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

		/// <summary>
		/// Executes the action
		/// </summary>
		public override void Execute()
		{
			_fileManager.Write(GetDestinationFileName(), _fileContent);
		}

		/// <summary>
		/// Rollback to initial state
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
		/// Used to create a backup of the file, however, as this command is doing no modification 
		/// on existing file we keep this method empty
		/// </summary>
		public void Backup()
		{
			//	Otherwise backup means removing added item
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
