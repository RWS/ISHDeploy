using System.IO;
using ISHDeploy.Extensions;

namespace ISHDeploy.Models
{
    /// <summary>
    /// Wrapper for file path that provides relative, absolute and vanilla installation path of the file.
    /// </summary>
    public class ISHFilePath
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
        private readonly ISHDeploymentInternal _ishDeployment;

        /// <summary>
        /// Type of the deployment.
        /// </summary>
        private readonly IshDeploymentType _deploymentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHFilePath"/> class.
        /// </summary>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="deploymentType">Type of the deployment.</param>
        /// <param name="path">The path to the file.</param>
        public ISHFilePath(ISHDeploymentInternal ishDeployment, IshDeploymentType deploymentType, string path)
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
					case IshDeploymentType.App:
						return Path.Combine(_ishDeployment.AppFolderPath, RelativePath);
					case IshDeploymentType.Web:
						return Path.Combine(_ishDeployment.AuthorFolderPath, RelativePath);
					case IshDeploymentType.Data:
						return Path.Combine(_ishDeployment.DataFolderPath, RelativePath);
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
