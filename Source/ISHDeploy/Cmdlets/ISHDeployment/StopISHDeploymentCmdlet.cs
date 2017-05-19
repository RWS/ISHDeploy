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
    /// <para type="synopsis">Stops and disables all active components of specific Content Manager deployment.</para>
    /// <para type="description">The Stop-ISHDeployment cmdlet stops and disables all active components of specific Content Manager deployment, such as InfoShare Windows Services, COM+ components and IIS application pools.</para>
    /// <para type="link">Start-ISHDeployment</para>
    /// <para type="link">Restart-ISHDeployment</para>
    /// <para type="link">Disable-ISHCOMPlus</para>
    /// <para type="link">Disable-ISHIISAppPool</para>
    /// <para type="link">Disable-ISHServiceTranslationBuilder</para>
    /// <para type="link">Disable-ISHServiceTranslationOrganizer</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Stop-ISHDeployment -ISHDeployment $deployment</code>
    /// <para>This command disables all active components of specific Content Manager deployment.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Stop, "ISHDeployment")]
    public class StopISHDeploymentCmdlet : BaseISHDeploymentAdminRightsCmdlet
    {
        /// <summary>
        /// Executes revert changes cmdLet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new DisableISHComponentOperation(Logger, ISHDeployment, false, (ISHComponentName[])Enum.GetValues(typeof(ISHComponentName)));

            operation.Run();
        }
    }
}
