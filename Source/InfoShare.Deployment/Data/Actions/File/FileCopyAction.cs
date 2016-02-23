using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Actions;

namespace InfoShare.Deployment.Data.Actions.File
{
    public class FileCopyAction : IAction
    {
		private readonly string _sourcePath;
        private readonly string _destinationPath;
        private readonly bool _force;
        private readonly IFileManager _fileManager;

        public FileCopyAction(ILogger logger, string filePath, string destinationPath, bool force = false)
        {
            _sourcePath = filePath;
            _destinationPath = destinationPath;
            _force = force;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        public void Backup()
		{
		}

		public void Execute()
		{
			_fileManager.CopyToDirectory(_sourcePath, _destinationPath, _force);
		}

		public void Rollback()
		{
		}
	}
}
