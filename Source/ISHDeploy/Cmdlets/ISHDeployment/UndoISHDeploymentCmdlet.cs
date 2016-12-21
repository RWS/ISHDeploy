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
﻿using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHDeployment;

namespace ISHDeploy.Cmdlets.ISHDeployment
{
    /// <summary>
    /// <para type="synopsis">Reverts all customization done by cmdlets back to original state for specific Content Manager deployment.</para>
    /// <para type="description">The Undo-ISHDeployment cmdlet reverts all customization done by cmdlets back to original state for specific Content Manager deployment.</para>
    /// <para type="description">Original state means the state of the system when it was installed and no customization was made.</para>
    /// <para type="link">Clear-ISHDeploymentHistory</para>
    /// <para type="link">Get-ISHDeployment</para>
    /// <para type="link">Get-ISHDeploymentHistory</para>
    /// <para type="link">Get-ISHDeploymentParameters</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Undo-ISHDeployment -ISHDeployment $deployment</code>
    /// <para>This command reverts Content Manager to original state.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Undo, "ISHDeployment")]
    public class UndoISHDeploymentCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// Executes revert changes cmdLet
        /// </summary>
        public override void ExecuteCmdlet()
		{
			var cmdSet = new UndoISHDeploymentOperation(Logger, ISHDeployment);
			cmdSet.Run();
		}
    }
}
