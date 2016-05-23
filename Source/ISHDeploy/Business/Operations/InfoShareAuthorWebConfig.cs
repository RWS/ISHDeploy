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
		/// The path to ~\Web\Author\ASP\Web.config
		/// </summary>
		public static class InfoShareAuthorWebConfig
        {

            /// <summary>
            /// The path to ~\Web\Author\ASP\Web.config
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHFilePath.IshDeploymentType.Web,
                @"Author\ASP\Web.config");

            /// <summary>
            /// The xpath of "configuration/trisoft.infoshare.web.externalpreviewmodule/identity" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string ExternalPreviewModuleXPath = "configuration/trisoft.infoshare.web.externalpreviewmodule/identity";

            /// <summary>
            /// The attribute name of "configuration/trisoft.infoshare.web.externalpreviewmodule/identity" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string ExternalPreviewModuleAttributeName = "externalId";

            /// <summary>
            /// The xpath of "configuration/system.webServer/modules/add[@name='TrisoftExternalPreviewModule']" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string SystemWebServerModulesAddTrisoftExternalPreviewModuleXPath = "configuration/system.webServer/modules/add[@name='TrisoftExternalPreviewModule']";

            /// <summary>
            /// The xpath of "configuration/configSections/section[@name='trisoft.infoshare.web.externalpreviewmodule']" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string SectionExternalPreviewModuleXPath = "configuration/configSections/section[@name='trisoft.infoshare.web.externalpreviewmodule']";

            /// <summary>
            /// The xpath of "configuration/trisoft.infoshare.web.externalpreviewmodule" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string TrisoftInfoshareWebExternalPreviewModuleXPath = "configuration/trisoft.infoshare.web.externalpreviewmodule";

            /// <summary>
            /// The xpath of "&lt;add name='TrisoftExternalPreviewModule'" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string TrisoftExternalPreviewModuleSearchPattern = "<add name=\"TrisoftExternalPreviewModule\"";

            /// <summary>
            /// The xpath of "&lt;section name='trisoft.infoshare.web.externalpreviewmodule'" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string SectionTrisoftInfoshareWebExternalPreviewModuleSearchPattern = "<section name=\"trisoft.infoshare.web.externalpreviewmodule\"";

            /// <summary>
            /// The xpath of "&lt;trisoft.infoshare.web.externalpreviewmodule&gt;" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string TrisoftInfoshareWebExternalPreviewModuleSearchPattern = "<trisoft.infoshare.web.externalpreviewmodule>";

            /// <summary>
            /// The xpath of "configuration/system.identityModel.services/federationConfiguration/wsFederation" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string FederationConfigurationXPath = "configuration/system.identityModel.services/federationConfiguration/wsFederation";

            /// <summary>
            /// The attribute name of "configuration/system.identityModel.services/federationConfiguration/wsFederation" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string FederationConfigurationAttributeName = "issuer";

            /// <summary>
            /// The xpath of "configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@name='{0}']" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string IdentityTrustedIssuersByNameXPath = "configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@name='{0}']";

            /// <summary>
            /// The xpath of "configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@thumbprint='{0}']" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string IdentityTrustedIssuersXPath = "configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@name='{0}']";

            /// <summary>
            /// The xpath of "configuration/system.identityModel/identityConfiguration/certificateValidation/@certificateValidationMode" element in ~\Web\Author\ASP\Web.config file
            /// </summary>
            public const string CertificateValidationModeXPath = "configuration/system.identityModel/identityConfiguration/certificateValidation/@certificateValidationMode";
		}
	}
}
