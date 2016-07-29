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
        /// Provides constants related to ~\Web\InfoShareSTS\Web.config
        /// </summary>
        protected class InfoShareSTSWebConfig
		{
			/// <summary>
			/// Node to uncomment to enable infoshare sts to provide identity delegation for tokens issued by other sts
			/// </summary>
			public const string TrustedIssuerBehaviorExtensions = "<add name=\"addActAsTrustedIssuer\"";

            /// <summary>
            /// The xpath of "configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/addActAsTrustedIssuer[@issuer='{0}']" element in ~\Web\InfoShareSTS\Web.config file
            /// </summary>
            public const string ServiceBehaviorsTrustedUserByNameXPath = "configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/addActAsTrustedIssuer[@issuer='{0}']";

            /// <summary>
            /// The xpath of "configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/addActAsTrustedIssuer[@thumbprint='{0}']" element in ~\Web\InfoShareSTS\Web.config file
            /// </summary>
            public const string ServiceBehaviorsTrustedUserByThumbprintXPath = "configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/addActAsTrustedIssuer[@thumbprint='{0}']";

            /// <summary>
            /// The xpath of "configuration/system.web/authentication/@mode" attribute in ~\Web\InfoShareSTS\Web.config file
            /// </summary>
            public const string AuthenticationModeAttributeXPath = "configuration/system.web/authentication/@mode";
        }
	}
}