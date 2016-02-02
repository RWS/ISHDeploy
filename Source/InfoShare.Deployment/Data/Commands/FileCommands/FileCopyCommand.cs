using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.FileCommands
{
    public class FileCopyCommand : ICommand
    {
		private readonly string _destinationPath;
		private readonly IFileManager _fileManager;
		public FileCopyCommand(ILogger logger, string filePath, string destinationPath, bool force = false)
		{
			_destinationPath = destinationPath;
			_fileManager = new FileManager(logger, filePath);
		}

		public void Backup()
		{
			_fileManager.Backup();
		}

		public void Execute()
		{
			_fileManager.Copy(_destinationPath);
		}

		public void Rollback()
		{
			_fileManager.RestoreOriginal();
		}
	}
}
