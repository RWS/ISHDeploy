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
using System.Security.Principal;

namespace ISHDeploy.Cmdlets
{
    /// <summary>
    /// Provides base functionality for all cmdlets that use an instance of the Content Manager deployment and need administrator rights.
    /// </summary>
    public abstract class BaseISHDeploymentAdminRightsCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// Overrides BeginProcessing from base Cmdlet class with additinal debug information
        /// </summary>
        /// <exception cref="System.ArgumentException">You have not one installed environment, please specify one.</exception>
        protected override void BeginProcessing()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                throw new Exception("For this commandlet, please start powershell with administrator rights.");

            base.BeginProcessing();
        }
    }
}
