using System;
using System.IO;
using ISHDeploy.Models;
using ISHDeploy.Extensions;

namespace ISHDeploy.Business
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// </summary>
    public class ISHPaths
    {
        /// <summary>
        /// Specifies Content Manager main folders
        /// </summary>
		public enum IshDeploymentType
		{
            /// <summary>
            /// Content Manager Web folder
            /// </summary>
			Web,
            /// <summary>
            /// Content Manager Data folder
            /// </summary>
			Data,
            /// <summary>
            /// Content Manager App folder
            /// </summary>
			App
        }

        /// <summary>
        /// The instance of the deployment.
        /// </summary>
        private readonly ISHDeployment _ishDeployment;

        /// <summary>
        /// Provides absolute paths to all InfoShare files that are going to be used.
        /// </summary>
        /// <param name="ishDeployment">Instance of the current <see cref="ISHDeployment"/>.</param>
        public ISHPaths(ISHDeployment ishDeployment)
        {
            _ishDeployment = ishDeployment;
        }

        #region InfoShareDeployment folders

        /// <summary>
        /// Path to generated History.ps1 file
        /// </summary>
        public string HistoryFilePath => Path.Combine(_ishDeployment.GetDeploymentAppDataFolder(), "History.ps1");
        
        /// <summary>
        /// UNC path to packages folder
        /// </summary>
        public string PackagesFolderUNCPath => ConvertLocalFolderPathToUNCPath(_ishDeployment.GetDeploymenPackagesFolderPath());

        /// <summary>
        /// Deployment name
        /// </summary>
        public string DeploymentName => _ishDeployment.Name;

        #endregion


        /// <summary>
        /// Converts the local folder path to UNC path.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <returns>Path to folder in UTC format</returns>
        private static string ConvertLocalFolderPathToUNCPath(string localPath)
        {
            return $@"\\{Environment.MachineName}\{localPath.Replace(":", "$")}";
        }
    }
}
