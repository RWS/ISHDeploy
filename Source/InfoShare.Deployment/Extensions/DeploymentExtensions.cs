using System;
using System.IO;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Extensions
{
	public static class Deployment
	{
		public static string GetDeploymentAppDataFolder(this ISHDeployment deployment)
		{
			var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			var folderPath = $@"InfoShare.Deployment\ISH{deployment.Suffix}";
			var ishDeploymentFolder = Path.Combine(programData, folderPath);

			if (!Directory.Exists(ishDeploymentFolder))
			{
				Directory.CreateDirectory(ishDeploymentFolder);
			}

			return ishDeploymentFolder;
		}

		public static string GetDeploymentBackupFolder(this ISHDeployment deployment)
		{
			return Path.Combine(deployment.GetDeploymentAppDataFolder(), "Backup");
		}

		public static string GetDeploymentTypeBackupFolder(this ISHDeployment deployment, ISHPaths.IshDeploymentType deploymentType)
		{
			return Path.Combine(deployment.GetDeploymentBackupFolder(), deploymentType.ToString());
		}
	}
}
