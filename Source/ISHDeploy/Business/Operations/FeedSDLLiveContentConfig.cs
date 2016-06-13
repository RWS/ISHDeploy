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
        /// The path to ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config
        /// </summary>
        protected static class FeedSDLLiveContentConfig
        {
            /// <summary>
            /// The path to ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(ISHDeploymentInternal, ISHFilePath.IshDeploymentType.Data,
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
    }
}
