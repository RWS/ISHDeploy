using System;
using System.IO;
using System.Net;
using InfoShare.Deployment.Helpers;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Services
{
    public class LicenseManager : FileManager
    {
		private const string LICENSE_FILE_EXTENSION = ".txt";

		private readonly string _licenseFolderPath;
		public LicenseManager(ILogger logger, string licenseFolderPath) : base(logger)
		{
			_licenseFolderPath = licenseFolderPath;
		}

        public bool IsValid(string hostName)
        {
			IPAddress addr;
			if (IPAddress.TryParse(hostName, out addr))
			{
				return IsHostValid(addr);
			}
			else
			{
				while (!String.IsNullOrEmpty(hostName))
				{
					if (IsHostValid(hostName))
					{
						return true;
					}

					int i = hostName.IndexOf(".", StringComparison.InvariantCulture);
					if (i > 0)
					{
						hostName = hostName.Substring(i+1);
					}
					else
					{
						return false;
					}
				}

				return false;
			}
		}

		#region private methods

		private bool IsHostValid(IPAddress ip)
		{
			return IsHostValid(ip.ToString());
		}

		private bool IsHostValid(string hostName)
		{
			string filePath = Path.Combine(_licenseFolderPath, string.Concat(hostName, LICENSE_FILE_EXTENSION));
			if (Exists(filePath))
			{
				string licenseContent = File.ReadAllText(filePath);

				try
				{
					XopusLicenseChecker checker = new XopusLicenseChecker(licenseContent);
					return checker.IsValid(hostName);
				}
				catch (XopusLicenseException ex)
				{
					return false;
				}
			}

			return false;
		}

		#endregion
	}
}
