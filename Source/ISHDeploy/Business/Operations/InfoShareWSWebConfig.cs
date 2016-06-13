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
        /// The path to ~\Web\InfoShareWS\Web.config
        /// </summary>
        protected static class InfoShareWSWebConfig
        {
            /// <summary>
            /// The path to ~\Web\InfoShareWS\Web.config
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(ISHDeploymentInternal, ISHFilePath.IshDeploymentType.Web,
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

            /// <summary>
            /// The xpath of "configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@thumbprint='{0}']" element in ~\Web\InfoShareWS\Web.config file
            /// </summary>
            public const string IdentityTrustedIssuersPath = InfoShareAuthorWebConfig.IdentityTrustedIssuersXPath;

            /// <summary>
            /// The xpath of "configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@thumbprint='{0}']" element in ~\Web\InfoShareWS\Web.config file
            /// </summary>
            public const string IdentityTrustedIssuersByNameXPath = InfoShareAuthorWebConfig.IdentityTrustedIssuersByNameXPath;

            /// <summary>
            /// The xpath of "configuration/system.identityModel/identityConfiguration/certificateValidation/@certificateValidationMode" element in ~\Web\InfoShareWS\Web.config file
            /// </summary>
            public const string CertificateValidationModeXPath = InfoShareAuthorWebConfig.CertificateValidationModeXPath;

            /// <summary>
            /// The xpath of "configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/serviceCredentials[@useIdentityConfiguration='true']/serviceCertificate/@findValue" attribute in ~\Web\InfoShareWS\Web.config file
            /// </summary>
            public const string CertificateThumbprintXPath = "configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/serviceCredentials[@useIdentityConfiguration='true']/serviceCertificate/@findValue";
        }
    }
}
