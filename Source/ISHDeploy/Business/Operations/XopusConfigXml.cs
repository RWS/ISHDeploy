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
        /// Provides constants related to ~\Web\Author\ASP\Editors\Xopus\config\config.xml
        /// </summary>
        protected class XopusConfigXml
        {
           /// <summary>
            /// The Enrich integration bluelion plugin xpath comment placeholder
            /// </summary>
            public const string EnrichIntegrationXPath = "*/*[local-name()='javascript'][@src='../BlueLion-Plugin/Bootstrap/bootstrap.js']";

            /// <summary>
            /// The Enrich integration bluelion plugin comment placeholder
            /// </summary>
            public const string EnrichIntegration = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
        }
    }
}
