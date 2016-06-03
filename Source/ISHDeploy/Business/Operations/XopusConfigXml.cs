using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public partial class BasePathsOperation
    {
        /// <summary>
        /// The path to ~\Web\Author\ASP\Editors\Xopus\config\config.xml
        /// </summary>
        public static class XopusConfigXml
        {
            /// <summary>
            /// The path to ~\Web\Author\ASP\Editors\Xopus\config\config.xml
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(ISHDeploymentInternal, ISHFilePath.IshDeploymentType.Web,
                @"Author\ASP\Editors\Xopus\config\config.xml");

            /// <summary>
            /// The Enrich integration bluelion plugin xpath comment placeholder
            /// </summary>
            public const string EnrichIntegrationXPath = "*/*[local-name()='javascript'][@src='../BlueLion-Plugin/Bootstrap/bootstrap.js']";

            /// <summary>
            /// The Enrich integration bluelion plugin comment placeholder
            /// </summary>
            public const string EnrichIntegration = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
        }
    }
}
