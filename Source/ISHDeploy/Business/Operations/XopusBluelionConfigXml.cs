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
        /// The path to ~\Web\Author\ASP\Editors\Xopus\config\bluelion-config.xml
        /// </summary>
        public static class XopusBluelionConfigXml
        {
            /// <summary>
            /// The path to ~\Web\Author\ASP\Editors\Xopus\config\bluelion-config.xml
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.Web,
                @"Author\ASP\Editors\Xopus\config\bluelion-config.xml");

            /// <summary>
            /// The Enrich integration bluelion configuration xpath comment placeholder
            /// </summary>
            public const string EnrichIntegrationBluelionConfigXPath = "*/*[local-name()='import'][@src='../BlueLion-Plugin/create-toolbar.xml']";

            /// <summary>
            /// The Enrich integration bluelion configuration comment placeholder
            /// </summary>
            public const string EnrichIntegrationBluelionConfig = "../BlueLion-Plugin/create-toolbar.xml";
        }
    }
}
