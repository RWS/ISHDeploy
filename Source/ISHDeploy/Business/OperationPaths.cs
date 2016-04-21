using ISHDeploy.Models;

namespace ISHDeploy.Business
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public class OperationPaths
    {
        /// <summary>
        /// The instance of the deployment.
        /// </summary>
        private static ISHDeployment _ishDeployment;

        /// <summary>
        /// Initialization of a class to build the full paths to files
        /// </summary>
        /// <param name="ishDeployment">Instance of the current <see cref="ISHDeployment"/>.</param>
        public static void Initialize(ISHDeployment ishDeployment)
        {
            _ishDeployment = ishDeployment;
        }

        #region STS WS Trust Operation 

        /// <summary>
        /// The path to ~\Web\InfoShareWS\connectionconfiguration.xml
        /// </summary>
        public static class InfoShareWSConnectionConfig
        {
            /// <summary>
            /// The path to ~\Web\InfoShareWS\connectionconfiguration.xml
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.Web,
                @"InfoShareWS\connectionconfiguration.xml");

            /// <summary>
            /// The xpath of "connectionconfiguration/issuer/authenticationtype" element in file ~\Web\InfoShareWS\connectionconfiguration.xml
            /// </summary>
            public const string WSTrustBindingTypeXPath = "connectionconfiguration/issuer/authenticationtype";

            /// <summary>
            /// The xpath of "connectionconfiguration/issuer/url" element in file ~\Web\InfoShareWS\connectionconfiguration.xml
            /// </summary>
            public const string WSTrustEndpointUrlXPath = "connectionconfiguration/issuer/url";

        }

        /// <summary>
        /// The path to ~\Web\InfoShareWS\Web.config
        /// </summary>
        public static class InfoShareWSWebConfig
        {
            /// <summary>
            /// The path to ~\Web\InfoShareWS\Web.config
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.Web,
                @"InfoShareWS\Web.config");

            /// <summary>
            /// The xpath of "configuration/system.serviceModel/bindings/customBinding/binding[@name='InfoShareWS(http)']/security/secureConversationBootstrap/issuedTokenParameters/issuerMetadata" element in ~\Web\InfoShareWS\Web.config file
            /// </summary>
            public const string WSTrustMexEndpointUrlHttpXPath = "configuration/system.serviceModel/bindings/customBinding/binding[@name='InfoShareWS(http)']/security/secureConversationBootstrap/issuedTokenParameters/issuerMetadata";

            /// <summary>
            /// The xpath of "configuration/system.serviceModel/bindings/customBinding/binding[@name='InfoShareWS(https)']/security/secureConversationBootstrap/issuedTokenParameters/issuerMetadata" element in ~\Web\InfoShareWS\Web.config file
            /// </summary>
            public const string WSTrustMexEndpointUrlHttpsXPath = "configuration/system.serviceModel/bindings/customBinding/binding[@name='InfoShareWS(https)']/security/secureConversationBootstrap/issuedTokenParameters/issuerMetadata";

            /// <summary>
            /// The attribute name of WSTrustMexEndpointBinding element in Web\InfoShareWS\Web.config file where mexEndpoint url should be updated
            /// </summary>
            public const string WSTrustMexEndpointAttributeName = "address";
        }

        /// <summary>
        /// The path to ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config
        /// </summary>
        public static class FeedSDLLiveContentConfig
        {
            /// <summary>
            /// The path to ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.Data,
                @"PublishingService\Tools\FeedSDLLiveContent.ps1.config");

            /// <summary>
            /// The xpath of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config
            /// </summary>
            public const string WSTrustEndpointUrlXPath = "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer";

            /// <summary>
            /// The attribute name of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config where endpoint url should be updated
            /// </summary>
            public const string WSTrustEndpointUrlAttributeName = "wsTrustEndpoint";

            /// <summary>
            /// The attribute name of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config where BindingType should be updated
            /// </summary>
            public const string WSTrustBindingTypeAttributeName = "wsTrustBindingType";
        }

        /// <summary>
        /// The path to ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config file
        /// </summary>
        public static class TranslationOrganizerConfig
        {
            /// <summary>
            /// The path to ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.App,
                @"TranslationOrganizer\Bin\TranslationOrganizer.exe.config");

            /// <summary>
            /// The xpath of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config
            /// </summary>
            public const string WSTrustEndpointUrlXPath = "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer";

            /// <summary>
            /// The attribute name of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config where endpoint url should be updated
            /// </summary>
            public const string WSTrustEndpointUrlAttributeName = "wsTrustEndpoint";

            /// <summary>
            /// The attribute name of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config where BindingType should be updated
            /// </summary>
            public const string WSTrustBindingTypeAttributeName = "wsTrustBindingType";
        }

        /// <summary>
        /// The path to ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config file
        /// </summary>
        public static class SynchronizeToLiveContentConfig
        {
            /// <summary>
            /// The path to ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.App,
                @"Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config");

            /// <summary>
            /// The xpath of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config
            /// </summary>
            public const string WSTrustEndpointUrlXPath = "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer";

            /// <summary>
            /// The attribute name of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config where endpoint url should be updated
            /// </summary>
            public const string WSTrustEndpointUrlAttributeName = "wsTrustEndpoint";

            /// <summary>
            /// The attribute name of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config where BindingType should be updated
            /// </summary>
            public const string WSTrustBindingTypeAttributeName = "wsTrustBindingType";
        }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Trisoft.InfoShare.Client.config file
        /// </summary>
        public static class TrisoftInfoShareClientConfig
        {
            /// <summary>
            /// The path to ~\Web\Author\ASP\Trisoft.InfoShare.Client.config
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.Web,
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

        #endregion
    }
}
