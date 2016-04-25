using System.IO;
using ISHDeploy.Business;
using ISHDeploy.Extensions;

namespace ISHDeploy.Models
{
    /// <summary>
    /// Wrapper for file path that provides relative, absolute and vanilla installation path of the file.
    /// </summary>
    public class ISHFilePath
	{
        /// <summary>
        /// The instance of the deployment.
        /// </summary>
        private readonly ISHDeployment _ishDeployment;

        /// <summary>
        /// Type of the deployment.
        /// </summary>
        private readonly ISHPaths.IshDeploymentType _deploymentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHFilePath"/> class.
        /// </summary>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="deploymentType">Type of the deployment.</param>
        /// <param name="path">The path to the file.</param>
        public ISHFilePath(ISHDeployment ishDeployment, ISHPaths.IshDeploymentType deploymentType, string path)
		{
			_ishDeployment = ishDeployment;
			_deploymentType = deploymentType;

			RelativePath = path;
		}

		/// <summary>
		/// Absolute path to file or folder
		/// </summary>
		public string AbsolutePath
		{
			get
			{
				switch (_deploymentType)
				{
					case ISHPaths.IshDeploymentType.App:
						return Path.Combine(_ishDeployment.GetAppFolderPath(), RelativePath);
					case ISHPaths.IshDeploymentType.Web:
						return Path.Combine(_ishDeployment.GetAuthorFolderPath(), RelativePath);
					case ISHPaths.IshDeploymentType.Data:
						return Path.Combine(_ishDeployment.GetDataFolderPath(), RelativePath);
					default:
						return null;
				}
			}
		}

        /// <summary>
        /// Gets the relative path of the file.
        /// </summary>
        public string RelativePath { get; }

        /// <summary>
        /// Gets the vanilla installation path.
        /// </summary>
        public string VanillaPath => Path.Combine(_ishDeployment.GetDeploymentTypeBackupFolder(_deploymentType), RelativePath);
	}
}
