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

using System;
using System.Linq;
using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHDeployment;
using ISHDeploy.Validators;

namespace ISHDeploy.Cmdlets
{
    /// <summary>
    /// Provides base functionality for all cmdlets that use an instance of the Content Manager deployment.
    /// </summary>
    public abstract class BaseISHDeploymentCmdlet : BaseCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the name or instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "Either name or instance of the installed Content Manager deployment.")]
        [StringToISHDeploymentTransformation]
        [ValidateDeploymentVersion]
        public Models.ISHDeployment ISHDeployment { get; set; }

        /// <summary>
        /// Overrides BeginProcessing from base Cmdlet class with additinal debug information
        /// </summary>
        /// <exception cref="System.ArgumentException">You have not one installed environment, please specify one.</exception>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (ISHDeployment == null)
            {
                var operation = new GetISHDeploymentsOperation(Logger, string.Empty);
                var deployments = operation.Run().ToList();
                if (deployments.Count() == 1)
                {
                    ISHDeployment = deployments.First();
                }
                else
                {
                    throw new ArgumentException("You have not one installed environment, please specify one.");
                }
            }
        }
    }
}
