using System;
using System.IO;
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Extensions;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public abstract partial class BasePathsOperation
    {
        /// <summary>
        /// The instance of the deployment.
        /// </summary>
        private static Models.ISHDeployment _ishDeployment;

        /// <summary>
        /// The logger.
        /// </summary>
        private static ILogger _logger;

        /// <summary>
        /// The instance of extended description of the deployment.
        /// </summary>
        private static Models.ISHDeploymentInternal _ishDeploymentInternal;
        /// <summary>
        /// <para type="description">Internal extended description of the instance of the Content Manager deployment.</para>
        /// </summary>
        public static Models.ISHDeploymentInternal ISHDeploymentInternal
        {
            get
            {
                if (_ishDeploymentInternal == null || _ishDeployment.Name != _ishDeploymentInternal.Name)
                {
                    var action = new GetISHDeploymentExtendedAction(_logger, _ishDeployment.Name,
                        result => _ishDeploymentInternal = result);
                    action.Execute();
                }
                return _ishDeploymentInternal;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BasePathsOperation"/> class.
        /// </summary>
        /// <param name="ishDeployment">The ish deployment.</param>
        /// <param name="logger"></param>
        public BasePathsOperation(ILogger logger, Models.ISHDeployment ishDeployment)
        {
            _logger = logger;
            _ishDeployment = ishDeployment;
        }

        /// <summary>
        /// The path to generated History.ps1 file
        /// </summary>
        protected static string HistoryFilePath => Path.Combine(ISHDeploymentInternal.GetDeploymentAppDataFolder(), "History.ps1");

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
