using System;
using System.IO;
using ISHDeploy.Extensions;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public partial class OperationPaths
    {
        /// <summary>
        /// The instance of the deployment.
        /// </summary>
        private static Models.ISHDeploymentInternal _ishDeployment;

        /// <summary>
        /// The path to generated History.ps1 file
        /// </summary>
        public static string HistoryFilePath => Path.Combine(_ishDeployment.GetDeploymentAppDataFolder(), "History.ps1");
        
        /// <summary>
        /// Initialization of a class to build the full paths to files
        /// </summary>
        /// <param name="ishDeployment">Instance of the current <see cref="ISHDeployment"/>.</param>
        public static void Initialize(Models.ISHDeploymentInternal ishDeployment)
        {
            _ishDeployment = ishDeployment;
        }

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
