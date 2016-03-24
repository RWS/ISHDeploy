using System;
using System.IO;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Extensions
{
	/// <summary>
	/// Infoshare Deployment extensions
	/// </summary>
	public static class DeploymentExtension
	{
		/// <summary>
		/// Retrieves Application data location for deployment
		/// </summary>
		/// <param name="deployment">Deployment object <see cref="T:InfoShare.Deployment.Models.ISHDeployment"/>.</param>
		/// <returns>Path to application data folder</returns>
		public static string GetDeploymentAppDataFolder(this ISHDeployment deployment)
		{
			var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			var folderPath = Path.Combine("InfoShare.Deployment", $"v{deployment.SoftwareVersion}", deployment.Name);
			var ishDeploymentFolder = Path.Combine(programData, folderPath);

			if (!Directory.Exists(ishDeploymentFolder))
			{
				Directory.CreateDirectory(ishDeploymentFolder);
			}

			return ishDeploymentFolder;
		}

		/// <summary>
		/// Retrieves back up folder location for deployment
		/// </summary>
		/// <param name="deployment">Deployment object <see cref="T:InfoShare.Deployment.Models.ISHDeployment"/>.</param>
		/// <returns>Path to back up folder</returns>
		public static string GetDeploymentBackupFolder(this ISHDeployment deployment)
		{
			return Path.Combine(deployment.GetDeploymentAppDataFolder(), "Backup");
		}

		/// <summary>
		/// Retrieves back up folder location for deployment depending from deployment type
		/// </summary>
		/// <param name="deployment">Deployment object <see cref="T:InfoShare.Deployment.Models.ISHDeployment"/>.</param>
		/// <param name="deploymentType">Deployment type <see cref="T:InfoShare.Deployment.Business.ISHPaths.IshDeploymentType"/>.</param>
		/// <returns>Path to back up folder depending from deployment type</returns>
		public static string GetDeploymentTypeBackupFolder(this ISHDeployment deployment, ISHPaths.IshDeploymentType deploymentType)
		{
			return Path.Combine(deployment.GetDeploymentBackupFolder(), deploymentType.ToString());
		}
	}
}
