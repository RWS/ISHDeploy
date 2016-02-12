using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data
{
    public static class ISHPaths
    {
        #region Enable/Disable Content Editor (XOPUS)

        public const string FolderButtonbar = @"Author\ASP\XSL\FolderButtonbar.xml";
        public const string InboxButtonBar = @"Author\ASP\XSL\InboxButtonBar.xml";
        public const string LanguageDocumentButtonBar = @"Author\ASP\XSL\LanguageDocumentButtonbar.xml";
        public const string LicenceFolderPath = @"Author\ASP\Editors\Xopus\license\";

        #endregion

        #region Enable/Disable Enrich

        public const string EnrichConfig = @"Trisoft.InfoShare.Web\ASP\Editors\Xopus\config\bluelion-config.xml";
		public const string XopusConfig = @"Trisoft.InfoShare.Web\ASP\Editors\Xopus\config\config.xml";

		#endregion
        
        #region InstallTool

        public const string InputParametersFile = "inputparameters.xml";

        #endregion
	}
}
