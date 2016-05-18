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
        /// The path to ~\Web\Author\ASP\Trisoft.InfoShare.Client.config file
        /// </summary>
        public static class TrisoftInfoShareClientConfig
        {
            /// <summary>
            /// The path to ~\Web\Author\ASP\Trisoft.InfoShare.Client.config
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHFilePath.IshDeploymentType.Web,
                @"Author\ASP\Trisoft.InfoShare.Client.config");

            /// <summary>
            /// The xpath of "configuration/trisoft.infoshare.client.settings/datasources/datasource/issuer/uri" element in file ~\Web\Author\ASP\Trisoft.InfoShare.Client.config
            /// </summary>
            public const string WSTrustEndpointUrlXPath = "configuration/trisoft.infoshare.client.settings/datasources/datasource/issuer/uri";

            /// <summary>
            /// The xpath of "configuration/trisoft.infoshare.client.settings/datasources/datasource/bindingtype" element in file ~\Web\Author\ASP\Trisoft.InfoShare.Client.config
            /// </summary>
            public const string WSTrustBindingTypeXPath = "configuration/trisoft.infoshare.client.settings/datasources/datasource/issuer/bindingtype";

            /// <summary>
            /// The xpath of "configuration/trisoft.infoshare.client.settings/datasources/datasource/issuer/credentials/username" element in file ~\Web\Author\ASP\Trisoft.InfoShare.Client.config
            /// </summary>
            public const string WSTrustActorUserNameXPath = "configuration/trisoft.infoshare.client.settings/datasources/datasource/actor/credentials/username";

            /// <summary>
            /// The xpath of "configuration/trisoft.infoshare.client.settings/datasources/datasource/issuer/credentials/password" element in file ~\Web\Author\ASP\Trisoft.InfoShare.Client.config
            /// </summary>
            public const string WSTrustActorPasswordXPath = "configuration/trisoft.infoshare.client.settings/datasources/datasource/actor/credentials/password";
        }
    }
}
