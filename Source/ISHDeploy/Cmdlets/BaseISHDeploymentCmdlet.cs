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
using System.Management.Automation;
using ISHDeploy.Validators;
using ISHDeploy.Business.Operations.ISHDeployment;
using System;
using System.Linq;

namespace ISHDeploy.Cmdlets
{
    /// <summary>
    /// Provides base functionality for all cmdlets that use an instance of the Content Manager deployment.
    /// </summary>
    public abstract class BaseISHDeploymentCmdlet : BaseCmdlet
    {
        private Models.ISHDeployment _ISHDeployment;
        /// <summary>
        /// <para type="description">Specifies the name or instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Name or instance of the installed Content Manager deployment.")]
        [StringToISHDeploymentTransformation]
        [ValidateDeploymentVersion]
        public Models.ISHDeployment ISHDeployment
        {
            get
            {
                if (_ISHDeployment == null)
                {
                    var operation = new GetISHDeploymentsOperation(CmdletsLogger.Instance(), null);
                    var deployments = operation.Run();
                    if (deployments.Count() == 1)
                    {
                        _ISHDeployment = deployments.First();
                    }
                    else
                    {
                        throw new ArgumentException("You have not one installed environment, please specify one.");
                    }
                }
                return _ISHDeployment;
            }
            set { _ISHDeployment = value; }
        }
    }
}
