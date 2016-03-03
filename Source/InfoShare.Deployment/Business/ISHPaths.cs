using InfoShare.Deployment.Models;
using System.IO;
using InfoShare.Deployment.Extensions;

namespace InfoShare.Deployment.Business
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

        private readonly ISHDeployment _ishDeployment;

        /// <summary>
        /// Provides absolute paths to all ISH files that are going to be used
        /// </summary>
        /// <param name="ishDeployment">ISHDeployment instance</param>
        public ISHPaths(ISHDeployment ishDeployment)
        {
            _ishDeployment = ishDeployment;
        }

        private ISHFilePath GetIshFilePath(IshDeploymentType deploymentType, string filePath)
        {
            return new ISHFilePath(_ishDeployment, deploymentType, filePath);
        }
        
        #region Enable/Disable Content Editor (XOPUS)

        /// <summary>
        /// Path to FolderButtonbar.xml file
        /// </summary>
        public ISHFilePath FolderButtonbar => GetIshFilePath(IshDeploymentType.Web, @"Author\ASP\XSL\FolderButtonbar.xml");

        /// <summary>
        /// Path to InboxButtonBar.xml file
        /// </summary>
		public ISHFilePath InboxButtonBar => GetIshFilePath(IshDeploymentType.Web, @"Author\ASP\XSL\InboxButtonBar.xml");

        /// <summary>
        /// Path to LanguageDocumentButtonbar.xml file
        /// </summary>
		public ISHFilePath LanguageDocumentButtonBar => GetIshFilePath(IshDeploymentType.Web, @"Author\ASP\XSL\LanguageDocumentButtonbar.xml");
        
        /// <summary>
        /// Path to license folder
        /// </summary>
		public ISHFilePath LicenceFolderPath => GetIshFilePath(IshDeploymentType.Web, @"Author\ASP\Editors\Xopus\license\");

        #endregion

        #region Enable/Disable Enrich

        /// <summary>
        /// Path to bluelion-config.xml file
        /// </summary>
		public ISHFilePath EnrichConfig => GetIshFilePath(IshDeploymentType.Web, @"Author\ASP\Editors\Xopus\config\bluelion-config.xml");

        /// <summary>
        /// Path to Xopus config.xml file
        /// </summary>
		public ISHFilePath XopusConfig => GetIshFilePath(IshDeploymentType.Web, @"Author\ASP\Editors\Xopus\config\config.xml");

		#endregion
        
        #region Enable/Disable ExternalPreview

        /// <summary>
        /// Path to Web.config file
        /// </summary>
		public ISHFilePath AuthorAspWebConfig => GetIshFilePath(IshDeploymentType.Web, @"Author\ASP\Web.config");

        #endregion

        #region Enable/Disable Translation Job
        
        /// <summary>
        /// Path to TopDocumentButtonbar.xml file
        /// </summary>
        public ISHFilePath TopDocumentButtonbar => GetIshFilePath(IshDeploymentType.Web, @"Author\ASP\XSL\TopDocumentButtonbar.xml");

        /// <summary>
        /// Path to Tree.htm file
        /// </summary>
        public ISHFilePath TreeHtm => GetIshFilePath(IshDeploymentType.Web, @"Author\ASP\Tree.htm");

        #endregion

        #region Event Monitor

        /// <summary>
        /// Path to EventMonitorMenuBar.xml file
        /// </summary>
        public ISHFilePath EventMonitorMenuBar => GetIshFilePath(IshDeploymentType.Web, @"Author\ASP\XSL\EventMonitorMenuBar.xml");

        #endregion

        #region InfoShareDeployment folders

        /// <summary>
        /// Path to generated History.ps1 file
        /// </summary>
        public string HistoryFilePath => Path.Combine(_ishDeployment.GetDeploymentAppDataFolder(), "History.ps1");

        /// <summary>
        /// ISHDeployment suffix
        /// </summary>
        public string DeploymentSuffix => _ishDeployment.Suffix;

        #endregion
	}
}
