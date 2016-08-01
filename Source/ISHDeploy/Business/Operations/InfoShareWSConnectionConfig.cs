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
    /// Provides xpaths, search patterns and constants of deployment files
    /// </summary>
    partial class BaseOperationPaths
    {
        /// <summary>
        /// Provides constants related to ~\Web\InfoShareWS\connectionconfiguration.xml
        /// </summary>
        protected class InfoShareWSConnectionConfig
        {
            /// <summary>
            /// The xpath of "connectionconfiguration/issuer/authenticationtype" element in file ~\Web\InfoShareWS\connectionconfiguration.xml
            /// </summary>
            public const string WSTrustBindingTypeXPath = "connectionconfiguration/issuer/authenticationtype";

            /// <summary>
            /// The xpath of "connectionconfiguration/issuer/url" element in file ~\Web\InfoShareWS\connectionconfiguration.xml
            /// </summary>
            public const string WSTrustEndpointUrlXPath = "connectionconfiguration/issuer/url";

            /// <summary>
            /// The xpath of "connectionconfiguration/infosharewscertificatevalidationmode" element in ~\Web\Author\ASP\Trisoft.InfoShare.Client.config file
            /// </summary>
            public const string InfoShareWSServiceCertificateValidationModeXPath = "connectionconfiguration/infosharewscertificatevalidationmode";
        }
    }
}
