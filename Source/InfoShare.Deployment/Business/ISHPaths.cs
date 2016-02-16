using InfoShare.Deployment.Models;
using System.IO;

namespace InfoShare.Deployment.Business
{
    public class ISHPaths
    {
        #region Enable/Disable Content Editor (XOPUS)

        public string FolderButtonbar { get { return CombineAuthorFolderPath(@"Author\ASP\XSL\FolderButtonbar.xml"); } }
        public string InboxButtonBar { get { return CombineAuthorFolderPath(@"Author\ASP\XSL\InboxButtonBar.xml"); } }
        public string LanguageDocumentButtonBar { get { return CombineAuthorFolderPath(@"Author\ASP\XSL\LanguageDocumentButtonbar.xml"); } }
        public string LicenceFolderPath { get { return CombineAuthorFolderPath(@"Author\ASP\Editors\Xopus\license\"); } }

        #endregion

        #region Enable/Disable Enrich

        public string EnrichConfig { get { return CombineAuthorFolderPath(@"Author\ASP\Editors\Xopus\config\bluelion-config.xml"); } }
		public string XopusConfig { get { return CombineAuthorFolderPath(@"Author\ASP\Editors\Xopus\config\config.xml"); } }

		#endregion
        
        #region Enable/Disable ExternalPreview

        public string AuthorAspWebConfig { get { return CombineAuthorFolderPath(@"Author\ASP\Web.config"); } }

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
