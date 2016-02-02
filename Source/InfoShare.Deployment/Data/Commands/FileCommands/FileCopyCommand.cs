using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.FileCommands
{
    public class FileCopyCommand : ICommand
    {
		private readonly string _sourcePath;
        private readonly string _destinationPath;
        private readonly IFileManager _fileManager;

        public FileCopyCommand(ILogger logger, string filePath, string destinationPath, bool force = false)
        {
            _sourcePath = filePath;
            _destinationPath = destinationPath;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        public void Backup()
		{
		}

		public void Execute()
		{
			_fileManager.Copy(_sourcePath, _destinationPath);
		}

		public void Rollback()
		{
		}
	}
}
