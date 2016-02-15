using System;
using System.IO;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.LicenseCommands
{
    public class LicenseTestCommand : ICommand
    {
		public const string LICENSE_FILE_EXTENSION = ".txt";

		private readonly Action<bool> _returnResult;
        private readonly ILogger _logger;
        private readonly string _licenseFolderPath;
        private readonly string _hostname;
        private readonly IFileManager _fileManager;

        public LicenseTestCommand(ILogger logger, string licenseFolderPath, string hostname, Action<bool> returnResult)
        {
            _logger = logger;
            _licenseFolderPath = licenseFolderPath;
            _hostname = hostname;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();

            _returnResult = returnResult;
		}

		public void Execute()
		{
		    string filePath;
		    if (_fileManager.TryToFindLicenseFile(_licenseFolderPath, _hostname, LICENSE_FILE_EXTENSION, out filePath))
		    {
                _returnResult?.Invoke(_fileManager.TryToFindLicenseFile(_licenseFolderPath, _hostname, LICENSE_FILE_EXTENSION, out filePath));
            }
		    else
            {
                throw new FileNotFoundException($"The license file for host \"{_hostname}\" not found");
            }
        }
	}
}
