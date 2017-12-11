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
        /// Provides constants related to ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config
        /// </summary>
        protected class FeedSDLLiveContentConfig
        {
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

            /// <summary>
            /// The xpath of "configuration/trisoft.utilities.serviceReferences/serviceUser/uri/@infoShareWSServiceCertificateValidationMode" element in ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config file
            /// </summary>
            public const string InfoShareWSServiceCertificateValidationModeAttributeXPath = "configuration/trisoft.utilities.serviceReferences/serviceUser/uri/@infoShareWSServiceCertificateValidationMode";

            /// <summary>
            /// The xpath of "configuration/trisoft.utilities.serviceReferences/serviceUser/user/@username" element in ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config file
            /// </summary>
            public const string ServiceUserUserNameAttributeXPath = "configuration/trisoft.utilities.serviceReferences/serviceUser/user/@username";

            /// <summary>
            /// The xpath of "configuration/trisoft.utilities.serviceReferences/serviceUser/user/@password" element in ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config file
            /// </summary>
            public const string ServiceUserPasswordAttributeXPath = "configuration/trisoft.utilities.serviceReferences/serviceUser/user/@password";
        }
    }
}
