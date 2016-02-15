using System;
using System.IO;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Commands.LicenseCommands
{
    public class LicenseTestCommand : BaseCommandWithResult<bool>
    {
		public const string LICENSE_FILE_EXTENSION = ".txt";
        
        private readonly string _licenseFolderPath;
        private readonly string _hostname;
        private readonly IFileManager _fileManager;

        public LicenseTestCommand(ILogger logger, string licenseFolderPath, string hostname, Action<bool> returnResult)
            : base(logger, returnResult)
        {
            _licenseFolderPath = licenseFolderPath;
            _hostname = hostname;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
		}

        protected override bool ExecuteWithResult()
        {
		    string filePath;
		    if (!_fileManager.TryToFindLicenseFile(_licenseFolderPath, _hostname, LICENSE_FILE_EXTENSION, out filePath))
		    {             
                throw new FileNotFoundException($"The license file for host \"{_hostname}\" not found");
            }

            return true;
        }
	}
}
