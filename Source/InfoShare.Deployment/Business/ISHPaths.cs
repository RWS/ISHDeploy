using System;
using InfoShare.Deployment.Models;
using System.IO;

namespace InfoShare.Deployment.Business
{
    public class ISHPaths
    {
        #region Enable/Disable Content Editor (XOPUS)

        public string FolderButtonbar => CombineAuthorFolderPath(@"Author\ASP\XSL\FolderButtonbar.xml");
        public string InboxButtonBar => CombineAuthorFolderPath(@"Author\ASP\XSL\InboxButtonBar.xml");
        public string LanguageDocumentButtonBar => CombineAuthorFolderPath(@"Author\ASP\XSL\LanguageDocumentButtonbar.xml");
        public string LicenceFolderPath => CombineAuthorFolderPath(@"Author\ASP\Editors\Xopus\license\");

        #endregion

        #region Enable/Disable Enrich

        public string EnrichConfig => CombineAuthorFolderPath(@"Author\ASP\Editors\Xopus\config\bluelion-config.xml");
		public string XopusConfig => CombineAuthorFolderPath(@"Author\ASP\Editors\Xopus\config\config.xml");

		#endregion
        
        #region Enable/Disable ExternalPreview

        public string AuthorAspWebConfig => CombineAuthorFolderPath(@"Author\ASP\Web.config");

        #endregion

        #region InfoShareDeployment folders

        public string InfoshareDeploymentDataFolder
        {
            get
            {
                var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var folderPath = $@"InfoShare.Deployment\ISH{_ishDeployment.Suffix}";
                var ishDeploymentFolder = Path.Combine(programData, folderPath);
                
                if (!Directory.Exists(ishDeploymentFolder))
                {
                    Directory.CreateDirectory(ishDeploymentFolder);
                }

                return ishDeploymentFolder;
            }
        }

        public string HistoryFilePath => Path.Combine(InfoshareDeploymentDataFolder, "History.ps1");

        public string DeploymentSuffix => _ishDeployment.Suffix;

        #endregion

        private readonly ISHDeployment _ishDeployment;
        public ISHPaths(ISHDeployment ishDeployment)
        {
            _ishDeployment = ishDeployment;
        }

        private string CombineAuthorFolderPath(string relativePath)
        {
            return Path.Combine(_ishDeployment.AuthorFolderPath, relativePath);
        }
	}
}
