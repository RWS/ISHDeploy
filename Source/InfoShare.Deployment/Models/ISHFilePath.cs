using System.IO;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Extensions;

namespace InfoShare.Deployment.Models
{
	public class ISHFilePath
	{
		private readonly ISHDeployment _ishDeployment;

		private readonly ISHPaths.IshDeploymentType _deploymentType;
        
		public string RelativePath { get; }
        
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
						return Path.Combine(_ishDeployment.AppPath, RelativePath);
					case ISHPaths.IshDeploymentType.Web:
						return Path.Combine(_ishDeployment.AuthorFolderPath, RelativePath);
					case ISHPaths.IshDeploymentType.Data:
						return Path.Combine(_ishDeployment.DataPath, RelativePath);
					default:
						return null;
				}
			}
		}

		public string DeploymentSuffix => _ishDeployment.Suffix;

		public string VanillaPath => Path.Combine(DeploymentBackupFolder, RelativePath);

		#region InfoShareDeployment folders

		public string DeploymentBackupFolder => _ishDeployment.GetDeploymentTypeBackupFolder(_deploymentType);

		#endregion
	}
}
