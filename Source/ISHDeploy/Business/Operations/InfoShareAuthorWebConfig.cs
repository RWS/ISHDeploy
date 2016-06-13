/**
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public partial class BasePathsOperation
    {
        /// <summary>
        /// The path to ~\Web\Author\ASP\Web.config
        /// </summary>
        protected static class InfoShareAuthorWebConfig
        {

            /// <summary>
            /// The path to ~\Web\Author\ASP\Web.config
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(ISHDeploymentInternal, ISHFilePath.IshDeploymentType.Web,
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
