using System;
using System.Net;
using System.Reflection;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.LicenseCommands
{
    public class LicenseTestCommand : ICommand
    {
		public const string LICENSE_FILE_EXTENSION = ".txt";

		private readonly Action<bool> _returnResult;
		private readonly string _hostname;
		private readonly LicenseManager _licenseManager;

		public LicenseTestCommand(ILogger logger, string licenseFolderPath, string hostname, Action<bool> returnResult)
		{
			_hostname = hostname;
			_licenseManager = new LicenseManager(logger, licenseFolderPath);

			_returnResult = returnResult;
		}

		public void Backup()
		{
		}

		public void Execute()
		{
			bool result = _licenseManager.IsValid(_hostname);
			_returnResult?.Invoke(result);
		}
	}
}
