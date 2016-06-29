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
﻿using System;
using System.Management.Automation;
using System.Text.RegularExpressions;
using ISHDeploy.Models;

namespace ISHDeploy.Validators
{
    /// <summary>
    /// Validates that deployment version corresponds to module version.
    /// </summary>
    /// <seealso cref="ValidateArgumentsAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    internal class ValidateDeploymentVersion : ValidateArgumentsAttribute
    {
        /// <summary>
        /// The very first release version of the ISHDeploy module.
        /// </summary>
        public static readonly Version ModuleInitVersion = new Version("13.0.0");

        /// <summary>
        /// Performs the validation.
        /// </summary>
        /// <param name="arguments">The instance of the <see cref="ISHDeployment"/>.</param>
        /// <param name="engineIntrinsics">The engine intrinsics.</param>
        /// <exception cref="ValidationMetadataException">
        /// Thrown when module version does not correspond to deployment version or <paramref name="arguments"/> is not instance of <see cref="ISHDeployment"/>.
        /// </exception>
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            ISHDeployment deployment = arguments as ISHDeployment;
            if (deployment == null)
            {
                throw new ValidationMetadataException($"Validation {nameof(ValidateDeploymentVersion)} should be only applied to parameter with type {nameof(ISHDeployment)}");
            }

            string errorMessage;
            if (!CheckDeploymentVersion(deployment.SoftwareVersion, out errorMessage))
            {
                throw new ValidationMetadataException(errorMessage);
            }
        }

		/// <summary>
		/// Checks if deployment version corresponds to module version.
		/// </summary>
		/// <param name="deploymentVersion">The deployment version.</param>
		/// <param name="errorMessage">The error message in case versions are different.</param>
		/// <returns>True if versions correspond; otherwise False.</returns>
		public static bool CheckDeploymentVersion(Version deploymentVersion, out string errorMessage)
		{
            errorMessage = null;



            //***************************************************************************************
            // TODO Attention!!!!! Need to be removed before release
            // OR story [TS-11041- ISHDeploy - Start using the -Force parameter] will be implemented. 
            // TS-11395 hardcoded "13.0.*.65534" for 13 version only
            //***************************************************************************************
            Regex regex11395 = new Regex(@"13.0.\d+.65534");
            if (regex11395.IsMatch(deploymentVersion.ToString()))
            {
                return true;
            }




			var moduleName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            var cmVersion = new Version(deploymentVersion.Major, deploymentVersion.Minor, deploymentVersion.Revision); // don't count about Build version.

			Regex regex = new Regex("^\\w+\\.(?<MajorVersion>\\d+)\\.(?<MinorVersion>\\d+)\\.(?<Revision>\\d+)$");
			Version moduleVersion;
			if (regex.IsMatch(moduleName) &&
	            Version.TryParse(regex.Replace(moduleName, "${MajorVersion}.${MinorVersion}.${Revision}"),
		            out moduleVersion))
			{
				if (cmVersion.CompareTo(moduleVersion) != 0)
				{
					errorMessage = $"Module version `{moduleVersion}` does not correspond to deployment version `{deploymentVersion}`.";

					if (cmVersion.CompareTo(ModuleInitVersion) < 0)
				{
						errorMessage += "\r\nPlease install newer Content Manager version.";
					}
					else
					{
						errorMessage += $"\r\nPlease install module `{moduleName}`.";
					}
				}
			}
	        else
	        {
				// As if this method can be not only used in `ISHDeploy.Validators.ValidateDeploymentVersion.Validate` 
				// it is not enought to return error message in case of not valid module.
				throw new ValidationMetadataException($"Module name `{moduleName}` has different definition than expected.");
			}

			return String.IsNullOrEmpty(errorMessage);
		}
    }
}
