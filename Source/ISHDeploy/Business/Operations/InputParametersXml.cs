/*
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

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides xpaths to XML elements and attributes in these files
    /// </summary>
    partial class BaseOperationPaths
    {
        /// <summary>
        /// Provides xpaths to XML elements and attributes in inputparameters.xml file
        /// </summary>
        protected class InputParametersXml
        {
            /// <summary>
            /// The xpath of "inputconfig/param[@name='issueractorpassword']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string IssuerActorPasswordXPath = "inputconfig/param[@name='issueractorpassword']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='issueractorusername']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string IssuerActorUserNameXPath = "inputconfig/param[@name='issueractorusername']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='issuercertificatethumbprint']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string IssuerCertificateThumbprintXPath = "inputconfig/param[@name='issuercertificatethumbprint']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='issuercertificatevalidationmode']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string IssuerCertificateValidationModeXPath = "inputconfig/param[@name='issuercertificatevalidationmode']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='issuerwsfederationendpointurl']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string IssuerWSFederationEndpointUrlXPath = "inputconfig/param[@name='issuerwsfederationendpointurl']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='issuerwstrustbindingtype']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string IssuerWSTrustBindingTypeXPath = "inputconfig/param[@name='issuerwstrustbindingtype']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='issuerwstrustendpointurl']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string IssuerWSTrustEndpointUrlXPath = "inputconfig/param[@name='issuerwstrustendpointurl']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='authenticationtype']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string AuthenticationTypeXPath = "inputconfig/param[@name='authenticationtype']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='infosharestswindowsauthenticationenabled']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string InfoshareSTSWindowsAuthenticationEnabledXPath = "inputconfig/param[@name='infosharestswindowsauthenticationenabled']/currentvalue";
            
            /// <summary>
            /// The xpath of "inputconfig/param[@name='issuerwstrustendpointurl_normalized']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string IssuerWSTrustEndpointUrl_NormalizedXPath = "inputconfig/param[@name='issuerwstrustendpointurl_normalized']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='issuerwstrustmexurl']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string IssuerWSTrustMexUrlXPath = "inputconfig/param[@name='issuerwstrustmexurl']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='servicecertificatethumbprint']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string ServiceCertificateThumbprintXPath = "inputconfig/param[@name='servicecertificatethumbprint']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='servicecertificatevalidationmode']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string ServiceCertificateValidationModeXPath = "inputconfig/param[@name='servicecertificatevalidationmode']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='servicecertificatesubjectname']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string ServiceCertificateSubjectNameXPath = "inputconfig/param[@name='servicecertificatesubjectname']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='connectstring']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string ConnectionStringXPath = "inputconfig/param[@name='connectstring']/currentvalue";

            /// <summary>
            /// The xpath of "inputconfig/param[@name='databasetype']/currentvalue" element in inputparameters.xml file
            /// </summary>
            public const string DatabaseTypeXPath = "inputconfig/param[@name='databasetype']/currentvalue";
        }
    }
}
