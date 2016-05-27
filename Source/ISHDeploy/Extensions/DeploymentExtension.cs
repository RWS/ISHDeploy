using System;
using System.IO;
using ISHDeploy.Models;

namespace ISHDeploy.Extensions
{
	/// <summary>
	/// Infoshare Deployment extensions
	/// </summary>
	public static class DeploymentExtension
	{
		/// <summary>
		/// Retrieves Application data location for deployment
		/// </summary>
		/// <param name="deployment">Deployment object <see cref="T:ISHDeploy.Models.ISHDeployment"/>.</param>
		/// <returns>Path to application data folder</returns>
		public static string GetDeploymentAppDataFolder(this ISHDeploymentExtended deployment)
		{
			var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var moduleName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
			var ishDeploymentFolder = Path.Combine(programData, moduleName, deployment.Name);

			if (!Directory.Exists(ishDeploymentFolder))
			{
				Directory.CreateDirectory(ishDeploymentFolder);
			}

			return ishDeploymentFolder;
		}
      
        /// <summary>
        /// Retrieves back up folder location for deployment
        /// </summary>
        /// <param name="deployment">Deployment object <see cref="T:ISHDeploy.Models.ISHDeployment"/>.</param>
        /// <returns>Path to back up folder</returns>
        public static string GetDeploymentBackupFolder(this ISHDeploymentExtended deployment)
        {
            return Path.Combine(deployment.GetDeploymentAppDataFolder(), "Backup");
        }

        /// <summary>
        /// Retrieves back up folder location for deployment depending from deployment type
        /// </summary>
        /// <param name="deployment">Deployment object <see cref="T:ISHDeploy.Models.ISHDeployment"/>.</param>
        /// <param name="deploymentType">Deployment type <see cref="T:ISHDeploy.Business.ISHFilePath.IshDeploymentType"/>.</param>
        /// <returns>Path to back up folder depending from deployment type</returns>
        public static string GetDeploymentTypeBackupFolder(this ISHDeploymentExtended deployment, ISHFilePath.IshDeploymentType deploymentType)
		{
			return Path.Combine(deployment.GetDeploymentBackupFolder(), deploymentType.ToString());
		}
	}
}
