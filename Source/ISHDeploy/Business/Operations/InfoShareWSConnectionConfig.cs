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
        /// The path to ~\Web\InfoShareWS\connectionconfiguration.xml
        /// </summary>
        public static class InfoShareWSConnectionConfig
        {
            /// <summary>
            /// The path to ~\Web\InfoShareWS\connectionconfiguration.xml
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(ISHDeploymentInternal, ISHFilePath.IshDeploymentType.Web,
                @"InfoShareWS\connectionconfiguration.xml");

            /// <summary>
            /// The xpath of "connectionconfiguration/issuer/authenticationtype" element in file ~\Web\InfoShareWS\connectionconfiguration.xml
            /// </summary>
            public const string WSTrustBindingTypeXPath = "connectionconfiguration/issuer/authenticationtype";

            /// <summary>
            /// The xpath of "connectionconfiguration/issuer/url" element in file ~\Web\InfoShareWS\connectionconfiguration.xml
            /// </summary>
            public const string WSTrustEndpointUrlXPath = "connectionconfiguration/issuer/url";

            /// <summary>
            /// The xpath of "configuration/trisoft.infoshare.client.settings/datasources/datasource/certificatevalidationmode" element in ~\Web\Author\ASP\Trisoft.InfoShare.Client.config file
            /// </summary>
            public const string InfoShareWSServiceCertificateValidationModeXPath = "connectionconfiguration/infosharewscertificatevalidationmode";
        }
    }
}
