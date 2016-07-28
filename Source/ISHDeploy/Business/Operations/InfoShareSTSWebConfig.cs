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
        /// The path to ~\Web\InfoShareSTS\Web.config
        /// </summary>
        protected static class InfoShareSTSWebConfig
		{
			/// <summary>
			/// The path to ~\Web\InfoShareSTS\Web.config
			/// </summary>
			public static ISHFilePath Path => new ISHFilePath(ISHDeploymentInternal, ISHFilePath.IshDeploymentType.Web,
				@"InfoShareSTS\Web.config");

			/// <summary>
			/// Node to uncomment to enable infoshare sts to provide identity delegation for tokens issued by other sts
			/// </summary>
			public const string TrustedIssuerBehaviorExtensions = "<add name=\"addActAsTrustedIssuer\"";

            /// <summary>
            /// The xpath of "configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/addActAsTrustedIssuer[@name='{0}']" element in ~\Web\InfoShareSTS\Web.config file
            /// </summary>
            public const string ServiceBehaviorsTrustedUserByNameXPath = "configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/addActAsTrustedIssuer[@name='{0}']";

            /// <summary>
            /// The xpath of "configuration/system.web/authentication/@mode" attribute in ~\Web\InfoShareSTS\Web.config file
            /// </summary>
            public const string AuthenticationModeAttributeXPath = "configuration/system.web/authentication/@mode";
        }
	}
}