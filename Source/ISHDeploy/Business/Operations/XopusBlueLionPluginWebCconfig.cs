using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public partial class OperationPaths
    {
        /// <summary>
        /// The path to ~\Web\Author\ASP\Editors\Xopus\BlueLion-Plugin\web.config
        /// </summary>
        public static class XopusBlueLionPluginWebCconfig
        {
            /// <summary>
            /// The path to ~\Web\Author\ASP\Editors\Xopus\BlueLion-Plugin\web.config
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.Web,
                @"Author\ASP\Editors\Xopus\BlueLion-Plugin\web.config");

            /// <summary>
            /// The XPath to Json mimeMap in enrich bluelion web.config file
            /// </summary>
            public const string EnrichBluelionWebConfigJsonMimeMapXPath = "configuration/system.webServer/staticContent/mimeMap[@fileExtension='.json']";

            /// <summary>
            /// The XPath to Json mimeMap in enrich bluelion web.config file
            /// </summary>
            public const string EnrichBluelionWebConfigRemoveJsonMimeMapXmlString = "<remove fileExtension=\".json\"/>";
        }
    }
}
