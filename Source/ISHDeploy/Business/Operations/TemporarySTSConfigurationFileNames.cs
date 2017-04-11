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
﻿namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides xpaths, search patterns and constants of deployment files
    /// </summary>
    partial class BaseOperationPaths
    {
        /// <summary>
        /// The path to ~\App\Utilities\PublishingService\Tools\FeedSDLLiveContent.ps1.config
        /// </summary>
        protected class TemporarySTSConfigurationFileNames
        {
            /// <summary>
            /// The name of ISH WS certificate file
            /// </summary>
            public const string ISHWSCertificateFileName = "ishws.cer";

            /// <summary>
            /// The CM security token service template
            /// </summary>
            public const string CMSecurityTokenServiceTemplateFileName = "CM Security Token Service Requirements.md";

            /// <summary>
            /// The template for ADFS invocation script
            /// </summary>
            public const string ADFSInvokeTemplate = "Invoke-ADFSIntegrationISH.ps1";
        }
    }
}
