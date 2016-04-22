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
			public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.Web,
				@"Author\ASP\Web.config");

			/// <summary>
			/// The Trisoft external preview module search placeholder
			/// </summary>
			public const string TrisoftExternalPreviewModuleSearchPattern = "<add name=\"TrisoftExternalPreviewModule\"";

			/// <summary>
			/// The section Trisoft InfoShare web external preview module search placeholder
			/// </summary>
			public const string SectionTrisoftInfoshareWebExternalPreviewModuleSearchPattern = "<section name=\"trisoft.infoshare.web.externalpreviewmodule\"";

			/// <summary>
			/// The Trisoft InfoShare web external preview module search placeholder
			/// </summary>
			public const string TrisoftInfoshareWebExternalPreviewModuleSearchPattern = "<trisoft.infoshare.web.externalpreviewmodule>";

			/// <summary>
			/// The Trisoft external preview module xpath
			/// </summary>
			public const string TrisoftExternalPreviewModuleXPath = "configuration/system.webServer/modules/add[@name='TrisoftExternalPreviewModule']";

			/// <summary>
			/// The section Trisoft Infoshare web external preview module xpath
			/// </summary>
			public const string SectionTrisoftInfoshareWebExternalPreviewModuleXPath = "configuration/configSections/section[@name='trisoft.infoshare.web.externalpreviewmodule']";

			/// <summary>
			/// The Trisoft InfoShare web external preview module xpath
			/// </summary>
			public const string TrisoftInfoshareWebExternalPreviewModuleXPath = "configuration/trisoft.infoshare.web.externalpreviewmodule";

			/// <summary>
			/// The Trisoft InfoShare web external x path
			/// </summary>
			public const string TrisoftInfoshareWebExternalXPath = "configuration/trisoft.infoshare.web.externalpreviewmodule/identity";

			/// <summary>
			/// The Trisoft InfoShare web external attribute name
			/// </summary>
			public const string TrisoftInfoshareWebExternalAttributeName = "externalId";

			/// <summary>
			/// STS identity trusted issuers path
			/// </summary>
			public const string IdentityTrustedIssuersPath = "configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@thumbprint='{0}']";

			/// <summary>
			/// STS identity trusted issuers path
			/// </summary>
			public const string CertificateValidationModePath = "configuration/system.identityModel/identityConfiguration/certificateValidation/@certificateValidationMode";
		}
	}
}
