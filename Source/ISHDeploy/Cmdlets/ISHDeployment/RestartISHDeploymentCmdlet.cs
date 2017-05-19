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
using System.Management.Automation;
﻿using ISHDeploy.Business.Operations.ISHComponent;
﻿using ISHDeploy.Common.Enums;

namespace ISHDeploy.Cmdlets.ISHDeployment
{
    /// <summary>
    /// <para type="synopsis">Restarts components of specific Content Manager deployment that marked as "Enabled" in tracking file.</para>
    /// <para type="description">The Restart-ISHDeployment cmdlet restarts components of specific Content Manager deployment that marked as "Enabled" in tracking file.</para>
    /// <para type="link">Start-ISHDeployment</para>
    /// <para type="link">Stop-ISHDeployment</para>
    /// <para type="link">Enable-ISHCOMPlus</para>
    /// <para type="link">Enable-ISHIISAppPool</para>
    /// <para type="link">Enable-ISHServiceTranslationBuilder</para>
    /// <para type="link">Enable-ISHServiceTranslationOrganizer</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Restart-ISHDeployment -ISHDeployment $deployment</code>
    /// <para>This command restarts components of specific Content Manager deployment that marked as "Enabled" in tracking file.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Restart, "ISHDeployment")]
    public class RestartISHDeploymentCmdlet : BaseISHDeploymentAdminRightsCmdlet
    {
        /// <summary>
        /// Executes revert changes cmdLet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var stopOperation = new DisableISHComponentOperation(Logger, ISHDeployment, false, (ISHComponentName[])Enum.GetValues(typeof(ISHComponentName)));
            stopOperation.Run();

            var startOperation = new EnableISHComponentOperation(Logger, ISHDeployment, false, (ISHComponentName[])Enum.GetValues(typeof(ISHComponentName)));
            startOperation.Run();
        }
    }
}
