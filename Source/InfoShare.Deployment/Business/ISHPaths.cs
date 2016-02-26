using System;
using InfoShare.Deployment.Models;
using System.IO;
using InfoShare.Deployment.Extensions;

namespace InfoShare.Deployment.Business
{
    public class ISHPaths
    {
		public enum IshDeploymentType
		{
			Web,
			Data,
			App
		}

		#region Enable/Disable Content Editor (XOPUS)

		public ISHFilePath FolderButtonbar => GetIshFilePath(IshDeploymentType.App, @"App\ASP\XSL\FolderButtonbar.xml");
		public ISHFilePath InboxButtonBar => GetIshFilePath(IshDeploymentType.App, @"App\ASP\XSL\InboxButtonBar.xml");
		public ISHFilePath LanguageDocumentButtonBar => GetIshFilePath(IshDeploymentType.App, @"App\ASP\XSL\LanguageDocumentButtonbar.xml");
		public ISHFilePath LicenceFolderPath => GetIshFilePath(IshDeploymentType.App, @"App\ASP\Editors\Xopus\license\");

		#endregion

		#region Enable/Disable Enrich

		public ISHFilePath EnrichConfig => GetIshFilePath(IshDeploymentType.App, @"App\ASP\Editors\Xopus\config\bluelion-config.xml");
		public ISHFilePath XopusConfig => GetIshFilePath(IshDeploymentType.App, @"App\ASP\Editors\Xopus\config\config.xml");

		#endregion

		#region Enable/Disable ExternalPreview

		public ISHFilePath AuthorAspWebConfig => GetIshFilePath(IshDeploymentType.App, @"App\ASP\Web.config");

		#endregion

		#region InfoShareDeployment folders

        public string HistoryFilePath => Path.Combine(_ishDeployment.GetDeploymentAppDataFolder(), "History.ps1");

        public string DeploymentSuffix => _ishDeployment.Suffix;

        #endregion

        private readonly ISHDeployment _ishDeployment;
        public ISHPaths(ISHDeployment ishDeployment)
        {
            _ishDeployment = ishDeployment;
        }

		public ISHFilePath GetIshFilePath(IshDeploymentType deploymentType, string filePath)
		{
			return new ISHFilePath(_ishDeployment, deploymentType, filePath);
		}

	}
}
