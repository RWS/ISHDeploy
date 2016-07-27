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
    partial class BasePathsOperation
    {
        /// <summary>
        /// Provides constants related to ~\Web\Author\ASP\Trisoft.InfoShare.Client.config file
        /// </summary>
        protected class TrisoftInfoShareClientConfig
        {
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

            /// <summary>
            /// The xpath of "configuration/trisoft.infoshare.client.settings/datasources/datasource/certificatevalidationmode" element in ~\Web\Author\ASP\Trisoft.InfoShare.Client.config file
            /// </summary>
            public const string InfoShareWSServiceCertificateValidationModeXPath = "configuration/trisoft.infoshare.client.settings/datasources/datasource/certificatevalidationmode";
        }
    }
}
